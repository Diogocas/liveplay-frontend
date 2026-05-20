using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UObject = UnityEngine.Object;

[BepInPlugin("br.liveplay.sonsforest.bridge", "LivePlay Sons Forest Bridge", "0.4.38")]
public sealed class LivePlaySonsForestBridge : BasePlugin
{
    private static LivePlaySonsForestBridge? Instance;

    private readonly ConcurrentQueue<PendingLivePlayCommand> _pendingCommands = new ConcurrentQueue<PendingLivePlayCommand>();
    private TcpListener? _listener;
    private Thread? _serverThread;
    private volatile bool _running;
    private int _port = 35952;
    private ManualLogSource? _log;
    private readonly ConcurrentQueue<LivePlayNotification> _notificationQueue = new ConcurrentQueue<LivePlayNotification>();
    private LivePlayNotification? _currentNotification;
    private float _currentNotificationTimeLeft;
    private GUIStyle? _notificationTitleStyle;
    private GUIStyle? _notificationMessageStyle;
    private GUIStyle? _notificationSubMessageStyle;
    private readonly List<PendingWorldLabelRequest> _pendingWorldLabelRequests = new List<PendingWorldLabelRequest>();
    private readonly List<LivePlayWorldLabel> _worldLabels = new List<LivePlayWorldLabel>();
    private GUIStyle? _worldLabelStyle;
    private readonly List<LivePlayGameChatLine> _gameChatLines = new List<LivePlayGameChatLine>();
    private GUIStyle? _gameChatTitleStyle;
    private GUIStyle? _gameChatMessageStyle;
    private const int GameChatMaxLines = 6;
    private const float GameChatDurationSeconds = 11f;



    public override void Load()
    {
        Instance = this;
        _log = Log;
        LoadConfig();
        try
        {
            AddComponent<LivePlaySonsForestDispatcher>();
            _log.LogInfo("LivePlay dispatcher registrado no Unity main thread.");
        }
        catch (Exception ex)
        {
            _log.LogWarning("Falha ao registrar dispatcher. Alguns comandos podem não executar no jogo: " + ex.Message);
        }
        StartServer();
        _log.LogInfo($"LivePlay Sons Forest Bridge ativo na porta {_port}");
    }

    private void LoadConfig()
    {
        try
        {
            string configPath = Path.Combine(Paths.ConfigPath, "LivePlaySonsForestBridge.json");
            if (!File.Exists(configPath)) return;

            string raw = File.ReadAllText(configPath);
            Match port = Regex.Match(raw, "\\\"bridgePort\\\"\\s*:\\s*(\\d+)");
            if (port.Success && int.TryParse(port.Groups[1].Value, out int parsed) && parsed > 0)
            {
                _port = parsed;
            }
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao ler config LivePlay: " + ex.Message);
        }
    }

    private void StartServer()
    {
        try
        {
            _running = true;
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
            _listener.Start();

            _serverThread = new Thread(ServerLoop)
            {
                IsBackground = true,
                Name = "LivePlaySonsForestBridgeHttp"
            };
            _serverThread.Start();
        }
        catch (Exception ex)
        {
            _log?.LogError("Erro ao iniciar servidor local LivePlay: " + ex);
        }
    }

