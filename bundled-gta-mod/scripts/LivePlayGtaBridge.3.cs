using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using GTA;
using GTA.Math;
using GTA.Native;

public class LivePlayGtaBridge : Script
{
    private sealed class NamedPed
    {
        public Ped Ped;
        public string Name;
    }

    private sealed class GameChatLine
    {
        public string Text;
        public int ExpiresAtGameTime;
    }

    private sealed class LivePlayCommand
    {
        public string Command;
        public string OwnerName;
    }

    private sealed class ActiveLightMeteor
    {
        public Vector3 Position;
        public Vector3 Target;
        public Vector3 Velocity;
        public int CreatedAtGameTime;
    }

    private readonly List<NamedPed> _namedPeds = new List<NamedPed>();
    private readonly List<GameChatLine> _gameChatLines = new List<GameChatLine>();
    private const int GameChatMaxLines = 6;
    private const int GameChatDurationMs = 11000;
    private const string DefaultNpcOwnerName = "LivePlay";
    private const float NamedPedDrawDistance = 65.0f;
    private const int LogicTickIntervalMs = 50;
    private int _nextLogicTickGameTime = 0;
    private string _currentNpcOwnerName = DefaultNpcOwnerName;
    private readonly ConcurrentQueue<LivePlayCommand> _commands = new ConcurrentQueue<LivePlayCommand>();
    private readonly Dictionary<string, Queue<string>> _timedEffectQueues = new Dictionary<string, Queue<string>>(StringComparer.OrdinalIgnoreCase);
    private static readonly string[] TimedEffectQueueKeys = new[]
    {
        "invincible",
        "night_vision",
        "heat_vision",
        "no_hud",
        "no_radar",
        "blackout",
        "need_for_speed",
        "black_hole",
        "super_jump",
        "drunk",
        "earthquake",
        "meteor_shower",
        "invisible_vehicles",
        "explosive_zombies"
    };
    private TcpListener _listener;
    private Thread _serverThread;
    private bool _running;
    private bool _bridgeConnected = false;
    private volatile bool _pendingAppConnectedNotify = false;
    private int _port = 35951;
    private const int TimedEffectDurationMs = 15000;
    private const int NeedForSpeedDurationMs = 20000;
    private int _invincibleUntilGameTime = 0;
    private int _nightVisionUntilGameTime = 0;
    private int _heatVisionUntilGameTime = 0;
    private int _hudHiddenUntilGameTime = 0;
    private int _radarHiddenUntilGameTime = 0;
    private int _blackoutUntilGameTime = 0;
    private bool _blackoutRestoreTimeCaptured = false;
    private int _blackoutRestoreHour = 12;
    private int _blackoutRestoreMinute = 0;
    private int _blackoutRestoreSecond = 0;
    private int _needForSpeedUntilGameTime = 0;
    private int _needForSpeedNextPulseGameTime = 0;
    private int _blackHoleUntilGameTime = 0;
    private int _blackHoleNextPulseGameTime = 0;
    private Vector3 _blackHoleCenter = new Vector3(0f, 0f, 0f);
    private int _superJumpUntilGameTime = 0;
    private int _drunkUntilGameTime = 0;
    private int _drunkNextPulseGameTime = 0;
    private int _earthquakeUntilGameTime = 0;
    private int _earthquakeNextPulseGameTime = 0;
    private int _meteorUntilGameTime = 0;
    private int _meteorNextDropGameTime = 0;
    private readonly List<ActiveLightMeteor> _activeLightMeteors = new List<ActiveLightMeteor>();
    private const int MeteorMaxActive = 12;
    private const int MeteorFallTimeoutMs = 3800;
    private int _invisibleVehiclesUntilGameTime = 0;
    private readonly List<Vehicle> _invisibleVehicles = new List<Vehicle>();
    private readonly List<Ped> _explosiveZombies = new List<Ped>();
    private int _explosiveZombiesUntilGameTime = 0;
    private int _explosiveZombieNextSpawnGameTime = 0;
    private bool _explosiveZombiesNightActive = false;
    private int _explosiveZombiesRestoreHour = 12;
    private int _explosiveZombiesRestoreMinute = 0;
    private int _explosiveZombiesRestoreSecond = 0;
    private const int ExplosiveZombiesDurationMs = 32000;
    private const int ExplosiveZombiesMaxActive = 18;
    private const int ExplosiveZombiesInitialWave = 8;
    private const int ExplosiveZombiesSpawnIntervalMs = 900;
    private const int ExplosiveZombiesSpawnPerWave = 2;
    private static readonly Random _random = new Random();
    private static readonly Vector3[] _randomTeleportGroundLocations = new Vector3[]
    {
        new Vector3(215f, -810f, 30f),
        new Vector3(-1034f, -2733f, 20f),
        new Vector3(-1604f, -1030f, 13f),
        new Vector3(-1205f, -1485f, 4f),
        new Vector3(838f, -1517f, 29f),
        new Vector3(895f, -3080f, 5f),
        new Vector3(1079f, -709f, 57f),
        new Vector3(1182f, 2650f, 37f),
        new Vector3(1697f, 4924f, 42f),
        new Vector3(1856f, 3675f, 33f),
        new Vector3(-449f, 6025f, 31f),
        new Vector3(-2300f, 3385f, 31f),
        new Vector3(2480f, 4960f, 45f),
        new Vector3(-755f, 5578f, 36f),
        new Vector3(-3020f, 85f, 11f),
        new Vector3(2550f, 385f, 108f),
        new Vector3(1440f, -2600f, 48f),
        new Vector3(-420f, 1130f, 325f),
        new Vector3(501f, 5604f, 797f),
        new Vector3(-75f, -818f, 326f)
    };

    public LivePlayGtaBridge()
    {
        WriteLog("Bridge constructor iniciado");
        // Interval 0 desenha elementos visuais todo frame.
        // A lógica pesada continua limitada pelo LogicTickIntervalMs dentro do OnTick.
        Interval = 0;
        Tick += OnTick;
        Aborted += OnAborted;
        LoadConfig();
        StartServer();
        WriteLog("Bridge iniciado na porta " + _port);
    }

    private void LoadConfig()
    {
        try
        {
            string configPath = Path.Combine("scripts", "LivePlayGtaBridge.json");
            if (!File.Exists(configPath)) return;
            string raw = File.ReadAllText(configPath);
            Match port = Regex.Match(raw, "\\\"bridgePort\\\"\\s*:\\s*(\\d+)");
            int parsed;
            if (port.Success && int.TryParse(port.Groups[1].Value, out parsed) && parsed > 0) _port = parsed;
        }
        catch { }
    }

    private void StartServer()
    {
        try
        {
            _running = true;
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
            _listener.Start();
            WriteLog("Bridge carregado; iniciando servidor local na porta " + _port);
            _serverThread = new Thread(ServerLoop);
            _serverThread.IsBackground = true;
            _serverThread.Start();
        }
        catch (Exception ex)
        {
            WriteLog("Erro ao iniciar servidor: " + ex);
            Notify("~r~LivePlay GTA Bridge erro: " + ex.Message);
        }
    }

    private void ServerLoop()
    {
        while (_running)
        {
            try
            {
                TcpClient client = _listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(delegate(object state) { HandleClient(client); });
            }
            catch
            {
                if (!_running) return;
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        using (client)
        {
            try
            {
                client.ReceiveTimeout = 1500;
                client.SendTimeout = 1500;
                NetworkStream stream = client.GetStream();
                string raw = ReadHttpRequest(stream);

                if (raw.StartsWith("POST /liveplay/ping", StringComparison.OrdinalIgnoreCase))
                {
                    if (!_bridgeConnected)
                    {
                        _bridgeConnected = true;
                        _pendingAppConnectedNotify = true;
                    }

                    WriteHttp(stream, 200, "pong");
                    return;
                }

                if (!raw.StartsWith("POST /liveplay/command", StringComparison.OrdinalIgnoreCase))
                {
                    WriteHttp(stream, 404, "not-found");
                    return;
                }

                string body = ExtractBody(raw);
                string command = ExtractJsonString(body, "command");
                if (string.IsNullOrWhiteSpace(command))
                {
                    WriteHttp(stream, 400, "empty-command");
                    return;
                }

                string ownerName = ExtractViewerNameFromJson(body);
                if (string.IsNullOrWhiteSpace(ownerName)) ownerName = DefaultNpcOwnerName;

                if (!_bridgeConnected)
                {
                    _bridgeConnected = true;
                    _pendingAppConnectedNotify = true;
                }

                _commands.Enqueue(new LivePlayCommand
                {
                    Command = command.Trim(),
                    OwnerName = SanitizeNpcOwnerName(ownerName)
                });
                WriteHttp(stream, 200, "queued:" + command.Trim());
            }
            catch (Exception ex)
            {
                try { WriteHttp(client.GetStream(), 500, ex.Message); } catch { }
            }
        }
    }

    private static string ReadHttpRequest(NetworkStream stream)
    {
        byte[] buffer = new byte[16384];
        using (MemoryStream ms = new MemoryStream())
        {
            int read = stream.Read(buffer, 0, buffer.Length);
            if (read <= 0) return "";
            ms.Write(buffer, 0, read);
            string partial = Encoding.UTF8.GetString(ms.ToArray());
            int headerEnd = partial.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            if (headerEnd < 0) return partial;

            int contentLength = 0;
            Match len = Regex.Match(partial.Substring(0, headerEnd), "Content-Length:\\s*(\\d+)", RegexOptions.IgnoreCase);
            if (len.Success) int.TryParse(len.Groups[1].Value, out contentLength);

            int bodyStart = headerEnd + 4;
            int currentBodyBytes = Encoding.UTF8.GetByteCount(partial.Substring(bodyStart));
            while (currentBodyBytes < contentLength)
            {
                read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0) break;
                ms.Write(buffer, 0, read);
                partial = Encoding.UTF8.GetString(ms.ToArray());
                currentBodyBytes = Encoding.UTF8.GetByteCount(partial.Substring(bodyStart));
            }
            return partial;
        }
    }

    private static string ExtractBody(string raw)
    {
        int index = raw.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        return index >= 0 ? raw.Substring(index + 4) : "";
    }

    private static string ExtractJsonString(string json, string key)
    {
        if (string.IsNullOrEmpty(json)) return "";
        var match = Regex.Match(json, "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*\\\"((?:\\\\.|[^\\\"])*)\\\"");
        if (!match.Success) return "";
        return Regex.Unescape(match.Groups[1].Value);
    }

    private static string ExtractViewerNameFromJson(string json)
    {
        string viewerName = ExtractJsonString(json, "viewerName");
        if (!string.IsNullOrWhiteSpace(viewerName)) return viewerName;

        string nickname = ExtractJsonString(json, "nickname");
        if (!string.IsNullOrWhiteSpace(nickname)) return nickname;

        string username = ExtractJsonString(json, "username");
        if (!string.IsNullOrWhiteSpace(username)) return username;

        return DefaultNpcOwnerName;
    }

    private static void WriteHttp(NetworkStream stream, int status, string text)
    {
        byte[] body = Encoding.UTF8.GetBytes(text ?? "");
        string statusText = status == 200 ? "OK" : status == 400 ? "Bad Request" : status == 404 ? "Not Found" : "Error";
        string header = "HTTP/1.1 " + status + " " + statusText + "\r\n" +
                        "Content-Type: text/plain; charset=utf-8\r\n" +
                        "Content-Length: " + body.Length + "\r\n" +
                        "Connection: close\r\n\r\n";
        byte[] head = Encoding.UTF8.GetBytes(header);
        stream.Write(head, 0, head.Length);
        stream.Write(body, 0, body.Length);
    }

    private void OnTick(object sender, EventArgs e)
    {
        if (_pendingAppConnectedNotify)
        {
            _pendingAppConnectedNotify = false;
            Notify("~g~LivePlay GTA conectado ao app~s~");
        }

        // Texto 3D e markers do GTA precisam ser redesenhados todo frame;
        // se desenhar só a cada 50ms/100ms, o nome dos NPCs fica piscando.
        MaintainFrameVisuals();

        if (Game.GameTime < _nextLogicTickGameTime) return;
        _nextLogicTickGameTime = Game.GameTime + LogicTickIntervalMs;

        MaintainTimedStates();
        int limit = 10;
        LivePlayCommand livePlayCommand;
        while (limit-- > 0 && _commands.TryDequeue(out livePlayCommand))
        {
            string previousOwner = _currentNpcOwnerName;
            try
            {
                _currentNpcOwnerName = SanitizeNpcOwnerName(livePlayCommand != null ? livePlayCommand.OwnerName : DefaultNpcOwnerName);
                ExecuteLivePlayCommand(livePlayCommand != null ? livePlayCommand.Command : string.Empty);
            }
            finally
            {
                _currentNpcOwnerName = previousOwner;
            }
        }
    }

    private void MaintainFrameVisuals()
    {
        try { MaintainNamedPeds(); } catch { }
        try { DrawGameChatLines(); } catch { }
        try { MaintainLightMeteors(); } catch { }
        try
        {
            if (_blackHoleUntilGameTime > 0 && Game.GameTime <= _blackHoleUntilGameTime) DrawBlackHoleVisual();
        }
        catch { }
    }

    private void MaintainTimedStates()
    {
        try
        {
            if (_invincibleUntilGameTime > 0 && Game.GameTime > _invincibleUntilGameTime)
            {
                Game.Player.Character.IsInvincible = false;
                _invincibleUntilGameTime = 0;
                Notify("LivePlay GTA: invencibilidade encerrada");
            }

            if (_nightVisionUntilGameTime > 0)
            {
                if (Game.GameTime > _nightVisionUntilGameTime)
                {
                    Function.Call(Hash.SET_NIGHTVISION, false);
                    _nightVisionUntilGameTime = 0;
                    Notify("LivePlay GTA: visão noturna encerrada");
                }
                else Function.Call(Hash.SET_NIGHTVISION, true);
            }

            if (_heatVisionUntilGameTime > 0)
            {
                if (Game.GameTime > _heatVisionUntilGameTime)
                {
                    Function.Call(Hash.SET_SEETHROUGH, false);
                    _heatVisionUntilGameTime = 0;
                    Notify("LivePlay GTA: visão térmica encerrada");
                }
                else Function.Call(Hash.SET_SEETHROUGH, true);
            }

            if (_hudHiddenUntilGameTime > 0)
            {
                if (Game.GameTime > _hudHiddenUntilGameTime)
                {
                    Function.Call(Hash.DISPLAY_HUD, true);
                    _hudHiddenUntilGameTime = 0;
                    Notify("LivePlay GTA: HUD restaurado");
                }
                else Function.Call(Hash.DISPLAY_HUD, false);
            }

            if (_radarHiddenUntilGameTime > 0)
            {
                if (Game.GameTime > _radarHiddenUntilGameTime)
                {
                    Function.Call(Hash.DISPLAY_RADAR, true);
                    _radarHiddenUntilGameTime = 0;
                    Notify("LivePlay GTA: radar restaurado");
                }
                else Function.Call(Hash.DISPLAY_RADAR, false);
            }

            if (_blackoutUntilGameTime > 0)
            {
                if (Game.GameTime > _blackoutUntilGameTime)
                {
                    StopBlackout(false);
                }
                else
                {
                    try { Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, true); } catch { }
                    try { Function.Call(Hash.NETWORK_OVERRIDE_CLOCK_TIME, 0, 0, 0); } catch { }
                }
            }

            if (_meteorUntilGameTime > 0)
            {
                if (Game.GameTime > _meteorUntilGameTime)
                {
                    _meteorUntilGameTime = 0;
                    _meteorNextDropGameTime = 0;
                    Notify("LivePlay GTA: chuva de meteoros encerrada");
                }
                else if (Game.GameTime >= _meteorNextDropGameTime)
                {
                    DropMeteorColumn();
                    _meteorNextDropGameTime = Game.GameTime + 430;
                }
            }

            if (_superJumpUntilGameTime > 0)
            {
                if (Game.GameTime > _superJumpUntilGameTime)
                {
                    _superJumpUntilGameTime = 0;
                    Notify("LivePlay GTA: super jump encerrado");
                }
                else
                {
                    try { Function.Call(Hash.SET_SUPER_JUMP_THIS_FRAME, Game.Player.Handle); } catch { }
                }
            }

            if (_drunkUntilGameTime > 0)
            {
                if (Game.GameTime > _drunkUntilGameTime)
                {
                    StopDrunk();
                }
                else
                {
                    MaintainDrunk();
                }
            }

            if (_earthquakeUntilGameTime > 0)
            {
                if (Game.GameTime > _earthquakeUntilGameTime)
                {
                    _earthquakeUntilGameTime = 0;
                    _earthquakeNextPulseGameTime = 0;
                    Notify("LivePlay GTA: terremoto encerrado");
                }
                else
                {
                    if (Game.GameTime >= _earthquakeNextPulseGameTime)
                    {
                        ApplyEarthquakePulse();
                        _earthquakeNextPulseGameTime = Game.GameTime + 260;
                    }
                }
            }

            if (_needForSpeedUntilGameTime > 0)
            {
                if (Game.GameTime > _needForSpeedUntilGameTime)
                {
                    ResetNeedForSpeed();
                    _needForSpeedUntilGameTime = 0;
                    _needForSpeedNextPulseGameTime = 0;
                    Notify("LivePlay GTA: Need For Speed encerrado");
                }
                else
                {
                    ApplyNeedForSpeedPlayerSafety();
                    if (Game.GameTime >= _needForSpeedNextPulseGameTime)
                    {
                        ApplyNeedForSpeedPulse();
                        _needForSpeedNextPulseGameTime = Game.GameTime + 220;
                    }
                }
            }

            if (_blackHoleUntilGameTime > 0)
            {
                DrawBlackHoleVisual();

                if (Game.GameTime > _blackHoleUntilGameTime)
                {
                    ReleaseBlackHoleTargets();
                    _blackHoleUntilGameTime = 0;
                    _blackHoleNextPulseGameTime = 0;
                    try { Interval = 0; } catch { }
                    Notify("LivePlay GTA: black hole encerrado");
                }
                else if (Game.GameTime >= _blackHoleNextPulseGameTime)
                {
                    ApplyBlackHolePulse();
                    _blackHoleNextPulseGameTime = Game.GameTime + 160;
                }
            }

            MaintainInvisibleVehicles();
            MaintainExplosiveZombies();
            ProcessTimedEffectQueues();
        }
        catch { }
    }

