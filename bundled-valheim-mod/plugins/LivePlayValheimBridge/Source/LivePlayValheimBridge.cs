// LivePlay Valheim Bridge - source snapshot
// Compile este arquivo como plugin BepInEx 5 para Valheim e empacote a DLL em:
// public/bundled-valheim-mod/plugins/LivePlayValheimBridge.dll

using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

[BepInPlugin("br.liveplay.valheim.bridge", "LivePlay Valheim Bridge", "0.1.8")]
public sealed class LivePlayValheimBridge : BaseUnityPlugin
{
    private sealed class PendingCommand
    {
        public string Command;
        public string ViewerName;
        public PendingCommand(string command, string viewerName)
        {
            Command = command;
            ViewerName = viewerName;
        }
    }

    private sealed class GameChatLine
    {
        public string PrefixText = "";
        public string ActorText = "";
        public string MessageText = "";
        public float ExpiresAt;
        public Color PrefixColor = Color.white;
        public Color ActorColor = Color.white;
        public Color MessageColor = Color.white;
    }

    private readonly ConcurrentQueue<PendingCommand> _pendingCommands = new ConcurrentQueue<PendingCommand>();
    private readonly List<GameChatLine> _gameChatLines = new List<GameChatLine>();
    private const int GameChatMaxLines = 6;
    private const float GameChatDurationSeconds = 11f;

    private TcpListener _listener;
    private Thread _serverThread;
    private volatile bool _running;
    private int _port = 35954;
    private GUIStyle _titleStyle;
    private GUIStyle _messageStyle;
    private string _toastTitle = "";
    private string _toastMessage = "";
    private float _toastUntil;

    private void Awake()
    {
        try
        {
            _port = Config.Bind("Bridge", "Port", 35954, "Porta HTTP local do LivePlay Valheim Bridge.").Value;
            StartServer();
            Logger.LogInfo("LivePlay Valheim Bridge 0.1.8 clear-inventory ativo na porta " + _port);
        }
        catch (Exception ex)
        {
            Logger.LogError("Falha ao iniciar LivePlay Valheim Bridge: " + ex);
        }
    }

    private void OnDestroy()
    {
        _running = false;
        try { _listener?.Stop(); } catch { }
        try
        {
            if (_serverThread != null && _serverThread.IsAlive)
            {
                _serverThread.Join(250);
            }
        }
        catch { }
    }

    private void Update()
    {
        int safety = 0;
        while (safety++ < 3 && _pendingCommands.TryDequeue(out PendingCommand pending))
        {
            ExecuteCommand(pending.Command, pending.ViewerName);
        }
    }