    private void ServerLoop()
    {
        while (_running)
        {
            try
            {
                if (_listener == null) return;
                using TcpClient client = _listener.AcceptTcpClient();
                HandleClient(client);
            }
            catch (SocketException)
            {
                if (_running) Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                _log?.LogWarning("Falha no loop HTTP LivePlay: " + ex.Message);
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            client.ReceiveTimeout = 1500;
            client.SendTimeout = 1500;

            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[8192];
            int read = stream.Read(buffer, 0, buffer.Length);
            if (read <= 0) return;

            string request = Encoding.UTF8.GetString(buffer, 0, read);
            string firstLine = request.Split('\n')[0].Trim();
            bool isPing = firstLine.Contains("/liveplay/sonsforest/ping", StringComparison.OrdinalIgnoreCase);
            bool isCommand = firstLine.Contains("/liveplay/sonsforest/command", StringComparison.OrdinalIgnoreCase);

            if (!isPing && !isCommand)
            {
                WriteJson(stream, 404, "{\"ok\":false,\"error\":\"not_found\"}");
                return;
            }

            string command = ExtractJsonString(request, "command");
            if (isCommand && !string.IsNullOrWhiteSpace(command))
            {
                var normalized = NormalizeLivePlayCommand(command.Trim());
                if (normalized.Length == 0)
                {
                    WriteJson(stream, 400, "{\"ok\":false,\"error\":\"empty_command\",\"bridge\":\"sons-forest\",\"version\":\"0.4.38\"}");
                    return;
                }

                if (!IsSupportedCommand(normalized))
                {
                    _log?.LogWarning("Comando não suportado pelo bridge Sons Forest: " + normalized);
                    WriteJson(stream, 400, "{\"ok\":false,\"error\":\"unsupported_command\",\"bridge\":\"sons-forest\",\"version\":\"0.4.38\"}");
                    return;
                }

                _pendingCommands.Enqueue(new PendingLivePlayCommand(normalized, ExtractViewerName(request)));
                WriteJson(stream, 200, "{\"ok\":true,\"queued\":true,\"bridge\":\"sons-forest\",\"version\":\"0.4.38\"}");
                return;
            }

            WriteJson(stream, 200, "{\"ok\":true,\"bridge\":\"sons-forest\",\"version\":\"0.4.38\"}");
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao processar requisição LivePlay: " + ex.Message);
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
        catch
        {
            return string.Empty;
        }
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
        string statusText = status == 200 ? "OK" : status == 400 ? "Bad Request" : "Not Found";
        string head = $"HTTP/1.1 {status} {statusText}\r\nContent-Type: application/json; charset=utf-8\r\nContent-Length: {bodyBytes.Length}\r\nConnection: close\r\n\r\n";
        byte[] headBytes = Encoding.ASCII.GetBytes(head);
        stream.Write(headBytes, 0, headBytes.Length);
        stream.Write(bodyBytes, 0, bodyBytes.Length);
    }

    private static string NormalizeLivePlayCommand(string raw)
    {
        string clean = (raw ?? string.Empty).Trim();
        if (clean.StartsWith("sotf:chat ", StringComparison.OrdinalIgnoreCase) ||
            clean.StartsWith("chat ", StringComparison.OrdinalIgnoreCase))
        {
            return clean;
        }

        return clean.ToLowerInvariant();
    }

    private static bool IsSupportedCommand(string command)
    {
        if (command.StartsWith("console:", StringComparison.OrdinalIgnoreCase)) return true;
        if (command.StartsWith("sotf:", StringComparison.OrdinalIgnoreCase)) return true;
        switch (command)
        {
            case "ping":
            case "app-open":
            case "spawn_cannibal":
            case "spawn_mutant":
            case "set_time_day":
            case "set_time_night":
            case "heal_player":
            case "damage_player":
            case "clear_enemies":
            case "spawn_fingers":
            case "spawn_twins":
            case "spawn_baby":
            case "spawn_demon":
            case "spawn_john2":
            case "spawn_heavy":
                return true;
            default:
                return false;
        }
    }

    internal void ProcessPendingCommands()
    {
        int safety = 0;
        while (safety++ < 8 && _pendingCommands.TryDequeue(out PendingLivePlayCommand pending))
        {
            ExecuteCommandOnMainThread(pending);
        }
    }

    private void ExecuteCommandOnMainThread(PendingLivePlayCommand pending)
    {
        string command = pending.Command;
        string viewerName = pending.ViewerName;
        if (command == "ping" || command == "app-open")
        {
            _log?.LogInfo("LivePlay ping recebido.");
            return;
        }

        if (TryHandleGameChatCommand(command))
        {
            return;
        }

        string consoleCommand = MapToSonsConsoleCommand(command);
        _log?.LogInfo("LivePlay command: " + command + " => " + consoleCommand);

        bool executed = TryExecuteSonsConsoleCommand(consoleCommand, command);
        if (executed)
        {
            _log?.LogInfo("Comando Sons Of The Forest enviado ao jogo: " + consoleCommand);
            QueueSuccessNotification(viewerName, command, consoleCommand);
        }
        else
        {
            _log?.LogWarning("Não foi possível executar o comando no jogo. Comando console tentado: " + consoleCommand);
        }
    }


    private bool TryHandleGameChatCommand(string command)
    {
        string raw = (command ?? string.Empty).Trim();
        if (raw.Length == 0) return false;

        string message = string.Empty;
        if (raw.StartsWith("sotf:chat ", StringComparison.OrdinalIgnoreCase))
        {
            message = raw.Substring("sotf:chat ".Length).Trim();
        }
        else if (raw.Equals("sotf:chat", StringComparison.OrdinalIgnoreCase))
        {
            message = "Mensagem de teste do LivePlay";
        }
        else if (raw.StartsWith("chat ", StringComparison.OrdinalIgnoreCase))
        {
            message = raw.Substring("chat ".Length).Trim();
        }
        else if (raw.Equals("chat", StringComparison.OrdinalIgnoreCase))
        {
            message = "Mensagem de teste do LivePlay";
        }
        else
        {
            return false;
        }

        LivePlayGameChatLine line = ParseGameChatPayload(message);
        AddGameChatLine(line);
        _log?.LogInfo("LivePlay chat no jogo: " + line.Text);
        return true;
    }



    private void AddGameChatMessage(string message, Color color)
    {
        string safe = SanitizeGameChatMessage(message);
        if (string.IsNullOrWhiteSpace(safe)) safe = "Mensagem de teste do LivePlay";
        AddGameChatLine(new LivePlayGameChatLine("[LivePlay]", new Color(0.84f, 0.42f, 1f, 1f), "LivePlay:", color, safe, Color.white, GameChatDurationSeconds));
    }

    private void AddGameChatLine(LivePlayGameChatLine line)
    {
        if (line == null) return;

        _gameChatLines.Add(line);
        while (_gameChatLines.Count > GameChatMaxLines)
        {
            _gameChatLines.RemoveAt(0);
        }
    }



    private static LivePlayGameChatLine ParseGameChatPayload(string message)
    {
        Color prefixColor = new Color(0.84f, 0.42f, 1f, 1f);
        Color actorColor = new Color(1f, 0.88f, 0.41f, 1f);
        Color messageColor = Color.white;

        string prefixText = "[LivePlay]";
        string actorText = "LivePlay:";
        string bodyText = string.IsNullOrWhiteSpace(message) ? string.Empty : message.Trim();

        Match full = Regex.Match(
            bodyText,
            "^#?([0-9a-fA-F]{6})\\|#?([0-9a-fA-F]{6})\\|#?([0-9a-fA-F]{6})\\|([^|]*)\\|([^|]*)\\|(.*)$",
            RegexOptions.Singleline
        );

        Match shortPayload = Regex.Match(
            bodyText,
            "^#?([0-9a-fA-F]{6})\\|#?([0-9a-fA-F]{6})\\|([^|]*)\\|([^|]*)\\|(.*)$",
            RegexOptions.Singleline
        );

        Match oldPayload = Regex.Match(
            bodyText,
            "^#?([0-9a-fA-F]{6})\\|(.*)$",
            RegexOptions.Singleline
        );

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
            // Formato que veio no seu teste:
            // #COR_NOME|#COR_MENSAGEM|[LivePlay]|LivePlay:|oi
            actorColor = ParseHexColor(shortPayload.Groups[1].Value, actorColor);
            messageColor = ParseHexColor(shortPayload.Groups[2].Value, messageColor);
            prefixText = shortPayload.Groups[3].Value.Trim();
            actorText = shortPayload.Groups[4].Value.Trim();
            bodyText = shortPayload.Groups[5].Value.Trim();
        }
        else if (oldPayload.Success)
        {
            actorColor = ParseHexColor(oldPayload.Groups[1].Value, actorColor);
            bodyText = oldPayload.Groups[2].Value.Trim();

            int colon = bodyText.IndexOf(':');
            if (colon > 0)
            {
                actorText = bodyText.Substring(0, colon + 1).Trim();
                bodyText = bodyText.Substring(colon + 1).Trim();
            }
        }
        else
        {
            int colon = bodyText.IndexOf(':');
            if (colon > 0)
            {
                actorText = bodyText.Substring(0, colon + 1).Trim();
                bodyText = bodyText.Substring(colon + 1).Trim();
            }
            else
            {
                actorColor = GetGameChatColor(bodyText);
            }
        }

        prefixText = SanitizeGameChatMessage(prefixText);
        actorText = SanitizeGameChatMessage(actorText);
        bodyText = SanitizeGameChatMessage(bodyText);

        if (string.IsNullOrWhiteSpace(prefixText)) prefixText = "[LivePlay]";
        if (string.IsNullOrWhiteSpace(actorText)) actorText = "LivePlay:";
        if (string.IsNullOrWhiteSpace(bodyText)) bodyText = "Mensagem de teste do LivePlay";

        return new LivePlayGameChatLine(
            prefixText,
            prefixColor,
            actorText,
            actorColor,
            bodyText,
            messageColor,
            GameChatDurationSeconds
        );
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
        catch
        {
            return fallback;
        }
    }

    private static string SanitizeGameChatMessage(string value)
    {
        string text = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        text = Regex.Replace(text, @"[\r\n\t]+", " ");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        if (text.Length > 150) text = text.Substring(0, 150);
        return text;
    }

    private static Color GetGameChatColor(string message)
    {
        string lower = (message ?? string.Empty).ToLowerInvariant();

        if (lower.Contains("presente") || lower.Contains("gift") || lower.Contains("enviou"))
        {
            return new Color(1f, 0.78f, 0.22f, 1f);
        }

        if (lower.Contains("seguiu") || lower.Contains("follow"))
        {
            return new Color(0.32f, 0.92f, 0.62f, 1f);
        }

        if (lower.Contains("compartilhou") || lower.Contains("share"))
        {
            return new Color(0.38f, 0.72f, 1f, 1f);
        }

        if (lower.Contains("sub") || lower.Contains("inscreveu") || lower.Contains("assinou"))
        {
            return new Color(0.74f, 0.55f, 1f, 1f);
        }

        return Color.white;
    }

    private void UpdateGameChat()
    {
        if (_gameChatLines.Count == 0) return;

        float dt = Time.unscaledDeltaTime;
        for (int index = _gameChatLines.Count - 1; index >= 0; index--)
        {
            _gameChatLines[index].TimeLeft -= dt;
            if (_gameChatLines[index].TimeLeft <= 0f)
            {
                _gameChatLines.RemoveAt(index);
            }
        }
    }




    private void DrawGameChat()
    {
        if (_gameChatLines.Count == 0) return;
        EnsureGameChatStyles();

        float width = 520f;
        float lineHeight = 23f;
        float labelHeight = 24f;
        float paddingTop = 8f;
        float paddingBottom = 11f;
        float height = (_gameChatLines.Count * lineHeight) + paddingTop + paddingBottom;
        float x = 8f;
        float y = Screen.height - height - 126f;

        if (y < 20f) y = 20f;

        Color previousColor = GUI.color;

        Rect bg = new Rect(x, y, width, height);
        GUI.color = new Color(0f, 0f, 0f, 0.45f);
        GUI.Box(bg, GUIContent.none);

        float lineY = y + paddingTop;
        for (int i = 0; i < _gameChatLines.Count; i++)
        {
            LivePlayGameChatLine line = _gameChatLines[i];
            float fade = line.TimeLeft < 0.6f ? Mathf.Clamp01(line.TimeLeft / 0.6f) : 1f;
            float textX = x + 6f;
            float textY = lineY + (i * lineHeight);

            GUI.color = new Color(line.PrefixColor.r, line.PrefixColor.g, line.PrefixColor.b, fade);
            GUI.Label(new Rect(textX, textY, width - 12f, labelHeight), line.PrefixText, _gameChatMessageStyle);
            textX += _gameChatMessageStyle.CalcSize(new GUIContent(line.PrefixText + " ")).x;

            GUI.color = new Color(line.ActorColor.r, line.ActorColor.g, line.ActorColor.b, fade);
            GUI.Label(new Rect(textX, textY, width - 12f, labelHeight), line.ActorText, _gameChatMessageStyle);
            textX += _gameChatMessageStyle.CalcSize(new GUIContent(line.ActorText + " ")).x;

            GUI.color = new Color(line.MessageColor.r, line.MessageColor.g, line.MessageColor.b, fade);
            GUI.Label(new Rect(textX, textY, width - 12f, labelHeight), line.MessageText, _gameChatMessageStyle);
        }

        GUI.color = previousColor;
    }




    private void EnsureGameChatStyles()
    {
        if (_gameChatTitleStyle != null && _gameChatMessageStyle != null) return;

        _gameChatTitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            richText = false
        };
        _gameChatTitleStyle.normal.textColor = new Color(0.70f, 0.88f, 1f, 1f);

        _gameChatMessageStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            richText = false,
            wordWrap = false
        };
        _gameChatMessageStyle.normal.textColor = Color.white;
    }

    private string MapToSonsConsoleCommand(string command)
    {
        if (command.StartsWith("console:", StringComparison.OrdinalIgnoreCase))
            return command.Substring("console:".Length).Trim();
        if (command.StartsWith("sotf:", StringComparison.OrdinalIgnoreCase))
            return command.Substring("sotf:".Length).Trim();

        switch (command)
        {
            case "spawn_cannibal":
                return "addcharacter cannibal 1";
            case "spawn_mutant":
                // Sons Of The Forest does not have a generic `mutant` addcharacter target.
                // Fingers is a reliable mutant enemy target for the generic LivePlay mutant preset.
                return "addcharacter fingers 1";
            case "spawn_fingers":
                return "addcharacter fingers 1";
            case "spawn_twins":
                return "addcharacter twins 1";
            case "spawn_baby":
                return "addcharacter baby 1";
            case "spawn_demon":
                return "addcharacter demon 1";
            case "spawn_john2":
                return "addcharacter john2 1";
            case "spawn_heavy":
                return "addcharacter heavy 1";
            case "set_time_day":
                return "settimeofday 12";
            case "set_time_night":
                return "settimeofday 23";
            case "heal_player":
                return "regenhealth";
            case "damage_player":
                return "damageplayer 25";
                return "forcerain 5";
            case "clear_enemies":
                return "killradius 40";
            default:
                return command;
        }
    }

    private bool TryExecuteSonsConsoleCommand(string consoleCommand, string livePlayCommand)
    {
        if (string.IsNullOrWhiteSpace(consoleCommand)) return false;

        bool isCustomConsoleCommand = livePlayCommand.StartsWith("console:", StringComparison.OrdinalIgnoreCase)
            || livePlayCommand.StartsWith("sotf:", StringComparison.OrdinalIgnoreCase);

        // Não procurar VailDebugConsole antes do spawn.
        // A busca antiga varria todos os MonoBehaviours e causava travada antes de invocar inimigos/amigos.
        // O comando segue pela rota direta do TheForest.DebugConsole abaixo.
        if (consoleCommand.StartsWith("addcharacter", StringComparison.OrdinalIgnoreCase))
        {
            consoleCommand = "addcharacter " + NormalizeAddCharacterArgs(consoleCommand.Substring("addcharacter".Length).Trim());
        }

        // Preferred local path: call TheForest.DebugConsole methods directly.
        // For official LivePlay commands, do not call HandleConsoleInput; it is what causes the
        // visible in-game COMMAND FAILED message. Raw console:/sotf: commands may still use it.
        if (TryTheForestDebugConsoleCommand(consoleCommand, isCustomConsoleCommand)) return true;

        // Last fallback only for raw custom commands. It can trigger the game's visible command UI.
        if (isCustomConsoleCommand && TryRaiseBoltDebugCommand(consoleCommand)) return true;

        return false;
    }

    private bool TryTheForestDebugConsoleCommand(string consoleCommand, bool allowConsoleInputFallback)
    {
        try
        {
            Type? debugConsoleType = FindType("TheForest.DebugConsole") ?? FindType("DebugConsole");
            if (debugConsoleType == null)
            {
                _log?.LogWarning("TheForest.DebugConsole não encontrado nos assemblies carregados.");
                return false;
            }

            TryInvokeStatic(debugConsoleType, "SetCheatsAllowed", new object[] { true });
            TryInvokeStatic(debugConsoleType, "CheatsAllowedSet", new object[] { true });

            object? console = InvokeStaticNoArgs(debugConsoleType, "GetInstance");
            if (console == null)
            {
                console = FindMonoBehaviourByTypeName("TheForest.DebugConsole") ?? FindMonoBehaviourByShortName("DebugConsole");
            }

            if (console == null)
            {
                _log?.LogWarning("TheForest.DebugConsole existe, mas nenhuma instância foi encontrada. O menu/jogo talvez ainda não terminou de carregar.");
                return false;
            }

            // Keep the console unblocked, but do not send the literal cheatstick command here.
            // Sending cheatstick through HandleConsoleInput causes the visible COMMAND FAILED overlay.
            TryInvokeInstanceAny(console, "SetBlockConsole", new object[] { false });

            if (TryInvokeDirectDebugConsoleMethod(console, consoleCommand))
            {
                _log?.LogInfo("Comando executado via método direto TheForest.DebugConsole: " + consoleCommand);
                return true;
            }

            if (allowConsoleInputFallback && TryInvokeInstanceAny(console, "HandleConsoleInput", new object[] { consoleCommand }))
            {
                _log?.LogInfo("Comando executado via TheForest.DebugConsole.HandleConsoleInput: " + consoleCommand);
                return true;
            }

            _log?.LogWarning("TheForest.DebugConsole encontrado, mas não aceitou o comando sem usar fallback visual: " + consoleCommand);
            return false;
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao executar TheForest.DebugConsole: " + ex);
            return false;
        }
    }

    private bool TryInvokeDirectDebugConsoleMethod(object console, string consoleCommand)
    {
        string command = consoleCommand.Trim();
        string lower = command.ToLowerInvariant();

        if (lower.StartsWith("addammo", StringComparison.Ordinal))
        {
            string args = command.Substring("addammo".Length).Trim();
            return TrySetAmmoExact(console, args);
        }

        if (lower.StartsWith("addcharacter", StringComparison.Ordinal))
        {
            string args = NormalizeAddCharacterArgs(command.Substring("addcharacter".Length).Trim());
            string argsWithoutCount = RemoveTrailingCount(args);

            return TryInvokeStaticAny(console.GetType(), "TryRunDynamicCommand", new object[] { "addcharacter", args, console })
                || (!string.Equals(argsWithoutCount, args, StringComparison.OrdinalIgnoreCase) && TryInvokeStaticAny(console.GetType(), "TryRunDynamicCommand", new object[] { "addcharacter", argsWithoutCount, console }))
                || TryInvokeInstanceAny(console, "DebugCommandAddCharacter", new object[] { args })
                || (!string.Equals(argsWithoutCount, args, StringComparison.OrdinalIgnoreCase) && TryInvokeInstanceAny(console, "DebugCommandAddCharacter", new object[] { argsWithoutCount }))
                || TryInvokeInstanceAny(console, "DebugAddCharacter", new object[] { args, true })
                || TryInvokeInstanceAny(console, "DebugAddCharacter", new object[] { args, false });
        }

        if (lower.StartsWith("settimeofday", StringComparison.Ordinal))
        {
            string args = command.Substring("settimeofday".Length).Trim();
            return TryInvokeInstanceAny(console, "TrySetTimeOfDay", new object[] { args })
                || TryInvokeInstanceAny(console, "_setTimeOfDay", new object[] { args });
        }

        if (lower.StartsWith("forcerain", StringComparison.Ordinal))
        {
            string args = command.Substring("forcerain".Length).Trim();
            return TryInvokeInstanceAny(console, "_forcerain", new object[] { args })
                || TryInvokeInstanceAny(console, "_forcerain", new object[] { "heavy" })
                || TryInvokeInstanceAny(console, "ForceRain", new object[] { 5 })
                || TryInvokeInstanceAny(console, "ForceRain", new object[] { 4 })
                || TryInvokeInstanceAny(console, "ForceRain", new object[] { 2 });
        }

        if (lower.StartsWith("setstat", StringComparison.Ordinal))
        {
            string args = command.Substring("setstat".Length).Trim();
            return TryInvokeStaticAny(console.GetType(), "TryRunDynamicCommand", new object[] { "setstat", args, console })
                || TryInvokeInstanceAny(console, "_setstat", new object[] { args })
                || TryInvokeInstanceAny(console, "SetStat", new object[] { args });
        }

        if (lower.StartsWith("heallocalplayer", StringComparison.Ordinal) || lower.StartsWith("regenhealth", StringComparison.Ordinal) || lower.StartsWith("buffstats", StringComparison.Ordinal))
        {
            bool healed = false;
            // Try the visible health refill first, then vitals/stamina refill. Some builds accept
            // heallocalplayer without changing visible stats, while regenhealth/buffstats are reliable.
            healed |= TryInvokeInstanceAny(console, "_regenhealth", new object[] { "" });
            healed |= TryInvokeInstanceAny(console, "_regenhealth", new object[] { "100" });
            healed |= TryInvokeInstanceAny(console, "BuffStats", new object[] { "" });
            healed |= TryInvokeInstanceAny(console, "BuffStats", new object[] { "100" });
            healed |= TryInvokeInstanceAny(console, "_heallocalplayer", new object[] { "" });
            healed |= TryInvokeInstanceAny(console, "_healLocalPlayer", new object[] { "" });
            healed |= TryInvokeInstanceAny(console, "HealLocalPlayer", new object[] { "" });
            return healed;
        }

        if (lower.StartsWith("killradius", StringComparison.Ordinal))
        {
            string args = command.Substring("killradius".Length).Trim();
            return TryInvokeInstanceAny(console, "_killRadius", new object[] { args });
        }

        if (lower.StartsWith("damage", StringComparison.Ordinal))
        {
            string args = command.Contains(' ') ? command.Substring(command.IndexOf(' ') + 1).Trim() : "25";
            return TryInvokeInstanceAny(console, "_damageDebug", new object[] { args })
                || TryInvokeInstanceAny(console, "_damageRadius", new object[] { args });
        }

        return false;
    }

    private bool TrySetAmmoExact(object console, string rawArgs)
    {
        try
        {
            string args = (rawArgs ?? string.Empty).Trim();
            string[] parts = Regex.Split(args, @"\s+")
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();

            if (parts.Length < 2)
            {
                _log?.LogWarning("addammo exige itemId e quantidade. Exemplo: addammo 362 10");
                return false;
            }

            if (!int.TryParse(parts[0], out int itemId) || !int.TryParse(parts[1], out int amount))
            {
                _log?.LogWarning("addammo recebeu argumentos inválidos: " + args);
                return false;
            }

            if (amount <= 0) amount = 1;
            if (amount > 50) amount = 50;

            // Do not call DebugConsole additem here. For ammo, that route ignores quantity and creates huge stacks.
            if (TryPlayerInventoryAddItemExact(itemId, amount))
            {
                _log?.LogInfo("Munição adicionada via PlayerInventory: " + itemId + " x" + amount);
                return true;
            }

            if (TryTheForestModdingBridgeAddItem(itemId, amount))
            {
                _log?.LogInfo("Munição adicionada via TheForest.Modding.Bridge: " + itemId + " x" + amount);
                return true;
            }

            _log?.LogWarning("Não foi possível adicionar munição exata sem usar DebugConsole additem: " + itemId + " x" + amount);
            return false;
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao executar addammo: " + ex.Message);
            return false;
        }
    }

    private bool TryPlayerInventoryAddItemExact(int itemId, int amount)
    {
        try
        {
            object? inventory = FindLocalPlayerInventory();
            if (inventory == null)
            {
                _log?.LogWarning("PlayerInventory não encontrado para addammo.");
                return false;
            }

            // Prefer AddItem overloads before TryAddItems.
            // TryAddItems is reliable but usually silent. AddItem overloads are more likely
            // to trigger the game's normal "item received" feedback while still adding only once.

            if (TryInvokeInstanceConverted(inventory, "AddItem", new object[] { itemId, amount, true, true, null })) return true;
            if (TryInvokeInstanceConverted(inventory, "AddItem", new object[] { itemId, amount, true, false, null })) return true;
            if (TryInvokeInstanceConverted(inventory, "AddItem", new object[] { itemId, amount, false, true, null })) return true;
            if (TryInvokeInstanceConverted(inventory, "AddItem", new object[] { itemId, amount, false, false, null })) return true;

            if (TryInvokeInstanceConverted(inventory, "AddItem", new object[] { itemId, amount, true })) return true;
            if (TryInvokeInstanceConverted(inventory, "AddItem", new object[] { itemId, amount, false })) return true;

            // Silent fallback if AddItem overloads are unavailable in this game build.
            if (TryInvokeInstanceConverted(inventory, "TryAddItems", new object[] { itemId, amount, true })) return true;
            if (TryInvokeInstanceConverted(inventory, "TryAddItems", new object[] { itemId, amount, false })) return true;

            if (TryInvokeInstanceConverted(inventory, "ClientRequestAddItems", new object[] { itemId, amount, null })) return true;

            return false;
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao usar PlayerInventory para addammo: " + ex.Message);
            return false;
        }
    }

    private object? FindLocalPlayerInventory()
    {
        try
        {
            Type? localPlayerType = FindType("TheForest.Utils.LocalPlayer") ?? FindType("LocalPlayer");
            if (localPlayerType != null)
            {
                object? fromLocalPlayer =
                    GetStaticMemberValue(localPlayerType, "Inventory") ??
                    GetStaticMemberValue(localPlayerType, "_inventory") ??
                    InvokeStaticNoArgs(localPlayerType, "get_Inventory") ??
                    InvokeStaticNoArgs(localPlayerType, "GetInventory");

                if (fromLocalPlayer != null) return fromLocalPlayer;
            }
        }
        catch
        {
        }

        return FindMonoBehaviourByTypeName("TheForest.Items.Inventory.PlayerInventory")
            ?? FindMonoBehaviourByShortName("PlayerInventory");
    }

    private bool TryTheForestModdingBridgeAddItem(int itemId, int amount)
    {
        try
        {
            Type? bridgeType = FindType("TheForest.Modding.Bridge");
            if (bridgeType == null) return false;

            object? cheats =
                GetStaticMemberValue(bridgeType, "Cheats") ??
                InvokeStaticNoArgs(bridgeType, "get_Cheats");

            if (cheats == null) return false;

            if (TryInvokeInstanceConverted(cheats, "AddItem", new object[] { itemId, amount, true })) return true;
            if (TryInvokeInstanceConverted(cheats, "AddItem", new object[] { itemId, amount, false })) return true;
            if (TryInvokeInstanceConverted(cheats, "AddItem", new object[] { itemId, amount })) return true;

            return false;
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao usar TheForest.Modding.Bridge.AddItem: " + ex.Message);
            return false;
        }
    }

    private bool TryForceRainDirect()
    {
        try
        {
            // The DebugConsole _forcerain command can return successfully without forcing weather
            // in some scenes/biomes. Calling TheForest.World.WeatherSystem directly is cleaner and
            // does not show the visible COMMAND FAILED console overlay.
            object? weather = FindMonoBehaviourByTypeName("TheForest.World.WeatherSystem")
                ?? FindMonoBehaviourByShortName("WeatherSystem");

            if (weather == null)
            {
                _log?.LogWarning("WeatherSystem não encontrado para toggle_storm.");
                return false;
            }

            bool ok = false;
            ok |= TryInvokeInstanceAny(weather, "ForceRain", new object[] { 5 });
            ok |= TryInvokeInstanceAny(weather, "ForceRain", new object[] { 4 });
            ok |= TryInvokeInstanceAny(weather, "ForceRain", new object[] { 2 });
            ok |= TryInvokeInstanceAny(weather, "ForceRain", new object[] { 1 });
            ok |= TryInvokeInstanceAny(weather, "StartRaining", Array.Empty<object>());

            if (ok)
            {
                _log?.LogInfo("Temporal executado via TheForest.World.WeatherSystem.");
            }
            return ok;
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao forçar chuva pelo WeatherSystem: " + ex.Message);
            return false;
        }
    }

    private bool TryRaiseBoltDebugCommand(string consoleCommand)
    {
        try
        {
            Type? debugType = FindType("debugCommand") ?? FindType("bolt.user.debugCommand");
            if (debugType == null)
            {
                _log?.LogWarning("Tipo debugCommand não encontrado.");
                return false;
            }

            object? evt = InvokeStaticNoArgs(debugType, "Create") ?? InvokeStaticNoArgs(debugType, "Raise");
            if (evt == null)
            {
                _log?.LogWarning("Não foi possível criar evento debugCommand.");
                return false;
            }

            SetMember(evt, "Command", consoleCommand);
            SetMember(evt, "AllowCheats", true);

            bool sent = InvokeInstanceNoArgs(evt, "Send") || InvokeInstanceNoArgs(evt, "Raise");
            if (!sent)
            {
                _log?.LogWarning("debugCommand criado, mas método Send/Raise não foi encontrado.");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao usar debugCommand: " + ex.Message);
            return false;
        }
    }

    private bool TryVailDebugConsoleAddCharacter(string addCharacterArgs)
    {
        try
        {
            // In BepInEx IL2CPP, UnityEngine.Object.FindObjectsOfType(Type) expects
            // Il2CppSystem.Type, not System.Type. Use a generic MonoBehaviour scan
            // and filter by runtime type name to avoid System.Type -> Il2CppSystem.Type
            // conversion errors during compilation.
            var behaviours = UObject.FindObjectsOfType<MonoBehaviour>();
            if (behaviours == null || behaviours.Length == 0)
            {
                _log?.LogWarning("Nenhum MonoBehaviour encontrado para procurar VailDebugConsole.");
                return false;
            }

            bool foundConsole = false;

            foreach (var behaviour in behaviours)
            {
                if (behaviour == null) continue;

                string typeName = behaviour.GetType().FullName ?? behaviour.GetType().Name ?? string.Empty;
                if (typeName != "Sons.Ai.Vail.VailDebugConsole" && !typeName.EndsWith(".VailDebugConsole", StringComparison.Ordinal))
                {
                    continue;
                }

                foundConsole = true;

                // Try SpawnActor(string) first because interop commonly exposes SpawnActor_Public_Void_String.
                if (InvokeInstance(behaviour, "SpawnActor", new object[] { addCharacterArgs })) return true;

                // Then try TryAddCharacters(string, bool).
                if (InvokeInstance(behaviour, "TryAddCharacters", new object[] { addCharacterArgs, true })) return true;
                if (InvokeInstance(behaviour, "TryAddCharacters", new object[] { addCharacterArgs, false })) return true;
            }

            if (!foundConsole)
            {
                _log?.LogWarning("Nenhuma instância VailDebugConsole encontrada.");
            }
            else
            {
                _log?.LogWarning("VailDebugConsole encontrada, mas nenhum método compatível aceitou o comando.");
            }

            return false;
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha no fallback VailDebugConsole: " + ex.Message);
            return false;
        }
    }

    private static string RemoveTrailingCount(string args)
    {
        string clean = (args ?? string.Empty).Trim();
        if (clean.Length == 0) return clean;
        var parts = clean.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1) return clean;
        if (int.TryParse(parts[parts.Length - 1], out _))
        {
            return string.Join(" ", parts.Take(parts.Length - 1));
        }
        return clean;
    }

    private static MonoBehaviour? FindMonoBehaviourByTypeName(string fullName)
    {
        try
        {
            var behaviours = UObject.FindObjectsOfType<MonoBehaviour>();
            if (behaviours == null) return null;
            foreach (var behaviour in behaviours)
            {
                if (behaviour == null) continue;
                string name = behaviour.GetType().FullName ?? string.Empty;
                if (name == fullName) return behaviour;
            }
        }
        catch
        {
        }
        return null;
    }

    private static MonoBehaviour? FindMonoBehaviourByShortName(string shortName)
    {
        try
        {
            var behaviours = UObject.FindObjectsOfType<MonoBehaviour>();
            if (behaviours == null) return null;
            foreach (var behaviour in behaviours)
            {
                if (behaviour == null) continue;
                string name = behaviour.GetType().Name ?? string.Empty;
                if (name == shortName) return behaviour;
            }
        }
        catch
        {
        }
        return null;
    }

    private static Type? FindType(string fullOrShortName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                Type? direct = asm.GetType(fullOrShortName, false, false);
                if (direct != null) return direct;

                foreach (var type in asm.GetTypes())
                {
                    if (type.FullName == fullOrShortName || type.Name == fullOrShortName) return type;
                }
            }
            catch
            {
                // Some dynamically loaded IL2CPP assemblies can throw during GetTypes. Ignore and continue.
            }
        }
        return null;
    }

    private static object? InvokeStaticNoArgs(Type type, string methodName)
    {
        var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, Type.EmptyTypes, null);
        return method?.Invoke(null, Array.Empty<object>());
    }

    private static bool InvokeInstanceNoArgs(object target, string methodName)
    {
        var method = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
        if (method == null) return false;
        method.Invoke(target, Array.Empty<object>());
        return true;
    }

    private static bool InvokeInstance(object target, string methodName, object[] args)
    {
        foreach (var method in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (method.Name != methodName) continue;
            var parameters = method.GetParameters();
            if (parameters.Length != args.Length) continue;
            try
            {
                object? result = method.Invoke(target, args);
                if (method.ReturnType == typeof(bool)) return result is bool value && value;
                return true;
            }
            catch
            {
                // Try other overload.
            }
        }
        return false;
    }

    private static bool TryInvokeStatic(Type type, string methodName, object[] args)
    {
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (method.Name != methodName) continue;
            var parameters = method.GetParameters();
            if (parameters.Length != args.Length) continue;
            try
            {
                method.Invoke(null, args);
                return true;
            }
            catch
            {
            }
        }
        return false;
    }

    private static bool TryInvokeInstanceAny(object target, string methodName, object[] args)
    {
        foreach (var method in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (method.Name != methodName) continue;
            var parameters = method.GetParameters();
            if (parameters.Length != args.Length) continue;
            try
            {
                object? result = method.Invoke(target, args);
                if (method.ReturnType == typeof(bool)) return result is bool value && value;
                return true;
            }
            catch
            {
            }
        }
        return false;
    }

    private static bool TryInvokeStaticAny(Type type, string methodName, object[] args)
    {
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (method.Name != methodName) continue;
            var parameters = method.GetParameters();
            if (parameters.Length != args.Length) continue;
            try
            {
                object? result = method.Invoke(null, args);
                if (method.ReturnType == typeof(bool)) return result is bool value && value;
                return true;
            }
            catch
            {
            }
        }
        return false;
    }

    private static bool TryInvokeInstanceConverted(object target, string methodName, object[] args)
    {
        foreach (var method in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (method.Name != methodName) continue;

            var parameters = method.GetParameters();
            if (parameters.Length != args.Length) continue;
            if (!TryBuildConvertedArguments(parameters, args, out object?[] convertedArgs)) continue;

            try
            {
                object? result = method.Invoke(target, convertedArgs);
                if (method.ReturnType == typeof(bool)) return result is bool value && value;
                return true;
            }
            catch
            {
            }
        }

        return false;
    }

    private static bool TryBuildConvertedArguments(ParameterInfo[] parameters, object[] args, out object?[] convertedArgs)
    {
        convertedArgs = new object?[args.Length];

        for (int index = 0; index < args.Length; index++)
        {
            Type targetType = Nullable.GetUnderlyingType(parameters[index].ParameterType) ?? parameters[index].ParameterType;
            object? value = args[index];

            if (value == null)
            {
                if (targetType.IsValueType) return false;
                convertedArgs[index] = null;
                continue;
            }

            if (targetType.IsInstanceOfType(value))
            {
                convertedArgs[index] = value;
                continue;
            }

            string text = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)?.Trim() ?? string.Empty;

            try
            {
                if (targetType == typeof(string))
                {
                    convertedArgs[index] = text;
                    continue;
                }

                if (targetType == typeof(int))
                {
                    if (!int.TryParse(text, out int parsed)) return false;
                    convertedArgs[index] = parsed;
                    continue;
                }

                if (targetType == typeof(float))
                {
                    if (!float.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsed)) return false;
                    convertedArgs[index] = parsed;
                    continue;
                }

                if (targetType == typeof(double))
                {
                    if (!double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double parsed)) return false;
                    convertedArgs[index] = parsed;
                    continue;
                }

                if (targetType == typeof(bool))
                {
                    if (!bool.TryParse(text, out bool parsed)) return false;
                    convertedArgs[index] = parsed;
                    continue;
                }

                if (targetType.IsEnum)
                {
                    convertedArgs[index] = Enum.Parse(targetType, text, true);
                    continue;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        return true;
    }

    private static object? GetStaticMemberValue(Type type, string name)
    {
        try
        {
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (property != null) return property.GetValue(null);
        }
        catch
        {
        }

        try
        {
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null) return field.GetValue(null);
        }
        catch
        {
        }

        return null;
    }

    private void QueueSuccessNotification(string viewerName, string livePlayCommand, string consoleCommand)
    {
        try
        {
            if (!TryBuildNotification(viewerName, livePlayCommand, consoleCommand, out LivePlayNotification? notification) || notification == null)
            {
                return;
            }

            _notificationQueue.Enqueue(notification);
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao enfileirar notificação LivePlay: " + ex.Message);
        }
    }

    private bool TryBuildNotification(string viewerName, string livePlayCommand, string consoleCommand, out LivePlayNotification? notification)
    {
        notification = null;

        string normalizedLivePlay = NormalizeLivePlayCommand(livePlayCommand);
        string normalizedConsole = (consoleCommand ?? string.Empty).Trim().ToLowerInvariant();

        if (normalizedLivePlay.StartsWith("sotf:addammo", StringComparison.OrdinalIgnoreCase) || normalizedConsole.StartsWith("addammo", StringComparison.Ordinal))
        {
            string[] parts = Regex.Split(consoleCommand ?? string.Empty, @"\s+")
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();

            if (parts.Length >= 3 && int.TryParse(parts[1], out int ammoId) && int.TryParse(parts[2], out int ammoAmount))
            {
                notification = new LivePlayNotification(
                    "LIVEPLAY",
                    GetAmmoDisplayName(ammoId),
                    "+" + ammoAmount + " unidades • " + viewerName,
                    new Color(0.14f, 0.76f, 0.93f, 1f)
                );
                return true;
            }

            notification = new LivePlayNotification(
                "LIVEPLAY",
                "Munição adicionada",
                "Comando executado com sucesso",
                new Color(0.14f, 0.76f, 0.93f, 1f)
            );
            return true;
        }

        if (normalizedLivePlay.StartsWith("spawn_", StringComparison.Ordinal))
        {
            notification = new LivePlayNotification(
                "LIVEPLAY",
                GetSpawnDisplayName(normalizedLivePlay),
                "Enviado por " + viewerName,
                new Color(0.89f, 0.29f, 0.27f, 1f)
            );
            return true;
        }

        if (normalizedConsole.StartsWith("addcharacter", StringComparison.Ordinal))
        {
            string args = normalizedConsole.Substring("addcharacter".Length).Trim();
            notification = new LivePlayNotification(
                "LIVEPLAY",
                GetAddCharacterDisplayName(args),
                "Enviado por " + viewerName,
                new Color(0.89f, 0.29f, 0.27f, 1f)
            );
            return true;
        }

        switch (normalizedLivePlay)
        {
            case "set_time_day":
                notification = new LivePlayNotification(
                    "LIVEPLAY",
                    "Horário alterado",
                    "Enviado por " + viewerName,
                    new Color(0.99f, 0.73f, 0.21f, 1f)
                );
                return true;
            case "set_time_night":
                notification = new LivePlayNotification(
                    "LIVEPLAY",
                    "Horário alterado",
                    "Enviado por " + viewerName,
                    new Color(0.37f, 0.45f, 0.95f, 1f)
                );
                return true;
            case "heal_player":
                notification = new LivePlayNotification(
                    "LIVEPLAY",
                    "Jogador curado",
                    "Enviado por " + viewerName,
                    new Color(0.19f, 0.82f, 0.46f, 1f)
                );
                return true;
            case "damage_player":
                notification = new LivePlayNotification(
                    "LIVEPLAY",
                    "Jogador atingido",
                    "Enviado por " + viewerName,
                    new Color(0.95f, 0.34f, 0.34f, 1f)
                );
                return true;
            case "toggle_storm":
                notification = new LivePlayNotification(
                    "LIVEPLAY",
                    "Tempestade ativada",
                    "Clima alterado",
                    new Color(0.40f, 0.71f, 0.98f, 1f)
                );
                return true;
            default:
                return false;
        }
    }

    private static string NormalizeAddCharacterArgs(string rawArgs)
    {
        string clean = (rawArgs ?? string.Empty).Trim();
        if (clean.Length == 0) return clean;

        string[] parts = Regex.Split(clean, @"\s+")
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .ToArray();

        if (parts.Length == 0) return clean;

        string key = parts[0].Trim().ToLowerInvariant();
        string amount = parts.Length > 1 ? parts[1].Trim() : "1";

        // Sluggy The Blob command ID varies in public lists, but this game build reports
        // "addcharacter slug" as ID NOT FOUND. Normalize every Sluggy alias to mutantboss.
        if (key == "slug" || key == "sluggy" || key == "sluggytheblob" || key == "sluggy_the_blob" || key == "blob")
        {
            return "mutantboss " + amount;
        }

        return clean;
    }

    private static string GetAddCharacterDisplayName(string rawArgs)
    {
        string clean = (rawArgs ?? string.Empty).Trim().ToLowerInvariant();
        if (clean.Length == 0) return "Inimigo invocado";

        if (clean.StartsWith("mr puffy", StringComparison.Ordinal)) return "Mr. Puffy invocado";
        if (clean.StartsWith("miss puffy", StringComparison.Ordinal)) return "Miss Puffy invocada";

        string[] parts = Regex.Split(clean, @"\s+")
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .ToArray();

        string key = parts.Length > 0 ? parts[0] : clean;

        switch (key)
        {
            case "cannibal":
                return "Canibal invocado";
            case "male":
            case "female":
            case "dirty":
            case "muddymale":
            case "muddyfemale":
            case "muddy":
                return "Canibal invocado";
            case "fat":
            case "femalefat":
            case "heavy":
                return "Canibal pesado invocado";
            case "fire":
                return "Canibal com tocha invocado";
            case "goldmask":
                return "Gold Mask Cannibal invocado";
            case "fingers":
                return "Fingers invocado";
            case "twins":
                return "Twins invocado";
            case "baby":
                return "Bebê mutante invocado";
            case "demon":
                return "Demônio invocado";
            case "john2":
                return "John 2 invocado";
            case "mutantboss":
            case "slug":
            case "sluggy":
            case "blob":
                return "Sluggy The Blob invocado";
            case "armsy":
                return "Armsy invocado";
            case "holey":
                return "Holey invocado";
            case "legsy":
                return "Legsy invocado";
            case "virginia":
                return "Virginia invocada";
            case "creepyvirginia":
                return "Creepy Virginia invocada";
            case "mrpuffy":
                return "Mr. Puffy invocado";
            case "robby":
                return "Kelvin invocado";
            case "misspuffy":
                return "Miss Puffy invocada";
            default:
                return "Inimigo invocado";
        }
    }

    private static string GetAmmoDisplayName(int ammoId)
    {
        switch (ammoId)
        {
            case 362:
                return "Munição Pistola 9mm";
            case 363:
                return "Munição Slug";
            case 364:
                return "Munição Buckshot";
            case 368:
                return "Virotes de Besta";
            case 369:
                return "Munição Stun Gun";
            case 387:
                return "Munição Rifle";
            case 507:
                return "Flechas Artesanais";
            default:
                return "Munição adicionada";
        }
    }

    private static string GetSpawnDisplayName(string normalizedLivePlay)
    {
        switch (normalizedLivePlay)
        {
            case "spawn_cannibal":
                return "Canibal invocado";
            case "spawn_mutant":
                return "Mutante Fingers invocado";
            case "spawn_fingers":
                return "Fingers invocado";
            case "spawn_twins":
                return "Twins invocado";
            case "spawn_baby":
                return "Bebê mutante invocado";
            case "spawn_demon":
                return "Demônio invocado";
            case "spawn_john2":
                return "John 2 invocado";
            case "spawn_heavy":
                return "Heavy invocado";
            default:
                return "Inimigo invocado";
        }
    }

    private static bool IsCharacterSpawnConsoleCommand(string consoleCommand)
    {
        return (consoleCommand ?? string.Empty).Trim().StartsWith("addcharacter", StringComparison.OrdinalIgnoreCase);
    }

    private void QueueWorldLabelForSpawn(string viewerName, string livePlayCommand, string consoleCommand, HashSet<int> actorSnapshot)
    {
        try
        {
            _pendingWorldLabelRequests.Add(new PendingWorldLabelRequest(
                viewerName,
                consoleCommand,
                actorSnapshot,
                GetWorldLabelColor(consoleCommand),
                1.35f
            ));
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao enfileirar label de mundo LivePlay: " + ex.Message);
        }
    }

    private HashSet<int> CaptureActorInstanceIds()
    {
        var ids = new HashSet<int>();
        foreach (Transform transform in FindActorTransforms())
        {
            if (transform == null || transform.gameObject == null) continue;
            ids.Add(transform.gameObject.GetInstanceID());
        }
        return ids;
    }

    private List<Transform> FindActorTransforms()
    {
        var result = new List<Transform>();
        try
        {
            var behaviours = UObject.FindObjectsOfType<MonoBehaviour>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour == null) continue;
                string typeName = behaviour.GetType().FullName ?? behaviour.GetType().Name ?? string.Empty;
                if (!IsActorBehaviourType(typeName)) continue;
                Transform transform = behaviour.transform;
                if (transform != null && transform.gameObject != null)
                {
                    result.Add(transform);
                }
            }
        }
        catch (Exception ex)
        {
            _log?.LogWarning("Falha ao procurar actors para label LivePlay: " + ex.Message);
        }
        return result;
    }

    private static bool IsActorBehaviourType(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName)) return false;
        return typeName.Contains("VailActor", StringComparison.OrdinalIgnoreCase)
            || typeName.Contains("PlayerNpc", StringComparison.OrdinalIgnoreCase)
            || typeName.Contains("Npc", StringComparison.OrdinalIgnoreCase);
    }

    private Transform? FindNewActorTransform(HashSet<int> beforeIds)
    {
        foreach (Transform transform in FindActorTransforms())
        {
            if (transform == null || transform.gameObject == null) continue;
            if (!beforeIds.Contains(transform.gameObject.GetInstanceID())) return transform;
        }
        return null;
    }

    private void UpdateWorldLabels()
    {
        float dt = Time.unscaledDeltaTime;

        for (int index = _pendingWorldLabelRequests.Count - 1; index >= 0; index--)
        {
            PendingWorldLabelRequest request = _pendingWorldLabelRequests[index];
            request.TimeLeft -= dt;
            Transform? target = FindNewActorTransform(request.BeforeIds);
            if (target != null)
            {
                _worldLabels.Add(new LivePlayWorldLabel(target, request.ViewerName, request.Color, 14f));
                _pendingWorldLabelRequests.RemoveAt(index);
                continue;
            }
            if (request.TimeLeft <= 0f)
            {
                _pendingWorldLabelRequests.RemoveAt(index);
            }
        }

        for (int index = _worldLabels.Count - 1; index >= 0; index--)
        {
            LivePlayWorldLabel label = _worldLabels[index];
            label.TimeLeft -= dt;
            if (label.TimeLeft <= 0f || label.Target == null)
            {
                _worldLabels.RemoveAt(index);
            }
        }
    }

    private void DrawWorldLabels()
    {
        if (_worldLabels.Count == 0) return;
        Camera? camera = Camera.main;
        if (camera == null) return;
        EnsureWorldLabelStyle();

        Color previousColor = GUI.color;
        foreach (LivePlayWorldLabel label in _worldLabels)
        {
            if (label.Target == null) continue;
            Vector3 world = label.Target.position + Vector3.up * 2.35f;
            Vector3 screen = camera.WorldToScreenPoint(world);
            if (screen.z <= 0f) continue;

            float fade = label.TimeLeft < 0.45f ? Mathf.Clamp01(label.TimeLeft / 0.45f) : 1f;
            float width = 190f;
            float height = 28f;
            float x = screen.x - width / 2f;
            float y = Screen.height - screen.y - 48f;
            Rect bg = new Rect(x, y, width, height);

            GUI.color = new Color(0f, 0f, 0f, 0.62f * fade);
            GUI.Box(bg, GUIContent.none);
            GUI.color = new Color(label.Color.r, label.Color.g, label.Color.b, 1f * fade);
            GUI.DrawTexture(new Rect(x, y + height - 3f, width, 3f), Texture2D.whiteTexture);
            GUI.color = new Color(1f, 1f, 1f, fade);
            GUI.Label(new Rect(x + 8f, y + 5f, width - 16f, height - 6f), label.ViewerName, _worldLabelStyle);
        }
        GUI.color = previousColor;
    }

    private void EnsureWorldLabelStyle()
    {
        if (_worldLabelStyle != null) return;
        _worldLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 15,
            richText = false
        };
        _worldLabelStyle.normal.textColor = Color.white;
    }

    private static Color GetWorldLabelColor(string consoleCommand)
    {
        string lower = (consoleCommand ?? string.Empty).ToLowerInvariant();
        if (lower.Contains("virginia")) return new Color(0.20f, 0.78f, 1f, 1f);
        if (lower.Contains("robby")) return new Color(0.26f, 0.92f, 0.44f, 1f);
        if (lower.Contains("slug") || lower.Contains("mutantboss")) return new Color(0.76f, 0.42f, 1f, 1f);
        if (lower.Contains("demon")) return new Color(1f, 0.18f, 0.16f, 1f);
        if (lower.Contains("fingers")) return new Color(1f, 0.34f, 0.74f, 1f);
        if (lower.Contains("twins")) return new Color(1f, 0.58f, 0.20f, 1f);
        if (lower.Contains("baby")) return new Color(1f, 0.72f, 0.24f, 1f);
        if (lower.Contains("john2")) return new Color(0.58f, 0.46f, 1f, 1f);
        if (lower.Contains("cannibal")) return new Color(1f, 0.35f, 0.23f, 1f);
        if (lower.Contains("heavy") || lower.Contains("fat")) return new Color(1f, 0.78f, 0.20f, 1f);
        return new Color(0.95f, 0.35f, 0.25f, 1f);
    }

    private void UpdateNotifications()
    {
        if (_currentNotification == null)
        {
            if (_notificationQueue.TryDequeue(out LivePlayNotification? next))
            {
                _currentNotification = next;
                _currentNotificationTimeLeft = 2.8f;
            }

            return;
        }

        _currentNotificationTimeLeft -= Time.unscaledDeltaTime;
        if (_currentNotificationTimeLeft <= 0f)
        {
            _currentNotification = null;
            _currentNotificationTimeLeft = 0f;
        }
    }

    private void DrawNotifications()
    {
        if (_currentNotification == null)
        {
            return;
        }

        EnsureNotificationStyles();

        float fade = 1f;
        if (_currentNotificationTimeLeft < 0.35f)
        {
            fade = Mathf.Clamp01(_currentNotificationTimeLeft / 0.35f);
        }

        float width = 430f;
        float height = 108f;
        float x = Screen.width - width - 24f;
        float y = 24f;

        Rect rect = new Rect(x, y, width, height);
        Color prevColor = GUI.color;

        // background
        GUI.color = new Color(0.05f, 0.08f, 0.11f, 0.94f * fade);
        GUI.Box(rect, GUIContent.none);

        // accent bar
        GUI.color = new Color(_currentNotification.Accent.r, _currentNotification.Accent.g, _currentNotification.Accent.b, 1f * fade);
        GUI.DrawTexture(new Rect(x, y, 8f, height), Texture2D.whiteTexture);

        // text
        GUI.color = new Color(1f, 1f, 1f, fade);
        GUI.Label(new Rect(x + 22f, y + 12f, width - 36f, 22f), _currentNotification.Title + " • Sons Of The Forest", _notificationTitleStyle);
        GUI.Label(new Rect(x + 22f, y + 39f, width - 36f, 30f), _currentNotification.Message, _notificationMessageStyle);

        if (!string.IsNullOrWhiteSpace(_currentNotification.SubMessage))
        {
            GUI.Label(new Rect(x + 22f, y + 71f, width - 36f, 22f), _currentNotification.SubMessage, _notificationSubMessageStyle);
        }

        GUI.color = prevColor;
    }

    private void EnsureNotificationStyles()
    {
        if (_notificationTitleStyle != null && _notificationMessageStyle != null && _notificationSubMessageStyle != null)
        {
            return;
        }

        _notificationTitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,            richText = false
        };
        _notificationTitleStyle.normal.textColor = new Color(0.72f, 0.80f, 0.89f, 1f);

        _notificationMessageStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 21,            wordWrap = false
        };
        _notificationMessageStyle.normal.textColor = Color.white;

        _notificationSubMessageStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,            wordWrap = false
        };
        _notificationSubMessageStyle.normal.textColor = new Color(0.83f, 0.88f, 0.92f, 1f);
    }

    private sealed class PendingLivePlayCommand
    {
        public PendingLivePlayCommand(string command, string viewerName)
        {
            Command = command;
            ViewerName = string.IsNullOrWhiteSpace(viewerName) ? "LivePlay" : viewerName;
        }

        public string Command { get; }
        public string ViewerName { get; }
    }

    private sealed class PendingWorldLabelRequest
    {
        public PendingWorldLabelRequest(string viewerName, string consoleCommand, HashSet<int> beforeIds, Color color, float timeLeft)
        {
            ViewerName = string.IsNullOrWhiteSpace(viewerName) ? "LivePlay" : viewerName;
            ConsoleCommand = consoleCommand;
            BeforeIds = beforeIds;
            Color = color;
            TimeLeft = timeLeft;
        }

        public string ViewerName { get; }
        public string ConsoleCommand { get; }
        public HashSet<int> BeforeIds { get; }
        public Color Color { get; }
        public float TimeLeft { get; set; }
    }

    private sealed class LivePlayWorldLabel
    {
        public LivePlayWorldLabel(Transform target, string viewerName, Color color, float timeLeft)
        {
            Target = target;
            ViewerName = string.IsNullOrWhiteSpace(viewerName) ? "LivePlay" : viewerName;
            Color = color;
            TimeLeft = timeLeft;
        }

        public Transform Target { get; }
        public string ViewerName { get; }
        public Color Color { get; }
        public float TimeLeft { get; set; }
    }


    private sealed class LivePlayGameChatLine
    {
        public LivePlayGameChatLine(string message, Color color, float timeLeft)
        {
            PrefixText = "[LivePlay]";
            PrefixColor = new Color(0.84f, 0.42f, 1f, 1f);
            ActorText = "LivePlay:";
            ActorColor = color;
            MessageText = message;
            MessageColor = Color.white;
            TimeLeft = timeLeft;
        }

        public LivePlayGameChatLine(string prefixText, Color prefixColor, string actorText, Color actorColor, string messageText, Color messageColor, float timeLeft)
        {
            PrefixText = prefixText;
            PrefixColor = prefixColor;
            ActorText = actorText;
            ActorColor = actorColor;
            MessageText = messageText;
            MessageColor = messageColor;
            TimeLeft = timeLeft;
        }

        public string PrefixText { get; }
        public Color PrefixColor { get; }
        public string ActorText { get; }
        public Color ActorColor { get; }
        public string MessageText { get; }
        public Color MessageColor { get; }
        public string Text => PrefixText + " " + ActorText + " " + MessageText;
        public string Message => Text;
        public Color Color => ActorColor;
        public float TimeLeft;
    }


    private sealed class LivePlayNotification
    {
        public LivePlayNotification(string title, string message, string subMessage, Color accent)
        {
            Title = title;
            Message = message;
            SubMessage = subMessage;
            Accent = accent;
        }

        public string Title { get; }
        public string Message { get; }
        public string SubMessage { get; }
        public Color Accent { get; }
    }

    private static void SetMember(object target, string name, object value)
    {
        Type type = target.GetType();
        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value);
            return;
        }

        var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
        }
    }

    private sealed class LivePlaySonsForestDispatcher : MonoBehaviour
    {
        public void Update()
        {
            Instance?.ProcessPendingCommands();
            Instance?.UpdateNotifications();
            Instance?.UpdateGameChat();
        }

        public void OnGUI()
        {
            Instance?.DrawGameChat();
            Instance?.DrawNotifications();
        }
    }
}