    private bool TryHandleTimedEffectCommand(string command)
    {
        string key = GetTimedEffectKey(command);
        if (string.IsNullOrWhiteSpace(key)) return false;

        if (IsTimedEffectActive(key))
        {
            EnqueueTimedEffect(key, command);
            return true;
        }

        ExecuteTimedEffectByKey(key);
        return true;
    }

    private string GetTimedEffectKey(string command)
    {
        string normalized = (command ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized)) return string.Empty;

        string slug = normalized;
        if (slug.StartsWith("lp "))
        {
            slug = NormalizeSlug(slug.Substring(3).Trim());
        }
        else
        {
            string[] parts = slug.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            slug = NormalizeSlug(parts.Length > 0 ? parts[0] : slug);
        }

        if (slug == "blackout_off") return string.Empty;
        if (slug == "invincible" || slug.Contains("invincibility") || slug.Contains("immortality")) return "invincible";
        if (slug == "night_vision") return "night_vision";
        if (slug == "heat_vision") return "heat_vision";
        if (slug == "no_hud") return "no_hud";
        if (slug == "no_radar") return "no_radar";
        if (slug == "blackout_on" || slug == "blackout") return "blackout";
        if (slug == "need_for_speed") return "need_for_speed";
        if (slug == "black_hole" || slug == "fake_black_hole" || slug == "gravity_sphere" || slug == "gravity_field") return "black_hole";
        if (slug == "super_jump" || slug.Contains("super_jump")) return "super_jump";
        if (slug == "drunk" || slug.Contains("drunk") || slug.Contains("sick") || slug.Contains("lsd")) return "drunk";
        if (slug == "earthquake" || slug.Contains("earthquake") || slug.Contains("tremor") || slug.Contains("quake")) return "earthquake";
        if (slug == "meteor_shower" || slug.Contains("meteor")) return "meteor_shower";
        if (slug == "invisible_vehicles") return "invisible_vehicles";
        if (slug == "explosive_zombies") return "explosive_zombies";