    private void OnGUI()
    {
        EnsureStyles();
        Color oldColor = GUI.color;

        if (Time.realtimeSinceStartup <= _toastUntil && !string.IsNullOrWhiteSpace(_toastMessage))
        {
            float width = 440f;
            float height = 92f;
            Rect rect = new Rect(Screen.width - width - 22f, 22f, width, height);
            GUI.color = new Color(0.08f, 0.07f, 0.04f, 0.92f);
            GUI.Box(rect, GUIContent.none);
            GUI.color = new Color(1f, 0.72f, 0.25f, 1f);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 7f, rect.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 20f, rect.y + 12f, rect.width - 32f, 22f), _toastTitle, _titleStyle);
            GUI.Label(new Rect(rect.x + 20f, rect.y + 42f, rect.width - 32f, 34f), _toastMessage, _messageStyle);
        }

        DrawGameChat();
        GUI.color = oldColor;
    }

    private void EnsureStyles()
    {
        if (_titleStyle != null && _messageStyle != null) return;
        _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, richText = false };
        _titleStyle.normal.textColor = new Color(1f, 0.86f, 0.45f, 1f);
        _messageStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, richText = false, wordWrap = true };
        _messageStyle.normal.textColor = Color.white;
    }

    private void Toast(string message)
    {
        _toastTitle = "LIVEPLAY • VALHEIM";
        _toastMessage = message;
        _toastUntil = Time.realtimeSinceStartup + 2.8f;
    }

    private void StartServer()
    {
        _running = true;
        _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
        _listener.Start();

        _serverThread = new Thread(ServerLoop)
        {
            IsBackground = true,
            Name = "LivePlayValheimBridgeHttp"
        };
        _serverThread.Start();
    }

    private void ServerLoop()
    {
        while (_running)
        {
            try
            {
                TcpClient client = _listener.AcceptTcpClient();
                try { HandleClient(client); }
                finally { try { client.Close(); } catch { } }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                break;
            }
            catch (ObjectDisposedException)
            {
                if (_running) Thread.Sleep(100);
            }
            catch (SocketException ex)
            {
                if (_running)
                {
                    Logger.LogWarning("Falha no loop HTTP LivePlay socket: " + ex.Message);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                if (_running)
                {
                    Logger.LogWarning("Falha no loop HTTP LivePlay: " + ex.Message);
                    Thread.Sleep(100);
                }
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            client.ReceiveTimeout = 1500;
            client.SendTimeout = 1500;

            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[8192];
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0) return;

                string request = Encoding.UTF8.GetString(buffer, 0, read);
                string firstLine = request.Split('\n')[0].Trim();
                bool isPing = firstLine.IndexOf("/liveplay/valheim/ping", StringComparison.OrdinalIgnoreCase) >= 0;
                bool isCommand = firstLine.IndexOf("/liveplay/valheim/command", StringComparison.OrdinalIgnoreCase) >= 0;

                if (!isPing && !isCommand)
                {
                    WriteJson(stream, 404, "{\"ok\":false,\"error\":\"not_found\"}");
                    return;
                }

                string command = ExtractJsonString(request, "command");
                if (isCommand && !string.IsNullOrWhiteSpace(command))
                {
                    string normalized = NormalizeCommand(command);
                    if (normalized.Length == 0)
                    {
                        WriteJson(stream, 400, "{\"ok\":false,\"error\":\"empty_command\",\"bridge\":\"valheim\",\"version\":\"0.1.0\"}");
                        return;
                    }

                    int repeat = ExtractRepeatCount(request);
                    repeat = Mathf.Clamp(repeat, 1, 50);
                    string viewerName = ExtractViewerName(request);

                    for (int i = 0; i < repeat; i++)
                    {
                        _pendingCommands.Enqueue(new PendingCommand(normalized, viewerName));
                    }

                    Logger.LogInfo("LivePlay Valheim queued: " + normalized + " repeat=" + repeat);
                    WriteJson(stream, 200, "{\"ok\":true,\"queued\":true,\"repeat\":" + repeat + ",\"bridge\":\"valheim\",\"version\":\"0.1.0\"}");
                    return;
                }

                WriteJson(stream, 200, "{\"ok\":true,\"bridge\":\"valheim\",\"version\":\"0.1.0\"}");
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao processar requisição LivePlay: " + ex.Message);
        }
    }

    private static string ExtractJsonString(string request, string key)
    {
        try
        {
            Match match = Regex.Match(request, "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*\\\"((?:\\\\.|[^\\\"])*)\\\"");
            if (!match.Success) return string.Empty;
            return Regex.Unescape(match.Groups[1].Value);
        }
        catch { return string.Empty; }
    }

    private static int ExtractRepeatCount(string request)
    {
        try
        {
            Match match = Regex.Match(request, "\\\"repeat\\\"\\s*:\\s*(\\d+)", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int repeat))
            {
                return Mathf.Clamp(repeat, 1, 50);
            }
        }
        catch { }
        return 1;
    }

    private static string ExtractViewerName(string request)
    {
        string nickname = ExtractJsonString(request, "viewerName");
        if (string.IsNullOrWhiteSpace(nickname)) nickname = ExtractJsonString(request, "nickname");
        if (string.IsNullOrWhiteSpace(nickname)) nickname = ExtractJsonString(request, "username");
        nickname = (nickname ?? string.Empty).Trim();
        if (nickname.Length == 0) nickname = "LivePlay";
        if (nickname.Length > 24) nickname = nickname.Substring(0, 24);
        return nickname;
    }

    private static void WriteJson(NetworkStream stream, int status, string body)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
        string statusText = status == 200 ? "OK" : status == 400 ? "Bad Request" : status == 404 ? "Not Found" : "Error";
        string head = $"HTTP/1.1 {status} {statusText}\r\nContent-Type: application/json; charset=utf-8\r\nContent-Length: {bodyBytes.Length}\r\nConnection: close\r\n\r\n";
        byte[] headBytes = Encoding.ASCII.GetBytes(head);
        stream.Write(headBytes, 0, headBytes.Length);
        stream.Write(bodyBytes, 0, bodyBytes.Length);
    }

    private static string NormalizeCommand(string raw)
    {
        string command = (raw ?? string.Empty).Trim();
        if (command.StartsWith("vh:", StringComparison.OrdinalIgnoreCase)) command = command.Substring(3).Trim();
        if (command.StartsWith("valheim:", StringComparison.OrdinalIgnoreCase)) command = command.Substring("valheim:".Length).Trim();
        if (command.StartsWith("chat ", StringComparison.OrdinalIgnoreCase)) return "chat " + command.Substring(5).Trim();
        if (command.StartsWith("console:", StringComparison.OrdinalIgnoreCase)) return "console:" + command.Substring("console:".Length).Trim();
        return command.ToLowerInvariant();
    }

    private void ExecuteCommand(string command, string viewerName)
    {
        Logger.LogInfo("LivePlay Valheim command: " + command);

        if (command.StartsWith("chat ", StringComparison.OrdinalIgnoreCase))
        {
            AddGameChatMessage(command.Substring(5).Trim());
            return;
        }

        // Spawn/itens no Valheim funcionam melhor chamando os prefabs diretamente.
        // O console interno pode recusar comandos de devcommands via reflection em algumas versões.
        if (TryExecuteDirectCommand(command, viewerName))
        {
            return;
        }

        string consoleCommand = MapToValheimConsoleCommand(command);
        if (string.IsNullOrWhiteSpace(consoleCommand))
        {
            Toast("Comando desconhecido: " + command);
            return;
        }

        if (TryRunConsoleCommand(consoleCommand))
        {
            Toast(viewerName + " enviou " + command);
            return;
        }

        Logger.LogWarning("Não foi possível executar comando Valheim: " + consoleCommand);
        Toast("Valheim não aceitou: " + command);
    }

    private bool TryExecuteDirectCommand(string command, string viewerName)
    {
        string normalized = (command ?? string.Empty).Trim().ToLowerInvariant();
        Logger.LogInfo("Valheim direct path check: " + normalized);

        switch (normalized)
        {
            case "spawn_eikthyr":
                return TrySpawnValheimPrefab("Eikthyr", 1, 7f, viewerName, "Spawn Eikthyr");
            case "spawn_elder":
                return TrySpawnValheimPrefab("gd_king", 1, 7f, viewerName, "Spawn The Elder");
            case "spawn_bonemass":
                return TrySpawnValheimPrefab("Bonemass", 1, 7f, viewerName, "Spawn Bonemass");
            case "spawn_moder":
                return TrySpawnValheimPrefab("Dragon", 1, 7f, viewerName, "Spawn Moder");
            case "spawn_yagluth":
                return TrySpawnValheimPrefab("GoblinKing", 1, 7f, viewerName, "Spawn Yagluth");
            case "spawn_seeker_queen":
                return TrySpawnValheimPrefab("SeekerQueen", 1, 7f, viewerName, "Spawn Seeker Queen");
            case "spawn_fader":
                return TrySpawnValheimPrefab("Fader", 1, 7f, viewerName, "Spawn Fader");
            case "spawn_boar":
                return TrySpawnValheimPrefab("Boar", 1, 6f, viewerName, "Spawn Boar");
            case "spawn_boar_piggy":
                return TrySpawnValheimPrefab("Boar_piggy", 1, 6f, viewerName, "Spawn Boar Piggy");
            case "spawn_deer":
                return TrySpawnValheimPrefab("Deer", 1, 6f, viewerName, "Spawn Deer");
            case "spawn_neck":
                return TrySpawnValheimPrefab("Neck", 1, 6f, viewerName, "Spawn Neck");
            case "spawn_wolf":
                return TrySpawnValheimPrefab("Wolf", 1, 6f, viewerName, "Spawn Wolf");
            case "spawn_wolf_cub":
                return TrySpawnValheimPrefab("Wolf_cub", 1, 6f, viewerName, "Spawn Wolf Cub");
            case "spawn_lox":
                return TrySpawnValheimPrefab("Lox", 1, 7f, viewerName, "Spawn Lox");
            case "spawn_lox_calf":
                return TrySpawnValheimPrefab("Lox_Calf", 1, 7f, viewerName, "Spawn Lox Calf");
            case "spawn_hare":
                return TrySpawnValheimPrefab("Hare", 1, 6f, viewerName, "Spawn Hare");
            case "spawn_chicken":
                return TrySpawnValheimPrefab("Chicken", 1, 6f, viewerName, "Spawn Chicken");
            case "spawn_hen":
                return TrySpawnValheimPrefab("Hen", 1, 6f, viewerName, "Spawn Hen");
            case "spawn_asksvin":
                return TrySpawnValheimPrefab("Asksvin", 1, 7f, viewerName, "Spawn Asksvin");
            case "spawn_asksvin_hatchling":
                return TrySpawnValheimPrefab("Asksvin_hatchling", 1, 7f, viewerName, "Spawn Asksvin Hatchling");
            case "spawn_dverger":
                return TrySpawnValheimPrefab("Dverger", 1, 7f, viewerName, "Spawn Dverger");
            case "spawn_dverger_mage":
                return TrySpawnValheimPrefab("DvergerMage", 1, 7f, viewerName, "Spawn Dverger Mage");
            case "spawn_dverger_mage_fire":
                return TrySpawnValheimPrefab("DvergerMageFire", 1, 7f, viewerName, "Spawn Dverger Mage Fire");
            case "spawn_dverger_mage_ice":
                return TrySpawnValheimPrefab("DvergerMageIce", 1, 7f, viewerName, "Spawn Dverger Mage Ice");
            case "spawn_dverger_mage_support":
                return TrySpawnValheimPrefab("DvergerMageSupport", 1, 7f, viewerName, "Spawn Dverger Mage Support");
            case "spawn_greyling":
                return TrySpawnValheimPrefab("Greyling", 1, 6f, viewerName, "Spawn Greyling");
            case "spawn_greydwarf":
                return TrySpawnValheimPrefab("Greydwarf", 1, 6f, viewerName, "Spawn Greydwarf");
            case "spawn_greydwarf_elite":
                return TrySpawnValheimPrefab("Greydwarf_Elite", 1, 7f, viewerName, "Spawn Greydwarf Elite");
            case "spawn_greydwarf_shaman":
                return TrySpawnValheimPrefab("Greydwarf_Shaman", 1, 7f, viewerName, "Spawn Greydwarf Shaman");
            case "spawn_troll":
                return TrySpawnValheimPrefab("Troll", 1, 7f, viewerName, "Spawn Troll");
            case "spawn_troll_friendly":
                return TrySpawnValheimPrefab("Troll", 1, 7f, viewerName, "Spawn Troll Amigo", true);
            case "spawn_skeleton":
                return TrySpawnValheimPrefab("Skeleton", 1, 6f, viewerName, "Spawn Skeleton");
            case "spawn_skeleton_no_archer":
                return TrySpawnValheimPrefab("Skeleton_NoArcher", 1, 6f, viewerName, "Spawn Skeleton No Archer");
            case "spawn_skeleton_poison":
                return TrySpawnValheimPrefab("Skeleton_Poison", 1, 7f, viewerName, "Spawn Skeleton Poison");
            case "spawn_skeleton_friendly":
                return TrySpawnValheimPrefab("Skeleton_Friendly", 1, 7f, viewerName, "Spawn Skeleton Friendly");
            case "spawn_abomination":
                return TrySpawnValheimPrefab("Abomination", 1, 7f, viewerName, "Spawn Abomination");
            case "spawn_blob":
                return TrySpawnValheimPrefab("Blob", 1, 6f, viewerName, "Spawn Blob");
            case "spawn_blob_elite":
                return TrySpawnValheimPrefab("BlobElite", 1, 7f, viewerName, "Spawn Blob Elite");
            case "spawn_draugr":
                return TrySpawnValheimPrefab("Draugr", 1, 6f, viewerName, "Spawn Draugr");
            case "spawn_draugr_elite":
                return TrySpawnValheimPrefab("Draugr_Elite", 1, 7f, viewerName, "Spawn Draugr Elite");
            case "spawn_draugr_ranged":
                return TrySpawnValheimPrefab("Draugr_Ranged", 1, 7f, viewerName, "Spawn Draugr Ranged");
            case "spawn_leech":
                return TrySpawnValheimPrefab("Leech", 1, 6f, viewerName, "Spawn Leech");
            case "spawn_leech_cave":
                return TrySpawnValheimPrefab("Leech_cave", 1, 6f, viewerName, "Spawn Leech Cave");
            case "spawn_surtling":
                return TrySpawnValheimPrefab("Surtling", 1, 6f, viewerName, "Spawn Surtling");
            case "spawn_wraith":
                return TrySpawnValheimPrefab("Wraith", 1, 7f, viewerName, "Spawn Wraith");
            case "spawn_bat":
                return TrySpawnValheimPrefab("Bat", 1, 6f, viewerName, "Spawn Bat");
            case "spawn_drake":
                return TrySpawnValheimPrefab("Hatchling", 1, 6f, viewerName, "Spawn Drake");
            case "spawn_fenring":
                return TrySpawnValheimPrefab("Fenring", 1, 7f, viewerName, "Spawn Fenring");
            case "spawn_fenring_cultist":
                return TrySpawnValheimPrefab("Fenring_Cultist", 1, 7f, viewerName, "Spawn Fenring Cultist");
            case "spawn_stone_golem":
                return TrySpawnValheimPrefab("StoneGolem", 1, 7f, viewerName, "Spawn Stone Golem");
            case "spawn_ulv":
                return TrySpawnValheimPrefab("Ulv", 1, 6f, viewerName, "Spawn Ulv");
            case "spawn_deathsquito":
                return TrySpawnValheimPrefab("Deathsquito", 1, 6f, viewerName, "Spawn Deathsquito");
            case "spawn_fuling":
                return TrySpawnValheimPrefab("Goblin", 1, 6f, viewerName, "Spawn Fuling");
            case "spawn_fuling_archer":
                return TrySpawnValheimPrefab("GoblinArcher", 1, 7f, viewerName, "Spawn Fuling Archer");
            case "spawn_fuling_berserker":
                return TrySpawnValheimPrefab("GoblinBrute", 1, 7f, viewerName, "Spawn Fuling Berserker");
            case "spawn_fuling_shaman":
                return TrySpawnValheimPrefab("GoblinShaman", 1, 7f, viewerName, "Spawn Fuling Shaman");
            case "spawn_serpent":
                return TrySpawnValheimPrefab("Serpent", 1, 7f, viewerName, "Spawn Serpent");
            case "spawn_gjall":
                return TrySpawnValheimPrefab("Gjall", 1, 7f, viewerName, "Spawn Gjall");
            case "spawn_seeker":
                return TrySpawnValheimPrefab("Seeker", 1, 6f, viewerName, "Spawn Seeker");
            case "spawn_seeker_brood":
                return TrySpawnValheimPrefab("SeekerBrood", 1, 6f, viewerName, "Spawn Seeker Brood");
            case "spawn_seeker_brute":
                return TrySpawnValheimPrefab("SeekerBrute", 1, 7f, viewerName, "Spawn Seeker Brute");
            case "spawn_tick":
                return TrySpawnValheimPrefab("Tick", 1, 6f, viewerName, "Spawn Tick");
            case "spawn_mistile":
                return TrySpawnValheimPrefab("Mistile", 1, 7f, viewerName, "Spawn Mistile");
            case "spawn_bonemaw":
                return TrySpawnValheimPrefab("BonemawSerpent", 1, 7f, viewerName, "Spawn Bonemaw");
            case "spawn_charred_archer":
                return TrySpawnValheimPrefab("Charred_Archer", 1, 7f, viewerName, "Spawn Charred Marksman");
            case "spawn_charred_mage":
                return TrySpawnValheimPrefab("Charred_Mage", 1, 7f, viewerName, "Spawn Charred Warlock");
            case "spawn_charred_melee":
                return TrySpawnValheimPrefab("Charred_Melee", 1, 7f, viewerName, "Spawn Charred Warrior");
            case "spawn_charred_twitcher":
                return TrySpawnValheimPrefab("Charred_Twitcher", 1, 6f, viewerName, "Spawn Charred Twitcher");
            case "spawn_fallen_valkyrie":
                return TrySpawnValheimPrefab("FallenValkyrie", 1, 7f, viewerName, "Spawn Fallen Valkyrie");
            case "spawn_morgen":
                return TrySpawnValheimPrefab("Morgen", 1, 7f, viewerName, "Spawn Morgen");
            case "spawn_morgen_awake":
                return TrySpawnValheimPrefab("Morgen_NonSleeping", 1, 7f, viewerName, "Spawn Morgen NonSleeping");
            case "spawn_volture":
                return TrySpawnValheimPrefab("Volture", 1, 6f, viewerName, "Spawn Volture");
case "clear_enemies":
                return TryKillEnemiesNearPlayer(30f, viewerName);
            case "heal_player":
                return TryHealLocalPlayer(viewerName);
            case "give_hammer":
                return TryGiveValheimItemOrDrop("Hammer", 1, 4.5f, viewerName, "Give Hammer");
            case "give_hoe":
                return TryGiveValheimItemOrDrop("Hoe", 1, 4.5f, viewerName, "Give Hoe");
            case "give_cultivator":
                return TryGiveValheimItemOrDrop("Cultivator", 1, 4.5f, viewerName, "Give Cultivator");
            case "give_pickaxe_iron":
                return TryGiveValheimItemOrDrop("PickaxeIron", 1, 4.5f, viewerName, "Give Iron Pickaxe");
            case "give_axe_iron":
                return TryGiveValheimItemOrDrop("AxeIron", 1, 4.5f, viewerName, "Give Iron Axe");
            case "give_fishing_rod":
                return TryGiveValheimItemOrDrop("FishingRod", 1, 4.5f, viewerName, "Give Fishing Rod");
            case "give_iron_mace":
                return TryGiveValheimItemOrDrop("MaceIron", 1, 4.5f, viewerName, "Give Iron Mace");
            case "give_frostner":
                return TryGiveValheimItemOrDrop("MaceSilver", 1, 4.5f, viewerName, "Give Frostner");
            case "give_iron_sword":
                return TryGiveValheimItemOrDrop("SwordIron", 1, 4.5f, viewerName, "Give Iron Sword");
            case "give_blackmetal_sword":
                return TryGiveValheimItemOrDrop("SwordBlackmetal", 1, 4.5f, viewerName, "Give Blackmetal Sword");
            case "give_mistwalker":
                return TryGiveValheimItemOrDrop("SwordMistwalker", 1, 4.5f, viewerName, "Give Mistwalker");
            case "give_draugr_fang":
                return TryGiveValheimItemOrDrop("BowDraugrFang", 1, 4.5f, viewerName, "Give Draugr Fang");
            case "give_spinesnap":
                return TryGiveValheimItemOrDrop("BowSpineSnap", 1, 4.5f, viewerName, "Give Spine Snap");
            case "give_arbalest":
                return TryGiveValheimItemOrDrop("CrossbowArbalest", 1, 4.5f, viewerName, "Give Arbalest");
            case "give_arrow_fire":
                return TryGiveValheimItemOrDrop("ArrowFire", 1, 2.2f, viewerName, "Give Fire Arrows");
            case "give_arrow_poison":
                return TryGiveValheimItemOrDrop("ArrowPoison", 1, 2.2f, viewerName, "Give Poison Arrows");
            case "give_arrow_frost":
                return TryGiveValheimItemOrDrop("ArrowFrost", 1, 2.2f, viewerName, "Give Frost Arrows");
            case "give_arrow_needle":
                return TryGiveValheimItemOrDrop("ArrowNeedle", 1, 2.2f, viewerName, "Give Needle Arrows");
            case "give_bolt_blackmetal":
                return TryGiveValheimItemOrDrop("BoltBlackmetal", 1, 2.2f, viewerName, "Give Blackmetal Bolts");
            case "give_serpent_shield":
                return TryGiveValheimItemOrDrop("ShieldSerpentscale", 1, 4.5f, viewerName, "Give Serpent Shield");
            case "give_blackmetal_shield":
                return TryGiveValheimItemOrDrop("ShieldBlackmetal", 1, 4.5f, viewerName, "Give Blackmetal Shield");
            case "give_carapace_shield":
                return TryGiveValheimItemOrDrop("ShieldCarapace", 1, 4.5f, viewerName, "Give Carapace Shield");
            case "give_root_chest":
                return TryGiveValheimItemOrDrop("ArmorRootChest", 1, 4.5f, viewerName, "Give Root Chest");
            case "give_root_legs":
                return TryGiveValheimItemOrDrop("ArmorRootLegs", 1, 4.5f, viewerName, "Give Root Legs");
            case "give_root_mask":
                return TryGiveValheimItemOrDrop("HelmetRoot", 1, 4.5f, viewerName, "Give Root Mask");
            case "give_wolf_chest":
                return TryGiveValheimItemOrDrop("ArmorWolfChest", 1, 4.5f, viewerName, "Give Wolf Chest");
            case "give_wolf_legs":
                return TryGiveValheimItemOrDrop("ArmorWolfLegs", 1, 4.5f, viewerName, "Give Wolf Legs");
            case "give_drake_helmet":
                return TryGiveValheimItemOrDrop("HelmetDrake", 1, 4.5f, viewerName, "Give Drake Helmet");
            case "give_padded_chest":
                return TryGiveValheimItemOrDrop("ArmorPaddedCuirass", 1, 4.5f, viewerName, "Give Padded Chest");
            case "give_padded_legs":
                return TryGiveValheimItemOrDrop("ArmorPaddedGreaves", 1, 4.5f, viewerName, "Give Padded Legs");
            case "give_padded_helmet":
                return TryGiveValheimItemOrDrop("HelmetPadded", 1, 4.5f, viewerName, "Give Padded Helmet");
            case "give_carapace_chest":
                return TryGiveValheimItemOrDrop("ArmorCarapaceChest", 1, 4.5f, viewerName, "Give Carapace Chest");
            case "give_carapace_legs":
                return TryGiveValheimItemOrDrop("ArmorCarapaceLegs", 1, 4.5f, viewerName, "Give Carapace Legs");
            case "give_carapace_helmet":
                return TryGiveValheimItemOrDrop("HelmetCarapace", 1, 4.5f, viewerName, "Give Carapace Helmet");
            case "give_feather_cape":
                return TryGiveValheimItemOrDrop("CapeFeather", 1, 4.5f, viewerName, "Give Feather Cape");
            case "give_wolf_cape":
                return TryGiveValheimItemOrDrop("CapeWolf", 1, 4.5f, viewerName, "Give Wolf Cape");
            case "give_honey":
                return TryGiveValheimItemOrDrop("Honey", 1, 2.2f, viewerName, "Give Honey");
            case "give_sausages":
                return TryGiveValheimItemOrDrop("Sausages", 1, 2.2f, viewerName, "Give Sausages");
            case "give_serpent_stew":
                return TryGiveValheimItemOrDrop("SerpentStew", 1, 2.2f, viewerName, "Give Serpent Stew");
            case "give_lox_pie":
                return TryGiveValheimItemOrDrop("LoxPie", 1, 2.2f, viewerName, "Give Lox Pie");
            case "give_blood_pudding":
                return TryGiveValheimItemOrDrop("BloodPudding", 1, 2.2f, viewerName, "Give Blood Pudding");
            case "give_fish_wraps":
                return TryGiveValheimItemOrDrop("FishWraps", 1, 2.2f, viewerName, "Give Fish Wraps");
            case "give_salad":
                return TryGiveValheimItemOrDrop("Salad", 1, 2.2f, viewerName, "Give Salad");
            case "give_misthare_supreme":
                return TryGiveValheimItemOrDrop("MisthareSupreme", 1, 2.2f, viewerName, "Give Misthare Supreme");
            case "give_honey_glazed_chicken":
                return TryGiveValheimItemOrDrop("HoneyGlazedChicken", 1, 2.2f, viewerName, "Give Honey Glazed Chicken");
            case "give_seeker_aspic":
                return TryGiveValheimItemOrDrop("SeekerAspic", 1, 2.2f, viewerName, "Give Seeker Aspic");
            case "give_yggdrasil_porridge":
                return TryGiveValheimItemOrDrop("YggdrasilPorridge", 1, 2.2f, viewerName, "Give Yggdrasil Porridge");
            case "give_mead_health_medium":
                return TryGiveValheimItemOrDrop("MeadHealthMedium", 1, 2.2f, viewerName, "Give Medium Healing Mead");
            case "give_mead_stamina_medium":
                return TryGiveValheimItemOrDrop("MeadStaminaMedium", 1, 2.2f, viewerName, "Give Medium Stamina Mead");
            case "give_mead_poison_resist":
                return TryGiveValheimItemOrDrop("MeadPoisonResist", 1, 2.2f, viewerName, "Give Poison Resist Mead");
            case "give_mead_frost_resist":
                return TryGiveValheimItemOrDrop("MeadFrostResist", 1, 2.2f, viewerName, "Give Frost Resist Mead");
            case "give_mead_fire_resist":
                return TryGiveValheimItemOrDrop("MeadFireResist", 1, 2.2f, viewerName, "Give Fire Resist Mead");
            case "give_mead_eitr_medium":
                return TryGiveValheimItemOrDrop("MeadEitrMedium", 1, 2.2f, viewerName, "Give Medium Eitr Mead");
            case "spawn_big_tree_log":
                return TrySpawnValheimPrefab("Oak_log", 1, 5.5f, viewerName, "Spawn Tronco Grande");
            case "time_day":
                return TrySetValheimTime(true, viewerName);
            case "time_night":
                return TrySetValheimTime(false, viewerName);
            case "clear_inventory":
                return TryClearValheimInventory(viewerName);



default:
                Logger.LogInfo("Comando Valheim sem caminho direto: " + normalized);
                return false;
        }
    }

    private bool TryClearValheimInventory(string viewerName)
    {
        try
        {
            object localPlayer = GetLocalPlayerObject();
            if (localPlayer == null)
            {
                Logger.LogWarning("Player local não encontrado para limpar inventário.");
                return false;
            }

            object inventory = GetValheimPlayerInventory(localPlayer);
            if (inventory == null)
            {
                Logger.LogWarning("Inventário do player não encontrado para limpar.");
                return false;
            }

            Type inventoryType = inventory.GetType();

            // Tenta métodos diretos primeiro, variando conforme versão do Valheim.
            foreach (string methodName in new string[] { "RemoveAll", "RemoveAllItems", "Clear" })
            {
                MethodInfo method = inventoryType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (method == null) continue;

                try
                {
                    method.Invoke(inventory, null);
                    Logger.LogInfo("Inventário Valheim limpo por " + methodName + ".");
                    Toast(viewerName + " limpou o inventário");
                    return true;
                }
                catch
                {
                    // tenta próximo método
                }
            }

            // Fallback: pega a lista interna m_inventory e remove item por item.
            FieldInfo inventoryField = inventoryType.GetField("m_inventory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            object inventoryList = inventoryField != null ? inventoryField.GetValue(inventory) : null;
            System.Collections.IEnumerable enumerable = inventoryList as System.Collections.IEnumerable;
            if (enumerable == null)
            {
                Logger.LogWarning("Lista interna m_inventory não encontrada para limpar inventário.");
                return false;
            }

            List<object> items = new List<object>();
            foreach (object item in enumerable)
            {
                if (item != null) items.Add(item);
            }

            int removed = 0;
            foreach (object item in items)
            {
                if (TryRemoveInventoryItem(inventory, inventoryType, item))
                {
                    removed++;
                }
            }

            if (removed > 0 || items.Count == 0)
            {
                Logger.LogInfo("Inventário Valheim limpo por fallback. Itens removidos: " + removed + ".");
                Toast(viewerName + " limpou o inventário");
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao limpar inventário Valheim: " + ex);
        }

        return false;
    }

    private object GetValheimPlayerInventory(object localPlayer)
    {
        if (localPlayer == null) return null;

        Type playerType = localPlayer.GetType();

        try
        {
            MethodInfo getInventory = playerType.GetMethod("GetInventory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (getInventory != null && getInventory.GetParameters().Length == 0)
            {
                object inventory = getInventory.Invoke(localPlayer, null);
                if (inventory != null) return inventory;
            }
        }
        catch { }

        try
        {
            FieldInfo inventoryField = playerType.GetField("m_inventory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (inventoryField != null)
            {
                return inventoryField.GetValue(localPlayer);
            }
        }
        catch { }

        return null;
    }

    private bool TryRemoveInventoryItem(object inventory, Type inventoryType, object item)
    {
        if (inventory == null || inventoryType == null || item == null) return false;

        foreach (MethodInfo method in inventoryType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (!string.Equals(method.Name, "RemoveItem", StringComparison.OrdinalIgnoreCase)) continue;

            ParameterInfo[] ps = method.GetParameters();
            object[] args = null;

            if (ps.Length == 1 && ps[0].ParameterType.IsAssignableFrom(item.GetType()))
            {
                args = new object[] { item };
            }
            else if (ps.Length == 2 && ps[0].ParameterType.IsAssignableFrom(item.GetType()) && ps[1].ParameterType == typeof(int))
            {
                args = new object[] { item, 999999 };
            }

            if (args == null) continue;

            try
            {
                method.Invoke(inventory, args);
                return true;
            }
            catch
            {
                // tenta outra assinatura
            }
        }

        return false;
    }

    private bool TryGiveValheimItemOrDrop(string prefabName, int count, float radius, string viewerName, string label)
    {
        int safeCount = Mathf.Clamp(count, 1, 100);
        try
        {
            if (TryAddValheimItemToInventory(prefabName, safeCount))
            {
                Logger.LogInfo("Item Valheim adicionado ao inventário: " + prefabName + " x" + safeCount);
                Toast(viewerName + " enviou " + label);
                return true;
            }

            Logger.LogInfo("Inventário cheio ou AddItem falhou. Item vai cair no chão: " + prefabName + " x" + safeCount);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao adicionar item no inventário Valheim (" + prefabName + "): " + ex.Message);
        }

        return TrySpawnValheimPrefab(prefabName, safeCount, radius, viewerName, label);
    }

    private bool TryAddValheimItemToInventory(string prefabName, int count)
    {
        object localPlayer = GetLocalPlayerObject();
        if (localPlayer == null)
        {
            Logger.LogWarning("Player local não encontrado para adicionar item no inventário: " + prefabName);
            return false;
        }

        Type playerType = localPlayer.GetType();
        object inventory = null;

        try
        {
            MethodInfo getInventory = playerType.GetMethod("GetInventory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (getInventory != null && getInventory.GetParameters().Length == 0)
            {
                inventory = getInventory.Invoke(localPlayer, null);
            }
        }
        catch { }

        if (inventory == null)
        {
            try
            {
                FieldInfo inventoryField = playerType.GetField("m_inventory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (inventoryField != null) inventory = inventoryField.GetValue(localPlayer);
            }
            catch { }
        }

        if (inventory == null)
        {
            Logger.LogWarning("Inventário do player não encontrado para item: " + prefabName);
            return false;
        }

        GameObject prefab = FindValheimPrefab(prefabName);
        if (prefab == null)
        {
            Logger.LogWarning("Prefab Valheim não encontrado para inventário: " + prefabName);
            return false;
        }

        Type inventoryType = inventory.GetType();

        foreach (MethodInfo method in inventoryType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (!string.Equals(method.Name, "AddItem", StringComparison.OrdinalIgnoreCase)) continue;

            ParameterInfo[] ps = method.GetParameters();
            object[] args = BuildInventoryAddItemArgs(ps, prefab, prefabName, count);
            if (args == null) continue;

            try
            {
                object result = method.Invoke(inventory, args);
                if (method.ReturnType == typeof(bool))
                {
                    return result is bool ok && ok;
                }

                // Algumas assinaturas retornam ItemData/null. Se retornou algo, consideramos sucesso.
                if (method.ReturnType != typeof(void))
                {
                    return result != null;
                }

                return true;
            }
            catch
            {
                // tenta a próxima assinatura
            }
        }

        Logger.LogWarning("Nenhuma assinatura AddItem aceitou o item: " + prefabName);
        return false;
    }

    private object[] BuildInventoryAddItemArgs(ParameterInfo[] parameters, GameObject prefab, string prefabName, int count)
    {
        object[] args = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            Type parameterType = parameters[i].ParameterType;
            string name = (parameters[i].Name ?? string.Empty).ToLowerInvariant();

            if (parameterType == typeof(string))
            {
                args[i] = prefabName;
                continue;
            }

            if (parameterType == typeof(GameObject))
            {
                args[i] = prefab;
                continue;
            }

            if (parameterType == typeof(int))
            {
                if (name.Contains("amount") || name.Contains("stack") || name.Contains("count") || i == 1)
                {
                    args[i] = count;
                }
                else
                {
                    args[i] = 1;
                }
                continue;
            }

            if (parameterType == typeof(float))
            {
                args[i] = 1f;
                continue;
            }

            if (parameterType == typeof(bool))
            {
                args[i] = false;
                continue;
            }

            if (string.Equals(parameterType.Name, "Vector2i", StringComparison.OrdinalIgnoreCase))
            {
                args[i] = Activator.CreateInstance(parameterType, new object[] { -1, -1 });
                continue;
            }

            if (parameterType.IsEnum)
            {
                Array values = Enum.GetValues(parameterType);
                args[i] = values.Length > 0 ? values.GetValue(0) : Activator.CreateInstance(parameterType);
                continue;
            }

            if (parameterType.IsValueType)
            {
                args[i] = Activator.CreateInstance(parameterType);
                continue;
            }

            args[i] = null;
        }

        return args;
    }

    private bool TrySpawnValheimPrefab(string prefabName, int count, float radius, string viewerName, string label, bool makeFriendly = false)
    {
        try
        {
            GameObject prefab = FindValheimPrefab(prefabName);
            if (prefab == null)
            {
                Logger.LogWarning("Prefab Valheim não encontrado: " + prefabName);
                return false;
            }

            Transform playerTransform = GetLocalPlayerTransform();
            if (playerTransform == null)
            {
                Logger.LogWarning("Player local do Valheim não encontrado para spawn: " + prefabName);
                return false;
            }

            Vector3 forward = playerTransform.forward;
            if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;

            int spawned = 0;
            int safeCount = Mathf.Clamp(count, 1, 50);
            for (int i = 0; i < safeCount; i++)
            {
                float angle = safeCount <= 1 ? 0f : ((360f / safeCount) * i);
                Vector3 dir = Quaternion.Euler(0f, angle, 0f) * forward;
                float spread = Mathf.Max(1.5f, radius);
                Vector3 pos = playerTransform.position + dir.normalized * spread + Vector3.up * 1.2f;

                // Mantém acima do jogador para evitar dependência direta de UnityEngine.PhysicsModule no build local.
                // O próprio Valheim estabiliza boa parte dos prefabs após instanciar.
                pos += Vector3.up * 0.6f;

                Quaternion rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
                Logger.LogInfo("Instanciando prefab Valheim: " + prefabName + " em " + pos);
                GameObject spawnedObject = UnityEngine.Object.Instantiate(prefab, pos, rot) as GameObject;
                if (makeFriendly && spawnedObject != null)
                {
                    TryMakeValheimCreatureFriendly(spawnedObject);
                }
                spawned++;
            }

            if (spawned > 0)
            {
                Logger.LogInfo("Spawn direto Valheim OK: " + prefabName + " x" + spawned);
                Toast(viewerName + " enviou " + label);
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha no spawn direto Valheim (" + prefabName + "): " + ex);
        }

        Logger.LogWarning("Spawn direto Valheim retornou false para: " + prefabName);
        return false;
    }

    private void TryMakeValheimCreatureFriendly(GameObject spawnedObject)
    {
        try
        {
            if (spawnedObject == null) return;

            foreach (MonoBehaviour behaviour in spawnedObject.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour == null) continue;

                Type type = behaviour.GetType();
                if (!string.Equals(type.Name, "Character", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(type.BaseType != null ? type.BaseType.Name : string.Empty, "Character", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                FieldInfo factionField = type.GetField("m_faction", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (factionField != null && factionField.FieldType.IsEnum)
                {
                    object faction = null;
                    try { faction = Enum.Parse(factionField.FieldType, "Players"); } catch { }
                    if (faction == null)
                    {
                        try { faction = Enum.Parse(factionField.FieldType, "Player"); } catch { }
                    }
                    if (faction != null)
                    {
                        factionField.SetValue(behaviour, faction);
                        Logger.LogInfo("Criatura Valheim marcada como amiga: " + spawnedObject.name);
                    }
                }

                FieldInfo tameField = type.GetField("m_tamed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (tameField != null && tameField.FieldType == typeof(bool))
                {
                    tameField.SetValue(behaviour, true);
                }
            }

            foreach (MonoBehaviour behaviour in spawnedObject.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (!string.Equals(type.Name, "Tameable", StringComparison.OrdinalIgnoreCase)) continue;

                FieldInfo tamedField = type.GetField("m_tamed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (tamedField != null && tamedField.FieldType == typeof(bool))
                {
                    tamedField.SetValue(behaviour, true);
                }

                MethodInfo tameMethod = type.GetMethod("Tame", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (tameMethod != null && tameMethod.GetParameters().Length == 0)
                {
                    try { tameMethod.Invoke(behaviour, null); } catch { }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao tornar criatura Valheim amiga: " + ex.Message);
        }
    }

    private GameObject FindValheimPrefab(string prefabName)
    {
        if (string.IsNullOrWhiteSpace(prefabName)) return null;

        try
        {
            Type znetSceneType = FindTypeByName("ZNetScene");
            if (znetSceneType != null)
            {
                object scene =
                    GetStaticMember(znetSceneType, "instance") ??
                    GetStaticMember(znetSceneType, "m_instance") ??
                    GetStaticMember(znetSceneType, "s_instance");

                if (scene != null)
                {
                    MethodInfo getPrefab = znetSceneType.GetMethod("GetPrefab", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
                    if (getPrefab != null)
                    {
                        object result = getPrefab.Invoke(scene, new object[] { prefabName });
                        GameObject prefab = result as GameObject;
                        if (prefab != null) return prefab;
                    }

                    FieldInfo namedPrefabsField = znetSceneType.GetField("m_namedPrefabs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    object namedPrefabs = namedPrefabsField != null ? namedPrefabsField.GetValue(scene) : null;
                    GameObject fromDict = TryGetGameObjectFromDictionary(namedPrefabs, prefabName);
                    if (fromDict != null) return fromDict;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha consultando ZNetScene para prefab " + prefabName + ": " + ex.Message);
        }

        try
        {
            foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (obj == null) continue;
                if (string.Equals(obj.name, prefabName, StringComparison.OrdinalIgnoreCase))
                {
                    return obj;
                }
            }
        }
        catch { }

        return null;
    }

    private static GameObject TryGetGameObjectFromDictionary(object dictionary, string prefabName)
    {
        if (dictionary == null || string.IsNullOrWhiteSpace(prefabName)) return null;

        try
        {
            Type type = dictionary.GetType();
            MethodInfo tryGetValue = type.GetMethod("TryGetValue");
            if (tryGetValue != null)
            {
                object[] args = new object[] { StableHashCode(prefabName), null };
                bool ok = (bool)tryGetValue.Invoke(dictionary, args);
                if (ok && args.Length > 1) return args[1] as GameObject;
            }
        }
        catch { }

        try
        {
            System.Collections.IEnumerable enumerable = dictionary as System.Collections.IEnumerable;
            if (enumerable == null) return null;

            foreach (object entry in enumerable)
            {
                if (entry == null) continue;
                Type entryType = entry.GetType();
                PropertyInfo valueProperty = entryType.GetProperty("Value");
                object value = valueProperty != null ? valueProperty.GetValue(entry, null) : null;
                GameObject go = value as GameObject;
                if (go != null && string.Equals(go.name, prefabName, StringComparison.OrdinalIgnoreCase))
                {
                    return go;
                }
            }
        }
        catch { }

        return null;
    }

    private object GetLocalPlayerObject()
    {
        try
        {
            Type playerType = FindTypeByName("Player");
            if (playerType == null) return null;

            return GetStaticMember(playerType, "m_localPlayer") ??
                   GetStaticMember(playerType, "s_localPlayer") ??
                   GetStaticMember(playerType, "instance") ??
                   InvokeStaticNoArgs(playerType, "GetLocalPlayer");
        }
        catch
        {
            return null;
        }
    }

    private Transform GetLocalPlayerTransform()
    {
        try
        {
            Component component = GetLocalPlayerObject() as Component;
            if (component != null) return component.transform;
        }
        catch { }

        try
        {
            Camera camera = Camera.main;
            if (camera != null) return camera.transform;
        }
        catch { }

        return null;
    }

    private bool TryHealLocalPlayer(string viewerName)
    {
        try
        {
            Type playerType = FindTypeByName("Player");
            object localPlayer =
                playerType != null
                    ? (GetStaticMember(playerType, "m_localPlayer") ?? GetStaticMember(playerType, "s_localPlayer") ?? InvokeStaticNoArgs(playerType, "GetLocalPlayer"))
                    : null;

            if (localPlayer == null) return false;

            Type type = localPlayer.GetType();
            foreach (string methodName in new string[] { "Heal", "AddHealth" })
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (!string.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase)) continue;
                    ParameterInfo[] parameters = method.GetParameters();
                    try
                    {
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(float))
                        {
                            method.Invoke(localPlayer, new object[] { 50f });
                            Toast(viewerName + " curou o jogador");
                            return true;
                        }
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
                        {
                            method.Invoke(localPlayer, new object[] { 50 });
                            Toast(viewerName + " curou o jogador");
                            return true;
                        }
                    }
                    catch { }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao curar player Valheim: " + ex.Message);
        }

        return false;
    }

    private bool TryKillEnemiesNearPlayer(float radius, string viewerName)
    {
        try
        {
            Transform playerTransform = GetLocalPlayerTransform();
            if (playerTransform == null) return false;

            int removed = 0;
            Type characterType = FindTypeByName("Character");
            if (characterType == null)
            {
                Logger.LogWarning("Tipo Character do Valheim não encontrado para clear_enemies.");
                return false;
            }

            foreach (MonoBehaviour behaviour in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (!characterType.IsAssignableFrom(type)) continue;

                bool isPlayer = false;
                try
                {
                    MethodInfo isPlayerMethod = type.GetMethod("IsPlayer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (isPlayerMethod != null)
                    {
                        object value = isPlayerMethod.Invoke(behaviour, null);
                        isPlayer = value is bool ok && ok;
                    }
                }
                catch { }

                if (isPlayer) continue;

                float distance = Vector3.Distance(playerTransform.position, behaviour.transform.position);
                if (distance > radius) continue;

                try
                {
                    UnityEngine.Object.Destroy(behaviour.gameObject);
                    removed++;
                }
                catch { }
            }

            if (removed > 0)
            {
                Logger.LogInfo("Valheim clear_enemies removeu " + removed + " inimigos por reflexão.");
                Toast(viewerName + " limpou inimigos");
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha no clear_enemies direto Valheim: " + ex);
        }

        return false;
    }

    private static string MapToValheimConsoleCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return string.Empty;

        if (command.StartsWith("console:", StringComparison.OrdinalIgnoreCase))
        {
            return command.Substring("console:".Length).Trim();
        }

        switch (command.ToLowerInvariant())
        {
            case "spawn_eikthyr": return "spawn Eikthyr 1";
            case "spawn_elder": return "spawn gd_king 1";
            case "spawn_bonemass": return "spawn Bonemass 1";
            case "spawn_moder": return "spawn Dragon 1";
            case "spawn_yagluth": return "spawn GoblinKing 1";
            case "spawn_seeker_queen": return "spawn SeekerQueen 1";
            case "spawn_fader": return "spawn Fader 1";
            case "spawn_boar": return "spawn Boar 1";
            case "spawn_boar_piggy": return "spawn Boar_piggy 1";
            case "spawn_deer": return "spawn Deer 1";
            case "spawn_neck": return "spawn Neck 1";
            case "spawn_wolf": return "spawn Wolf 1";
            case "spawn_wolf_cub": return "spawn Wolf_cub 1";
            case "spawn_lox": return "spawn Lox 1";
            case "spawn_lox_calf": return "spawn Lox_Calf 1";
            case "spawn_hare": return "spawn Hare 1";
            case "spawn_chicken": return "spawn Chicken 1";
            case "spawn_hen": return "spawn Hen 1";
            case "spawn_asksvin": return "spawn Asksvin 1";
            case "spawn_asksvin_hatchling": return "spawn Asksvin_hatchling 1";
            case "spawn_dverger": return "spawn Dverger 1";
            case "spawn_dverger_mage": return "spawn DvergerMage 1";
            case "spawn_dverger_mage_fire": return "spawn DvergerMageFire 1";
            case "spawn_dverger_mage_ice": return "spawn DvergerMageIce 1";
            case "spawn_dverger_mage_support": return "spawn DvergerMageSupport 1";
            case "spawn_greyling": return "spawn Greyling 1";
            case "spawn_greydwarf": return "spawn Greydwarf 1";
            case "spawn_greydwarf_elite": return "spawn Greydwarf_Elite 1";
            case "spawn_greydwarf_shaman": return "spawn Greydwarf_Shaman 1";
            case "spawn_troll": return "spawn Troll 1";
            case "spawn_troll_friendly": return "spawn Troll 1";
            case "spawn_skeleton": return "spawn Skeleton 1";
            case "spawn_skeleton_no_archer": return "spawn Skeleton_NoArcher 1";
            case "spawn_skeleton_poison": return "spawn Skeleton_Poison 1";
            case "spawn_skeleton_friendly": return "spawn Skeleton_Friendly 1";
            case "spawn_abomination": return "spawn Abomination 1";
            case "spawn_blob": return "spawn Blob 1";
            case "spawn_blob_elite": return "spawn BlobElite 1";
            case "spawn_draugr": return "spawn Draugr 1";
            case "spawn_draugr_elite": return "spawn Draugr_Elite 1";
            case "spawn_draugr_ranged": return "spawn Draugr_Ranged 1";
            case "spawn_leech": return "spawn Leech 1";
            case "spawn_leech_cave": return "spawn Leech_cave 1";
            case "spawn_surtling": return "spawn Surtling 1";
            case "spawn_wraith": return "spawn Wraith 1";
            case "spawn_bat": return "spawn Bat 1";
            case "spawn_drake": return "spawn Hatchling 1";
            case "spawn_fenring": return "spawn Fenring 1";
            case "spawn_fenring_cultist": return "spawn Fenring_Cultist 1";
            case "spawn_stone_golem": return "spawn StoneGolem 1";
            case "spawn_ulv": return "spawn Ulv 1";
            case "spawn_deathsquito": return "spawn Deathsquito 1";
            case "spawn_fuling": return "spawn Goblin 1";
            case "spawn_fuling_archer": return "spawn GoblinArcher 1";
            case "spawn_fuling_berserker": return "spawn GoblinBrute 1";
            case "spawn_fuling_shaman": return "spawn GoblinShaman 1";
            case "spawn_serpent": return "spawn Serpent 1";
            case "spawn_gjall": return "spawn Gjall 1";
            case "spawn_seeker": return "spawn Seeker 1";
            case "spawn_seeker_brood": return "spawn SeekerBrood 1";
            case "spawn_seeker_brute": return "spawn SeekerBrute 1";
            case "spawn_tick": return "spawn Tick 1";
            case "spawn_mistile": return "spawn Mistile 1";
            case "spawn_bonemaw": return "spawn BonemawSerpent 1";
            case "spawn_charred_archer": return "spawn Charred_Archer 1";
            case "spawn_charred_mage": return "spawn Charred_Mage 1";
            case "spawn_charred_melee": return "spawn Charred_Melee 1";
            case "spawn_charred_twitcher": return "spawn Charred_Twitcher 1";
            case "spawn_fallen_valkyrie": return "spawn FallenValkyrie 1";
            case "spawn_morgen": return "spawn Morgen 1";
            case "spawn_morgen_awake": return "spawn Morgen_NonSleeping 1";
            case "spawn_volture": return "spawn Volture 1";
case "heal_player": return "heal";
            case "damage_player": return "damage 25";
            case "weather_storm": return "env ThunderStorm";
            case "clear_enemies": return "killenemies";
            case "give_hammer": return "spawn Hammer 1";
            case "give_hoe": return "spawn Hoe 1";
            case "give_cultivator": return "spawn Cultivator 1";
            case "give_pickaxe_iron": return "spawn PickaxeIron 1";
            case "give_axe_iron": return "spawn AxeIron 1";
            case "give_fishing_rod": return "spawn FishingRod 1";
            case "give_iron_mace": return "spawn MaceIron 1";
            case "give_frostner": return "spawn MaceSilver 1";
            case "give_iron_sword": return "spawn SwordIron 1";
            case "give_blackmetal_sword": return "spawn SwordBlackmetal 1";
            case "give_mistwalker": return "spawn SwordMistwalker 1";
            case "give_draugr_fang": return "spawn BowDraugrFang 1";
            case "give_spinesnap": return "spawn BowSpineSnap 1";
            case "give_arbalest": return "spawn CrossbowArbalest 1";
            case "give_arrow_fire": return "spawn ArrowFire 1";
            case "give_arrow_poison": return "spawn ArrowPoison 1";
            case "give_arrow_frost": return "spawn ArrowFrost 1";
            case "give_arrow_needle": return "spawn ArrowNeedle 1";
            case "give_bolt_blackmetal": return "spawn BoltBlackmetal 1";
            case "give_serpent_shield": return "spawn ShieldSerpentscale 1";
            case "give_blackmetal_shield": return "spawn ShieldBlackmetal 1";
            case "give_carapace_shield": return "spawn ShieldCarapace 1";
            case "give_root_chest": return "spawn ArmorRootChest 1";
            case "give_root_legs": return "spawn ArmorRootLegs 1";
            case "give_root_mask": return "spawn HelmetRoot 1";
            case "give_wolf_chest": return "spawn ArmorWolfChest 1";
            case "give_wolf_legs": return "spawn ArmorWolfLegs 1";
            case "give_drake_helmet": return "spawn HelmetDrake 1";
            case "give_padded_chest": return "spawn ArmorPaddedCuirass 1";
            case "give_padded_legs": return "spawn ArmorPaddedGreaves 1";
            case "give_padded_helmet": return "spawn HelmetPadded 1";
            case "give_carapace_chest": return "spawn ArmorCarapaceChest 1";
            case "give_carapace_legs": return "spawn ArmorCarapaceLegs 1";
            case "give_carapace_helmet": return "spawn HelmetCarapace 1";
            case "give_feather_cape": return "spawn CapeFeather 1";
            case "give_wolf_cape": return "spawn CapeWolf 1";
            case "give_honey": return "spawn Honey 1";
            case "give_sausages": return "spawn Sausages 1";
            case "give_serpent_stew": return "spawn SerpentStew 1";
            case "give_lox_pie": return "spawn LoxPie 1";
            case "give_blood_pudding": return "spawn BloodPudding 1";
            case "give_fish_wraps": return "spawn FishWraps 1";
            case "give_salad": return "spawn Salad 1";
            case "give_misthare_supreme": return "spawn MisthareSupreme 1";
            case "give_honey_glazed_chicken": return "spawn HoneyGlazedChicken 1";
            case "give_seeker_aspic": return "spawn SeekerAspic 1";
            case "give_yggdrasil_porridge": return "spawn YggdrasilPorridge 1";
            case "give_mead_health_medium": return "spawn MeadHealthMedium 1";
            case "give_mead_stamina_medium": return "spawn MeadStaminaMedium 1";
            case "give_mead_poison_resist": return "spawn MeadPoisonResist 1";
            case "give_mead_frost_resist": return "spawn MeadFrostResist 1";
            case "give_mead_fire_resist": return "spawn MeadFireResist 1";
            case "give_mead_eitr_medium": return "spawn MeadEitrMedium 1";
            case "spawn_big_tree_log": return "spawn Oak_log 1";
            case "time_day": return "tod 0.5";
            case "time_night": return "tod 0";
            case "clear_inventory": return "clearinventory";



default: return command;
        }
    }

    private bool TrySetValheimTime(bool day, string viewerName)
    {
        string consoleCommand = day ? "tod 0.5" : "tod 0";
        if (TryRunConsoleCommand(consoleCommand))
        {
            Toast(day ? "LivePlay deixou de dia" : "LivePlay deixou de noite");
            return true;
        }

        try
        {
            Type envManType = FindTypeByName("EnvMan");
            object envMan =
                envManType != null
                    ? (GetStaticMember(envManType, "instance") ?? GetStaticMember(envManType, "m_instance") ?? GetStaticMember(envManType, "s_instance"))
                    : null;

            if (envMan != null)
            {
                Type type = envMan.GetType();

                FieldInfo timeField = type.GetField("m_debugTimeOfDay", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (timeField != null && timeField.FieldType == typeof(float))
                {
                    timeField.SetValue(envMan, day ? 0.5f : 0f);
                    Toast(day ? "LivePlay deixou de dia" : "LivePlay deixou de noite");
                    return true;
                }

                MethodInfo setDebugTime = type.GetMethod("SetDebugTimeOfDay", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (setDebugTime != null)
                {
                    ParameterInfo[] ps = setDebugTime.GetParameters();
                    if (ps.Length == 1 && ps[0].ParameterType == typeof(float))
                    {
                        setDebugTime.Invoke(envMan, new object[] { day ? 0.5f : 0f });
                        Toast(day ? "LivePlay deixou de dia" : "LivePlay deixou de noite");
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao alterar horário Valheim: " + ex.Message);
        }

        Logger.LogWarning("Não foi possível alterar horário Valheim: " + consoleCommand);
        return false;
    }

    private bool TryRunConsoleCommand(string consoleCommand)
    {
        try
        {
            object console = FindValheimConsole();
            if (console == null)
            {
                Logger.LogWarning("Console do Valheim não encontrado. Abra um mundo antes de testar comandos.");
                return false;
            }

            // Valheim bloqueia spawn/heal/env/killenemies quando devcommands não está ativo.
            // Envia antes de cada comando pesado. Se já estiver ativo, o jogo normalmente só confirma no console.
            if (!string.Equals(consoleCommand, "devcommands", StringComparison.OrdinalIgnoreCase))
            {
                TryInvokeConsoleCommand(console, "devcommands");
            }

            return TryInvokeConsoleCommand(console, consoleCommand);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao chamar console Valheim: " + ex.Message);
        }

        return false;
    }

    private bool TryInvokeConsoleCommand(object console, string consoleCommand)
    {
        if (console == null || string.IsNullOrWhiteSpace(consoleCommand)) return false;

        Type type = console.GetType();
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        string[] preferredNames = new string[]
        {
            "TryRunCommand",
            "RunCommand",
            "InputText",
            "Execute",
            "ExecCommand"
        };

        foreach (string methodName in preferredNames)
        {
            foreach (MethodInfo method in methods)
            {
                if (!string.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase)) continue;

                object[] args;
                if (!TryBuildConsoleArgs(method.GetParameters(), consoleCommand, out args)) continue;

                try
                {
                    object target = method.IsStatic ? null : console;
                    object result = method.Invoke(target, args);

                    if (method.ReturnType == typeof(bool))
                    {
                        if (result is bool ok && ok) return true;
                        continue;
                    }

                    return true;
                }
                catch
                {
                    // tenta a próxima assinatura
                }
            }
        }

        Logger.LogWarning("Nenhuma assinatura compatível do console Valheim aceitou: " + consoleCommand);
        return false;
    }

    private static bool TryBuildConsoleArgs(ParameterInfo[] parameters, string command, out object[] args)
    {
        args = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            Type parameterType = parameters[i].ParameterType;
            string name = (parameters[i].Name ?? string.Empty).ToLowerInvariant();

            if (parameterType == typeof(string))
            {
                args[i] = command;
                continue;
            }

            if (parameterType == typeof(bool))
            {
                args[i] = true;
                continue;
            }

            if (parameterType == typeof(int))
            {
                args[i] = 0;
                continue;
            }

            if (parameterType == typeof(float))
            {
                args[i] = 0f;
                continue;
            }

            if (parameterType.IsEnum)
            {
                Array values = Enum.GetValues(parameterType);
                args[i] = values.Length > 0 ? values.GetValue(0) : Activator.CreateInstance(parameterType);
                continue;
            }

            if (parameterType.IsValueType)
            {
                args[i] = Activator.CreateInstance(parameterType);
                continue;
            }

            // Algumas versões podem receber player/contexto. Tenta null nesses casos.
            args[i] = null;
        }

        return parameters.Any(parameter => parameter.ParameterType == typeof(string));
    }

    private object FindValheimConsole()
    {
        try
        {
            Type consoleType = FindTypeByName("Console");
            if (consoleType == null) return null;

            object instance =
                GetStaticMember(consoleType, "instance") ??
                GetStaticMember(consoleType, "m_instance") ??
                GetStaticMember(consoleType, "s_instance") ??
                GetStaticMember(consoleType, "m_console") ??
                GetStaticMember(consoleType, "ConsoleInstance") ??
                InvokeStaticNoArgs(consoleType, "GetInstance");

            if (instance != null) return instance;

            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (type == consoleType || string.Equals(type.Name, "Console", StringComparison.OrdinalIgnoreCase))
                {
                    return behaviour;
                }
            }
        }
        catch { }

        return null;
    }

    private static int StableHashCode(string text)
    {
        unchecked
        {
            int hash = 5381;
            string value = text ?? string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                hash = ((hash << 5) + hash) ^ value[i];
            }
            return hash;
        }
    }

    private static Type FindTypeByName(string shortName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types = new Type[0];
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }
            catch { }

            foreach (Type type in types)
            {
                if (type == null) continue;
                if (string.Equals(type.Name, shortName, StringComparison.OrdinalIgnoreCase)) return type;
                if (string.Equals(type.FullName, shortName, StringComparison.OrdinalIgnoreCase)) return type;
            }
        }
        return null;
    }

    private static object GetStaticMember(Type type, string name)
    {
        try
        {
            FieldInfo f = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (f != null) return f.GetValue(null);

            PropertyInfo p = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (p != null && p.CanRead) return p.GetValue(null, null);
        }
        catch { }
        return null;
    }

    private static object InvokeStaticNoArgs(Type type, string name)
    {
        try
        {
            MethodInfo m = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, Type.EmptyTypes, null);
            return m != null ? m.Invoke(null, null) : null;
        }
        catch { return null; }
    }

    private void AddGameChatMessage(string message)
    {
        Color prefixColor = new Color(0.84f, 0.42f, 1f, 1f);
        Color actorColor = new Color(1f, 0.88f, 0.41f, 1f);
        Color messageColor = Color.white;

        string prefixText = "[LivePlay]";
        string actorText = "LivePlay:";
        string bodyText = string.IsNullOrWhiteSpace(message) ? "LivePlay Chat" : message.Trim();

        Match full = Regex.Match(bodyText, "^#?([0-9a-fA-F]{6})\\|#?([0-9a-fA-F]{6})\\|#?([0-9a-fA-F]{6})\\|([^|]*)\\|([^|]*)\\|(.*)$", RegexOptions.Singleline);
        Match shortPayload = Regex.Match(bodyText, "^#?([0-9a-fA-F]{6})\\|#?([0-9a-fA-F]{6})\\|([^|]*)\\|([^|]*)\\|(.*)$", RegexOptions.Singleline);

        if (full.Success)
        {
            prefixColor = ParseHexColor(full.Groups[1].Value, prefixColor);
            actorColor = ParseHexColor(full.Groups[2].Value, actorColor);
            messageColor = ParseHexColor(full.Groups[3].Value, messageColor);
            prefixText = full.Groups[4].Value.Trim();
            actorText = full.Groups[5].Value.Trim();
            bodyText = full.Groups[6].Value.Trim();
        }
        else if (shortPayload.Success)
        {
            actorColor = ParseHexColor(shortPayload.Groups[1].Value, actorColor);
            messageColor = ParseHexColor(shortPayload.Groups[2].Value, messageColor);
            prefixText = shortPayload.Groups[3].Value.Trim();
            actorText = shortPayload.Groups[4].Value.Trim();
            bodyText = shortPayload.Groups[5].Value.Trim();
        }
        else
        {
            int colon = bodyText.IndexOf(':');
            if (colon > 0)
            {
                actorText = bodyText.Substring(0, colon + 1).Trim();
                bodyText = bodyText.Substring(colon + 1).Trim();
            }
        }

        prefixText = SanitizeGameChatMessage(prefixText);
        actorText = SanitizeGameChatMessage(actorText);
        bodyText = SanitizeGameChatMessage(bodyText);

        if (string.IsNullOrWhiteSpace(prefixText)) prefixText = "[LivePlay]";
        if (string.IsNullOrWhiteSpace(actorText)) actorText = "LivePlay:";
        if (string.IsNullOrWhiteSpace(bodyText)) bodyText = "LivePlay Chat";
        if (PrefixShouldBeHiddenForLiveChat(prefixText, actorText)) prefixText = string.Empty;

        float now = Time.realtimeSinceStartup;
        _gameChatLines.RemoveAll(line => line == null || line.ExpiresAt <= now);
        _gameChatLines.Add(new GameChatLine
        {
            PrefixText = prefixText,
            ActorText = actorText,
            MessageText = bodyText,
            PrefixColor = prefixColor,
            ActorColor = actorColor,
            MessageColor = messageColor,
            ExpiresAt = now + GameChatDurationSeconds
        });

        while (_gameChatLines.Count > GameChatMaxLines)
        {
            _gameChatLines.RemoveAt(0);
        }
    }

    private void DrawGameChat()
    {
        float now = Time.realtimeSinceStartup;
        _gameChatLines.RemoveAll(line => line == null || line.ExpiresAt <= now);
        if (_gameChatLines.Count == 0) return;

        float width = 520f;
        float lineHeight = 42f;
        float labelHeight = 40f;
        float paddingTop = 8f;
        float paddingBottom = 11f;
        float height = (_gameChatLines.Count * lineHeight) + paddingTop + paddingBottom;
        Rect rect = new Rect(8f, Screen.height - height - 126f, width, height);

        Color old = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.45f);
        GUI.Box(rect, GUIContent.none);

        float y = rect.y + paddingTop;
        foreach (GameChatLine line in _gameChatLines)
        {
            if (line == null) continue;

            float x = rect.x + 6f;
            float maxRight = rect.x + rect.width - 12f;

            if (!string.IsNullOrWhiteSpace(line.PrefixText))
            {
                GUI.color = line.PrefixColor;
                GUI.Label(new Rect(x, y, maxRight - x, labelHeight), line.PrefixText, _messageStyle);
                x += _messageStyle.CalcSize(new GUIContent(line.PrefixText + " ")).x;
            }

            GUI.color = line.ActorColor;
            GUI.Label(new Rect(x, y, maxRight - x, labelHeight), line.ActorText, _messageStyle);
            x += _messageStyle.CalcSize(new GUIContent(line.ActorText + " ")).x;

            GUI.color = line.MessageColor;
            GUI.Label(new Rect(x, y, Mathf.Max(80f, maxRight - x), labelHeight), line.MessageText, _messageStyle);

            y += lineHeight;
        }

        GUI.color = old;
    }

    private static Color ParseHexColor(string hex, Color fallback)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            hex = hex.Trim().TrimStart('#');
            if (!Regex.IsMatch(hex, "^[0-9a-fA-F]{6}$")) return fallback;

            int r = Convert.ToInt32(hex.Substring(0, 2), 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);
            return new Color(r / 255f, g / 255f, b / 255f, 1f);
        }
        catch { return fallback; }
    }

    private static bool PrefixShouldBeHiddenForLiveChat(string prefixText, string actorText)
    {
        string prefix = (prefixText ?? string.Empty).Trim();
        string actor = (actorText ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(prefix)) return false;
        if (!string.Equals(prefix, "[LivePlay]", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(prefix, "LivePlay", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !actor.StartsWith("LivePlay", StringComparison.OrdinalIgnoreCase);
    }

    private static string SanitizeGameChatMessage(string value)
    {
        string text = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        text = Regex.Replace(text, @"[\r\n\t]+", " ");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        if (text.Length > 150) text = text.Substring(0, 150);
        return text;
    }
}