        return string.Empty;
    }

    private bool IsTimedEffectActive(string key)
    {
        int now = Game.GameTime;
        switch (key)
        {
            case "invincible":
                return _invincibleUntilGameTime > now;
            case "night_vision":
                return _nightVisionUntilGameTime > now;
            case "heat_vision":
                return _heatVisionUntilGameTime > now;
            case "no_hud":
                return _hudHiddenUntilGameTime > now || _radarHiddenUntilGameTime > now;
            case "no_radar":
                return _radarHiddenUntilGameTime > now;
            case "blackout":
                return _blackoutUntilGameTime > now;
            case "need_for_speed":
                return _needForSpeedUntilGameTime > now;
            case "black_hole":
                return _blackHoleUntilGameTime > now;
            case "super_jump":
                return _superJumpUntilGameTime > now;
            case "drunk":
                return _drunkUntilGameTime > now;
            case "earthquake":
                return _earthquakeUntilGameTime > now;
            case "meteor_shower":
                return _meteorUntilGameTime > now;
            case "invisible_vehicles":
                return _invisibleVehiclesUntilGameTime > now;
            case "explosive_zombies":
                return _explosiveZombiesUntilGameTime > now || _explosiveZombies.Count > 0 || _explosiveZombiesNightActive;
            default:
                return false;
        }
    }

    private void EnqueueTimedEffect(string key, string command)
    {
        Queue<string> queue;
        if (!_timedEffectQueues.TryGetValue(key, out queue))
        {
            queue = new Queue<string>();
            _timedEffectQueues[key] = queue;
        }

        queue.Enqueue(command ?? string.Empty);
        Notify("LivePlay GTA: efeito na fila (" + FormatTimedEffectName(key) + ") x" + queue.Count);
    }

    private void ProcessTimedEffectQueues()
    {
        foreach (string key in TimedEffectQueueKeys)
        {
            if (IsTimedEffectActive(key)) continue;

            Queue<string> queue;
            if (!_timedEffectQueues.TryGetValue(key, out queue) || queue.Count == 0) continue;

            queue.Dequeue();
            ExecuteTimedEffectByKey(key);
        }
    }

    private void ExecuteTimedEffectByKey(string key)
    {
        switch (key)
        {
            case "invincible":
                InvinciblePlayer();
                return;
            case "night_vision":
                StartNightVision();
                return;
            case "heat_vision":
                StartHeatVision();
                return;
            case "no_hud":
                HideHudTimed();
                return;
            case "no_radar":
                HideRadarTimed();
                return;
            case "blackout":
                StartBlackout();
                return;
            case "need_for_speed":
                NeedForSpeed();
                return;
            case "black_hole":
                BlackHole();
                return;
            case "super_jump":
                SuperJump();
                return;
            case "drunk":
                Drunk();
                return;
            case "earthquake":
                Earthquake();
                return;
            case "meteor_shower":
                MeteorShower();
                return;
            case "invisible_vehicles":
                InvisibleVehicles();
                return;
            case "explosive_zombies":
                StartExplosiveZombies();
                return;
        }
    }

    private static string FormatTimedEffectName(string key)
    {
        switch (key)
        {
            case "invincible": return "Invencível";
            case "night_vision": return "Visão noturna";
            case "heat_vision": return "Visão térmica";
            case "no_hud": return "HUD oculto";
            case "no_radar": return "Radar oculto";
            case "blackout": return "Blackout";
            case "need_for_speed": return "Need For Speed";
            case "black_hole": return "Black Hole";
            case "super_jump": return "Super Jump";
            case "drunk": return "Bêbado";
            case "earthquake": return "Terremoto";
            case "meteor_shower": return "Meteoros";
            case "invisible_vehicles": return "Veículos invisíveis";
            case "explosive_zombies": return "Zumbis explosivos";
            default: return key;
        }
    }

    private void ExecuteLivePlayCommand(string command)
    {
        string normalized = (command ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized)) return;

        if (TryHandleGameChatCommand(command)) return;
        if (TryHandleTimedEffectCommand(command)) return;

        if (normalized.StartsWith("lp ")) { ExecuteLivePlayEffectSlug(normalized.Substring(3).Trim()); return; }
        if (normalized.StartsWith("earthquake")) { Earthquake(); return; }
        if (normalized.StartsWith("drunk")) { Drunk(); return; }
        if (normalized.StartsWith("ragdoll")) { Ragdoll(2500); return; }
        if (normalized.StartsWith("launch_player")) { LaunchPlayer(); return; }
        if (normalized.StartsWith("heal")) { Heal(); return; }
        if (normalized.StartsWith("armor")) { Armor(); return; }
        if (normalized.StartsWith("remove_weapons")) { Game.Player.Character.Weapons.RemoveAll(); Notify("LivePlay: armas removidas"); return; }
        if (normalized.StartsWith("give_weapon")) { GiveWeapon(Arg(command, 1, "pistol")); return; }
        if (normalized.StartsWith("wanted")) { int level = Clamp(ParseInt(Arg(command, 1, "1"), 1), 0, 5); Game.Player.WantedLevel = level; Notify("LivePlay: wanted " + level); return; }
        if (normalized.StartsWith("clear_wanted")) { Game.Player.WantedLevel = 0; Notify("LivePlay: polícia limpa"); return; }
        if (normalized.StartsWith("explosion_ring")) { ExplosionRing(); return; }
        if (normalized.StartsWith("explosive_zombies")) { StartExplosiveZombies(); return; }
        if (normalized.StartsWith("explode")) { ExplodeFront(); return; }
        if (normalized.StartsWith("spawn_moto_cops")) { SpawnMotoGroup("s_m_y_cop_01", true, 2, "LivePlay: moto cops"); return; }
        if (normalized.StartsWith("spawn_moto_bandits")) { SpawnMotoGroup("g_m_y_lost_01", true, 2, "LivePlay: moto bandidos"); return; }
        if (normalized.StartsWith("spawn_vehicle")) { SpawnVehicle(Arg(command, 1, "adder")); return; }
        if (normalized.StartsWith("spawn_ped")) { SpawnPed(Arg(command, 1, "s_m_y_cop_01")); return; }
        if (normalized.StartsWith("spawn_rifle_chimp")) { SpawnRifleChimp(); return; }
        if (normalized.StartsWith("invisible_vehicles")) { InvisibleVehicles(); return; }
        if (normalized.StartsWith("spawn_single_armed_attacker")) { SpawnAttackers("g_m_y_lost_01", 1, true, WeaponHash.Pistol, true, "LivePlay GTA: 1 inimigo armado"); return; }
        if (normalized.StartsWith("repair_vehicle")) { RepairVehicle(); return; }
        if (normalized.StartsWith("boost_vehicle")) { BoostVehicle(); return; }
        if (normalized.StartsWith("need_for_speed")) { NeedForSpeed(); return; }
        if (normalized.StartsWith("flip_vehicle")) { FlipVehicle(); return; }
        if (normalized.StartsWith("break_vehicle")) { BreakVehicle(); return; }
        if (normalized.StartsWith("weather")) { SetWeather(Arg(command, 1, "THUNDER")); return; }
        if (normalized.StartsWith("time")) { SetTime(ParseInt(Arg(command, 1, "12"), 12)); return; }
        if (normalized.StartsWith("blackout_on")) { StartBlackout(); return; }
        if (normalized.StartsWith("blackout_off")) { StopBlackout(); return; }

        Notify("LivePlay comando desconhecido: " + command);
    }

    private bool TryHandleGameChatCommand(string command)
    {
        string raw = (command ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(raw)) return false;

        string lower = raw.ToLowerInvariant();
        string message = string.Empty;

        if (lower.StartsWith("lp chat ")) message = raw.Substring(8).Trim();
        else if (lower == "lp chat") message = "Mensagem de teste do LivePlay";
        else if (lower.StartsWith("lp_chat ")) message = raw.Substring(8).Trim();
        else if (lower == "lp_chat") message = "Mensagem de teste do LivePlay";
        else if (lower.StartsWith("gta:chat ")) message = raw.Substring(9).Trim();
        else if (lower == "gta:chat") message = "Mensagem de teste do LivePlay";
        else if (lower.StartsWith("chat ")) message = raw.Substring(5).Trim();
        else return false;

        ShowGameChatMessage(message);
        return true;
    }

    private void ShowGameChatMessage(string message)
    {
        AddGameChatMessage(message);
    }

    private void AddGameChatMessage(string message)
    {
        string safe = SanitizeGameChatMessage(message);
        if (string.IsNullOrWhiteSpace(safe)) safe = "Mensagem de teste do LivePlay";

        int now = Game.GameTime;
        for (int i = _gameChatLines.Count - 1; i >= 0; i--)
        {
            if (_gameChatLines[i] == null || _gameChatLines[i].ExpiresAtGameTime <= now)
            {
                _gameChatLines.RemoveAt(i);
            }
        }

        _gameChatLines.Add(new GameChatLine
        {
            Text = safe,
            ExpiresAtGameTime = now + GameChatDurationMs
        });

        while (_gameChatLines.Count > GameChatMaxLines)
        {
            _gameChatLines.RemoveAt(0);
        }
    }


    private void DrawGameChatLines()
    {
        if (_gameChatLines.Count == 0) return;

        int now = Game.GameTime;
        for (int i = _gameChatLines.Count - 1; i >= 0; i--)
        {
            if (_gameChatLines[i] == null || _gameChatLines[i].ExpiresAtGameTime <= now)
            {
                _gameChatLines.RemoveAt(i);
            }
        }

        if (_gameChatLines.Count == 0) return;

        float x = 0.004f;
        float bottomY = 0.780f;
        float lineHeight = 0.022f;
        float width = 0.235f;
        float paddingY = 0.007f;
        float boxHeight = (_gameChatLines.Count * lineHeight) + (paddingY * 2f);
        float topY = bottomY - boxHeight;

        Function.Call(Hash.DRAW_RECT, x + width / 2f, topY + boxHeight / 2f, width, boxHeight, 0, 0, 0, 105);

        float y = topY + paddingY;
        for (int i = 0; i < _gameChatLines.Count; i++)
        {
            string text = _gameChatLines[i] != null ? _gameChatLines[i].Text : string.Empty;
            DrawGameChatText(text, x + 0.006f, y, 0.235f, 255, 255, 255, 238, false);
            y += lineHeight;
        }
    }



    private static void DrawGameChatText(string text, float x, float y, float scale, int r, int g, int b, int a, bool header)
    {
        string safe = SanitizeGameChatMessage(text);
        if (safe.Length > 96) safe = safe.Substring(0, 96) + "...";

        Function.Call(Hash.SET_TEXT_FONT, 0);
        Function.Call(Hash.SET_TEXT_SCALE, 0.0f, scale);
        Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);
        Function.Call(Hash.SET_TEXT_OUTLINE);
        Function.Call(Hash.SET_TEXT_DROPSHADOW, 1, 0, 0, 0, 200);
        Function.Call(Hash.SET_TEXT_WRAP, x, x + 0.220f);
        Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
        Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, safe);
        Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y);
    }


    private static string SanitizeGameChatMessage(string value)
    {
        string text = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        text = Regex.Replace(text, @"[\r\n\t]+", " ");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        if (text.Length > 140) text = text.Substring(0, 140);
        return text;
    }

    private static string Arg(string command, int index, string fallback)
    {
        string[] parts = (command ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > index ? parts[index] : fallback;
    }

    private static int ParseInt(string value, int fallback)
    {
        int parsed;
        return int.TryParse(value, out parsed) ? parsed : fallback;
    }
    private static int Clamp(int value, int min, int max) { if (value < min) return min; if (value > max) return max; return value; }


    private void StartExplosiveZombies()
    {
        _explosiveZombiesUntilGameTime = Game.GameTime + ExplosiveZombiesDurationMs;
        _explosiveZombieNextSpawnGameTime = 0;
        CleanupExplosiveZombies(false);
        StartExplosiveZombieNight();

        for (int i = 0; i < ExplosiveZombiesInitialWave; i++) SpawnExplosiveZombie(i);

        try { Interval = 0; } catch { }
        Notify("LivePlay GTA: invasão de zumbis explosivos por 32s");
    }

    private void StartExplosiveZombieNight()
    {
        try { _explosiveZombiesRestoreHour = Clamp(Function.Call<int>(Hash.GET_CLOCK_HOURS), 0, 23); } catch { _explosiveZombiesRestoreHour = 12; }
        try { _explosiveZombiesRestoreMinute = Clamp(Function.Call<int>(Hash.GET_CLOCK_MINUTES), 0, 59); } catch { _explosiveZombiesRestoreMinute = 0; }
        try { _explosiveZombiesRestoreSecond = Clamp(Function.Call<int>(Hash.GET_CLOCK_SECONDS), 0, 59); } catch { _explosiveZombiesRestoreSecond = 0; }

        _explosiveZombiesNightActive = true;
        ApplyExplosiveZombieNight();
    }

    private void ApplyExplosiveZombieNight()
    {
        try { Function.Call(Hash.NETWORK_OVERRIDE_CLOCK_TIME, 0, 0, 0); } catch { }
        try { Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, "THUNDER"); } catch { }
    }

    private void StopExplosiveZombieNight()
    {
        if (!_explosiveZombiesNightActive) return;

        try { Function.Call(Hash.NETWORK_OVERRIDE_CLOCK_TIME, _explosiveZombiesRestoreHour, _explosiveZombiesRestoreMinute, _explosiveZombiesRestoreSecond); } catch { }
        try { Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, "CLEAR"); } catch { }
        _explosiveZombiesNightActive = false;
    }

    private void MaintainExplosiveZombies()
    {
        if (_explosiveZombiesUntilGameTime <= 0 && _explosiveZombies.Count == 0)
        {
            StopExplosiveZombieNight();
            return;
        }

        Ped player = Game.Player.Character;
        if (player == null || !player.Exists())
        {
            CleanupExplosiveZombies(false);
            _explosiveZombiesUntilGameTime = 0;
            StopExplosiveZombieNight();
            return;
        }

        if (_explosiveZombiesNightActive) ApplyExplosiveZombieNight();

        for (int i = _explosiveZombies.Count - 1; i >= 0; i--)
        {
            Ped zombie = _explosiveZombies[i];
            if (zombie == null || !zombie.Exists())
            {
                _explosiveZombies.RemoveAt(i);
                continue;
            }

            bool shouldExplode = false;
            try { if (zombie.IsDead || zombie.Health <= 0) shouldExplode = true; } catch { }
            try { if (!shouldExplode && zombie.Health < zombie.MaxHealth - 3) shouldExplode = true; } catch { }
            try { if (!shouldExplode && zombie.Position.DistanceTo(player.Position) <= 2.2f) shouldExplode = true; } catch { }

            if (shouldExplode)
            {
                ExplodeZombie(zombie);
                _explosiveZombies.RemoveAt(i);
                continue;
            }

            try
            {
                if (Game.GameTime % 520 < 70)
                {
                    Function.Call(Hash.TASK_COMBAT_PED, zombie.Handle, player.Handle, 0, 16);
                    Function.Call(Hash.SET_PED_KEEP_TASK, zombie.Handle, true);
                }
            }
            catch { }
        }

        if (Game.GameTime <= _explosiveZombiesUntilGameTime)
        {
            if (Game.GameTime >= _explosiveZombieNextSpawnGameTime && _explosiveZombies.Count < ExplosiveZombiesMaxActive)
            {
                int freeSlots = Math.Max(0, ExplosiveZombiesMaxActive - _explosiveZombies.Count);
                int waveCount = Math.Min(ExplosiveZombiesSpawnPerWave, freeSlots);
                for (int i = 0; i < waveCount; i++) SpawnExplosiveZombie(_explosiveZombies.Count + i);
                _explosiveZombieNextSpawnGameTime = Game.GameTime + ExplosiveZombiesSpawnIntervalMs;
            }
        }
        else if (_explosiveZombies.Count == 0)
        {
            _explosiveZombiesUntilGameTime = 0;
            _explosiveZombieNextSpawnGameTime = 0;
            StopExplosiveZombieNight();
            try { Interval = 0; } catch { }
            Notify("LivePlay GTA: invasão de zumbis encerrada");
        }
    }

    private void SpawnExplosiveZombie(int index)
    {
        Ped player = Game.Player.Character;
        if (player == null || !player.Exists()) return;

        // Usa somente o modelo zumbi nativo do GTA V. Sem fallback para ped normal.
        Model model = new Model("u_m_y_zombie_01");
        model.Request(1200);
        if (!model.IsLoaded)
        {
            try { model.MarkAsNoLongerNeeded(); } catch { }
            return;
        }

        Vector3 side = player.RightVector * RandomFloat(-9.0f, 9.0f);
        Vector3 front = player.ForwardVector * RandomFloat(10.0f, 18.0f);
        Vector3 pos = player.Position + front + side;
        try { pos = World.GetNextPositionOnStreet(pos); } catch { }

        Ped zombie = World.CreatePed(model, pos, player.Heading + 180f);
        model.MarkAsNoLongerNeeded();
        if (zombie == null || !zombie.Exists()) return;

        try { Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, zombie.Handle); } catch { }
        try { zombie.MaxHealth = 155; } catch { }
        try { Function.Call(Hash.SET_ENTITY_MAX_HEALTH, zombie.Handle, 155); } catch { }
        try { zombie.Health = 155; } catch { }
        try { Function.Call(Hash.SET_ENTITY_HEALTH, zombie.Handle, 155); } catch { }
        try { Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, zombie.Handle, false); } catch { }
        try { Function.Call(Hash.SET_PED_DIES_WHEN_INJURED, zombie.Handle, false); } catch { }
        try { Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, zombie.Handle, true); } catch { }
        try { Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, zombie.Handle, 0, false); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ABILITY, zombie.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, zombie.Handle, 3); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_RANGE, zombie.Handle, 0); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, zombie.Handle, 5, true); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, zombie.Handle, 46, true); } catch { }
        try { Function.Call(Hash.SET_PED_AS_ENEMY, zombie.Handle, true); } catch { }
        try { zombie.Weapons.Give(WeaponHash.Knife, 1, true, true); } catch { }
        try { Function.Call(Hash.TASK_COMBAT_PED, zombie.Handle, player.Handle, 0, 16); } catch { }
        try { Function.Call(Hash.SET_PED_KEEP_TASK, zombie.Handle, true); } catch { }

        _explosiveZombies.Add(zombie);
        RegisterNamedPed(zombie);
    }

    private void ExplodeZombie(Ped zombie)
    {
        if (zombie == null || !zombie.Exists()) return;
        Vector3 pos = zombie.Position;
        try { World.AddExplosion(pos, ExplosionType.Grenade, 5.6f, 1.0f); } catch { }
        try { zombie.Delete(); } catch { }
    }

    private void CleanupExplosiveZombies(bool explode)
    {
        for (int i = _explosiveZombies.Count - 1; i >= 0; i--)
        {
            Ped zombie = _explosiveZombies[i];
            if (zombie != null && zombie.Exists())
            {
                if (explode) ExplodeZombie(zombie);
                else { try { zombie.Delete(); } catch { } }
            }
        }
        _explosiveZombies.Clear();
    }

    private void SpawnVehicle(string modelName)
    {
        Model model = new Model(modelName);
        TrySpawnVehicleModel(model, modelName, true);
    }

    private bool TrySpawnVehicleModel(Model model, string modelName, bool notifyFailure)
    {
        try
        {
            model.Request(1500);
            if (!model.IsLoaded)
            {
                if (notifyFailure) Notify("LivePlay: veículo não carregou " + modelName);
                return false;
            }

            Ped player = Game.Player.Character;
            Vehicle current = null;
            bool replacingCurrentVehicle = false;
            Vector3 spawnPos = player.Position + player.ForwardVector * 6f + new Vector3(0f, 0f, 0.5f);
            float heading = player.Heading;

            try
            {
                if (player != null && player.Exists() && player.IsInVehicle())
                {
                    current = player.CurrentVehicle;
                    if (current != null && current.Exists())
                    {
                        replacingCurrentVehicle = true;
                        spawnPos = current.Position + new Vector3(0f, 0f, 0.7f);
                        heading = current.Heading;
                    }
                }
            }
            catch { }

            if (replacingCurrentVehicle && current != null && current.Exists())
            {
                try { Function.Call(Hash.SET_ENTITY_AS_MISSION_ENTITY, current.Handle, true, true); } catch { }
                try { Function.Call(Hash.CLEAR_PED_TASKS_IMMEDIATELY, player.Handle); } catch { }
                try { current.Delete(); } catch { }
            }

            Vehicle vehicle = World.CreateVehicle(model, spawnPos, heading);
            if (vehicle == null)
            {
                if (notifyFailure) Notify("LivePlay: veículo não criou " + modelName);
                return false;
            }

            try
            {
                if (ShouldPlaceVehicleOnGround(modelName)) vehicle.PlaceOnGround();
            }
            catch { }

            vehicle.Heading = heading;
            vehicle.IsEngineRunning = true;
            try { Function.Call(Hash.SET_PED_INTO_VEHICLE, player.Handle, vehicle.Handle, -1); } catch { try { player.SetIntoVehicle(vehicle, VehicleSeat.Driver); } catch { } }
            try { Function.Call(Hash.SET_VEHICLE_ENGINE_ON, vehicle.Handle, true, true, false); } catch { }

            Notify(replacingCurrentVehicle ? "LivePlay: veículo trocado por " + modelName : "LivePlay: entrou no veículo " + modelName);
            return true;
        }
        catch
        {
            if (notifyFailure) Notify("LivePlay: falha ao criar veículo " + modelName);
            return false;
        }
        finally
        {
            try { model.MarkAsNoLongerNeeded(); } catch { }
        }
    }

    private static bool ShouldPlaceVehicleOnGround(string modelName)
    {
        string name = (modelName ?? "").ToLowerInvariant();
        if (name.Contains("buzzard") || name.Contains("blimp") || name.Contains("cargoplane") || name.Contains("stunt") || name.Contains("jet") || name.Contains("plane") || name.Contains("dinghy") || name.Contains("boat")) return false;
        if (name.Contains("hydra") || name.Contains("lazer") || name.Contains("cuban") || name.Contains("mammatus") || name.Contains("dodo") || name.Contains("duster") || name.Contains("shamal") || name.Contains("luxor") || name.Contains("nimbus") || name.Contains("velum") || name.Contains("vestra") || name.Contains("miljet") || name.Contains("seabreeze") || name.Contains("molotok") || name.Contains("nokota") || name.Contains("starling") || name.Contains("bombushka") || name.Contains("tula") || name.Contains("avenger") || name.Contains("alphaz1")) return false;
        if (name.Contains("cargobob") || name.Contains("frogger") || name.Contains("maverick") || name.Contains("polmav") || name.Contains("volatus") || name.Contains("swift") || name.Contains("supervolito") || name.Contains("annihilator") || name.Contains("valkyrie") || name.Contains("savage") || name.Contains("hunter") || name.Contains("akula") || name.Contains("havok") || name.Contains("seasparrow")) return false;
        if (name.Contains("marquis") || name.Contains("seashark") || name.Contains("speeder") || name.Contains("squalo") || name.Contains("suntrap") || name.Contains("toro") || name.Contains("tropic") || name.Contains("tug") || name.Contains("submersible") || name.Contains("jetmax") || name.Contains("predator")) return false;
        return true;
    }

    private void SpawnPed(string modelName)
    {
        Model model = new Model(modelName);
        model.Request(1200);
        if (!model.IsLoaded) { Notify("LivePlay: ped não carregou " + modelName); return; }
        Ped player = Game.Player.Character;
        Vector3 pos = player.Position + player.ForwardVector * 5f;
        Ped ped = World.CreatePed(model, pos, player.Heading + 180f);
        if (ped != null)
        {
            RegisterNamedPed(ped);
            ped.Weapons.Give(WeaponHash.Pistol, 120, true, true);
            ped.Task.FightAgainst(player);
        }
        model.MarkAsNoLongerNeeded();
        Notify("LivePlay: ped " + modelName);
    }

    private void InvisibleVehicles()
    {
        Ped player = Game.Player.Character;
        if (player == null || !player.Exists()) return;

        try { RestoreInvisibleVehicles(false); } catch { }

        _invisibleVehiclesUntilGameTime = Game.GameTime + 30000;
        int count = ScanInvisibleVehiclesAroundPlayer();

        Notify(count > 0 ? "LivePlay GTA: veículos invisíveis por 30s" : "LivePlay GTA: nenhum veículo próximo");
    }

    private int ScanInvisibleVehiclesAroundPlayer()
    {
        Ped player = Game.Player.Character;
        if (player == null || !player.Exists()) return 0;

        int count = 0;

        try
        {
            if (player.IsInVehicle())
            {
                Vehicle currentVehicle = player.CurrentVehicle;
                if (currentVehicle != null && currentVehicle.Exists() && !IsVehicleAlreadyInvisible(currentVehicle))
                {
                    MakeVehicleInvisibleOnly(currentVehicle);
                    count++;
                }
            }
        }
        catch { }

        Vehicle[] vehicles = World.GetNearbyVehicles(player, 280f);
        foreach (Vehicle vehicle in vehicles)
        {
            if (vehicle == null || !vehicle.Exists()) continue;
            if (_invisibleVehicles.Count >= 180) break;
            if (IsVehicleAlreadyInvisible(vehicle)) continue;

            try
            {
                MakeVehicleInvisibleOnly(vehicle);
                count++;
            }
            catch { }
        }

        return count;
    }

    private bool IsVehicleAlreadyInvisible(Vehicle vehicle)
    {
        if (vehicle == null || !vehicle.Exists()) return false;

        for (int i = _invisibleVehicles.Count - 1; i >= 0; i--)
        {
            try
            {
                Vehicle current = _invisibleVehicles[i];
                if (current == null || !current.Exists())
                {
                    _invisibleVehicles.RemoveAt(i);
                    continue;
                }

                if (current.Handle == vehicle.Handle) return true;
            }
            catch
            {
                try { _invisibleVehicles.RemoveAt(i); } catch { }
            }
        }

        return false;
    }

    private void MakeVehicleInvisibleOnly(Vehicle vehicle)
    {
        if (vehicle == null || !vehicle.Exists()) return;
        if (IsVehicleAlreadyInvisible(vehicle)) return;

        try { Function.Call(Hash.SET_ENTITY_VISIBLE, vehicle.Handle, true, false); } catch { }
        try { Function.Call(Hash.SET_ENTITY_ALPHA, vehicle.Handle, 0, false); } catch { }
        try { KeepVehicleOccupantsVisible(vehicle); } catch { }

        _invisibleVehicles.Add(vehicle);
    }

    private void KeepVehicleOccupantsVisible(Vehicle vehicle)
    {
        if (vehicle == null || !vehicle.Exists()) return;

        for (int seat = -1; seat <= 15; seat++)
        {
            try
            {
                Ped ped = vehicle.GetPedOnSeat((VehicleSeat)seat);
                if (ped == null || !ped.Exists()) continue;

                Function.Call(Hash.SET_ENTITY_VISIBLE, ped.Handle, true, false);
                Function.Call(Hash.RESET_ENTITY_ALPHA, ped.Handle);
            }
            catch { }
        }

        try
        {
            Ped player = Game.Player.Character;
            if (player != null && player.Exists())
            {
                Function.Call(Hash.SET_ENTITY_VISIBLE, player.Handle, true, false);
                Function.Call(Hash.RESET_ENTITY_ALPHA, player.Handle);
            }
        }
        catch { }
    }

    private void MaintainInvisibleVehicles()
    {
        if (_invisibleVehiclesUntilGameTime <= 0) return;

        if (Game.GameTime <= _invisibleVehiclesUntilGameTime)
        {
            try { ScanInvisibleVehiclesAroundPlayer(); } catch { }

            for (int i = _invisibleVehicles.Count - 1; i >= 0; i--)
            {
                Vehicle vehicle = _invisibleVehicles[i];
                try
                {
                    if (vehicle == null || !vehicle.Exists())
                    {
                        _invisibleVehicles.RemoveAt(i);
                        continue;
                    }

                    Function.Call(Hash.SET_ENTITY_VISIBLE, vehicle.Handle, true, false);
                    Function.Call(Hash.SET_ENTITY_ALPHA, vehicle.Handle, 0, false);
                    KeepVehicleOccupantsVisible(vehicle);
                }
                catch { }
            }
            return;
        }

        RestoreInvisibleVehicles(true);
    }

    private void RestoreInvisibleVehicles(bool notify)
    {
        for (int i = _invisibleVehicles.Count - 1; i >= 0; i--)
        {
            Vehicle vehicle = _invisibleVehicles[i];
            try
            {
                if (vehicle != null && vehicle.Exists())
                {
                    Function.Call(Hash.SET_ENTITY_VISIBLE, vehicle.Handle, true, false);
                    Function.Call(Hash.RESET_ENTITY_ALPHA, vehicle.Handle);
                    KeepVehicleOccupantsVisible(vehicle);
                }
            }
            catch { }
        }

        _invisibleVehicles.Clear();
        _invisibleVehiclesUntilGameTime = 0;
        if (notify) Notify("LivePlay GTA: veículos visíveis novamente");
    }

    private void SpawnAttackers(string pedModelName, int count, bool armed, WeaponHash weapon, bool hostile, string message)
    {
        Ped player = Game.Player.Character;
        Model model = new Model(pedModelName);
        model.Request(1200);
        if (!model.IsLoaded) { Notify("LivePlay GTA: modelo não carregou " + pedModelName); return; }

        int spawned = 0;
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = player.ForwardVector * (5f + i * 1.8f) + player.RightVector * ((i % 2 == 0 ? 1f : -1f) * (2.5f + i));
            Ped ped = World.CreatePed(model, player.Position + offset, player.Heading + 180f);
            if (ped == null) continue;
            RegisterNamedPed(ped);
            spawned++;
            if (armed)
            {
                try { ped.Weapons.Give(weapon, 600, true, true); } catch { }
            }
            if (hostile)
            {
                try { Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, ped.Handle, true); } catch { }
                try { Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, ped.Handle, 0, false); } catch { }
                try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Handle, 46, true); } catch { }
                try { Function.Call(Hash.SET_PED_AS_ENEMY, ped.Handle, true); } catch { }
                try { ped.Task.FightAgainst(player); } catch { }
            }
        }
        model.MarkAsNoLongerNeeded();
        Notify(spawned > 0 ? message : "LivePlay GTA: nenhum NPC criado");
    }

    private void SpawnMixedKnifeAndPistolAttackers()
    {
        Ped player = Game.Player.Character;
        Model model = new Model("g_m_y_lost_01");
        model.Request(1200);
        if (!model.IsLoaded) { Notify("LivePlay GTA: modelo não carregou g_m_y_lost_01"); return; }

        int spawned = 0;
        WeaponHash[] weapons = new WeaponHash[] { WeaponHash.Knife, WeaponHash.Bat };
        string[] labels = new string[] { "faca", "taco" };

        for (int i = 0; i < 2; i++)
        {
            Vector3 offset = player.ForwardVector * (5.4f + i * 1.8f) + player.RightVector * (i == 0 ? -2.8f : 2.8f);
            Ped ped = World.CreatePed(model, player.Position + offset, player.Heading + 180f);
            if (ped == null) continue;
            RegisterNamedPed(ped);
            spawned++;

            try { ped.Weapons.Give(weapons[i], i == 0 ? 1 : 180, true, true); } catch { }
            try { Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, ped.Handle, true); } catch { }
            try { Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, ped.Handle, 0, false); } catch { }
            try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Handle, 5, true); } catch { }
            try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Handle, 46, true); } catch { }
            try { Function.Call(Hash.SET_PED_AS_ENEMY, ped.Handle, true); } catch { }
            try { ped.Task.FightAgainst(player); } catch { }
        }

        model.MarkAsNoLongerNeeded();
        Notify(spawned > 0 ? "LivePlay GTA: 2 inimigos, " + labels[0] + " e " + labels[1] : "LivePlay GTA: nenhum NPC criado");
    }

    private void SpawnExtremeAngryCopStrong()
    {
        Ped player = Game.Player.Character;
        Model model = new Model("s_m_y_swat_01");
        model.Request(1400);
        if (!model.IsLoaded) { Notify("LivePlay GTA: SWAT não carregou"); return; }

        Vector3 offset = player.ForwardVector * 6.2f + player.RightVector * -1.6f;
        Ped cop = World.CreatePed(model, player.Position + offset, player.Heading + 180f);
        if (cop == null)
        {
            model.MarkAsNoLongerNeeded();
            Notify("LivePlay GTA: SWAT não criado");
            return;
        }

        RegisterNamedPed(cop);
        try { Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, cop.Handle); } catch { }
        try { cop.MaxHealth = 620; } catch { }
        try { Function.Call(Hash.SET_ENTITY_MAX_HEALTH, cop.Handle, 620); } catch { }
        try { cop.Health = 620; } catch { }
        try { Function.Call(Hash.SET_ENTITY_HEALTH, cop.Handle, 620); } catch { }
        try { Function.Call(Hash.SET_PED_ARMOUR, cop.Handle, 120); } catch { }
        try { Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, cop.Handle, false); } catch { }
        try { Function.Call(Hash.SET_PED_DIES_WHEN_INJURED, cop.Handle, false); } catch { }
        try { Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, cop.Handle, true); } catch { }
        try { Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, cop.Handle, 0, false); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ABILITY, cop.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, cop.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_RANGE, cop.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_ACCURACY, cop.Handle, 62); } catch { }
        try { Function.Call(Hash.SET_PED_SHOOT_RATE, cop.Handle, 650); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, cop.Handle, 5, true); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, cop.Handle, 46, true); } catch { }
        try { Function.Call(Hash.SET_PED_AS_ENEMY, cop.Handle, true); } catch { }

        try { cop.Weapons.Give(WeaponHash.CarbineRifle, 900, true, true); } catch { }
        try { cop.Task.FightAgainst(player); } catch { }
        try { Function.Call(Hash.TASK_COMBAT_PED, cop.Handle, player.Handle, 0, 16); } catch { }
        try { Function.Call(Hash.SET_PED_KEEP_TASK, cop.Handle, true); } catch { }

        model.MarkAsNoLongerNeeded();
        Notify("LivePlay GTA: SWAT extremo forte");
    }

    private void SpawnAnimalGroup(string modelName, int count, bool hostile, string message)
    {
        Ped player = Game.Player.Character;
        Model model = new Model(modelName);
        model.Request(1200);
        if (!model.IsLoaded) { Notify("LivePlay GTA: animal não carregou " + modelName); return; }

        int spawned = 0;
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = player.ForwardVector * (4f + i * 1.4f) + player.RightVector * ((i % 2 == 0 ? 1f : -1f) * (1.5f + i));
            Ped animal = World.CreatePed(model, player.Position + offset, player.Heading + 180f);
            if (animal == null) continue;
            RegisterNamedPed(animal);
            spawned++;
            if (hostile)
            {
                try { animal.Task.FightAgainst(player); } catch { }
            }
            else
            {
                // Animais neutros, como o poodle, não devem ficar congelados no ponto de spawn.
                // Deixa o ped com comportamento ambiente natural para ele andar/fugir como animal comum do GTA.
                try { Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, animal.Handle, false); } catch { }
                try { Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, animal.Handle, 0, false); } catch { }
                try { Function.Call(Hash.TASK_WANDER_STANDARD, animal.Handle, 10.0f, 10); } catch { }
                try { Function.Call(Hash.SET_PED_KEEP_TASK, animal.Handle, true); } catch { }
            }
        }
        model.MarkAsNoLongerNeeded();
        Notify(spawned > 0 ? message : "LivePlay GTA: nenhum animal criado");
    }

    private void SpawnAngryChimpWithHatchet()
    {
        Ped player = Game.Player.Character;
        Model model = new Model("a_c_chimp");
        model.Request(1200);
        if (!model.IsLoaded) { Notify("LivePlay GTA: chimp não carregou"); return; }

        Vector3 offset = player.ForwardVector * 4.8f + player.RightVector * 1.4f;
        Ped chimp = World.CreatePed(model, player.Position + offset, player.Heading + 180f);
        if (chimp == null)
        {
            model.MarkAsNoLongerNeeded();
            Notify("LivePlay GTA: chimp não criado");
            return;
        }

        RegisterNamedPed(chimp);
        try { chimp.Health = 260; } catch { }
        try { chimp.MaxHealth = 260; } catch { }
        try { Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, chimp.Handle, true); } catch { }
        try { Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, chimp.Handle, 0, false); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ABILITY, chimp.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, chimp.Handle, 3); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_RANGE, chimp.Handle, 0); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, chimp.Handle, 5, true); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, chimp.Handle, 46, true); } catch { }
        try { Function.Call(Hash.SET_PED_AS_ENEMY, chimp.Handle, true); } catch { }

        try { Function.Call(Hash.GIVE_WEAPON_TO_PED, chimp.Handle, unchecked((int)0xF9DCBF2D), 1, false, true); } catch { }
        try { Function.Call(Hash.SET_CURRENT_PED_WEAPON, chimp.Handle, unchecked((int)0xF9DCBF2D), true); } catch { }

        try { chimp.Task.FightAgainst(player); } catch { }
        try { Function.Call(Hash.TASK_COMBAT_PED, chimp.Handle, player.Handle, 0, 16); } catch { }
        try { Function.Call(Hash.SET_PED_KEEP_TASK, chimp.Handle, true); } catch { }

        model.MarkAsNoLongerNeeded();
        Notify("LivePlay GTA: chimp com machado");
    }


    private void SpawnRifleChimp()
    {
        Ped player = Game.Player.Character;
        if (player == null || !player.Exists()) return;

        Model model = new Model("a_c_chimp");
        model.Request(1200);
        if (!model.IsLoaded) { Notify("LivePlay GTA: rifle chimp não carregou"); return; }

        Vector3 offset = player.ForwardVector * 5.2f + player.RightVector * 1.2f;
        Ped chimp = World.CreatePed(model, player.Position + offset, player.Heading + 180f);
        if (chimp == null)
        {
            model.MarkAsNoLongerNeeded();
            Notify("LivePlay GTA: rifle chimp não criado");
            return;
        }

        RegisterNamedPed(chimp);

        try { Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, chimp.Handle); } catch { }
        try { chimp.MaxHealth = 360; } catch { }
        try { Function.Call(Hash.SET_ENTITY_MAX_HEALTH, chimp.Handle, 360); } catch { }
        try { chimp.Health = 360; } catch { }
        try { Function.Call(Hash.SET_ENTITY_HEALTH, chimp.Handle, 360); } catch { }
        try { Function.Call(Hash.SET_PED_ARMOUR, chimp.Handle, 80); } catch { }
        try { Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, chimp.Handle, false); } catch { }
        try { Function.Call(Hash.SET_PED_DIES_WHEN_INJURED, chimp.Handle, false); } catch { }
        try { Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, chimp.Handle, true); } catch { }
        try { Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, chimp.Handle, 0, false); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ABILITY, chimp.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, chimp.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_RANGE, chimp.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_ACCURACY, chimp.Handle, 55); } catch { }
        try { Function.Call(Hash.SET_PED_SHOOT_RATE, chimp.Handle, 650); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, chimp.Handle, 5, true); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, chimp.Handle, 46, true); } catch { }
        try { Function.Call(Hash.SET_PED_AS_ENEMY, chimp.Handle, true); } catch { }

        try { chimp.Weapons.Give(WeaponHash.CarbineRifle, 900, true, true); } catch { }
        try { Function.Call(Hash.GIVE_WEAPON_TO_PED, chimp.Handle, unchecked((int)0x83BF0278), 900, false, true); } catch { }
        try { Function.Call(Hash.SET_CURRENT_PED_WEAPON, chimp.Handle, unchecked((int)0x83BF0278), true); } catch { }

        try { chimp.Task.FightAgainst(player); } catch { }
        try { Function.Call(Hash.TASK_COMBAT_PED, chimp.Handle, player.Handle, 0, 16); } catch { }
        try { Function.Call(Hash.SET_PED_KEEP_TASK, chimp.Handle, true); } catch { }

        model.MarkAsNoLongerNeeded();
        Notify("LivePlay GTA: rifle chimp");
    }

    private void SpawnAngryAlienWithRayPistol()
    {
        Ped player = Game.Player.Character;

        // Modelo alien nativo do GTA V. Sem ped/model custom instalado, este é o corpo alienígena mais próximo do jogo base.
        Model model = new Model("s_m_m_movalien_01");
        model.Request(1800);
        if (!model.IsLoaded) { Notify("LivePlay GTA: alien não carregou"); return; }

        Vector3 offset = player.ForwardVector * 6.0f + player.RightVector * 1.6f;
        Ped alien = World.CreatePed(model, player.Position + offset, player.Heading + 180f);
        if (alien == null)
        {
            model.MarkAsNoLongerNeeded();
            Notify("LivePlay GTA: alien não criado");
            return;
        }

        RegisterNamedPed(alien);
        // Força aparência/default do ped alienígena e deixa ele com resistência real.
        // Ordem importante: primeiro MaxHealth, depois Health. Antes isso podia deixar ele com vida padrão.
        try { Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, alien.Handle); } catch { }
        try { alien.MaxHealth = 720; } catch { }
        try { Function.Call(Hash.SET_ENTITY_MAX_HEALTH, alien.Handle, 720); } catch { }
        try { alien.Health = 720; } catch { }
        try { Function.Call(Hash.SET_ENTITY_HEALTH, alien.Handle, 720); } catch { }
        try { Function.Call(Hash.SET_PED_ARMOUR, alien.Handle, 80); } catch { }

        // Evita morte instantânea por headshot. Ele ainda morre, mas precisa de vários tiros.
        try { Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, alien.Handle, false); } catch { }
        try { Function.Call(Hash.SET_PED_DIES_WHEN_INJURED, alien.Handle, false); } catch { }
        try { Function.Call(unchecked((Hash)0xB128377056A54E2A), alien.Handle, false); } catch { } // SET_PED_CAN_RAGDOLL

        try { Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, alien.Handle, true); } catch { }
        try { Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, alien.Handle, 0, false); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ABILITY, alien.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, alien.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_RANGE, alien.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_ACCURACY, alien.Handle, 58); } catch { }
        try { Function.Call(Hash.SET_PED_SHOOT_RATE, alien.Handle, 650); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, alien.Handle, 5, true); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, alien.Handle, 17, true); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, alien.Handle, 46, true); } catch { }
        try { Function.Call(Hash.SET_PED_AS_ENEMY, alien.Handle, true); } catch { }

        // Up-n-Atomizer / Ray Pistol. Se o jogo/instalação não tiver esse DLC weapon, cai para StunGun.
        try { Function.Call(Hash.GIVE_WEAPON_TO_PED, alien.Handle, unchecked((int)0xAF3696A1), 999, false, true); } catch { }
        try { Function.Call(Hash.SET_CURRENT_PED_WEAPON, alien.Handle, unchecked((int)0xAF3696A1), true); } catch { }
        try { alien.Weapons.Give(WeaponHash.StunGun, 999, false, true); } catch { }

        try { alien.Task.FightAgainst(player); } catch { }
        try { Function.Call(Hash.TASK_COMBAT_PED, alien.Handle, player.Handle, 0, 16); } catch { }
        try { Function.Call(Hash.SET_PED_KEEP_TASK, alien.Handle, true); } catch { }

        model.MarkAsNoLongerNeeded();
        Notify("LivePlay GTA: alien resistente com arma alienígena");
    }

    private void SpawnKillerClownStrong()
    {
        Ped player = Game.Player.Character;
        Model model = new Model("s_m_y_clown_01");
        model.Request(1400);
        if (!model.IsLoaded) { Notify("LivePlay GTA: palhaço não carregou"); return; }

        Vector3 offset = player.ForwardVector * 5.5f + player.RightVector * -1.7f;
        Ped clown = World.CreatePed(model, player.Position + offset, player.Heading + 180f);
        if (clown == null)
        {
            model.MarkAsNoLongerNeeded();
            Notify("LivePlay GTA: palhaço não criado");
            return;
        }

        RegisterNamedPed(clown);
        try { clown.Health = 460; } catch { }
        try { clown.MaxHealth = 460; } catch { }
        try { Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, clown.Handle, true); } catch { }
        try { Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, clown.Handle, 0, false); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ABILITY, clown.Handle, 2); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, clown.Handle, 3); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_RANGE, clown.Handle, 0); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, clown.Handle, 5, true); } catch { }
        try { Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, clown.Handle, 46, true); } catch { }
        try { Function.Call(Hash.SET_PED_AS_ENEMY, clown.Handle, true); } catch { }

        try { clown.Weapons.Give(WeaponHash.Knife, 1, true, true); } catch { }
        try { clown.Task.FightAgainst(player); } catch { }
        try { Function.Call(Hash.TASK_COMBAT_PED, clown.Handle, player.Handle, 0, 16); } catch { }
        try { Function.Call(Hash.SET_PED_KEEP_TASK, clown.Handle, true); } catch { }

        model.MarkAsNoLongerNeeded();
        Notify("LivePlay GTA: killer clown forte");
    }

    private void Earthquake()
    {
        _earthquakeUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        _earthquakeNextPulseGameTime = 0;
        ApplyEarthquakePulse();
        Notify("LivePlay GTA: terremoto por 15s");
    }

    private void ApplyEarthquakePulse()
    {
        Ped player = Game.Player.Character;
        if (player == null || !player.Exists()) return;

        Vehicle currentVehicle = null;
        try
        {
            if (player.IsInVehicle()) currentVehicle = player.CurrentVehicle;
        }
        catch { currentVehicle = null; }

        if (currentVehicle != null && currentVehicle.Exists())
        {
            ApplyEarthquakeForceToVehicle(currentVehicle, true);
        }
        else
        {
            // A pé: só desequilibra levemente. Nada de explosão no personagem.
            try
            {
                Vector3 footForce = player.ForwardVector * RandomFloat(-1.6f, 1.6f) +
                                    player.RightVector * RandomFloat(-2.1f, 2.1f) +
                                    new Vector3(0f, 0f, RandomFloat(0.35f, 1.25f));
                player.ApplyForce(footForce);
            }
            catch { }
        }

        int count = 0;
        Vehicle[] vehicles = World.GetNearbyVehicles(player, 145f);
        foreach (Vehicle vehicle in vehicles)
        {
            if (vehicle == null || !vehicle.Exists()) continue;
            if (currentVehicle != null && currentVehicle.Exists() && vehicle.Handle == currentVehicle.Handle) continue;
            if (count++ >= 95) break;
            ApplyEarthquakeForceToVehicle(vehicle, false);
        }
    }

    private void ApplyEarthquakeForceToVehicle(Vehicle vehicle, bool isPlayerVehicle)
    {
        if (vehicle == null || !vehicle.Exists()) return;

        float side = RandomFloat(-1.0f, 1.0f);
        if (side > -0.12f && side < 0.12f) side = side < 0f ? -0.45f : 0.45f;

        Vector3 force = vehicle.ForwardVector * RandomFloat(-7.5f, 7.5f) +
                        vehicle.RightVector * (side * RandomFloat(8.0f, 15.0f)) +
                        new Vector3(0f, 0f, RandomFloat(1.8f, isPlayerVehicle ? 5.4f : 6.8f));

        try { vehicle.ApplyForce(force); } catch { }
    }


    private void Drunk()
    {
        _drunkUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        _drunkNextPulseGameTime = 0;

        Ped player = Game.Player.Character;
        try { Function.Call(Hash.REQUEST_ANIM_SET, "move_m@drunk@verydrunk"); } catch { }
        try { Function.Call(Hash.SET_PED_MOVEMENT_CLIPSET, player.Handle, "move_m@drunk@verydrunk", 1.0f); } catch { }
        try { Function.Call(unchecked((Hash)0x95D2D383D5396B8A), player.Handle, true); } catch { } // SET_PED_IS_DRUNK
        try { Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "spectator5"); } catch { }
        try { Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "DRUNK_SHAKE", 1.05f); } catch { }

        MaintainDrunk();
        Notify("LivePlay GTA: bêbado por 15s");
    }

    private void MaintainDrunk()
    {
        Ped player = Game.Player.Character;
        if (player == null || !player.Exists()) return;

        try { Function.Call(Hash.SET_PED_MOVEMENT_CLIPSET, player.Handle, "move_m@drunk@verydrunk", 1.0f); } catch { }
        try { Function.Call(unchecked((Hash)0x95D2D383D5396B8A), player.Handle, true); } catch { } // SET_PED_IS_DRUNK

        if (Game.GameTime < _drunkNextPulseGameTime) return;
        _drunkNextPulseGameTime = Game.GameTime + 420;

        try
        {
            if (player.IsInVehicle())
            {
                Vehicle vehicle = player.CurrentVehicle;
                if (vehicle != null && vehicle.Exists())
                {
                    float bias = RandomFloat(-0.55f, 0.55f);
                    try { Function.Call(unchecked((Hash)0x42A8EC77D5150CBE), vehicle.Handle, bias); } catch { } // SET_VEHICLE_STEER_BIAS
                    try { vehicle.ApplyForce(vehicle.RightVector * RandomFloat(-0.38f, 0.38f)); } catch { }
                }
            }
            else
            {
                try { Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "DRUNK_SHAKE", 0.85f); } catch { }
            }
        }
        catch { }
    }

    private void StopDrunk()
    {
        Ped player = Game.Player.Character;
        _drunkUntilGameTime = 0;
        _drunkNextPulseGameTime = 0;

        try { Function.Call(Hash.RESET_PED_MOVEMENT_CLIPSET, player.Handle, 0.0f); } catch { }
        try { Function.Call(unchecked((Hash)0x95D2D383D5396B8A), player.Handle, false); } catch { } // SET_PED_IS_DRUNK
        try { Function.Call(Hash.CLEAR_TIMECYCLE_MODIFIER); } catch { }
        try { Function.Call(Hash.STOP_GAMEPLAY_CAM_SHAKING, true); } catch { }

        try
        {
            Vehicle vehicle = player.CurrentVehicle;
            if (vehicle != null && vehicle.Exists()) Function.Call(unchecked((Hash)0x42A8EC77D5150CBE), vehicle.Handle, 0.0f); // SET_VEHICLE_STEER_BIAS
        }
        catch { }

        Notify("LivePlay GTA: bêbado encerrado");
    }


    private void Ragdoll(int ms)
    {
        Function.Call(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character.Handle, ms, ms + 500, 0, true, true, false);
        Notify("LivePlay: ragdoll");
    }

    private void LaunchPlayer()
    {
        Game.Player.Character.ApplyForce(new Vector3(0f, 0f, 18f));
        Notify("LivePlay: launch player");
    }

    private void SuperJump()
    {
        _superJumpUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        try { Function.Call(Hash.SET_SUPER_JUMP_THIS_FRAME, Game.Player.Handle); } catch { }
        Notify("LivePlay GTA: super jump por 15s");
    }


    private void Skydive()
    {
        Ped player = Game.Player.Character;
        try { Function.Call(Hash.GIVE_WEAPON_TO_PED, player.Handle, unchecked((int)0xFBAB5776), 1, false, true); } catch { }
        player.Position = player.Position + new Vector3(0f, 0f, 220f);
        player.ApplyForce(new Vector3(0f, 0f, 8f));
        Notify("LivePlay GTA: skydive");
    }

    private void ExplodeFront()
    {
        Vector3 pos = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 4f;
        World.AddExplosion(pos, ExplosionType.Grenade, 4f, 1f);
        Notify("LivePlay: explosão");
    }

    private void ExplosionRing()
    {
        Ped player = Game.Player.Character;
        Vector3 center = player.Position;
        for (int i = 0; i < 6; i++)
        {
            double angle = Math.PI * 2.0 * i / 6.0;
            Vector3 pos = center + new Vector3((float)Math.Cos(angle) * 7f, (float)Math.Sin(angle) * 7f, 0f);
            World.AddExplosion(pos, ExplosionType.Grenade, 2.6f, 0.75f);
        }
        Notify("LivePlay: explosion ring");
    }

    private void SpawnMotoGroup(string pedModelName, bool hostile, int count, string message)
    {
        Ped player = Game.Player.Character;
        for (int i = 0; i < count; i++)
        {
            string bikeModelName = (pedModelName ?? "").ToLowerInvariant().Contains("cop") ? "policeb" : "bati";
            Model bikeModel = new Model(bikeModelName);
            Model pedModel = new Model(pedModelName);
            bikeModel.Request(1200);
            pedModel.Request(1200);
            if (!bikeModel.IsLoaded || !pedModel.IsLoaded) continue;
            Vector3 offset = player.ForwardVector * (8f + i * 2f) + player.RightVector * (i % 2 == 0 ? 3f : -3f);
            Vehicle bike = World.CreateVehicle(bikeModel, player.Position + offset, player.Heading + 180f);
            Ped ped = World.CreatePed(pedModel, player.Position + offset + new Vector3(0f, 0f, 1f), player.Heading + 180f);
            if (ped != null)
            {
                RegisterNamedPed(ped);
                ped.Weapons.Give(WeaponHash.Pistol, 160, true, true);
                if (bike != null) ped.SetIntoVehicle(bike, VehicleSeat.Driver);
                if (hostile) ped.Task.FightAgainst(player);
            }
            bikeModel.MarkAsNoLongerNeeded();
            pedModel.MarkAsNoLongerNeeded();
        }
        Notify(message);
    }

    private void GiveWeapon(string weaponName)
    {
        string name = (weaponName ?? "pistol").ToUpperInvariant();
        WeaponHash weapon = WeaponHash.Pistol;
        if (name.Contains("RPG")) weapon = WeaponHash.RPG;
        else if (name.Contains("CARBINE")) weapon = WeaponHash.CarbineRifle;
        else if (name.Contains("SHOTGUN")) weapon = WeaponHash.PumpShotgun;
        else if (name.Contains("SMG")) weapon = WeaponHash.SMG;
        else if (name.Contains("SNIPER")) weapon = WeaponHash.SniperRifle;
        else if (name.Contains("MINIGUN")) weapon = WeaponHash.Minigun;
        else if (name.Contains("RAILGUN")) weapon = WeaponHash.Railgun;
        else if (name.Contains("STUN")) weapon = WeaponHash.StunGun;
        Game.Player.Character.Weapons.Give(weapon, 300, true, true);
        Notify("LivePlay: arma " + weaponName);
    }

    private void Heal()
    {
        Ped player = Game.Player.Character;
        player.Health = player.MaxHealth;
        player.Armor = 100;
        Notify("LivePlay: heal");
    }

    private void Armor()
    {
        Game.Player.Character.Armor = 100;
        Notify("LivePlay: armor");
    }

    private void RepairVehicle()
    {
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null) { Notify("LivePlay: entre em um veículo"); return; }
        vehicle.Repair();
        vehicle.IsEngineRunning = true;
        Notify("LivePlay: veículo reparado");
    }

    private void BoostVehicle()
    {
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null) { Notify("LivePlay: entre em um veículo"); return; }
        vehicle.ApplyForce(vehicle.ForwardVector * 32f + new Vector3(0f, 0f, 2f));
        Notify("LivePlay: boost veículo");
    }

    private void NeedForSpeed()
    {
        _needForSpeedUntilGameTime = Game.GameTime + NeedForSpeedDurationMs;
        _needForSpeedNextPulseGameTime = 0;
        ApplyNeedForSpeedPlayerSafety();
        ApplyNeedForSpeedPulse();
        Notify("LivePlay GTA: Need For Speed por 20s");
    }

    private void ApplyNeedForSpeedPulse()
    {
        ApplyNeedForSpeedPlayerSafety();
        Ped player = Game.Player.Character;
        Vehicle current = null;
        try
        {
            if (player != null && player.Exists() && player.IsInVehicle()) current = player.CurrentVehicle;
        }
        catch { }

        if (current != null && current.Exists()) ApplyNeedForSpeedToVehicle(current, true);

        Vehicle[] vehicles = World.GetNearbyVehicles(player, 390f);
        int count = 0;
        foreach (Vehicle vehicle in vehicles)
        {
            if (vehicle == null || !vehicle.Exists()) continue;
            if (count++ >= 180) break;
            bool isPlayerVehicle = current != null && current.Exists() && vehicle.Handle == current.Handle;
            ApplyNeedForSpeedToVehicle(vehicle, isPlayerVehicle);
        }
    }

    private void ApplyNeedForSpeedToVehicle(Vehicle vehicle, bool isPlayerVehicle)
    {
        if (vehicle == null || !vehicle.Exists()) return;
        try { vehicle.IsEngineRunning = true; } catch { }
        try { Function.Call(Hash.SET_VEHICLE_ENGINE_ON, vehicle.Handle, true, true, false); } catch { }

        // Aumenta limite e torque, mas também força velocidade real.
        // Só aumentar top speed não muda nada se o carro/IA não acelerar.
        try { Function.Call(unchecked((Hash)0x93A3996368C94158), vehicle.Handle, 130f); } catch { } // MODIFY_VEHICLE_TOP_SPEED
        try { Function.Call(unchecked((Hash)0xB59E4BD37AE292DB), vehicle.Handle, 10.5f); } catch { } // SET_VEHICLE_CHEAT_POWER_INCREASE

        float speed = isPlayerVehicle ? 105f : RandomFloat(88f, 128f);
        try { Function.Call(unchecked((Hash)0xAB54A438726D25D5), vehicle.Handle, speed); } catch { } // SET_VEHICLE_FORWARD_SPEED

        // Fallback leve para veículos do tráfego caso o native de velocidade não aplique em algum modelo.
        // Não aplica força no veículo do player para evitar derrubar/ejetar o personagem.
        if (!isPlayerVehicle)
        {
            try { vehicle.ApplyForce(vehicle.ForwardVector * RandomFloat(4.0f, 8.0f)); } catch { }
        }
    }

    private void ApplyNeedForSpeedPlayerSafety()
    {
        try
        {
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists()) return;

            // O Need For Speed NÃO deixa o personagem invencível.
            // Se o carro explodir, se estiver em moto/bike, ou se houver impacto letal real, ele ainda pode morrer.
            if (_invincibleUntilGameTime <= 0 || Game.GameTime > _invincibleUntilGameTime)
            {
                try { player.IsInvincible = false; } catch { }
            }

            if (player.IsInVehicle())
            {
                Vehicle vehicle = player.CurrentVehicle;
                if (vehicle != null && vehicle.Exists())
                {
                    try { vehicle.IsEngineRunning = true; } catch { }
                    try { Function.Call(Hash.SET_VEHICLE_ENGINE_ON, vehicle.Handle, true, true, false); } catch { }

                    if (IsNeedForSpeedSeatbeltVehicle(vehicle))
                    {
                        // Cinto temporário apenas para carros/caminhonetes/etc.
                        // Evita sair voando pelo para-brisa, mas não bloqueia dano nem explosão.
                        try { Function.Call(unchecked((Hash)0xB128377056A54E2A), player.Handle, false); } catch { } // SET_PED_CAN_RAGDOLL
                        try { Function.Call(unchecked((Hash)0x1913FE4CBF41C463), player.Handle, 32, false); } catch { } // SET_PED_CONFIG_FLAG: CanFlyThroughWindscreen false
                    }
                    else
                    {
                        // Moto/bike/avião/barco/etc: sem proteção. Se bater, pode cair e morrer normalmente.
                        try { Function.Call(unchecked((Hash)0xB128377056A54E2A), player.Handle, true); } catch { }
                        try { Function.Call(unchecked((Hash)0x1913FE4CBF41C463), player.Handle, 32, true); } catch { }
                    }
                }
            }
            else
            {
                // A pé: sem proteção especial.
                try { Function.Call(unchecked((Hash)0xB128377056A54E2A), player.Handle, true); } catch { }
                try { Function.Call(unchecked((Hash)0x1913FE4CBF41C463), player.Handle, 32, true); } catch { }
            }
        }
        catch { }
    }

    private bool IsNeedForSpeedSeatbeltVehicle(Vehicle vehicle)
    {
        if (vehicle == null || !vehicle.Exists()) return false;
        try
        {
            int vehicleClass = Function.Call<int>(Hash.GET_VEHICLE_CLASS, vehicle.Handle);

            // 8 = motorcycles, 13 = cycles/bikes, 14 = boats, 15 = helicopters,
            // 16 = planes, 21 = trains. Esses não recebem "cinto".
            if (vehicleClass == 8 || vehicleClass == 13 || vehicleClass == 14 ||
                vehicleClass == 15 || vehicleClass == 16 || vehicleClass == 21)
            {
                return false;
            }
        }
        catch { }
        return true;
    }

    private void ResetNeedForSpeedPlayerSafety()
    {
        try
        {
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists()) return;

            // Garante que o Need For Speed não deixe invencibilidade presa.
            // Se o efeito separado "invincible" estiver ativo, ele continua respeitado.
            if (_invincibleUntilGameTime <= 0 || Game.GameTime > _invincibleUntilGameTime)
            {
                try { player.IsInvincible = false; } catch { }
            }

            try { Function.Call(unchecked((Hash)0xB128377056A54E2A), player.Handle, true); } catch { } // SET_PED_CAN_RAGDOLL
            try { Function.Call(unchecked((Hash)0x1913FE4CBF41C463), player.Handle, 32, true); } catch { } // CanFlyThroughWindscreen volta ao padrão
        }
        catch { }
    }

    private void ResetNeedForSpeed()
    {
        try
        {
            Vehicle current = Game.Player.Character.CurrentVehicle;
            if (current != null && current.Exists()) ResetNeedForSpeedVehicle(current);

            Vehicle[] vehicles = World.GetNearbyVehicles(Game.Player.Character, 390f);
            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle == null || !vehicle.Exists()) continue;
                ResetNeedForSpeedVehicle(vehicle);
            }

            ResetNeedForSpeedPlayerSafety();
        }
        catch { }
    }

    private void ResetNeedForSpeedVehicle(Vehicle vehicle)
    {
        if (vehicle == null || !vehicle.Exists()) return;
        try { Function.Call(unchecked((Hash)0x93A3996368C94158), vehicle.Handle, 0f); } catch { }
        try { Function.Call(unchecked((Hash)0xB59E4BD37AE292DB), vehicle.Handle, 1f); } catch { }
    }

    private void FlipVehicle()
    {
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null) { Notify("LivePlay: entre em um veículo"); return; }
        vehicle.Rotation = new Vector3(0f, 0f, vehicle.Rotation.Z);
        vehicle.PlaceOnGround();
        Notify("LivePlay: veículo desvirado");
    }

    private void BreakVehicle()
    {
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null) { Notify("LivePlay: entre em um veículo"); return; }
        vehicle.EngineHealth = -4000f;
        vehicle.IsEngineRunning = false;
        Notify("LivePlay: motor quebrado");
    }

    private void DeletePlayerVehicle()
    {
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null) { Notify("LivePlay GTA: entre em um veículo"); return; }
        vehicle.Delete();
        Notify("LivePlay GTA: veículo removido");
    }

    private void ExplodeCurrentVehicle()
    {
        Ped player = Game.Player.Character;
        Vehicle currentVehicle = null;
        try
        {
            if (player != null && player.Exists() && player.IsInVehicle()) currentVehicle = player.CurrentVehicle;
        }
        catch { currentVehicle = null; }

        int exploded = 0;
        Vehicle[] vehicles = World.GetNearbyVehicles(player, 175f);
        foreach (Vehicle vehicle in vehicles)
        {
            if (vehicle == null || !vehicle.Exists()) continue;
            if (currentVehicle != null && currentVehicle.Exists() && vehicle.Handle == currentVehicle.Handle) continue;
            if (exploded++ >= 45) break;

            try { World.AddExplosion(vehicle.Position, ExplosionType.Grenade, 4.1f, 0.9f); } catch { }
        }

        if (exploded == 0)
        {
            Notify("LivePlay GTA: nenhum veículo próximo para explodir");
            return;
        }

        Notify("LivePlay GTA: veículos próximos explodidos");
    }


    private void SetWeather(string weather)
    {
        string value = (weather ?? "THUNDER").ToUpperInvariant();
        Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, value);
        Notify("LivePlay: clima " + value);
    }

    private void SetTime(int hour)
    {
        int clamped = Clamp(hour, 0, 23);
        Function.Call(Hash.NETWORK_OVERRIDE_CLOCK_TIME, clamped, 0, 0);
        Notify("LivePlay: hora " + clamped + ":00");
    }


    private void StartNightVision()
    {
        Function.Call(Hash.SET_NIGHTVISION, true);
        _nightVisionUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        Notify("LivePlay GTA: visão noturna por 15s");
    }

    private void StartHeatVision()
    {
        Function.Call(Hash.SET_SEETHROUGH, true);
        _heatVisionUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        Notify("LivePlay GTA: visão térmica por 15s");
    }

    private void HideRadarTimed()
    {
        Function.Call(Hash.DISPLAY_RADAR, false);
        _radarHiddenUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        Notify("LivePlay GTA: radar oculto por 15s");
    }

    private void HideHudTimed()
    {
        Function.Call(Hash.DISPLAY_RADAR, false);
        Function.Call(Hash.DISPLAY_HUD, false);
        _radarHiddenUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        _hudHiddenUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        Notify("LivePlay GTA: HUD oculto por 15s");
    }

    private void CaptureBlackoutRestoreTime()
    {
        try { _blackoutRestoreHour = Clamp(Function.Call<int>(Hash.GET_CLOCK_HOURS), 0, 23); } catch { _blackoutRestoreHour = 12; }
        try { _blackoutRestoreMinute = Clamp(Function.Call<int>(Hash.GET_CLOCK_MINUTES), 0, 59); } catch { _blackoutRestoreMinute = 0; }
        try { _blackoutRestoreSecond = Clamp(Function.Call<int>(Hash.GET_CLOCK_SECONDS), 0, 59); } catch { _blackoutRestoreSecond = 0; }
        _blackoutRestoreTimeCaptured = true;
    }

    private void StartBlackout()
    {
        if (_blackoutUntilGameTime <= 0 || !_blackoutRestoreTimeCaptured)
        {
            CaptureBlackoutRestoreTime();
        }

        try { Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, true); } catch { }
        try { Function.Call(Hash.NETWORK_OVERRIDE_CLOCK_TIME, 0, 0, 0); } catch { }
        try { Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, "THUNDER"); } catch { }
        _blackoutUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        Notify("LivePlay GTA: blackout por 15s");
    }

    private void StopBlackout()
    {
        StopBlackout(true);
    }

    private void StopBlackout(bool manual)
    {
        try { Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, false); } catch { }

        if (_blackoutRestoreTimeCaptured)
        {
            try { Function.Call(Hash.NETWORK_OVERRIDE_CLOCK_TIME, _blackoutRestoreHour, _blackoutRestoreMinute, _blackoutRestoreSecond); } catch { }
        }

        try { Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, "CLEAR"); } catch { }
        _blackoutUntilGameTime = 0;
        _blackoutRestoreTimeCaptured = false;
        Notify(manual ? "LivePlay GTA: blackout off" : "LivePlay GTA: blackout encerrado e horário restaurado");
    }

    private void ExecuteLivePlayEffectSlug(string slug)
    {
        slug = NormalizeSlug(slug);
        if (string.IsNullOrWhiteSpace(slug)) return;

        // Todos os comandos vindos do seletor chegam aqui como "lp <slug>".
        // Os efeitos abaixo cobrem por família de comando. Assim nenhum item do seletor fica morto.
        if (slug.Contains("nothing") || slug == "afk" || slug.Contains("pause") || slug.Contains("no_chaos")) { Notify("LivePlay GTA: " + Pretty(slug)); return; }

        // Correções específicas por ID do seletor GTA.
        // Esta parte evita que comandos com nomes parecidos caiam em uma regra genérica errada.
        if (slug == "earthquake") { Earthquake(); return; }
        if (slug == "drunk") { Drunk(); return; }
        if (slug == "ragdoll") { Ragdoll(3000); return; }
        if (slug == "launch_player") { LaunchPlayer(); return; }
        if (slug == "skydive") { Skydive(); return; }
        if (slug == "super_jump") { SuperJump(); return; }
        if (slug == "heal") { Heal(); return; }
        if (slug == "armor") { Armor(); return; }
        if (slug == "kill_player") { DamagePlayer(slug); return; }
        if (slug == "subtract_health") { DamagePlayer(slug); return; }
        if (slug == "invincible") { InvinciblePlayer(); return; }

        if (slug == "wanted_1") { Game.Player.WantedLevel = 1; Notify("LivePlay GTA: wanted 1"); return; }
        if (slug == "wanted_2") { Game.Player.WantedLevel = Math.Min(5, Math.Max(2, Game.Player.WantedLevel + 2)); Notify("LivePlay GTA: wanted " + Game.Player.WantedLevel); return; }
        if (slug == "wanted_3") { Game.Player.WantedLevel = 3; Notify("LivePlay GTA: wanted 3"); return; }
        if (slug == "wanted_5" || slug == "max_wanted") { Game.Player.WantedLevel = 5; Notify("LivePlay GTA: wanted 5"); return; }
        if (slug == "clear_wanted" || slug == "never_wanted") { Game.Player.WantedLevel = 0; Notify("LivePlay GTA: procurado removido"); return; }

        if (slug == "spawn_random_vehicle") { SpawnRandomVehicle(); return; }
        if (slug == "spawn_adder") { SpawnVehicle("adder"); return; }
        if (slug == "spawn_sultan") { SpawnVehicle("sultan"); return; }
        if (slug == "spawn_rhino") { SpawnVehicle("rhino"); return; }
        if (slug == "spawn_bmx") { SpawnVehicle("bmx"); return; }
        if (slug == "spawn_faggio") { SpawnVehicle("faggio"); return; }
        if (slug == "spawn_buzzard") { SpawnVehicle("buzzard"); return; }
        if (slug == "spawn_bus") { SpawnVehicle("bus"); return; }
        if (slug == "spawn_blimp") { SpawnVehicle("blimp"); return; }
        if (slug == "spawn_dump") { SpawnVehicle("dump"); return; }
        if (slug == "spawn_monster") { SpawnVehicle("monster"); return; }
        if (slug == "spawn_cargo_plane") { SpawnVehicle("cargoplane"); return; }
        if (slug == "spawn_boat") { SpawnVehicle("dinghy"); return; }
        if (slug == "repair_current_vehicle") { RepairVehicle(); return; }
        if (slug == "boost_vehicle") { BoostVehicle(); return; }
        if (slug == "need_for_speed") { NeedForSpeed(); return; }
        if (slug == "flip_vehicle") { FlipVehicle(); return; }
        if (slug == "break_vehicle_engine") { BreakVehicle(); return; }
        if (slug == "delete_player_vehicle") { DeletePlayerVehicle(); return; }
        if (slug == "explode_current_vehicle") { ExplodeCurrentVehicle(); return; }
        if (slug == "pop_vehicle_tires") { PopVehicleTires(); return; }
        if (slug == "launch_nearby_vehicles") { LaunchNearbyVehicles(slug); return; }
        if (slug == "flip_nearby_vehicles") { FlipNearbyVehicles(); return; }
        if (slug == "random_vehicle_complete_tuning") { TuneCurrentVehicle(true); return; }
        if (slug == "random_vehicle_part_tuning") { TuneCurrentVehicle(false); return; }
        if (slug == "invisible_vehicles") { InvisibleVehicles(); return; }

        if (slug == "spawn_attackers") { SpawnMixedKnifeAndPistolAttackers(); return; }
        if (slug == "explosive_zombies") { StartExplosiveZombies(); return; }
        if (slug == "spawn_armed_attackers") { SpawnAttackers("g_m_y_lost_01", 2, true, WeaponHash.Pistol, true, "LivePlay GTA: 2 inimigos com pistola"); return; }
        if (slug == "spawn_single_armed_attacker") { SpawnAttackers("g_m_y_lost_01", 1, true, WeaponHash.Pistol, true, "LivePlay GTA: 1 inimigo armado"); return; }
        if (slug == "spawn_angry_cop") { SpawnAttackers("s_m_y_cop_01", 1, true, WeaponHash.Pistol, true, "LivePlay GTA: policial agressivo"); return; }
        if (slug == "spawn_extreme_angry_cop") { SpawnExtremeAngryCopStrong(); return; }
        if (slug == "spawn_moto_cops") { SpawnMotoGroup("s_m_y_cop_01", true, 2, "LivePlay GTA: moto cops"); return; }
        if (slug == "spawn_moto_bandits") { SpawnMotoGroup("g_m_y_lost_01", true, 2, "LivePlay GTA: moto bandidos"); return; }
        if (slug == "spawn_angry_alien") { SpawnAngryAlienWithRayPistol(); return; }
        if (slug == "spawn_clown" || slug == "spawn_killer_clown") { SpawnKillerClownStrong(); return; }
        if (slug == "spawn_monkey" || slug == "spawn_angry_chimp") { SpawnAngryChimpWithHatchet(); return; }
        if (slug == "spawn_rifle_chimp") { SpawnRifleChimp(); return; }
        if (slug == "spawn_poodle") { SpawnAnimalGroup("a_c_poodle", 3, false, "LivePlay GTA: poodles"); return; }

        if (slug == "give_rpg") { GiveWeapon("rpg"); return; }
        if (slug == "give_sniper") { GiveWeapon("sniper"); return; }
        if (slug == "give_minigun") { GiveWeapon("minigun"); return; }
        if (slug == "give_railgun") { GiveWeapon("railgun"); return; }
        if (slug == "give_shotgun") { GiveWeapon("shotgun"); return; }
        if (slug == "give_carbine") { GiveWeapon("carbine"); return; }
        if (slug == "give_random_weapon") { GiveRandomWeapon(); return; }
        if (slug == "remove_weapons") { Game.Player.Character.Weapons.RemoveAll(); Notify("LivePlay GTA: armas removidas"); return; }
        if (slug == "give_everyone_rpg") { ArmNearbyPeds(slug); return; }
        if (slug == "give_everyone_minigun") { ArmNearbyPeds(slug); return; }

        if (slug == "extra_sunny_weather") { SetWeather("EXTRASUNNY"); return; }
        if (slug == "stormy_weather") { SetWeather("THUNDER"); return; }
        if (slug == "rainy_weather") { SetWeather("RAIN"); return; }
        if (slug == "foggy_weather") { SetWeather("FOGGY"); return; }
        if (slug == "snowy_weather") { SetWeather("SNOW"); return; }
        if (slug == "neutral_weather") { SetWeather("CLEAR"); return; }
        if (slug == "set_time_morning") { SetTime(8); return; }
        if (slug == "set_time_daytime") { SetTime(12); return; }
        if (slug == "set_time_evening") { SetTime(19); return; }
        if (slug == "set_time_night") { SetTime(0); return; }
        if (slug == "blackout_on") { StartBlackout(); return; }
        if (slug == "blackout_off") { StopBlackout(); return; }

        if (slug == "teleport_up") { TeleportEffect(slug); return; }
        if (slug == "teleport_forward") { TeleportEffect(slug); return; }
        if (slug == "teleport_random_location") { TeleportEffect(slug); return; }
        if (slug == "teleport_ls_airport") { TeleportEffect(slug); return; }
        if (slug == "teleport_maze_bank") { TeleportEffect(slug); return; }
        if (slug == "teleport_fort_zancudo") { TeleportEffect(slug); return; }
        if (slug == "teleport_mount_chiliad") { TeleportEffect(slug); return; }

        if (slug == "meteor_shower") { MeteorShower(); return; }
        if (slug == "explosion_ring") { ExplosionRing(); return; }
        if (slug == "fire_chaos") { FireChaos(slug); return; }
        if (slug == "low_gravity") { LowGravityPush(); return; }
        if (slug == "black_hole") { BlackHole(); return; }

        if (slug == "no_radar") { HideRadarTimed(); return; }
        if (slug == "no_hud") { HideHudTimed(); return; }
        if (slug == "night_vision") { StartNightVision(); return; }
        if (slug == "heat_vision") { StartHeatVision(); return; }
        if (slug == "shake_camera") { Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "LARGE_EXPLOSION_SHAKE", 1.2f); Notify("LivePlay GTA: câmera tremendo"); return; }

        if (slug.Contains("earthquake") || slug.Contains("tremor") || slug.Contains("quake")) { Earthquake(); return; }
        if (slug.Contains("meteor")) { MeteorShower(); return; }
        if (slug.Contains("black_hole") || slug.Contains("fake_black_hole") || slug.Contains("gravity_sphere") || slug.Contains("gravity_field")) { GravityChaos(slug); return; }
        if (slug.Contains("explode") || slug.Contains("detonate") || slug.Contains("fireworks") || slug.Contains("airstrike")) { ExplosiveChaos(slug); return; }
        if (slug.Contains("ignite") || slug.Contains("flamethrower") || slug.Contains("toast")) { FireChaos(slug); return; }

        if (slug.Contains("wanted") || slug.Contains("bad_boys")) { WantedFromSlug(slug); return; }
        if (slug.Contains("heal") || slug.Contains("hesoyam") || slug.Contains("health")) { Heal(); return; }
        if (slug.Contains("armor")) { Armor(); return; }
        if (slug.Contains("kill_player") || slug.Contains("suicide") || slug.Contains("one_hit") || slug.Contains("subtract_health")) { DamagePlayer(slug); return; }
        if (slug.Contains("invincible") || slug.Contains("invincibility") || slug.Contains("immortality")) { InvinciblePlayer(); return; }
        if (slug.Contains("drunk") || slug.Contains("sick") || slug.Contains("lsd")) { Drunk(); return; }
        if (slug.Contains("ragdoll") || slug.Contains("slap") || slug.Contains("launch_player") || slug.Contains("fling_player")) { LaunchPlayer(); Ragdoll(1800); return; }
        if (slug.Contains("super_jump")) { SuperJump(); return; }
        if (slug.Contains("jump") || slug.Contains("rocket_man") || slug.Contains("skydive")) { LaunchPlayer(); return; }
        if (slug.Contains("money") || slug.Contains("poor_boy") || slug.Contains("cryptocurrency")) { MoneyEffect(slug); return; }
        if (slug.Contains("clothing") || slug.Contains("clothes") || slug.Contains("famous")) { Notify("LivePlay GTA: " + Pretty(slug)); Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "SMALL_EXPLOSION_SHAKE", 0.35f); return; }

        if (slug.Contains("weapon") || slug.Contains("rpg") || slug.Contains("sniper") || slug.Contains("minigun") || slug.Contains("railgun") || slug.Contains("stun_gun") || slug.Contains("atomizer") || slug.Contains("combat") || slug.Contains("nailgun") || slug.Contains("recoil") || slug.Contains("aimbot")) { WeaponEffect(slug); return; }
        if (slug.Contains("remove_weapons")) { Game.Player.Character.Weapons.RemoveAll(); Notify("LivePlay GTA: armas removidas"); return; }

        if (slug.Contains("spawn") || slug.Contains("companion") || slug.Contains("npc") || slug.Contains("ped") || slug.Contains("cop") || slug.Contains("bandit") || slug.Contains("alien") || slug.Contains("monkey") || slug.Contains("chimp") || slug.Contains("animal") || slug.Contains("dog") || slug.Contains("poodle") || slug.Contains("pigeon") || slug.Contains("clown") || slug.Contains("mercenaries") || slug.Contains("minions") || slug.Contains("zombie"))
        {
            SpawnEffect(slug);
            return;
        }

        if (slug.Contains("vehicle") || slug.Contains("car") || slug.Contains("traffic") || slug.Contains("bike") || slug.Contains("moto") || slug.Contains("rhino") || slug.Contains("adder") || slug.Contains("dump") || slug.Contains("monster") || slug.Contains("bmx") || slug.Contains("bus") || slug.Contains("blimp") || slug.Contains("buzzard") || slug.Contains("plane") || slug.Contains("tug") || slug.Contains("faggio") || slug.Contains("trailer") || slug.Contains("wheel") || slug.Contains("tire") || slug.Contains("engine") || slug.Contains("flying_cars") || slug.Contains("tow") || slug.Contains("sultan") || slug.Contains("ufo") || slug.Contains("submersible") || slug.Contains("boat"))
        {
            VehicleEffect(slug);
            return;
        }

        if (slug.Contains("weather") || slug.Contains("snow") || slug.Contains("storm") || slug.Contains("fog") || slug.Contains("sunny") || slug.Contains("rain") || slug.Contains("thunder") || slug.Contains("blackout") || slug.Contains("time") || slug.Contains("morning") || slug.Contains("daytime") || slug.Contains("evening") || slug.Contains("night") || slug.Contains("drought")) { WorldEffect(slug); return; }
        if (slug.Contains("teleport") || slug.Contains("waypoint") || slug.Contains("chiliad") || slug.Contains("airport") || slug.Contains("maze_bank") || slug.Contains("zancudo") || slug.Contains("heaven") || slug.Contains("checkpoint")) { TeleportEffect(slug); return; }
        if (slug.Contains("hud") || slug.Contains("radar") || slug.Contains("phone") || slug.Contains("screen") || slug.Contains("vision") || slug.Contains("camera") || slug.Contains("pitch") || slug.Contains("fov") || slug.Contains("colors") || slug.Contains("noire") || slug.Contains("textureless") || slug.Contains("potato") || slug.Contains("static")) { VisualEffect(slug); return; }
        if (slug.Contains("arena") || slug.Contains("parkour")) { ArenaEffect(slug); return; }

        GenericChaos(slug);
    }

    private static string NormalizeSlug(string value)
    {
        return Regex.Replace((value ?? "").Trim().ToLowerInvariant(), "[^a-z0-9]+", "_").Trim('_');
    }

    private static string Pretty(string slug)
    {
        return Regex.Replace(slug ?? "", "_+", " ");
    }

    private void GenericChaos(string slug)
    {
        Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "LARGE_EXPLOSION_SHAKE", 0.65f);
        Game.Player.Character.ApplyForce(new Vector3(0f, 0f, 4f));
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void WantedFromSlug(string slug)
    {
        if (slug.Contains("never") || slug.Contains("decrease") || slug.Contains("clear") || slug.Contains("fake_wanted")) Game.Player.WantedLevel = 0;
        else if (slug.Contains("5") || slug.Contains("max")) Game.Player.WantedLevel = 5;
        else if (slug.Contains("3")) Game.Player.WantedLevel = 3;
        else if (slug.Contains("2")) Game.Player.WantedLevel = Math.Min(5, Game.Player.WantedLevel + 2);
        else if (slug.Contains("1")) Game.Player.WantedLevel = 1;
        else Game.Player.WantedLevel = Math.Min(5, Game.Player.WantedLevel + 1);
        Notify("LivePlay GTA: wanted " + Game.Player.WantedLevel);
    }

    private void DamagePlayer(string slug)
    {
        Ped player = Game.Player.Character;
        if (slug.Contains("kill") || slug.Contains("suicide") || slug.Contains("one_hit")) player.Health = 0;
        else player.Health = Math.Max(1, player.Health - 40);
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void InvinciblePlayer()
    {
        Ped player = Game.Player.Character;
        player.IsInvincible = true;
        _invincibleUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        Notify("LivePlay GTA: invencível por 15s");
    }

    private void MoneyEffect(string slug)
    {
        // GTA V story mode não tem API segura para dinheiro em todas as builds; damos feedback visual.
        Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "SMALL_EXPLOSION_SHAKE", 0.3f);
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void WeaponEffect(string slug)
    {
        if (slug.Contains("remove")) { Game.Player.Character.Weapons.RemoveAll(); Notify("LivePlay GTA: armas removidas"); return; }
        if (slug.Contains("everyone")) { ArmNearbyPeds(slug); return; }
        if (slug.Contains("random")) { GiveRandomWeapon(); return; }
        if (slug.Contains("rpg") || slug.Contains("rocket")) GiveWeapon("rpg");
        else if (slug.Contains("sniper")) GiveWeapon("sniper");
        else if (slug.Contains("minigun")) GiveWeapon("minigun");
        else if (slug.Contains("shotgun")) GiveWeapon("shotgun");
        else if (slug.Contains("railgun")) GiveWeapon("railgun");
        else if (slug.Contains("stun_gun") || slug.Contains("stun")) GiveWeapon("stun");
        else GiveWeapon("carbine");
    }

    private void GiveRandomWeapon()
    {
        string[] weapons = new[] { "pistol", "smg", "shotgun", "carbine", "sniper", "rpg", "minigun", "railgun", "stun" };
        GiveWeapon(RandomChoice(weapons));
    }

    private void ArmNearbyPeds(string slug)
    {
        Ped player = Game.Player.Character;
        WeaponHash weapon = WeaponHash.CarbineRifle;
        if (slug.Contains("minigun")) weapon = WeaponHash.Minigun;
        else if (slug.Contains("rpg") || slug.Contains("rocket")) weapon = WeaponHash.RPG;
        else if (slug.Contains("shotgun")) weapon = WeaponHash.PumpShotgun;
        else if (slug.Contains("sniper")) weapon = WeaponHash.SniperRifle;
        else if (slug.Contains("smg")) weapon = WeaponHash.SMG;

        int armed = 0;
        Ped[] peds = World.GetNearbyPeds(player, 45f);
        foreach (Ped ped in peds)
        {
            if (ped == null || !ped.Exists() || ped == player) continue;
            ped.Weapons.Give(weapon, 600, true, true);
            if (!slug.Contains("friendly")) ped.Task.FightAgainst(player);
            armed++;
        }
        Notify(armed > 0 ? "LivePlay GTA: NPCs armados com " + Pretty(slug) : "LivePlay GTA: nenhum NPC próximo");
    }

    private void SpawnEffect(string slug)
    {
        if (slug.Contains("extreme_angry_cop")) { SpawnExtremeAngryCopStrong(); return; }
        if (slug.Contains("angry_cop")) { SpawnAttackers("s_m_y_cop_01", 1, true, WeaponHash.Pistol, true, "LivePlay GTA: policial agressivo"); return; }
        if (slug.Contains("alien")) { SpawnAngryAlienWithRayPistol(); return; }
        if (slug.Contains("clown")) { SpawnKillerClownStrong(); return; }
        if (slug == "spawn_moto_cops") { SpawnMotoGroup("s_m_y_cop_01", true, 2, "LivePlay GTA: moto cops"); return; }
        if (slug == "spawn_moto_bandits") { SpawnMotoGroup("g_m_y_lost_01", true, 2, "LivePlay GTA: moto bandidos"); return; }
        if (slug == "spawn_random_vehicle") { SpawnRandomVehicle(); return; }
        if (slug.Contains("vehicle") || slug.Contains("car") || slug.Contains("bike") || slug.Contains("moto") || slug.Contains("rhino") || slug.Contains("adder") || slug.Contains("plane") || slug.Contains("bus") || slug.Contains("blimp") || slug.Contains("buzzard") || slug.Contains("boat")) { VehicleEffect(slug); return; }

        string ped = "g_m_y_lost_01";
        if (slug.Contains("cop") || slug.Contains("police")) ped = "s_m_y_cop_01";
        else if (slug.Contains("swat") || slug.Contains("juggernaut")) ped = "s_m_y_swat_01";
        else if (slug.Contains("alien") || slug.Contains("space")) ped = "s_m_m_movalien_01";
        else if (slug.Contains("chimp") || slug.Contains("monkey")) ped = "a_c_chimp";
        else if (slug.Contains("dog") || slug.Contains("doggo") || slug.Contains("poodle")) ped = "a_c_poodle";
        else if (slug.Contains("rabbit")) ped = "a_c_rabbit_01";
        else if (slug.Contains("pigeon")) ped = "a_c_pigeon";
        else if (slug.Contains("cat")) ped = "a_c_cat_01";
        else if (slug.Contains("clown")) ped = "s_m_y_clown_01";

        int count = slug.Contains("army") || slug.Contains("squad") || slug.Contains("everyone") || slug.Contains("all_peds") ? 6 : slug.Contains("couple") ? 2 : 3;
        for (int i = 0; i < count; i++) SpawnPed(ped);
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void VehicleEffect(string slug)
    {
        Ped player = Game.Player.Character;
        Vehicle current = player.CurrentVehicle;

        if (slug.Contains("complete_tuning")) { TuneCurrentVehicle(true); return; }
        if (slug.Contains("part_tuning") || slug.Contains("tuning")) { TuneCurrentVehicle(false); return; }
        if (slug.Contains("random_vehicle")) { SpawnRandomVehicle(); return; }
        if (slug.Contains("repair")) { RepairVehicle(); return; }
        if (slug.Contains("need_for_speed")) { NeedForSpeed(); return; }
        if (slug.Contains("boost") || slug.Contains("speed") || slug.Contains("acceleration") || slug.Contains("nitro")) { BoostVehicle(); return; }
        if (slug.Contains("flip") || slug.Contains("turtle")) { if (slug.Contains("nearby")) FlipNearbyVehicles(); else FlipVehicle(); return; }
        if (slug.Contains("delete") || slug.Contains("remove_current") || slug.Contains("remove_spawned") || slug.Contains("remove_current_vehicle")) { if (current != null) current.Delete(); Notify("LivePlay GTA: veículo removido"); return; }
        if (slug.Contains("explode") || slug.Contains("detonate")) { ExplodeCurrentVehicle(); return; }
        if (slug.Contains("break") || slug.Contains("engine")) { BreakVehicle(); return; }
        if (slug.Contains("tire") || slug.Contains("wheel")) { PopVehicleTires(); return; }
        if (slug.Contains("low_gravity")) { LowGravityPush(); return; }
        if (slug.Contains("launch") || slug.Contains("gravity") || slug.Contains("flying")) { LaunchNearbyVehicles(slug); return; }

        string model = "adder";
        if (slug.Contains("rhino") || slug.Contains("tank")) model = "rhino";
        else if (slug.Contains("dump")) model = "dump";
        else if (slug.Contains("monster")) model = "monster";
        else if (slug.Contains("bmx")) model = "bmx";
        else if (slug.Contains("tug")) model = "tug";
        else if (slug.Contains("cargo_plane")) model = "cargoplane";
        else if (slug.Contains("plane")) model = "stunt";
        else if (slug.Contains("bus") || slug.Contains("party_bus")) model = "bus";
        else if (slug.Contains("blimp")) model = "blimp";
        else if (slug.Contains("buzzard") || slug.Contains("helicopter")) model = "buzzard";
        else if (slug.Contains("faggio") || slug.Contains("scooter")) model = "faggio";
        else if (slug.Contains("bike") || slug.Contains("moto") || slug.Contains("biker")) model = "bati";
        else if (slug.Contains("sultan")) model = "sultan";
        else if (slug.Contains("boat") || slug.Contains("submersible")) model = "dinghy";
        else if (slug.Contains("ufo")) model = "blimp";
        SpawnVehicle(model);
    }

    private void SpawnRandomVehicle()
    {
        // Usa todos os veículos conhecidos pelo ScriptHookVDotNet/GTA em vez de só a lista curta do seletor.
        // Se algum modelo/DLC não carregar nessa instalação, ele tenta outro automaticamente.
        Array vehicles = Enum.GetValues(typeof(VehicleHash));
        for (int attempt = 0; attempt < 28; attempt++)
        {
            try
            {
                VehicleHash selected = (VehicleHash)vehicles.GetValue(RandomInt(0, vehicles.Length));
                string name = selected.ToString();
                if (!IsRandomVehicleAllowed(name)) continue;

                Model model = new Model((int)selected);
                if (TrySpawnVehicleModel(model, name, false)) return;
            }
            catch { }
        }

        // Fallback se o enum escolher muitos modelos bloqueados/indisponíveis.
        string[] fallbackModels = new[]
        {
            "adder", "sultan", "bati", "faggio", "bmx", "bus", "buzzard", "monster", "dump", "dinghy",
            "zentorno", "t20", "osiris", "entityxf", "turismor", "infernus", "comet2", "elegy2", "buffalo", "dominator",
            "ninef", "banshee", "coquette", "feltzer2", "futo", "kuruma", "mesa", "sandking", "rebel", "police"
        };
        SpawnVehicle(RandomChoice(fallbackModels));
    }

    private static bool IsRandomVehicleAllowed(string modelName)
    {
        string name = (modelName ?? "").ToLowerInvariant();
        if (name.Length == 0) return false;

        // Esses existem no enum, mas não são bons para colocar o jogador dentro automaticamente.
        if (name.Contains("trailer") || name.Contains("freight") || name.Contains("metrotrain") || name.Contains("cablecar")) return false;
        if (name.Contains("tankercar") || name == "tanker" || name == "tanker2" || name == "armytanker") return false;
        if (name == "tr2" || name == "tr3" || name == "tr4" || name == "trflat" || name == "tvtrailer") return false;
        if (name == "armytrailer" || name == "armytrailer2" || name == "docktrailer" || name == "proptrailer" || name == "raketrailer" || name == "graintrailer") return false;

        return true;
    }

    private void LowGravityPush()
    {
        Ped player = Game.Player.Character;
        Vehicle currentVehicle = null;
        try
        {
            if (player != null && player.Exists() && player.IsInVehicle()) currentVehicle = player.CurrentVehicle;
        }
        catch { currentVehicle = null; }

        float radius = 150f;
        int vehicleLimit = 90;

        if (currentVehicle != null && currentVehicle.Exists())
        {
            currentVehicle.ApplyForce(new Vector3(0f, 0f, 24f));
        }
        else if (player != null && player.Exists())
        {
            player.ApplyForce(new Vector3(0f, 0f, 16f));
        }

        int count = 0;
        foreach (Vehicle v in World.GetNearbyVehicles(player, radius))
        {
            if (v == null || !v.Exists()) continue;
            if (currentVehicle != null && currentVehicle.Exists() && v.Handle == currentVehicle.Handle) continue;
            if (count++ >= vehicleLimit) break;

            v.ApplyForce(new Vector3(0f, 0f, 17f));
        }

        Notify("LivePlay GTA: low gravity push");
    }

    private void TuneCurrentVehicle(bool complete)
    {
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null) { Notify("LivePlay GTA: entre em um veículo"); return; }

        Function.Call(Hash.SET_VEHICLE_MOD_KIT, vehicle.Handle, 0);
        if (complete)
        {
            int[] modTypes = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14, 15, 16 };
            foreach (int modType in modTypes) ApplyBestVehicleMod(vehicle, modType);
            Function.Call(Hash.TOGGLE_VEHICLE_MOD, vehicle.Handle, 18, true);
            Function.Call(Hash.SET_VEHICLE_WINDOW_TINT, vehicle.Handle, RandomInt(0, 6));
            Function.Call(Hash.SET_VEHICLE_COLOURS, vehicle.Handle, RandomInt(0, 160), RandomInt(0, 160));
            Function.Call(Hash.SET_VEHICLE_EXTRA_COLOURS, vehicle.Handle, RandomInt(0, 160), RandomInt(0, 160));
            vehicle.Repair();
            Notify("LivePlay GTA: tunagem completa aplicada");
            return;
        }

        int[] partTypes = new[] { 0, 1, 2, 3, 4, 6, 7, 11, 12, 13, 15, 16, 18 };
        int selected = RandomChoice(partTypes);
        if (selected == 18) Function.Call(Hash.TOGGLE_VEHICLE_MOD, vehicle.Handle, 18, true);
        else ApplyRandomVehicleMod(vehicle, selected);
        Notify("LivePlay GTA: peça/tunagem alterada");
    }

    private void ApplyBestVehicleMod(Vehicle vehicle, int modType)
    {
        int count = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, vehicle.Handle, modType);
        if (count <= 0) return;
        Function.Call(Hash.SET_VEHICLE_MOD, vehicle.Handle, modType, count - 1, false);
    }

    private void ApplyRandomVehicleMod(Vehicle vehicle, int modType)
    {
        int count = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, vehicle.Handle, modType);
        if (count <= 0) return;
        Function.Call(Hash.SET_VEHICLE_MOD, vehicle.Handle, modType, RandomInt(0, count), false);
    }

    private void PopVehicleTires()
    {
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null) { Notify("LivePlay GTA: sem veículo"); return; }
        for (int i = 0; i < 8; i++) Function.Call(Hash.SET_VEHICLE_TYRE_BURST, vehicle.Handle, i, true, 1000f);
        Notify("LivePlay GTA: pneus furados");
    }

    private void FlipNearbyVehicles()
    {
        Ped player = Game.Player.Character;
        int flipped = 0;
        foreach (Vehicle v in World.GetNearbyVehicles(player, 240f))
        {
            if (v == null || !v.Exists()) continue;
            if (flipped++ >= 170) break;
            try { v.Rotation = new Vector3(180f, 0f, v.Rotation.Z); } catch { }
            try { v.ApplyForce(new Vector3(RandomFloat(-2.0f, 2.0f), RandomFloat(-2.0f, 2.0f), 4.5f)); } catch { }
        }
        Notify(flipped > 0 ? "LivePlay GTA: veículos próximos virados" : "LivePlay GTA: nenhum veículo próximo");
    }

    private void LaunchNearbyVehicles(string slug)
    {
        Ped player = Game.Player.Character;
        Vehicle currentVehicle = null;
        try
        {
            if (player != null && player.Exists() && player.IsInVehicle()) currentVehicle = player.CurrentVehicle;
        }
        catch { currentVehicle = null; }

        float radius = slug.Contains("low") ? 150f : 105f;
        float upForce = slug.Contains("low") ? 17f : 30f;
        int limit = slug.Contains("low") ? 90 : 75;

        if (currentVehicle != null && currentVehicle.Exists())
        {
            try { currentVehicle.ApplyForce(new Vector3(0f, 0f, upForce + 8f)); } catch { }
        }

        int count = 0;
        Vehicle[] vehicles = World.GetNearbyVehicles(player, radius);
        foreach (Vehicle v in vehicles)
        {
            if (v == null || !v.Exists()) continue;
            if (currentVehicle != null && currentVehicle.Exists() && v.Handle == currentVehicle.Handle) continue;
            if (count++ >= limit) break;

            Vector3 lateral = v.RightVector * RandomFloat(-2.4f, 2.4f) + v.ForwardVector * RandomFloat(-1.6f, 1.6f);
            v.ApplyForce(lateral + new Vector3(0f, 0f, upForce));
        }

        Notify("LivePlay GTA: " + Pretty(slug));
    }


    private void WorldEffect(string slug)
    {
        if (slug.Contains("blackout")) { if (slug.Contains("off")) StopBlackout(); else StartBlackout(); return; }
        if (slug.Contains("morning")) { SetTime(8); return; }
        if (slug.Contains("daytime") || slug.Contains("noon")) { SetTime(12); return; }
        if (slug.Contains("evening")) { SetTime(19); return; }
        if (slug.Contains("night")) { SetTime(0); return; }
        if (slug.Contains("snow")) { SetWeather("SNOW"); return; }
        if (slug.Contains("storm") || slug.Contains("thunder")) { SetWeather("THUNDER"); return; }
        if (slug.Contains("fog")) { SetWeather("FOGGY"); return; }
        if (slug.Contains("rain")) { SetWeather("RAIN"); return; }
        if (slug.Contains("sunny") || slug.Contains("clear")) { SetWeather("EXTRASUNNY"); return; }
        SetWeather("CLEAR");
    }

    private void TeleportEffect(string slug)
    {
        Ped player = Game.Player.Character;
        Vector3 target = player.Position + player.ForwardVector * 35f + new Vector3(0f, 0f, 1f);
        bool placeOnGround = false;

        if (slug.Contains("up") || slug.Contains("heaven")) target = player.Position + new Vector3(0f, 0f, 120f);
        else if (slug.Contains("airport")) { target = new Vector3(-1034f, -2733f, 20f); placeOnGround = true; }
        else if (slug.Contains("maze")) target = new Vector3(-75f, -818f, 326f);
        else if (slug.Contains("zancudo")) { target = new Vector3(-2047f, 3132f, 32f); placeOnGround = true; }
        else if (slug.Contains("chiliad")) { target = new Vector3(501f, 5604f, 797f); placeOnGround = true; }
        else if (slug.Contains("random")) { target = GetRandomGroundTeleportTarget(player); placeOnGround = true; }

        TeleportPlayerOrVehicle(player, target, placeOnGround);
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private Vector3 GetRandomGroundTeleportTarget(Ped player)
    {
        Vector3 baseTarget = RandomChoice(_randomTeleportGroundLocations);
        if (baseTarget == new Vector3(0f, 0f, 0f)) baseTarget = player.Position;

        Vector3 target = baseTarget + new Vector3(RandomFloat(-42f, 42f), RandomFloat(-42f, 42f), 0f);
        float groundZ;
        if (TryGetGroundZ(target.X, target.Y, out groundZ)) target.Z = groundZ + 1.15f;
        else target.Z = baseTarget.Z + 1.15f;
        return target;
    }

    private bool TryGetGroundZ(float x, float y, out float groundZ)
    {
        groundZ = 0f;
        float[] heights = new float[] { 1200f, 1000f, 850f, 700f, 550f, 400f, 250f, 140f, 80f, 40f };
        for (int i = 0; i < heights.Length; i++)
        {
            try
            {
                OutputArgument outZ = new OutputArgument();
                bool ok = Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, x, y, heights[i], outZ, false, false);
                if (ok)
                {
                    groundZ = outZ.GetResult<float>();
                    return true;
                }
            }
            catch
            {
                try
                {
                    OutputArgument outZFallback = new OutputArgument();
                    bool okFallback = Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, x, y, heights[i], outZFallback, false);
                    if (okFallback)
                    {
                        groundZ = outZFallback.GetResult<float>();
                        return true;
                    }
                }
                catch { }
            }
        }
        return false;
    }

    private void TeleportPlayerOrVehicle(Ped player, Vector3 target, bool placeOnGround)
    {
        if (player == null || !player.Exists()) return;

        try
        {
            if (player.IsInVehicle())
            {
                Vehicle vehicle = player.CurrentVehicle;
                if (vehicle != null && vehicle.Exists())
                {
                    vehicle.Position = target + new Vector3(0f, 0f, 0.8f);
                    if (placeOnGround)
                    {
                        try { vehicle.PlaceOnGround(); } catch { }
                    }
                    try { Function.Call(Hash.SET_PED_INTO_VEHICLE, player.Handle, vehicle.Handle, -1); } catch { }
                    return;
                }
            }
        }
        catch { }

        player.Position = target;
    }

    private static float RandomFloat(float min, float max)
    {
        lock (_random) return (float)(_random.NextDouble() * (max - min) + min);
    }

    private static int RandomInt(int min, int maxExclusive)
    {
        lock (_random) return _random.Next(min, maxExclusive);
    }

    private static T RandomChoice<T>(T[] values)
    {
        if (values == null || values.Length == 0) return default(T);
        return values[RandomInt(0, values.Length)];
    }

    private void VisualEffect(string slug)
    {
        if (slug.Contains("no_hud") || slug.Contains("hud")) HideHudTimed();
        else if (slug.Contains("no_radar") || slug.Contains("radar")) HideRadarTimed();
        if (slug.Contains("night_vision")) StartNightVision();
        if (slug.Contains("heat_vision")) StartHeatVision();
        Function.Call(Hash.SHAKE_GAMEPLAY_CAM, slug.Contains("spinning") ? "DRUNK_SHAKE" : "SMALL_EXPLOSION_SHAKE", 0.8f);
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void ArenaEffect(string slug)
    {
        if (slug.Contains("super_jump")) { SuperJump(); return; }
        if (slug.Contains("add_life") || slug.Contains("add_time")) { LaunchPlayer(); return; }
        ExplosionRing();
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void MeteorShower()
    {
        _meteorUntilGameTime = Game.GameTime + 16000;
        _meteorNextDropGameTime = 0;
        _activeLightMeteors.Clear();
        DropMeteorColumn();
        Notify("LivePlay GTA: meteoros caindo do céu");
    }

    private void DropMeteorColumn()
    {
        try
        {
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists()) return;

            Vector3 target = player.Position + new Vector3(RandomFloat(-132f, 132f), RandomFloat(-132f, 132f), 0f);
            float groundZ;
            if (TryGetGroundZ(target.X, target.Y, out groundZ)) target.Z = groundZ + 0.55f;
            else target.Z = player.Position.Z + 0.55f;

            // Nasce alto e afastado, mas o impacto é sempre calculado no chão.
            Vector3 start = target + new Vector3(RandomFloat(-34f, 34f), RandomFloat(-34f, 34f), RandomFloat(118f, 155f));
            Vector3 direction = target - start;
            float length = direction.Length();
            if (length <= 0.01f) return;
            Vector3 velocity = direction * (RandomFloat(3.9f, 5.3f) / length);

            ActiveLightMeteor meteor = new ActiveLightMeteor();
            meteor.Position = start;
            meteor.Target = target;
            meteor.Velocity = velocity;
            meteor.CreatedAtGameTime = Game.GameTime;
            _activeLightMeteors.Add(meteor);

            while (_activeLightMeteors.Count > MeteorMaxActive) _activeLightMeteors.RemoveAt(0);
        }
        catch { }
    }

    private void MaintainLightMeteors()
    {
        if (_activeLightMeteors.Count == 0) return;

        for (int i = _activeLightMeteors.Count - 1; i >= 0; i--)
        {
            ActiveLightMeteor meteor = _activeLightMeteors[i];
            if (meteor == null)
            {
                _activeLightMeteors.RemoveAt(i);
                continue;
            }

            DrawLightMeteor(meteor);
            meteor.Position += meteor.Velocity;

            bool reached = DistanceBetween(meteor.Position, meteor.Target) <= 5.8f || meteor.Position.Z <= meteor.Target.Z + 2.5f;
            if (!reached && Game.GameTime - meteor.CreatedAtGameTime > MeteorFallTimeoutMs) reached = true;

            if (reached)
            {
                ImpactLightMeteor(meteor);
                _activeLightMeteors.RemoveAt(i);
            }
        }
    }

    private void DrawLightMeteor(ActiveLightMeteor meteor)
    {
        try
        {
            Vector3 p = meteor.Position;
            Vector3 tail = p - meteor.Velocity * 7.5f;

            // Visual apenas no céu: sem círculo, sem linha no chão, sem prop físico pesado.
            Function.Call(Hash.DRAW_MARKER, 28, p.X, p.Y, p.Z, 0f, 0f, 0f, 0f, 0f, 0f, 3.6f, 3.6f, 3.6f, 255, 108, 12, 235, false, true, 2, false, null, null, false);
            Function.Call(Hash.DRAW_MARKER, 28, p.X, p.Y, p.Z, 0f, 0f, 0f, 0f, 0f, 0f, 1.7f, 1.7f, 1.7f, 255, 220, 96, 240, false, true, 2, false, null, null, false);
            Function.Call(Hash.DRAW_LINE, p.X, p.Y, p.Z, tail.X, tail.Y, tail.Z, 255, 96, 0, 235);
            Function.Call(Hash.DRAW_LIGHT_WITH_RANGE, p.X, p.Y, p.Z, 255, 96, 24, 18f, 5.0f);
        }
        catch { }
    }

    private void ImpactLightMeteor(ActiveLightMeteor meteor)
    {
        try
        {
            Vector3 impact = meteor.Target;

            // A única explosão do meteoro: sempre no impacto no chão.
            World.AddExplosion(impact + new Vector3(0f, 0f, 0.25f), ExplosionType.Grenade, 5.2f, 0.24f);
            try { Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "SMALL_EXPLOSION_SHAKE", 0.10f); } catch { }

            try { Function.Call(Hash.START_SCRIPT_FIRE, impact.X, impact.Y, impact.Z, 5, true); } catch { }
            try { Function.Call(Hash.DRAW_LIGHT_WITH_RANGE, impact.X, impact.Y, impact.Z + 1.2f, 255, 94, 20, 16f, 4.6f); } catch { }

            try
            {
                foreach (Vehicle vehicle in World.GetNearbyVehicles(impact, 28f))
                {
                    if (vehicle == null || !vehicle.Exists()) continue;
                    Vector3 away = vehicle.Position - impact;
                    float len = away.Length();
                    if (len > 0.01f) away = away * (1.0f / len);
                    else away = new Vector3(RandomFloat(-1f, 1f), RandomFloat(-1f, 1f), 0f);
                    vehicle.ApplyForce(away * 13.0f + new Vector3(0f, 0f, 5.0f));
                    try { vehicle.EngineHealth = Math.Max(-1000f, vehicle.EngineHealth - 420f); } catch { }
                    try { vehicle.BodyHealth = Math.Max(40f, vehicle.BodyHealth - 340f); } catch { }
                }
            }
            catch { }
        }
        catch { }
    }

    private void GravityChaos(string slug)
    {
        foreach (Vehicle v in World.GetNearbyVehicles(Game.Player.Character, 50f))
        {
            if (v == null || !v.Exists()) continue;
            v.ApplyForce(new Vector3(0f, 0f, slug.Contains("invert") ? -25f : 18f));
        }
        Game.Player.Character.ApplyForce(new Vector3(0f, 0f, slug.Contains("invert") ? -10f : 12f));
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void BlackHole()
    {
        Ped player = Game.Player.Character;

        // Mantem o método estável que não crasha: ApplyForce em pulsos.
        // Durante o black hole, reduz o intervalo do Tick para desenhar o visual todo frame e evitar piscar.
        // Centro mais alto e mais afastado à frente do player para ficar visível no céu.
        try { Interval = 0; } catch { }
        _blackHoleCenter = player.Position + player.ForwardVector * 112f + new Vector3(0f, 0f, 172f);
        _blackHoleUntilGameTime = Game.GameTime + TimedEffectDurationMs;
        _blackHoleNextPulseGameTime = 0;

        try { Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "LARGE_EXPLOSION_SHAKE", 0.58f); } catch { }
        ApplyBlackHolePulse();
        Notify("LivePlay GTA: black hole no ceu ativo");
    }

    private void DrawBlackHoleVisual()
    {
        Vector3 center = _blackHoleCenter;

        // Visual sem partículas pesadas: núcleo escuro maior + aura roxa fixa.
        // DRAW_MARKER precisa ser redesenhado todo frame; por isso o BlackHole coloca Interval = 0 enquanto ativo.
        try { Function.Call(Hash.DRAW_MARKER, 28, center.X, center.Y, center.Z, 0f, 0f, 0f, 0f, 0f, 0f, 92f, 92f, 92f, 85, 0, 170, 96, false, true, 2, false, null, null, false); } catch { }
        try { Function.Call(Hash.DRAW_MARKER, 28, center.X, center.Y, center.Z, 0f, 0f, 0f, 0f, 0f, 0f, 68f, 68f, 68f, 70, 0, 150, 148, false, true, 2, false, null, null, false); } catch { }
        try { Function.Call(Hash.DRAW_MARKER, 28, center.X, center.Y, center.Z, 0f, 0f, 0f, 0f, 0f, 0f, 48f, 48f, 48f, 30, 0, 80, 232, false, true, 2, false, null, null, false); } catch { }
        try { Function.Call(Hash.DRAW_MARKER, 28, center.X, center.Y, center.Z, 0f, 0f, 0f, 0f, 0f, 0f, 31f, 31f, 31f, 0, 0, 0, 255, false, true, 2, false, null, null, false); } catch { }
        try { Function.Call(Hash.DRAW_MARKER, 28, center.X, center.Y, center.Z + 1.4f, 0f, 0f, 0f, 0f, 0f, 0f, 24f, 24f, 24f, 0, 0, 0, 255, false, true, 2, false, null, null, false); } catch { }
        try { Function.Call(Hash.DRAW_LIGHT_WITH_RANGE, center.X, center.Y, center.Z, 105, 0, 190, 76f, 9.8f); } catch { }
        try { Function.Call(Hash.DRAW_LIGHT_WITH_RANGE, center.X, center.Y, center.Z - 14f, 42, 0, 125, 48f, 5.6f); } catch { }
    }

    private void ApplyBlackHolePulse()
    {
        Ped player = Game.Player.Character;
        Vector3 center = _blackHoleCenter;

        DrawBlackHoleVisual();

        Vehicle[] vehicles = World.GetNearbyVehicles(player, 390f);
        int vehicleCount = 0;
        foreach (Vehicle v in vehicles)
        {
            if (v == null || !v.Exists()) continue;
            if (vehicleCount++ >= 86) break;

            Vector3 direction = DirectionTo(v.Position, center);
            float distance = DistanceBetween(v.Position, center);
            float strength = distance > 220f ? 58f : distance > 170f ? 52f : distance > 125f ? 44f : distance > 80f ? 36f : distance > 42f ? 27f : 18f;

            // A força extra para cima cria o efeito de sucção para o céu sem teleportar entidade.
            v.ApplyForce(direction * strength + new Vector3(0f, 0f, 15.8f));
        }

        Ped[] peds = World.GetNearbyPeds(player, 205f);
        int pedCount = 0;
        foreach (Ped ped in peds)
        {
            if (ped == null || !ped.Exists() || ped == player) continue;
            if (pedCount++ >= 86) break;

            Vector3 direction = DirectionTo(ped.Position, center);
            float distance = DistanceBetween(ped.Position, center);
            float strength = distance > 170f ? 35f : distance > 120f ? 30f : distance > 75f ? 24f : distance > 38f ? 17f : 11f;
            try { Function.Call(Hash.SET_PED_TO_RAGDOLL, ped.Handle, 950, 1450, 0, true, true, false); } catch { }
            ped.ApplyForce(direction * strength + new Vector3(0f, 0f, 10.8f));
        }

        if (player != null && player.Exists() && !player.IsInVehicle())
        {
            Vector3 playerDirection = DirectionTo(player.Position, center);
            try { Function.Call(Hash.SET_PED_TO_RAGDOLL, player.Handle, 550, 950, 0, true, true, false); } catch { }
            player.ApplyForce(playerDirection * 21f + new Vector3(0f, 0f, 9.8f));
        }
    }

    private void ReleaseBlackHoleTargets()
    {
        Ped player = Game.Player.Character;
        Vector3 center = _blackHoleCenter;

        try { Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "SMALL_EXPLOSION_SHAKE", 0.35f); } catch { }

        Vehicle[] vehicles = World.GetNearbyVehicles(player, 285f);
        int vehicleCount = 0;
        foreach (Vehicle v in vehicles)
        {
            if (v == null || !v.Exists()) continue;
            if (vehicleCount++ >= 86) break;

            Vector3 away = DirectionTo(center, v.Position);
            if (DistanceBetween(v.Position, center) < 2f) away = new Vector3(RandomFloat(-1f, 1f), RandomFloat(-1f, 1f), 0f);
            v.ApplyForce(away * 39f + new Vector3(RandomFloat(-20f, 20f), RandomFloat(-20f, 20f), -13.5f));
        }

        Ped[] peds = World.GetNearbyPeds(player, 230f);
        int pedCount = 0;
        foreach (Ped ped in peds)
        {
            if (ped == null || !ped.Exists() || ped == player) continue;
            if (pedCount++ >= 86) break;

            Vector3 away = DirectionTo(center, ped.Position);
            if (DistanceBetween(ped.Position, center) < 2f) away = new Vector3(RandomFloat(-1f, 1f), RandomFloat(-1f, 1f), 0f);
            try { Function.Call(Hash.SET_PED_TO_RAGDOLL, ped.Handle, 1200, 1800, 0, true, true, false); } catch { }
            ped.ApplyForce(away * 22f + new Vector3(RandomFloat(-9f, 9f), RandomFloat(-9f, 9f), -8.6f));
        }

        if (player != null && player.Exists() && !player.IsInVehicle())
        {
            Vector3 away = DirectionTo(center, player.Position);
            try { Function.Call(Hash.SET_PED_TO_RAGDOLL, player.Handle, 900, 1400, 0, true, true, false); } catch { }
            player.ApplyForce(away * 15f + new Vector3(RandomFloat(-7f, 7f), RandomFloat(-7f, 7f), -6.4f));
        }
    }

    private static float DistanceBetween(Vector3 a, Vector3 b)
    {
        Vector3 delta = b - a;
        return (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y + delta.Z * delta.Z);
    }

    private static Vector3 DirectionTo(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);
        if (length <= 0.001f) return new Vector3(0f, 0f, 0f);
        return new Vector3(direction.X / length, direction.Y / length, direction.Z / length);
    }

    private void ExplosiveChaos(string slug)
    {
        if (slug.Contains("all") || slug.Contains("nearby") || slug.Contains("ring")) ExplosionRing();
        else ExplodeFront();
    }

    private void FireChaos(string slug)
    {
        Ped player = Game.Player.Character;
        if (player == null || !player.Exists()) return;

        int fires = 0;
        for (int i = 0; i < 10; i++)
        {
            float angle = ((float)i / 10f) * 6.28318f + RandomFloat(-0.22f, 0.22f);
            float radius = RandomFloat(7f, 24f);
            Vector3 pos = player.Position + new Vector3((float)Math.Cos(angle) * radius, (float)Math.Sin(angle) * radius, 0f);
            try
            {
                World.AddExplosion(pos, ExplosionType.Molotov1, 1.75f, 0.5f);
                fires++;
            }
            catch { }
        }

        try { World.AddExplosion(player.Position + player.ForwardVector * 8f, ExplosionType.Molotov1, 1.8f, 0.5f); fires++; } catch { }
        Notify(fires > 0 ? "LivePlay GTA: fogo em volta do personagem" : "LivePlay GTA: " + Pretty(slug));
    }

    private void RegisterNamedPed(Ped ped)
    {
        RegisterNamedPed(ped, _currentNpcOwnerName);
    }

    private void RegisterNamedPed(Ped ped, string name)
    {
        try
        {
            if (ped == null || !ped.Exists()) return;

            string safeName = SanitizeNpcOwnerName(name);
            for (int i = _namedPeds.Count - 1; i >= 0; i--)
            {
                NamedPed current = _namedPeds[i];
                if (current == null || current.Ped == null || !current.Ped.Exists())
                {
                    _namedPeds.RemoveAt(i);
                    continue;
                }

                if (current.Ped.Handle == ped.Handle)
                {
                    current.Name = safeName;
                    return;
                }
            }

            _namedPeds.Add(new NamedPed { Ped = ped, Name = safeName });
        }
        catch { }
    }

    private static string SanitizeNpcOwnerName(string value)
    {
        string name = String.IsNullOrWhiteSpace(value) ? DefaultNpcOwnerName : value.Trim();
        name = Regex.Replace(name, "[\\r\\n\\t]+", " ");
        name = Regex.Replace(name, "\\s+", " ").Trim();
        if (name.Length > 18) name = name.Substring(0, 18);
        return String.IsNullOrWhiteSpace(name) ? DefaultNpcOwnerName : name;
    }

    private void MaintainNamedPeds()
    {
        if (_namedPeds.Count == 0) return;

        Ped player = null;
        try { player = Game.Player.Character; } catch { player = null; }

        for (int i = _namedPeds.Count - 1; i >= 0; i--)
        {
            NamedPed namedPed = _namedPeds[i];
            if (namedPed == null || namedPed.Ped == null || !namedPed.Ped.Exists())
            {
                _namedPeds.RemoveAt(i);
                continue;
            }

            bool dead = false;
            try { dead = namedPed.Ped.IsDead || namedPed.Ped.Health <= 0; } catch { }
            if (dead)
            {
                _namedPeds.RemoveAt(i);
                continue;
            }

            if (player != null && player.Exists())
            {
                float distance = 9999f;
                try { distance = DistanceBetween(player.Position, namedPed.Ped.Position); } catch { }
                if (distance > NamedPedDrawDistance) continue;
            }

            try
            {
                Vector3 labelPos = namedPed.Ped.Position + new Vector3(0f, 0f, 1.18f);
                DrawNpcName3D(labelPos, namedPed.Name);
            }
            catch { }
        }
    }

    private void DrawNpcName3D(Vector3 worldPos, string text)
    {
        try
        {
            OutputArgument screenX = new OutputArgument();
            OutputArgument screenY = new OutputArgument();
            bool onScreen = Function.Call<bool>(unchecked((Hash)0x34E82F05DF2974F5), worldPos.X, worldPos.Y, worldPos.Z, screenX, screenY); // GET_SCREEN_COORD_FROM_WORLD_COORD
            if (!onScreen) return;

            float x = screenX.GetResult<float>();
            float y = screenY.GetResult<float>();
            string label = SanitizeNpcOwnerName(text);

            Function.Call(Hash.SET_TEXT_FONT, 7);
            Function.Call(Hash.SET_TEXT_SCALE, 0.0f, 0.34f);
            Function.Call(Hash.SET_TEXT_COLOUR, 80, 220, 255, 245);
            Function.Call(Hash.SET_TEXT_CENTRE, true);
            Function.Call(Hash.SET_TEXT_OUTLINE);
            Function.Call(Hash.SET_TEXT_DROPSHADOW, 1, 0, 0, 0, 190);
            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, label);
            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y);
        }
        catch { }
    }

    private static void Notify(string message)
    {
        try { GTA.UI.Screen.ShowSubtitle(message, 2500); } catch { }
    }

    private static void WriteLog(string message)
    {
        try
        {
            File.AppendAllText(Path.Combine("scripts", "LivePlayGtaBridge.log"),
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message + Environment.NewLine);
        }
        catch { }
    }

    private void OnAborted(object sender, EventArgs e)
    {
        _running = false;
        try { StopDrunk(); } catch { }
        try { Function.Call(Hash.SET_NIGHTVISION, false); } catch { }
        try { Function.Call(Hash.SET_SEETHROUGH, false); } catch { }
        try { Function.Call(Hash.DISPLAY_HUD, true); } catch { }
        try { Function.Call(Hash.DISPLAY_RADAR, true); } catch { }
        try { Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, false); } catch { }
        try { _namedPeds.Clear(); } catch { }
        try { if (_listener != null) _listener.Stop(); } catch { }
    }

}
