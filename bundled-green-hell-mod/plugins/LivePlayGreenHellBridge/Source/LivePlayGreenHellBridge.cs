// LivePlay Green Hell Bridge - source snapshot
// A DLL empacotada em public/bundled-green-hell-mod/plugins/LivePlayGreenHellBridge.dll Ã© a referÃªncia executÃ¡vel desta versÃ£o.
// Este source Ã© material auxiliar para manutenÃ§Ã£o; recompilar exige teste real dentro do Green Hell antes de substituir a DLL distribuÃ­da.

using BepInEx;
using BepInEx.Configuration;
using System;
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

[BepInPlugin("br.liveplay.greenhell.bridge", "LivePlay Green Hell Bridge", "0.1.2")]
public sealed class LivePlayGreenHellBridge : BaseUnityPlugin
{
    private sealed class GameChatLine
    {
        public string Text;
        public string PrefixText = "";
        public string ActorText = "";
        public string MessageText = "";
        public float ExpiresAt;
        public Color PrefixColor = Color.white;
        public Color ActorColor = Color.white;
        public Color MessageColor = Color.white;
    }

    private readonly ConcurrentQueue<string> _pendingCommands = new ConcurrentQueue<string>();
    private readonly List<GameChatLine> _gameChatLines = new List<GameChatLine>();
    private readonly Dictionary<string, GameObject> _strictCandidateCache = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, float> _strictCandidateCacheAt = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
    private const float StrictCandidateCacheSeconds = 90f;
    private const int GameChatMaxLines = 6;
    private const float GameChatDurationSeconds = 11f;
    private TcpListener _listener;
    private Thread _serverThread;
    private volatile bool _running;
    private int _port = 35953;
    private GUIStyle _titleStyle;
    private GUIStyle _messageStyle;
    private string _toastTitle = "";
    private string _toastMessage = "";
    private float _toastUntil;

    private void Awake()
    {
        try
        {
            _port = Config.Bind("Bridge", "Port", 35953, "Porta HTTP local do LivePlay Green Hell Bridge.").Value;
            StartServer();
            Logger.LogInfo("LivePlay Green Hell Bridge 0.1.2 ativo na porta " + _port);
        }
        catch (Exception ex)
        {
            Logger.LogError("Falha ao iniciar LivePlay Green Hell Bridge: " + ex);
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
        while (safety++ < 3 && _pendingCommands.TryDequeue(out string command))
        {
            ExecuteCommand(command);
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
            GUI.color = new Color(0.03f, 0.09f, 0.04f, 0.92f);
            GUI.Box(rect, GUIContent.none);
            GUI.color = new Color(0.34f, 0.84f, 0.32f, 1f);
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
        _titleStyle.normal.textColor = new Color(0.75f, 0.95f, 0.75f, 1f);
        _messageStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, richText = false, wordWrap = true };
        _messageStyle.normal.textColor = Color.white;
    }

    private void Toast(string message)
    {
        _toastTitle = "LIVEPLAY â€¢ GREEN HELL";
        _toastMessage = message;
        _toastUntil = Time.realtimeSinceStartup + 2.8f;
    }


    private void AddGameChatMessage(string message)
    {
        Color prefixColor = new Color(0.84f, 0.42f, 1f, 1f);
        Color actorColor = new Color(1f, 0.88f, 0.41f, 1f);
        Color messageColor = Color.white;

        string prefixText = "[LivePlay]";
        string actorText = "LivePlay:";
        string bodyText = string.IsNullOrWhiteSpace(message) ? string.Empty : message.Trim();

        Match full = Regex.Match(bodyText, "^#?([0-9a-fA-F]{6})\\|#?([0-9a-fA-F]{6})\\|#?([0-9a-fA-F]{6})\\|([^|]*)\\|([^|]*)\\|(.*)$", RegexOptions.Singleline);
        Match shortPayload = Regex.Match(bodyText, "^#?([0-9a-fA-F]{6})\\|#?([0-9a-fA-F]{6})\\|([^|]*)\\|([^|]*)\\|(.*)$", RegexOptions.Singleline);

        if (full.Success)
        {
            try
            {
                string h1 = full.Groups[1].Value;
                string h2 = full.Groups[2].Value;
                string h3 = full.Groups[3].Value;

                prefixColor = new Color(Convert.ToInt32(h1.Substring(0, 2), 16) / 255f, Convert.ToInt32(h1.Substring(2, 2), 16) / 255f, Convert.ToInt32(h1.Substring(4, 2), 16) / 255f, 1f);
                actorColor = new Color(Convert.ToInt32(h2.Substring(0, 2), 16) / 255f, Convert.ToInt32(h2.Substring(2, 2), 16) / 255f, Convert.ToInt32(h2.Substring(4, 2), 16) / 255f, 1f);
                messageColor = new Color(Convert.ToInt32(h3.Substring(0, 2), 16) / 255f, Convert.ToInt32(h3.Substring(2, 2), 16) / 255f, Convert.ToInt32(h3.Substring(4, 2), 16) / 255f, 1f);

                prefixText = full.Groups[4].Value.Trim();
                actorText = full.Groups[5].Value.Trim();
                bodyText = full.Groups[6].Value.Trim();
            }
            catch
            {
                prefixColor = new Color(0.84f, 0.42f, 1f, 1f);
                actorColor = new Color(1f, 0.88f, 0.41f, 1f);
                messageColor = Color.white;
            }
        }
        else if (shortPayload.Success)
        {
            try
            {
                string h1 = shortPayload.Groups[1].Value;
                string h2 = shortPayload.Groups[2].Value;

                actorColor = new Color(Convert.ToInt32(h1.Substring(0, 2), 16) / 255f, Convert.ToInt32(h1.Substring(2, 2), 16) / 255f, Convert.ToInt32(h1.Substring(4, 2), 16) / 255f, 1f);
                messageColor = new Color(Convert.ToInt32(h2.Substring(0, 2), 16) / 255f, Convert.ToInt32(h2.Substring(2, 2), 16) / 255f, Convert.ToInt32(h2.Substring(4, 2), 16) / 255f, 1f);

                prefixText = shortPayload.Groups[3].Value.Trim();
                actorText = shortPayload.Groups[4].Value.Trim();
                bodyText = shortPayload.Groups[5].Value.Trim();
            }
            catch
            {
                actorColor = new Color(1f, 0.88f, 0.41f, 1f);
                messageColor = Color.white;
            }
        }
        else
        {
            Match oldColor = Regex.Match(bodyText, "^#?([0-9a-fA-F]{6})\\|(.*)$", RegexOptions.Singleline);
            if (oldColor.Success)
            {
                try
                {
                    string h = oldColor.Groups[1].Value;
                    actorColor = new Color(Convert.ToInt32(h.Substring(0, 2), 16) / 255f, Convert.ToInt32(h.Substring(2, 2), 16) / 255f, Convert.ToInt32(h.Substring(4, 2), 16) / 255f, 1f);
                    bodyText = oldColor.Groups[2].Value.Trim();
                }
                catch
                {
                    actorColor = Color.white;
                }
            }

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

        if (PrefixShouldBeHiddenForLiveChat(prefixText, actorText))
        {
            prefixText = string.Empty;
        }

        float now = Time.realtimeSinceStartup;
        _gameChatLines.RemoveAll(line => line == null || line.ExpiresAt <= now);
        _gameChatLines.Add(new GameChatLine
        {
            PrefixText = prefixText,
            ActorText = actorText,
            MessageText = bodyText,
            Text = prefixText + " " + actorText + " " + bodyText,
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
        if (text.Length > 140) text = text.Substring(0, 140);
        return text;
    }

    private void StartServer()
    {
        _running = true;
        _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
        _listener.Start();

        _serverThread = new Thread(ServerLoop)
        {
            IsBackground = true,
            Name = "LivePlayGreenHellBridgeHttp"
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
                try
                {
                    HandleClient(client);
                }
                finally
                {
                    try { client.Close(); } catch { }
                }
            }
            catch (ThreadAbortException)
            {
                // Evita log falso de erro/offline quando o jogo descarrega o plugin.
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
                bool isPing = firstLine.IndexOf("/liveplay/greenhell/ping", StringComparison.OrdinalIgnoreCase) >= 0;
                bool isCommand = firstLine.IndexOf("/liveplay/greenhell/command", StringComparison.OrdinalIgnoreCase) >= 0;

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
                        WriteJson(stream, 400, "{\"ok\":false,\"error\":\"empty_command\",\"bridge\":\"green-hell\",\"version\":\"0.1.2\"}");
                        return;
                    }

                    int repeat = ExtractRepeatCount(request);
                    repeat = Mathf.Clamp(repeat, 1, 50);

                    for (int i = 0; i < repeat; i++)
                    {
                        _pendingCommands.Enqueue(normalized);
                    }

                    Logger.LogInfo("LivePlay Green Hell queued: " + normalized + " repeat=" + repeat);

                    WriteJson(
                        stream,
                        200,
                        "{\"ok\":true,\"queued\":true,\"repeat\":" + repeat + ",\"bridge\":\"green-hell\",\"version\":\"0.1.2\"}"
                    );

                    return;
                    }

                    WriteJson(stream, 200, "{\"ok\":true,\"bridge\":\"green-hell\",\"version\":\"0.1.2\"}");
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao processar requisiÃ§Ã£o LivePlay: " + ex.Message);
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
            Match match = Regex.Match(
                request,
                "\\\"repeat\\\"\\s*:\\s*(\\d+)",
                RegexOptions.IgnoreCase
            );

            if (!match.Success)
            {
                return 1;
            }

            if (int.TryParse(match.Groups[1].Value, out int repeat))
            {
                return Mathf.Clamp(repeat, 1, 50);
            }
        }
        catch { }

        return 1;
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

    private static string NormalizeCommand(string raw)
    {
        string command = (raw ?? string.Empty).Trim();
        if (command.StartsWith("gh:", StringComparison.OrdinalIgnoreCase)) command = command.Substring(3).Trim();
        if (command.StartsWith("greenhell:", StringComparison.OrdinalIgnoreCase)) command = command.Substring("greenhell:".Length).Trim();
        if (command.StartsWith("chat ", StringComparison.OrdinalIgnoreCase)) return "chat " + command.Substring(5).Trim();
        return command.ToLowerInvariant();
    }

    private void ExecuteCommand(string command)
    {
        Logger.LogInfo("LivePlay Green Hell command: " + command);

        if (command.StartsWith("chat ", StringComparison.OrdinalIgnoreCase))
        {
            string message = command.Substring(5).Trim();
            AddGameChatMessage(string.IsNullOrWhiteSpace(message) ? "LivePlay Chat" : message);
            return;
        }

        switch (command)
        {

            case "damage_player":
                TrySetPlayerVitals(-20f, false);
                Toast("Dano aplicado");
                return;
            case "poison_player":
                if (TryApplyNegativeStatusSafe("Poison", new[] { "AddPoison", "SetPoison", "ApplyPoison", "Poison" })) Toast("Veneno enviado");
                else Toast("Veneno nÃ£o encontrado com seguranÃ§a");
                return;
            case "fever_player":
                if (TryApplyNegativeStatusSafe("Fever", new[] { "AddFever", "SetFever", "ApplyFever", "Fever" })) Toast("Febre enviada");
                else Toast("Febre nÃ£o encontrada com seguranÃ§a");
                return;
            case "spawn_snake":
                if (TrySpawnAnimalStrictSafe("Snake", new[] { "snake", "rattlesnake", "viper" })) Toast("Spawn Cobra criado");
                else Toast("Spawn Cobra nÃ£o encontrado com seguranÃ§a");
                return;
            case "spawn_jaguar":
                if (TrySpawnAnimalStrictSafe("Jaguar", new[] { "jaguar", "puma", "cat" })) Toast("Spawn Jaguar criado");
                else Toast("Spawn Jaguar nÃ£o encontrado com seguranÃ§a");
                return;

            default:
                if (TryHandleExtraCommand(command)) return;
                Toast("Comando desconhecido: " + command);
                return;
        }
    }

private int TrySetConditionCoreMembersDeltaOrMax(string label, string[] exactNames, float delta, bool restoreMode)
    {
        int changed = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (!IsPlayerConditionModuleType(type) && !IsPlayerStatusRelatedType(type)) continue;

                foreach (string exact in exactNames)
                {
                    foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!NamesMatchConditionCore(field.Name, exact)) continue;
                        if (MemberLooksUnsafeForConditionDelta(field.Name)) continue;
                        if (TrySetAnyConditionValue(behaviour, field.FieldType, field.Name, () => field.GetValue(behaviour), v => field.SetValue(behaviour, v), delta, restoreMode))
                        {
                            changed++;
                            Logger.LogInfo("condition-delta: " + label + " field " + type.Name + "." + field.Name);
                        }
                    }

                    foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!property.CanRead || !property.CanWrite) continue;
                        if (!NamesMatchConditionCore(property.Name, exact)) continue;
                        if (MemberLooksUnsafeForConditionDelta(property.Name)) continue;
                        if (TrySetAnyConditionValue(behaviour, property.PropertyType, property.Name, () => property.GetValue(behaviour, null), v => property.SetValue(behaviour, v, null), delta, restoreMode))
                        {
                            changed++;
                            Logger.LogInfo("condition-delta: " + label + " property " + type.Name + "." + property.Name);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-delta: falha " + label + ": " + ex.Message);
        }
        return changed;
    }

    private static bool NamesMatchConditionCore(string actual, string expected)
    {
        string a = NormalizeForCompare(actual);
        string e = NormalizeForCompare(expected);
        return a == e || a == "m" + e || a == e + "prop" || a == e + "repl" || a == "m" + e + "prop" || a == "m" + e + "repl";
    }

    private static bool MemberLooksUnsafeForConditionDelta(string memberName)
    {
        string lower = (memberName ?? string.Empty).ToLowerInvariant();
        return lower.Contains("consumption") ||
               lower.Contains("persecond") ||
               lower.Contains("timer") ||
               lower.Contains("time") ||
               lower.Contains("rate") ||
               lower.Contains("multiplier") ||
               lower.Contains("modifier") ||
               lower.Contains("cooldown") ||
               lower.Contains("damage") ||
               lower.Contains("wound") ||
               lower.Contains("bleed") ||
               lower.Contains("burn");
    }

    private bool TrySetAnyConditionValue(object owner, Type valueType, string memberName, Func<object> getter, Action<object> setter, float delta, bool restoreMode)
    {
        try
        {
            if (valueType == typeof(float))
            {
                float current = 0f;
                try { current = Convert.ToSingle(getter()); } catch { }
                float max = FindSafeMaxValue(owner, memberName);
                float target;

                if (restoreMode)
                {
                    if (current >= 0f && current <= 1.5f) target = Mathf.Clamp(current + (delta / 100f), 0f, 1f);
                    else target = max > 0f && max <= 1000f ? Mathf.Min(max, current + delta) : Mathf.Min(100f, current + delta);
                }
                else
                {
                    target = current >= 0f && current <= 1.5f ? 1f : Mathf.Max(1f, delta);
                }

                setter(target);
                Logger.LogInfo("condition-delta: set " + memberName + " " + current + " -> " + target);
                return true;
            }

            if (valueType == typeof(int))
            {
                int current = 0;
                try { current = Convert.ToInt32(getter()); } catch { }
                int target = restoreMode ? Math.Max(current + Mathf.RoundToInt(delta), 100) : Math.Max(1, Mathf.RoundToInt(delta));
                setter(target);
                Logger.LogInfo("condition-delta: set " + memberName + " " + current + " -> " + target);
                return true;
            }

            if (valueType == typeof(bool) && !restoreMode)
            {
                setter(true);
                Logger.LogInfo("condition-delta: set " + memberName + " true");
                return true;
            }

            // Suporte a wrappers/replication props: tenta setar Value/m_Value/current internos.
            object wrapper = null;
            try { wrapper = getter(); } catch { }
            if (wrapper != null && TrySetWrapperNumericValue(wrapper, memberName, delta, restoreMode))
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-delta: set falhou " + memberName + ": " + ex.Message);
        }

        return false;
    }

    private bool TrySetWrapperNumericValue(object wrapper, string outerName, float delta, bool restoreMode)
    {
        try
        {
            Type type = wrapper.GetType();
            string[] names = new[] { "Value", "m_Value", "value", "m_value", "Current", "m_Current" };

            foreach (string name in names)
            {
                FieldInfo f = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (f != null)
                {
                    if (TrySetWrapperMember(wrapper, f.FieldType, outerName + "." + f.Name, () => f.GetValue(wrapper), v => f.SetValue(wrapper, v), delta, restoreMode)) return true;
                }

                PropertyInfo p = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (p != null && p.CanRead && p.CanWrite)
                {
                    if (TrySetWrapperMember(wrapper, p.PropertyType, outerName + "." + p.Name, () => p.GetValue(wrapper, null), v => p.SetValue(wrapper, v, null), delta, restoreMode)) return true;
                }
            }
        }
        catch { }
        return false;
    }

    private bool TrySetWrapperMember(object wrapper, Type valueType, string name, Func<object> getter, Action<object> setter, float delta, bool restoreMode)
    {
        try
        {
            if (valueType == typeof(float))
            {
                float current = 0f;
                try { current = Convert.ToSingle(getter()); } catch { }
                float target = restoreMode
                    ? (current >= 0f && current <= 1.5f ? Mathf.Clamp(current + (delta / 100f), 0f, 1f) : Mathf.Min(100f, current + delta))
                    : (current >= 0f && current <= 1.5f ? 1f : Mathf.Max(1f, delta));
                setter(target);
                Logger.LogInfo("condition-delta: wrapper set " + name + " " + current + " -> " + target);
                return true;
            }

            if (valueType == typeof(int))
            {
                int current = 0;
                try { current = Convert.ToInt32(getter()); } catch { }
                int target = restoreMode ? Math.Max(current + Mathf.RoundToInt(delta), 100) : Math.Max(1, Mathf.RoundToInt(delta));
                setter(target);
                Logger.LogInfo("condition-delta: wrapper set " + name + " " + current + " -> " + target);
                return true;
            }

            if (valueType == typeof(bool) && !restoreMode)
            {
                setter(true);
                Logger.LogInfo("condition-delta: wrapper set " + name + " true");
                return true;
            }
        }
        catch { }
        return false;
    }
    private int TryInvokeExactConditionSetters(string label, string[] names, bool restore)
    {
        int changed = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (!IsPlayerConditionModuleType(type)) continue;

                foreach (string name in names)
                {
                    string[] methodNames = new[]
                    {
                        "Set" + name,
                        "Add" + name,
                        "Restore" + name,
                        "Set" + name + "Level",
                        "Add" + name + "Level"
                    };

                    foreach (string methodName in methodNames)
                    {
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                        {
                            if (!string.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase)) continue;
                            if (TryInvokeConditionNumericMethod(behaviour, method, restore ? 100f : 25f, true))
                            {
                                changed++;
                                Logger.LogInfo("condition-core: " + label + " setter " + type.Name + "." + method.Name);
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-core: setters falharam " + label + ": " + ex.Message);
        }
        return changed;
    }

    private int TryInvokeNegativeConditionMethods(string label, string[] methodNames)
    {
        int changed = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (!IsPlayerConditionModuleType(type) && !IsPlayerStatusRelatedType(type)) continue;

                foreach (string wanted in methodNames)
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!string.Equals(method.Name, wanted, StringComparison.OrdinalIgnoreCase)) continue;
                        if (TryInvokeConditionNumericMethod(behaviour, method, 1f, false))
                        {
                            changed++;
                            Logger.LogInfo("condition-core: " + label + " negative method " + type.Name + "." + method.Name);
                            return changed;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-core: negative methods falharam " + label + ": " + ex.Message);
        }
        return changed;
    }

    private int TryForceConditionExactMembers(string label, string[] exactNames, bool restore, bool negative)
    {
        int changed = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (!IsPlayerConditionModuleType(type) && !IsPlayerStatusRelatedType(type)) continue;

                foreach (string exact in exactNames)
                {
                    foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!string.Equals(field.Name, exact, StringComparison.OrdinalIgnoreCase)) continue;
                        if (TrySetConditionMemberValue(behaviour, field.FieldType, field.Name, () => field.GetValue(behaviour), v => field.SetValue(behaviour, v), restore, negative))
                        {
                            changed++;
                            Logger.LogInfo("condition-core: " + label + " exact field " + type.Name + "." + field.Name);
                        }
                    }

                    foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!property.CanRead || !property.CanWrite) continue;
                        if (!string.Equals(property.Name, exact, StringComparison.OrdinalIgnoreCase)) continue;
                        if (TrySetConditionMemberValue(behaviour, property.PropertyType, property.Name, () => property.GetValue(behaviour, null), v => property.SetValue(behaviour, v, null), restore, negative))
                        {
                            changed++;
                            Logger.LogInfo("condition-core: " + label + " exact property " + type.Name + "." + property.Name);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-core: exact members falharam " + label + ": " + ex.Message);
        }
        return changed;
    }

    private int TryForceConditionKeywordMembers(string label, string[] keywords, bool restore, bool negative)
    {
        int changed = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (!IsPlayerConditionModuleType(type) && !IsPlayerStatusRelatedType(type)) continue;

                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (!MemberHasAnyKeyword(field.Name, keywords)) continue;
                    if (MemberLooksUnsafeForNegativeStatus(field.Name)) continue;
                    if (TrySetConditionMemberValue(behaviour, field.FieldType, field.Name, () => field.GetValue(behaviour), v => field.SetValue(behaviour, v), restore, negative))
                    {
                        changed++;
                        Logger.LogInfo("condition-core: " + label + " keyword field " + type.Name + "." + field.Name);
                    }
                }

                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (!property.CanRead || !property.CanWrite) continue;
                    if (!MemberHasAnyKeyword(property.Name, keywords)) continue;
                    if (MemberLooksUnsafeForNegativeStatus(property.Name)) continue;
                    if (TrySetConditionMemberValue(behaviour, property.PropertyType, property.Name, () => property.GetValue(behaviour, null), v => property.SetValue(behaviour, v, null), restore, negative))
                    {
                        changed++;
                        Logger.LogInfo("condition-core: " + label + " keyword property " + type.Name + "." + property.Name);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-core: keyword members falharam " + label + ": " + ex.Message);
        }
        return changed;
    }

    private bool TrySetConditionMemberValue(object owner, Type valueType, string memberName, Func<object> getter, Action<object> setter, bool restore, bool negative)
    {
        try
        {
            if (valueType == typeof(float))
            {
                float current = 0f;
                try { current = Convert.ToSingle(getter()); } catch { }
                float value = negative ? NegativeStatusValueFor(memberName, current) : SafeRestoredValueFor(memberName, current, FindSafeMaxValue(owner, memberName));
                setter(value);
                return true;
            }

            if (valueType == typeof(int))
            {
                int current = 0;
                try { current = Convert.ToInt32(getter()); } catch { }
                int value = negative ? 1 : (int)Mathf.Round(SafeRestoredValueFor(memberName, current, FindSafeMaxValue(owner, memberName)));
                setter(value);
                return true;
            }

            if (valueType == typeof(bool) && negative)
            {
                setter(true);
                return true;
            }
        }
        catch { }
        return false;
    }

    private float NegativeStatusValueFor(string memberName, float current)
    {
        if (current >= 0f && current <= 1.5f) return 1f;
        return 100f;
    }

    private bool TryInvokeConditionNumericMethod(object target, MethodInfo method, float value, bool restore)
    {
        try
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                method.Invoke(target, null);
                return true;
            }

            if (parameters.Length == 1)
            {
                Type p = parameters[0].ParameterType;
                object arg = null;

                if (p == typeof(float)) arg = value;
                else if (p == typeof(int)) arg = Mathf.RoundToInt(value);
                else if (p == typeof(bool)) arg = true;
                else return false;

                method.Invoke(target, new object[] { arg });
                return true;
            }
        }
        catch { }
        return false;
    }

    private void TryRefreshPlayerConditionModules(string label)
    {
        try
        {
            string[] refreshNames = new[] { "Refresh", "RefreshValues", "UpdateValues", "UpdateStats", "UpdateStatus", "OnStatsChanged", "OnConditionChanged", "UpdateWatch", "UpdateHUD" };

            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (!IsPlayerConditionModuleType(type) && !IsPlayerStatusRelatedType(type)) continue;

                foreach (string refresh in refreshNames)
                {
                    MethodInfo method = type.GetMethod(refresh, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (method == null) continue;
                    if (method.GetParameters().Length != 0) continue;

                    try
                    {
                        method.Invoke(behaviour, null);
                        Logger.LogInfo("condition-core: " + label + " refresh " + type.Name + "." + method.Name);
                    }
                    catch { }
                }
            }
        }
        catch { }
    }

    private static bool IsPlayerConditionModuleType(Type type)
    {
        string name = type.FullName ?? type.Name ?? string.Empty;
        string lower = name.ToLowerInvariant();
        return lower.Contains("playerconditionmodule") ||
               (lower.Contains("player") && lower.Contains("condition"));
    }

    private static bool IsPlayerStatusRelatedType(Type type)
    {
        string name = type.FullName ?? type.Name ?? string.Empty;
        string lower = name.ToLowerInvariant();
        if (lower.Contains("ui") || lower.Contains("camera") || lower.Contains("audio")) return false;
        return lower.Contains("player") &&
               (lower.Contains("status") || lower.Contains("stats") || lower.Contains("disease") || lower.Contains("fever") || lower.Contains("poison") || lower.Contains("condition"));
    }

    private static bool MemberHasAnyKeyword(string memberName, string[] keywords)
    {
        string normalized = NormalizeForCompare(memberName);
        foreach (string keyword in keywords)
        {
            string k = NormalizeForCompare(keyword);
            if (normalized.Contains(k)) return true;
        }
        return false;
    }

    private static bool MemberLooksUnsafeForNegativeStatus(string memberName)
    {
        string lower = (memberName ?? string.Empty).ToLowerInvariant();
        return lower.Contains("timer") ||
               lower.Contains("time") ||
               lower.Contains("rate") ||
               lower.Contains("multiplier") ||
               lower.Contains("modifier") ||
               lower.Contains("cooldown") ||
               lower.Contains("max");
    }
    private int TrySetSafeNumericVitals(string label, string[] targets, bool restore)
    {
        int changed = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                string typeName = type.FullName ?? type.Name ?? string.Empty;
                if (!IsSafeVitalOwner(typeName)) continue;

                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (!IsSafeVitalMember(field.Name, targets)) continue;
                    if (TrySetSafeNumericMember(behaviour, field.FieldType, field.Name, () => field.GetValue(behaviour), v => field.SetValue(behaviour, v)))
                    {
                        changed++;
                        Logger.LogInfo("safe-status: " + label + " field " + type.Name + "." + field.Name);
                    }
                }

                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (!property.CanRead || !property.CanWrite) continue;
                    if (!IsSafeVitalMember(property.Name, targets)) continue;
                    if (TrySetSafeNumericMember(behaviour, property.PropertyType, property.Name, () => property.GetValue(behaviour, null), v => property.SetValue(behaviour, v, null)))
                    {
                        changed++;
                        Logger.LogInfo("safe-status: " + label + " property " + type.Name + "." + property.Name);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("safe-status: falha em " + label + ": " + ex.Message);
        }
        return changed;
    }

    private bool TrySetSafeNumericMember(object owner, Type valueType, string memberName, Func<object> getter, Action<object> setter)
    {
        try
        {
            if (valueType == typeof(float))
            {
                float current = 0f;
                try { current = Convert.ToSingle(getter()); } catch { }
                float desired = SafeRestoredValueFor(memberName, current, FindSafeMaxValue(owner, memberName));
                if (float.IsNaN(desired) || float.IsInfinity(desired)) return false;
                setter(desired);
                return true;
            }

            if (valueType == typeof(int))
            {
                int current = 0;
                try { current = Convert.ToInt32(getter()); } catch { }
                int max = (int)Mathf.Round(FindSafeMaxValue(owner, memberName));
                int desired = max > 0 ? max : (current <= 1 ? 1 : 100);
                setter(desired);
                return true;
            }
        }
        catch { }
        return false;
    }

    private float SafeRestoredValueFor(string memberName, float current, float discoveredMax)
    {
        if (discoveredMax > 0f && discoveredMax <= 1000f) return discoveredMax;
        if (current >= 0f && current <= 1.5f) return 1f;
        return 100f;
    }

    private float FindSafeMaxValue(object owner, string memberName)
    {
        try
        {
            Type type = owner.GetType();
            string normalized = NormalizeForCompare(memberName);
            string[] candidates = new[]
            {
                "max" + normalized,
                normalized + "max",
                "maximum" + normalized,
                normalized + "maximum",
                "mmax" + normalized,
                "m" + normalized + "max"
            };

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                string fieldNorm = NormalizeForCompare(field.Name);
                if (!candidates.Contains(fieldNorm)) continue;
                object raw = field.GetValue(owner);
                if (raw is float) return (float)raw;
                if (raw is int) return (int)raw;
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!property.CanRead) continue;
                string propNorm = NormalizeForCompare(property.Name);
                if (!candidates.Contains(propNorm)) continue;
                object raw = property.GetValue(owner, null);
                if (raw is float) return (float)raw;
                if (raw is int) return (int)raw;
            }
        }
        catch { }
        return 0f;
    }

    private static bool IsSafeVitalOwner(string typeName)
    {
        string lower = (typeName ?? string.Empty).ToLowerInvariant();

        if (lower.Contains("camera") || lower.Contains("audio") || lower.Contains("ui") || lower.Contains("canvas")) return false;
        if (lower.Contains("weapon") || lower.Contains("attack") || lower.Contains("enemy") || lower.Contains("animal") || lower.Contains("ai")) return false;
        if (lower.Contains("damage") || lower.Contains("wound") || lower.Contains("injury") || lower.Contains("bleed") || lower.Contains("burn")) return false;

        bool playerRelated = lower.Contains("player");
        bool statusRelated =
            lower.Contains("condition") ||
            lower.Contains("status") ||
            lower.Contains("stats") ||
            lower.Contains("stat") ||
            lower.Contains("vital") ||
            lower.Contains("survival") ||
            lower.Contains("need") ||
            lower.Contains("nutrition") ||
            lower.Contains("hydration");

        return playerRelated && statusRelated;
    }

    private static bool IsSafeVitalMember(string name, string[] targets)
    {
        string lower = (name ?? string.Empty).ToLowerInvariant();
        string normalized = NormalizeForCompare(name);

        if (lower.Contains("damage") || lower.Contains("poison") || lower.Contains("fever") || lower.Contains("disease") ||
            lower.Contains("wound") || lower.Contains("bleed") || lower.Contains("burn") || lower.Contains("injury") ||
            lower.Contains("rate") || lower.Contains("speed") || lower.Contains("multiplier") || lower.Contains("modifier") ||
            lower.Contains("timer") || lower.Contains("time") || lower.Contains("cooldown"))
        {
            return false;
        }

        foreach (string target in targets)
        {
            string t = NormalizeForCompare(target);
            if (normalized == t || normalized == "m" + t || normalized == t + "value" || normalized == "current" + t || normalized == t + "current")
            {
                return true;
            }
        }

        return false;
    }

    private int TryInvokeSafePlayerStatusMethods(string label, string[] methodNames, float value)
    {
        int invoked = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                string typeName = type.FullName ?? type.Name ?? string.Empty;
                if (!IsSafeMethodOwner(typeName)) continue;

                foreach (string wanted in methodNames)
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!string.Equals(method.Name, wanted, StringComparison.OrdinalIgnoreCase)) continue;
                        if (TryInvokeSafeNumericMethod(behaviour, method, value))
                        {
                            invoked++;
                            Logger.LogInfo("safe-status: " + label + " method " + type.Name + "." + method.Name);
                            return invoked;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("safe-status: method invoke falhou " + label + ": " + ex.Message);
        }
        return invoked;
    }

    private static bool IsSafeMethodOwner(string typeName)
    {
        string lower = (typeName ?? string.Empty).ToLowerInvariant();

        if (lower.Contains("camera") || lower.Contains("audio") || lower.Contains("ui") || lower.Contains("canvas")) return false;
        if (lower.Contains("weapon") || lower.Contains("attack") || lower.Contains("enemy") || lower.Contains("animal") || lower.Contains("ai")) return false;

        return lower.Contains("player") &&
               (lower.Contains("condition") || lower.Contains("status") || lower.Contains("stats") || lower.Contains("survival") || lower.Contains("need") || lower.Contains("nutrition") || lower.Contains("hydration"));
    }

    private bool TryInvokeSafeNumericMethod(object target, MethodInfo method, float value)
    {
        try
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                method.Invoke(target, null);
                return true;
            }

            if (parameters.Length == 1)
            {
                Type p = parameters[0].ParameterType;
                object arg = null;
                if (p == typeof(float)) arg = value;
                else if (p == typeof(int)) arg = Mathf.RoundToInt(value >= 1.5f ? value : 1f);
                else if (p == typeof(bool)) arg = true;
                else return false;

                method.Invoke(target, new object[] { arg });
                return true;
            }
        }
        catch { }
        return false;
    }

    private bool TryApplyNegativeStatusSafe(string label, string[] methodNames)
    {
        // 0.1.24: usar API real encontrada no log:
        // PlayerDiseasesModule.RequestDisease(Enums.ConsumeEffect type, float delay, int level, int delayed_level)
        if (TryRequestDiseaseReal(label))
        {
            Logger.LogInfo("disease-request: " + label + " aplicado via PlayerDiseasesModule.RequestDisease");
            return true;
        }

        Logger.LogWarning("disease-request: " + label + " nao aplicado; RequestDisease/ConsumeEffect nao encontrado.");
        return false;
    }

    private bool TrySpawnAnimalStrictSafe(string label, string[] aliases)
    {
        try
        {
            ExtraCommandSpec spec = new ExtraCommandSpec(
                "spawn",
                label,
                aliases,
                new[] { "group", "spawner", "spawnpoint", "shell", "corpse", "dead", "ragdoll", "audio", "ui", "marker" }
            );

            if (TrySpawnStrictObject(spec))
            {
                Logger.LogInfo("spawn-safe: " + label + " criado por TrySpawnStrictObject.");
                return true;
            }

            if (TryCloneLoadedCreatureByKeywords(aliases, label))
            {
                Logger.LogInfo("spawn-safe: " + label + " criado por clone fallback.");
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("spawn-safe: falha ao criar " + label + ": " + ex.Message);
        }

        Logger.LogWarning("spawn-safe: " + label + " nao encontrado com seguranca.");
        return false;
    }


    private int TryInvokeHealRestoreExactApis(bool fullRestore)
    {
        int invoked = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;

                Type type = behaviour.GetType();
                string typeName = type.FullName ?? type.Name ?? string.Empty;
                string lowerType = typeName.ToLowerInvariant();

                if (lowerType.Contains("ui") || lowerType.Contains("camera") || lowerType.Contains("audio") || lowerType.Contains("debug")) continue;
                if (!(lowerType.Contains("playerconditionmodule") || lowerType.Contains("playercondition") || lowerType.Contains("player") || lowerType.Contains("condition"))) continue;

                string[] methodNames = fullRestore
                    ? new[] {
                        "RestoreAll", "RestoreStatus", "RestoreStats", "RestoreCondition", "RestoreConditions",
                        "HealAll", "HealPlayer", "Heal", "ResetCondition", "ResetConditions",
                        "UpdateCondition", "UpdateConditions", "UpdateStats", "UpdateStatus", "UpdateParams",
                        "Refresh", "RefreshValues", "Recalculate", "RecalculateParams"
                    }
                    : new[] {
                        "Heal", "HealPlayer", "RestoreHealth", "RestoreHP", "AddHealth", "AddHP", "IncreaseHP",
                        "UpdateCondition", "UpdateStats", "UpdateStatus", "Refresh", "RefreshValues"
                    };

                foreach (string wanted in methodNames)
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!string.Equals(method.Name, wanted, StringComparison.OrdinalIgnoreCase)) continue;
                        if (MethodLooksUnsafeForHealRestore(method.Name)) continue;

                        if (TryInvokeHealRestoreMethod(behaviour, method, fullRestore))
                        {
                            invoked++;
                            Logger.LogInfo("heal-restore: called " + type.Name + "." + MethodSignature(method));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("heal-restore: exact api erro: " + ex.Message);
        }
        return invoked;
    }

    private bool TryInvokeHealRestoreMethod(object target, MethodInfo method, bool fullRestore)
    {
        try
        {
            ParameterInfo[] ps = method.GetParameters();
            if (ps.Length == 0)
            {
                method.Invoke(target, null);
                return true;
            }

            if (ps.Length == 1)
            {
                Type p = ps[0].ParameterType;
                object arg = null;

                if (p == typeof(float)) arg = fullRestore ? 100f : 100f;
                else if (p == typeof(int)) arg = 100;
                else if (p == typeof(bool)) arg = true;
                else return false;

                method.Invoke(target, new object[] { arg });
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("heal-restore: erro chamando " + method.Name + ": " + ex.Message);
        }

        return false;
    }

    private static bool MethodLooksUnsafeForHealRestore(string methodName)
    {
        string lower = (methodName ?? string.Empty).ToLowerInvariant();

        return lower.Contains("damage") ||
               lower.Contains("wound") ||
               lower.Contains("bleed") ||
               lower.Contains("burn") ||
               lower.Contains("poison") ||
               lower.Contains("fever") ||
               lower.Contains("disease") ||
               lower.Contains("consume") ||
               lower.Contains("consumption") ||
               lower.Contains("persecond") ||
               lower.Contains("rate") ||
               lower.Contains("timer") ||
               lower.Contains("cooldown") ||
               lower.StartsWith("repl") ||
               lower.Contains("ownership");
    }

    private int TryClearKnownDiseasesForRestore()
    {
        int invoked = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;

                Type type = behaviour.GetType();
                if (!string.Equals(type.Name, "PlayerDiseasesModule", StringComparison.OrdinalIgnoreCase)) continue;

                // Tenta chamar metodos de clear/cure/remove apenas no PlayerDiseasesModule.
                string[] methodNames = new[]
                {
                    "ClearAllDiseases", "RemoveAllDiseases", "CureAllDiseases", "HealAllDiseases",
                    "ClearDiseases", "RemoveDiseases", "CureDiseases",
                    "UpdateDiseases"
                };

                foreach (string wanted in methodNames)
                {
                    MethodInfo method = type.GetMethod(wanted, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (method == null) continue;

                    ParameterInfo[] ps = method.GetParameters();
                    try
                    {
                        if (ps.Length == 0)
                        {
                            method.Invoke(behaviour, null);
                            invoked++;
                            Logger.LogInfo("heal-restore: disease clear called " + type.Name + "." + method.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning("heal-restore: disease clear erro " + method.Name + ": " + ex.Message);
                    }
                }

                // Se nao existe ClearAll, tenta pegar todas as diseases e chamar Stop/Deactivate/Cure se existir.
                invoked += TryDeactivateActiveDiseaseObjects(behaviour);
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("heal-restore: clear diseases erro: " + ex.Message);
        }

        return invoked;
    }

    private int TryDeactivateActiveDiseaseObjects(object playerDiseasesModule)
    {
        int invoked = 0;
        try
        {
            Type moduleType = playerDiseasesModule.GetType();
            MethodInfo getAll = moduleType.GetMethod("GetAllDiseases", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (getAll == null) return 0;

            object all = getAll.Invoke(playerDiseasesModule, null);
            if (all == null) return 0;

            System.Collections.IEnumerable enumerable = all as System.Collections.IEnumerable;
            if (enumerable == null) return 0;

            foreach (object entry in enumerable)
            {
                object disease = null;

                // DictionaryEntry ou KeyValuePair<int,Disease>
                try
                {
                    PropertyInfo valueProp = entry.GetType().GetProperty("Value");
                    if (valueProp != null) disease = valueProp.GetValue(entry, null);
                }
                catch { }

                if (disease == null) continue;

                Type diseaseType = disease.GetType();
                string[] names = new[] { "Deactivate", "Stop", "Cure", "Remove", "Clear", "SetActive" };

                foreach (string name in names)
                {
                    foreach (MethodInfo method in diseaseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!string.Equals(method.Name, name, StringComparison.OrdinalIgnoreCase)) continue;
                        ParameterInfo[] ps = method.GetParameters();

                        try
                        {
                            if (ps.Length == 0)
                            {
                                method.Invoke(disease, null);
                                invoked++;
                                Logger.LogInfo("heal-restore: disease object called " + diseaseType.Name + "." + method.Name);
                            }
                            else if (ps.Length == 1 && ps[0].ParameterType == typeof(bool))
                            {
                                method.Invoke(disease, new object[] { false });
                                invoked++;
                                Logger.LogInfo("heal-restore: disease object called " + diseaseType.Name + "." + method.Name + "(false)");
                            }
                        }
                        catch { }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("heal-restore: deactivate disease objects erro: " + ex.Message);
        }

        return invoked;
    }
    private bool TryRequestDiseaseReal(string label)
    {
        try
        {
            string normalizedLabel = NormalizeForCompare(label);

            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;

                Type type = behaviour.GetType();
                string typeName = type.FullName ?? type.Name ?? string.Empty;

                if (!string.Equals(type.Name, "PlayerDiseasesModule", StringComparison.OrdinalIgnoreCase) &&
                    !typeName.EndsWith(".PlayerDiseasesModule", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (!string.Equals(method.Name, "RequestDisease", StringComparison.OrdinalIgnoreCase)) continue;

                    ParameterInfo[] ps = method.GetParameters();
                    if (ps.Length != 4) continue;
                    if (!ps[0].ParameterType.IsEnum) continue;
                    if (ps[1].ParameterType != typeof(float)) continue;
                    if (ps[2].ParameterType != typeof(int)) continue;
                    if (ps[3].ParameterType != typeof(int)) continue;

                    object diseaseType = ResolveConsumeEffectForDisease(ps[0].ParameterType, normalizedLabel);
                    if (diseaseType == null)
                    {
                        Logger.LogWarning("disease-request: ConsumeEffect nao encontrado para " + label);
                        DumpConsumeEffectNames(ps[0].ParameterType, label);
                        return false;
                    }

                    object[] args = new object[] { diseaseType, 0f, 1, 1 };
                    method.Invoke(behaviour, args);

                    Logger.LogInfo("disease-request: chamado " + type.Name + ".RequestDisease(" + diseaseType + ", 0, 1, 1)");
                    return true;
                }
            }

            Logger.LogWarning("disease-request: instancia PlayerDiseasesModule.RequestDisease nao encontrada.");
        }
        catch (Exception ex)
        {
            Logger.LogWarning("disease-request: erro aplicando " + label + ": " + ex);
        }

        return false;
    }

    private object ResolveConsumeEffectForDisease(Type enumType, string normalizedLabel)
    {
        try
        {
            string[] names = Enum.GetNames(enumType);

            string[][] priority;
            if (normalizedLabel.Contains("poison"))
            {
                priority = new[]
                {
                    new[] { "FoodPoisoning", "FoodPoison", "Poisoning", "Poison", "Food_Poisoning", "Food_Poison" },
                    new[] { "Toxin", "Venom" }
                };
            }
            else if (normalizedLabel.Contains("fever"))
            {
                priority = new[]
                {
                    new[] { "Fever" },
                    new[] { "Temperature", "HighTemperature" }
                };
            }
            else
            {
                priority = new[] { new[] { normalizedLabel } };
            }

            foreach (string[] group in priority)
            {
                foreach (string wanted in group)
                {
                    foreach (string name in names)
                    {
                        if (string.Equals(name, wanted, StringComparison.OrdinalIgnoreCase))
                        {
                            return Enum.Parse(enumType, name);
                        }
                    }
                }

                foreach (string wanted in group)
                {
                    string wn = NormalizeForCompare(wanted);
                    foreach (string name in names)
                    {
                        string nn = NormalizeForCompare(name);
                        if (nn == wn || nn.Contains(wn) || wn.Contains(nn))
                        {
                            Logger.LogInfo("disease-request: ConsumeEffect fuzzy " + wanted + " -> " + name);
                            return Enum.Parse(enumType, name);
                        }
                    }
                }
            }
        }
        catch { }

        return null;
    }

    private void DumpConsumeEffectNames(Type enumType, string label)
    {
        try
        {
            Logger.LogInfo("=== ConsumeEffect names for " + label + " start ===");
            foreach (string name in Enum.GetNames(enumType))
            {
                string lower = name.ToLowerInvariant();
                if (lower.Contains("poison") || lower.Contains("fever") || lower.Contains("disease") ||
                    lower.Contains("sick") || lower.Contains("parasite") || lower.Contains("dirt") ||
                    lower.Contains("venom") || lower.Contains("toxin") || lower.Contains("temperature"))
                {
                    Logger.LogInfo("[CONSUME-EFFECT] " + name);
                }
            }
            Logger.LogInfo("=== ConsumeEffect names for " + label + " end ===");
        }
        catch { }
    }
    private void DumpDiseaseStatusCandidatesStrict(string label)
    {
        try
        {
            Logger.LogInfo("=== disease-probe-safe candidates for " + label + " start ===");
            int count = 0;
            string normalizedLabel = NormalizeForCompare(label);

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = new Type[0];
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }
                catch { }

                foreach (Type type in types)
                {
                    if (type == null) continue;
                    string typeName = type.FullName ?? type.Name ?? string.Empty;
                    string lowerType = typeName.ToLowerInvariant();

                    if (lowerType.Contains("ui") || lowerType.Contains("camera") || lowerType.Contains("audio") || lowerType.Contains("debug")) continue;

                    bool typeRelevant =
                        lowerType.Contains("condition") ||
                        lowerType.Contains("status") ||
                        lowerType.Contains("disease") ||
                        lowerType.Contains("sickness") ||
                        lowerType.Contains("illness") ||
                        lowerType.Contains("affliction") ||
                        lowerType.Contains("poison") ||
                        lowerType.Contains("fever");

                    if (!typeRelevant) continue;

                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                    {
                        string lowerMethod = method.Name.ToLowerInvariant();

                        if (lowerMethod.StartsWith("repl") || lowerMethod.Contains("ownership")) continue;
                        if (lowerMethod.Contains("remove") || lowerMethod.Contains("clear") || lowerMethod.Contains("cure") || lowerMethod.Contains("heal")) continue;
                        if (lowerMethod.Contains("consumption") || lowerMethod.Contains("persecond") || lowerMethod.Contains("rate") || lowerMethod.Contains("timer")) continue;

                        bool methodRelevant = false;
                        if (normalizedLabel.Contains("poison"))
                        {
                            methodRelevant = lowerMethod.Contains("poison") || lowerMethod.Contains("venom") || lowerMethod.Contains("toxic") || lowerMethod.Contains("disease") || lowerMethod.Contains("sickness") || lowerMethod.Contains("affliction");
                        }
                        else if (normalizedLabel.Contains("fever"))
                        {
                            methodRelevant = lowerMethod.Contains("fever") || lowerMethod.Contains("temperature") || lowerMethod.Contains("disease") || lowerMethod.Contains("sickness") || lowerMethod.Contains("affliction");
                        }

                        if (!methodRelevant) continue;

                        Logger.LogInfo("[DISEASE-STRICT-CANDIDATE] " + typeName + "." + MethodSignature(method));
                        count++;
                        if (count >= 250)
                        {
                            Logger.LogInfo("=== disease-probe-safe candidates truncated ===");
                            return;
                        }
                    }
                }
            }

            Logger.LogInfo("=== disease-probe-safe candidates for " + label + " end count=" + count + " ===");
        }
        catch (Exception ex)
        {
            Logger.LogWarning("disease-probe-safe: dump candidates falhou: " + ex.Message);
        }
    }
    private int TryInvokeDiseaseStatusApi(string label)
    {
        int invoked = 0;
        try
        {
            string normalizedLabel = NormalizeForCompare(label);
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                if (!LooksLikeDiseaseStatusOwner(type)) continue;

                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (!LooksLikeDiseaseStatusApplyMethod(method.Name)) continue;
                    if (MethodLooksUnsafeForCondition(method.Name)) continue;

                    object[] args = BuildDiseaseStatusArgs(method, normalizedLabel);
                    if (args == null) continue;

                    try
                    {
                        method.Invoke(behaviour, args);
                        invoked++;
                        Logger.LogInfo("disease-api: " + label + " called " + type.Name + "." + MethodSignature(method));
                        return invoked;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning("disease-api: falha chamando " + type.Name + "." + method.Name + ": " + ex.Message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("disease-api: erro geral " + label + ": " + ex.Message);
        }
        return invoked;
    }

    private static bool LooksLikeDiseaseStatusOwner(Type type)
    {
        string name = (type.FullName ?? type.Name ?? string.Empty).ToLowerInvariant();

        if (name.Contains("ui") || name.Contains("camera") || name.Contains("audio") || name.Contains("debug")) return false;
        if (name.Contains("animal") || name.Contains("enemy") || name.Contains("weapon")) return false;

        return name.Contains("playerconditionmodule") ||
               name.Contains("disease") ||
               name.Contains("illness") ||
               name.Contains("sickness") ||
               name.Contains("affliction") ||
               name.Contains("condition") ||
               name.Contains("status") ||
               name.Contains("poison") ||
               name.Contains("fever") ||
               (name.Contains("player") && (name.Contains("condition") || name.Contains("status") || name.Contains("disease")));
    }

    private static bool LooksLikeDiseaseStatusApplyMethod(string methodName)
    {
        string lower = (methodName ?? string.Empty).ToLowerInvariant();

        if (lower.StartsWith("repl") || lower.Contains("ownership") || lower.Contains("remove") || lower.Contains("clear") || lower.Contains("cure") || lower.Contains("heal")) return false;
        if (lower.Contains("consumption") || lower.Contains("persecond") || lower.Contains("rate") || lower.Contains("timer")) return false;

        return (lower.Contains("poison") || lower.Contains("fever") || lower.Contains("disease") || lower.Contains("sickness") || lower.Contains("affliction") || lower.Contains("venom")) &&
               (lower.Contains("add") || lower.Contains("apply") || lower.Contains("set") || lower.Contains("start") || lower.Contains("activate") || lower.Contains("increase") || lower.Contains("change"));
    }

    private object[] BuildDiseaseStatusArgs(MethodInfo method, string normalizedLabel)
    {
        try
        {
            ParameterInfo[] ps = method.GetParameters();
            if (ps.Length == 0) return new object[0];
            if (ps.Length > 4) return null;

            object[] args = new object[ps.Length];

            for (int i = 0; i < ps.Length; i++)
            {
                Type p = ps[i].ParameterType;
                string pname = (ps[i].Name ?? string.Empty).ToLowerInvariant();

                if (p.IsEnum)
                {
                    object enumValue = FindDiseaseEnumValue(p, normalizedLabel);
                    if (enumValue == null) return null;
                    args[i] = enumValue;
                }
                else if (p == typeof(float))
                {
                    if (pname.Contains("duration") || pname.Contains("time")) args[i] = 30f;
                    else args[i] = 1f;
                }
                else if (p == typeof(int))
                {
                    if (pname.Contains("duration") || pname.Contains("time")) args[i] = 30;
                    else args[i] = 1;
                }
                else if (p == typeof(bool))
                {
                    args[i] = true;
                }
                else if (p == typeof(string))
                {
                    args[i] = normalizedLabel.Contains("poison") ? "Poison" : "Fever";
                }
                else
                {
                    return null;
                }
            }

            return args;
        }
        catch { }
        return null;
    }

    private object FindDiseaseEnumValue(Type enumType, string normalizedLabel)
    {
        try
        {
            foreach (string enumName in Enum.GetNames(enumType))
            {
                string n = NormalizeForCompare(enumName);

                if (normalizedLabel.Contains("poison"))
                {
                    if (n.Contains("poison") || n.Contains("venom") || n.Contains("foodpoison") || n.Contains("foodpoisoning"))
                    {
                        return Enum.Parse(enumType, enumName);
                    }
                }

                if (normalizedLabel.Contains("fever"))
                {
                    if (n.Contains("fever") || n.Contains("temperature"))
                    {
                        return Enum.Parse(enumType, enumName);
                    }
                }
            }
        }
        catch { }
        return null;
    }

    private void DumpDiseaseStatusCandidates(string label)
    {
        try
        {
            Logger.LogInfo("=== disease-api candidates for " + label + " start ===");
            int count = 0;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = new Type[0];
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }
                catch { }

                foreach (Type type in types)
                {
                    if (type == null) continue;
                    string typeName = type.FullName ?? type.Name ?? string.Empty;
                    string lower = typeName.ToLowerInvariant();

                    if (!(lower.Contains("condition") || lower.Contains("status") || lower.Contains("disease") ||
                          lower.Contains("poison") || lower.Contains("fever") || lower.Contains("affliction") ||
                          lower.Contains("sickness") || lower.Contains("illness")))
                    {
                        continue;
                    }

                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                    {
                        string ml = method.Name.ToLowerInvariant();
                        if (!(ml.Contains("poison") || ml.Contains("fever") || ml.Contains("disease") || ml.Contains("status") ||
                              ml.Contains("condition") || ml.Contains("add") || ml.Contains("apply") || ml.Contains("set")))
                        {
                            continue;
                        }

                        Logger.LogInfo("[DISEASE-CANDIDATE] " + typeName + "." + MethodSignature(method));
                        count++;
                        if (count >= 300)
                        {
                            Logger.LogInfo("=== disease-api candidates truncated ===");
                            return;
                        }
                    }
                }
            }

            Logger.LogInfo("=== disease-api candidates for " + label + " end count=" + count + " ===");
        }
        catch (Exception ex)
        {
            Logger.LogWarning("disease-api: dump candidates falhou: " + ex.Message);
        }
    }
    private int TryInvokeGreenHellConditionMethodsOnly(string label, string[] statNames, string[] verbs, float deltaValue, float setValue)
    {
        int invoked = 0;
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;

                Type type = behaviour.GetType();
                if (!LooksLikeGreenHellConditionOwner(type)) continue;

                foreach (string stat in statNames)
                {
                    foreach (string verb in verbs)
                    {
                        string[] candidateNames = BuildConditionMethodNames(verb, stat);
                        foreach (string candidateName in candidateNames)
                        {
                            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                            {
                                if (!string.Equals(method.Name, candidateName, StringComparison.OrdinalIgnoreCase)) continue;
                                if (MethodLooksUnsafeForCondition(method.Name)) continue;

                                float value = IsSetVerb(verb) ? setValue : deltaValue;
                                if (TryInvokeConditionMethodSafely(behaviour, method, value, label))
                                {
                                    invoked++;
                                    Logger.LogInfo("condition-methods: " + label + " called " + type.Name + "." + method.Name + "(" + value + ")");
                                    return invoked;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-methods: falha " + label + ": " + ex.Message);
        }

        if (invoked == 0)
        {
            Logger.LogWarning("condition-methods: " + label + " nenhum metodo seguro encontrado.");
        }

        return invoked;
    }

    private int TryInvokeGreenHellNegativeStatusMethodsOnly(string label, string[] oldMethodNames)
    {
        int invoked = 0;
        string normalizedLabel = NormalizeForCompare(label);

        string[] names;
        if (normalizedLabel.Contains("poison"))
        {
            names = new[]
            {
                "AddPoison", "ApplyPoison", "SetPoison",
                "AddFoodPoison", "ApplyFoodPoison", "SetFoodPoison",
                "AddFoodPoisoning", "ApplyFoodPoisoning", "SetFoodPoisoning",
                "AddVenom", "ApplyVenom", "SetVenom"
            };
        }
        else if (normalizedLabel.Contains("fever"))
        {
            names = new[]
            {
                "AddFever", "ApplyFever", "SetFever",
                "AddTemperature", "ApplyTemperature", "SetTemperature"
            };
        }
        else
        {
            names = oldMethodNames ?? new string[0];
        }

        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;

                Type type = behaviour.GetType();
                if (!LooksLikeGreenHellConditionOwner(type) && !LooksLikeGreenHellDiseaseOwner(type)) continue;

                foreach (string name in names)
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (!string.Equals(method.Name, name, StringComparison.OrdinalIgnoreCase)) continue;
                        if (MethodLooksUnsafeForCondition(method.Name)) continue;

                        if (TryInvokeConditionMethodSafely(behaviour, method, 1f, label) ||
                            TryInvokeEnumStatusMethodSafely(behaviour, method, normalizedLabel))
                        {
                            invoked++;
                            Logger.LogInfo("condition-methods: " + label + " called " + type.Name + "." + method.Name);
                            return invoked;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-methods: negative falha " + label + ": " + ex.Message);
        }

        if (invoked == 0)
        {
            Logger.LogWarning("condition-methods: " + label + " nenhum metodo seguro encontrado.");
        }

        return invoked;
    }

    private static bool LooksLikeGreenHellConditionOwner(Type type)
    {
        string name = (type.FullName ?? type.Name ?? string.Empty).ToLowerInvariant();

        if (name.Contains("ui") || name.Contains("camera") || name.Contains("audio") || name.Contains("debug")) return false;
        if (name.Contains("animal") || name.Contains("enemy") || name.Contains("weapon")) return false;

        return name.Contains("playerconditionmodule") ||
               name.Contains("playercondition") ||
               name.Contains("playerstats") ||
               name.Contains("playerstatus") ||
               name.Contains("playervitals") ||
               (name.Contains("player") && (name.Contains("condition") || name.Contains("status") || name.Contains("stat") || name.Contains("survival")));
    }

    private static bool LooksLikeGreenHellDiseaseOwner(Type type)
    {
        string name = (type.FullName ?? type.Name ?? string.Empty).ToLowerInvariant();

        if (name.Contains("ui") || name.Contains("camera") || name.Contains("audio") || name.Contains("debug")) return false;

        return name.Contains("disease") ||
               name.Contains("sickness") ||
               name.Contains("affliction") ||
               name.Contains("poison") ||
               name.Contains("fever") ||
               (name.Contains("player") && (name.Contains("condition") || name.Contains("status")));
    }

    private static string[] BuildConditionMethodNames(string verb, string stat)
    {
        return new[]
        {
            verb + stat,
            verb + "Player" + stat,
            verb + stat + "Value",
            verb + stat + "Level",
            stat + verb,
            stat + "Add",
            stat + "Change",
            stat + "Set",
            "On" + verb + stat
        };
    }

    private static bool IsSetVerb(string verb)
    {
        return string.Equals(verb, "Set", StringComparison.OrdinalIgnoreCase);
    }

    private static bool MethodLooksUnsafeForCondition(string methodName)
    {
        string lower = (methodName ?? string.Empty).ToLowerInvariant();
        return lower.Contains("consumption") ||
               lower.Contains("persecond") ||
               lower.Contains("rate") ||
               lower.Contains("timer") ||
               lower.Contains("cooldown") ||
               lower.Contains("multiplier") ||
               lower.Contains("modifier") ||
               lower.Contains("damage") ||
               lower.Contains("wound") ||
               lower.Contains("bleed") ||
               lower.Contains("burn");
    }

    private bool TryInvokeConditionMethodSafely(object target, MethodInfo method, float value, string label)
    {
        try
        {
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length == 0)
            {
                method.Invoke(target, null);
                return true;
            }

            if (parameters.Length == 1)
            {
                Type p = parameters[0].ParameterType;
                object arg = null;

                if (p == typeof(float)) arg = value;
                else if (p == typeof(int)) arg = Mathf.RoundToInt(value);
                else if (p == typeof(bool)) arg = true;
                else if (p == typeof(string)) arg = label;
                else return false;

                method.Invoke(target, new object[] { arg });
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-methods: erro chamando " + method.Name + ": " + ex.Message);
        }

        return false;
    }

    private bool TryInvokeEnumStatusMethodSafely(object target, MethodInfo method, string normalizedLabel)
    {
        try
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 2) return false;

            object enumArg = null;
            object valueArg = null;

            for (int i = 0; i < parameters.Length; i++)
            {
                Type p = parameters[i].ParameterType;

                if (p.IsEnum)
                {
                    foreach (string enumName in Enum.GetNames(p))
                    {
                        string n = NormalizeForCompare(enumName);
                        if ((normalizedLabel.Contains("poison") && (n.Contains("poison") || n.Contains("venom"))) ||
                            (normalizedLabel.Contains("fever") && n.Contains("fever")))
                        {
                            enumArg = Enum.Parse(p, enumName);
                            break;
                        }
                    }
                }
                else if (p == typeof(float))
                {
                    valueArg = 1f;
                }
                else if (p == typeof(int))
                {
                    valueArg = 1;
                }
                else if (p == typeof(bool))
                {
                    valueArg = true;
                }
            }

            if (enumArg == null || valueArg == null) return false;

            object[] args = new object[2];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = parameters[i].ParameterType.IsEnum ? enumArg : valueArg;
            }

            method.Invoke(target, args);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning("condition-methods: enum status erro " + method.Name + ": " + ex.Message);
            return false;
        }
    }
    private void TrySetPlayerVitals(float value, bool absolute, string[] filter = null)
    {
        try
        {
            foreach (var behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                string typeName = behaviour.GetType().FullName ?? behaviour.GetType().Name ?? string.Empty;
                string lowerType = typeName.ToLowerInvariant();
                if (!lowerType.Contains("player") && !lowerType.Contains("condition") && !lowerType.Contains("stats")) continue;

                foreach (var field in behaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    string name = field.Name.ToLowerInvariant();
                    if (!IsVitalName(name, filter)) continue;
                    if (field.FieldType == typeof(float))
                    {
                        float current = 0f;
                        try { current = (float)field.GetValue(behaviour); } catch {}
                        field.SetValue(behaviour, absolute ? value : Mathf.Clamp(current + value, 0f, 100f));
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        int current = 0;
                        try { current = (int)field.GetValue(behaviour); } catch {}
                        field.SetValue(behaviour, absolute ? (int)value : Mathf.Clamp(current + (int)value, 0, 100));
                    }
                }

                foreach (var property in behaviour.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (!property.CanRead || !property.CanWrite) continue;
                    string name = property.Name.ToLowerInvariant();
                    if (!IsVitalName(name, filter)) continue;
                    try
                    {
                        if (property.PropertyType == typeof(float))
                        {
                            float current = (float)property.GetValue(behaviour, null);
                            property.SetValue(behaviour, absolute ? value : Mathf.Clamp(current + value, 0f, 100f), null);
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            int current = (int)property.GetValue(behaviour, null);
                            property.SetValue(behaviour, absolute ? (int)value : Mathf.Clamp(current + (int)value, 0, 100), null);
                        }
                    }
                    catch {}
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao tentar ajustar status do player: " + ex.Message);
        }
    }

    private static bool IsVitalName(string name, string[] filter)
    {
        string[] terms = filter ?? new[] { "health", "life", "hp", "energy", "stamina", "sanity", "hunger", "thirst", "hydration", "water", "food", "carbo", "protein", "fat" };
        return terms.Any(term => name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private bool TryInvokeLikelyPlayerMethod(string[] methodNames, object[] args)
    {
        try
        {
            foreach (var behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                string typeName = behaviour.GetType().FullName ?? behaviour.GetType().Name ?? string.Empty;
                if (typeName.IndexOf("player", StringComparison.OrdinalIgnoreCase) < 0 && typeName.IndexOf("condition", StringComparison.OrdinalIgnoreCase) < 0) continue;

                foreach (string methodName in methodNames)
                {
                    if (TryInvokeMethod(behaviour, methodName, args)) return true;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao invocar mÃ©todo provÃ¡vel do player: " + ex.Message);
        }
        return false;
    }

private bool TryRunDebugConsoleCommand(string command)
    {
        try
        {
            foreach (var behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                string typeName = type.FullName ?? type.Name ?? string.Empty;
                string lowerType = typeName.ToLowerInvariant();
                if (!lowerType.Contains("console") && !lowerType.Contains("debug") && !lowerType.Contains("cheat") && !lowerType.Contains("command")) continue;

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    string methodName = method.Name.ToLowerInvariant();
                    if (!methodName.Contains("command") && !methodName.Contains("execute") && !methodName.Contains("run") && !methodName.Contains("submit") && !methodName.Contains("process")) continue;
                    var parameters = method.GetParameters();
                    if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string)) continue;

                    try
                    {
                        object target = method.IsStatic ? null : behaviour;
                        method.Invoke(target, new object[] { command });
                        return true;
                    }
                    catch { }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao tentar comando de console/debug Green Hell: " + ex.Message);
        }
        return false;
    }

    private bool TryCloneLoadedCreatureByKeywords(string[] keywords, string label)
    {
        try
        {
            Transform player = FindPlayerTransform();
            Vector3 basePosition = ResolveSpawnPositionNearPlayer(player, 12f);

            float randomX = UnityEngine.Random.Range(-8f, 8f);
            float randomZ = UnityEngine.Random.Range(-8f, 8f);

            Vector3 spawnPosition = new Vector3(
                basePosition.x + randomX,
                basePosition.y,
                basePosition.z + randomZ
            );
            Quaternion spawnRotation = player != null ? Quaternion.LookRotation(-player.forward, Vector3.up) : Quaternion.identity;

            GameObject candidate = FindLoadedCreatureCandidate(keywords);
            if (candidate == null)
            {
                Logger.LogWarning("Nenhum candidato carregado encontrado para " + label + ".");
                return false;
            }

            GameObject clone = Instantiate(candidate, spawnPosition, spawnRotation);
            clone.name = "LivePlay_" + label + "_Spawn";

            // Desativa temporariamente para estabilizar AI
            clone.SetActive(false);

            // Separa física imediatamente
            foreach (Rigidbody rb in clone.GetComponentsInChildren<Rigidbody>(true))
            {
                try
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                catch { }
            }

            // Garante posição correta
            clone.transform.position = spawnPosition;

            // Reseta IA
            foreach (MonoBehaviour mb in clone.GetComponentsInChildren<MonoBehaviour>(true))
            {
                try
                {
                    string n = mb.GetType().Name.ToLowerInvariant();

                    if (n.Contains("ai") ||
                        n.Contains("brain") ||
                        n.Contains("combat") ||
                        n.Contains("enemy") ||
                        n.Contains("hunter"))
                    {
                        mb.enabled = false;
                    }
                }
                catch { }
            }

            // Reativa clone
            clone.SetActive(true);

            // Reativa IA limpa
            foreach (MonoBehaviour mb in clone.GetComponentsInChildren<MonoBehaviour>(true))
            {
                try
                {
                    string n = mb.GetType().Name.ToLowerInvariant();

                    if (n.Contains("ai") ||
                        n.Contains("brain") ||
                        n.Contains("combat") ||
                        n.Contains("enemy") ||
                        n.Contains("hunter"))
                    {
                        mb.enabled = true;
                    }
                }
                catch { }
            }

            // Reativa rigidbody
            foreach (Rigidbody rb in clone.GetComponentsInChildren<Rigidbody>(true))
            {
                try
                {
                    rb.isKinematic = false;
                }
                catch { }
            }
			
            clone.transform.position += new Vector3(
                UnityEngine.Random.Range(-6f, 6f),
                0f,
                UnityEngine.Random.Range(-6f, 6f)
            );
			
            StabilizeSpawnedObject(clone, spawnPosition);

            Logger.LogInfo(label + " clonado perto do jogador a partir de: " + candidate.name);

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao clonar " + label + " perto do jogador: " + ex.Message);
            return false;
        }
    }

    private Transform FindPlayerTransform()
    {
        try
        {
            foreach (var behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                string typeName = behaviour.GetType().FullName ?? behaviour.GetType().Name ?? string.Empty;
                string lower = typeName.ToLowerInvariant();
                if (!lower.Contains("player")) continue;
                if (lower.Contains("camera") || lower.Contains("audio") || lower.Contains("ui")) continue;
                return behaviour.transform;
            }
        }
        catch { }
        return null;
    }

    private Vector3 ResolveSpawnPositionNearPlayer(Transform player, float distance)
    {
        Vector3 basePosition = Vector3.zero;
        Vector3 forward = Vector3.forward;

        if (player != null)
        {
            basePosition = player.position;
            forward = player.forward;
            if (forward.sqrMagnitude < 0.01f) forward = Vector3.forward;
        }

        Vector3 position = basePosition + forward.normalized * distance + Vector3.up * 0.4f;
        try
        {
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 8f, Vector3.down, out hit, 40f))
            {
                position = hit.point + Vector3.up * 0.08f;
            }
        }
        catch { }
        return position;
    }

    private GameObject FindLoadedCreatureCandidate(string[] keywords)
    {
        try
        {
            GameObject[] objects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in objects)
            {
                if (obj == null) continue;
                if (!LooksLikeCandidate(obj, keywords)) continue;
                if (LooksLikePlayerOrUi(obj)) continue;
                return obj;
            }
        }
        catch { }
        return null;
    }

    private static bool LooksLikeCandidate(GameObject obj, string[] keywords)
    {
        string name = obj.name ?? string.Empty;
        string lowerName = name.ToLowerInvariant();
        if (keywords.Any(keyword => lowerName.Contains(keyword.ToLowerInvariant()))) return true;

        try
        {
            foreach (var component in obj.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue;
                string typeName = component.GetType().FullName ?? component.GetType().Name ?? string.Empty;
                string lowerType = typeName.ToLowerInvariant();
                if (keywords.Any(keyword => lowerType.Contains(keyword.ToLowerInvariant()))) return true;
            }
        }
        catch { }
        return false;
    }

    private static bool LooksLikePlayerOrUi(GameObject obj)
    {
        string lower = (obj.name ?? string.Empty).ToLowerInvariant();
        if (lower.Contains("player") || lower.Contains("camera") || lower.Contains("canvas") || lower.Contains("ui") || lower.Contains("audio")) return true;
        return false;
    }

    private void StabilizeSpawnedObject(GameObject clone, Vector3 spawnPosition)
    {
        try { clone.transform.position = spawnPosition; } catch { }
        try { clone.transform.rotation = Quaternion.Euler(0f, clone.transform.rotation.eulerAngles.y, 0f); } catch { }

        try
        {
            foreach (var rigidbody in clone.GetComponentsInChildren<Rigidbody>(true))
            {
                if (rigidbody == null) continue;
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
            }
        }
        catch { }

        try
        {
            foreach (var animator in clone.GetComponentsInChildren<Animator>(true))
            {
                if (animator == null) continue;
                animator.enabled = true;
                animator.Rebind();
                animator.Update(0f);
            }
        }
        catch { }

        TryWarpAgentsByReflection(clone, spawnPosition);
    }

    private void TryWarpAgentsByReflection(GameObject root, Vector3 spawnPosition)
    {
        try
        {
            foreach (var component in root.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue;
                Type type = component.GetType();
                string typeName = type.FullName ?? type.Name ?? string.Empty;
                if (typeName.IndexOf("NavMeshAgent", StringComparison.OrdinalIgnoreCase) < 0) continue;

                try
                {
                    var enabledProp = type.GetProperty("enabled", BindingFlags.Public | BindingFlags.Instance);
                    if (enabledProp != null && enabledProp.CanWrite) enabledProp.SetValue(component, true, null);
                }
                catch { }

                try
                {
                    var warp = type.GetMethod("Warp", BindingFlags.Public | BindingFlags.Instance);
                    if (warp != null) warp.Invoke(component, new object[] { spawnPosition });
                }
                catch { }
            }
        }
        catch { }
    }

    private bool TryInvokeSpawner(string[] keywords)
    {
        try
        {
            foreach (var behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                string typeName = behaviour.GetType().FullName ?? behaviour.GetType().Name ?? string.Empty;
                string lower = typeName.ToLowerInvariant();
                if (!lower.Contains("spawn") && !lower.Contains("debug") && !lower.Contains("animal") && !lower.Contains("enemy")) continue;

                foreach (var method in behaviour.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    string methodLower = method.Name.ToLowerInvariant();
                    if (!keywords.Any(keyword => methodLower.Contains(keyword)) && !methodLower.Contains("spawn")) continue;
                    var parameters = method.GetParameters();
                    try
                    {
                        if (parameters.Length == 0)
                        {
                            method.Invoke(behaviour, Array.Empty<object>());
                            return true;
                        }
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                        {
                            method.Invoke(behaviour, new object[] { keywords[0] });
                            return true;
                        }
                    }
                    catch {}
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Falha ao tentar spawn provÃ¡vel: " + ex.Message);
        }
        return false;
    }


    private sealed class ExtraCommandSpec
    {
        public string Kind;
        public string Label;
        public string[] Aliases;
        public string[] Reject;
        public ExtraCommandSpec(string kind, string label, string[] aliases, string[] reject = null)
        {
            Kind = kind;
            Label = label;
            Aliases = aliases;
            Reject = reject ?? new string[0];
        }
    }

    private static readonly Dictionary<string, ExtraCommandSpec> ExtraCommands = new Dictionary<string, ExtraCommandSpec>(StringComparer.OrdinalIgnoreCase)
    {
        { "spawn_armadillo", new ExtraCommandSpec("spawn", "Armadillo", new[] { "armadillo" }) },
        { "spawn_capybara", new ExtraCommandSpec("spawn", "Capybara", new[] { "capybara" }, new[] { "group", "spawner", "spawnpoint" }) },
        { "spawn_puma", new ExtraCommandSpec("spawn", "Puma", new[] { "puma" }, new[] { "jaguar", "group", "spawner", "spawnpoint" }) },
        { "spawn_tortoise", new ExtraCommandSpec("spawn", "Tortoise", new[] { "tortoise", "turtle" }, new[] { "shell", "turtle_shell" }) },
        { "give_banana", new ExtraCommandSpec("give", "Banana", new[] { "banana" }, new[] { "leaf", "bed", "banana_leaf" }) },
        { "give_coconut", new ExtraCommandSpec("give", "Coconut", new[] { "coconut" }, new[] { "tree", "palm" }) },

        { "give_ants", new ExtraCommandSpec("give", "Ants", new[] { "ants", "ant" }) },
        { "give_ash_dressing", new ExtraCommandSpec("give", "Ash Dressing", new[] { "ash dressing", "ash_dressing" }) },
        { "give_goliath_dressing", new ExtraCommandSpec("give", "Goliath Dressing", new[] { "goliath dressing", "goliath_dressing" }) },
        { "give_honey_dressing", new ExtraCommandSpec("give", "Honey Dressing", new[] { "honey dressing", "honey_dressing" }) },
        { "give_leaf_bandage", new ExtraCommandSpec("give", "Leaf Bandage", new[] { "leaf bandage", "leaf_bandage", "bandage" }) },
        { "give_lily_dressing", new ExtraCommandSpec("give", "Lily Dressing", new[] { "lily dressing", "lily_dressing" }) },
        { "give_painkillers", new ExtraCommandSpec("give", "Painkillers", new[] { "painkillers", "painkiller", "pills" }) },

        { "give_arrow", new ExtraCommandSpec("give", "Arrow", new[] { "arrow" }) },
        { "give_axe", new ExtraCommandSpec("give", "Axe", new[] { "axe" }) },
        { "give_blade_axe", new ExtraCommandSpec("give", "Blade Axe", new[] { "blade axe", "blade_axe" }) },
        { "give_bone_axe", new ExtraCommandSpec("give", "Bone Axe", new[] { "bone axe", "bone_axe" }) },
        { "give_bow", new ExtraCommandSpec("give", "Bow", new[] { "bow" }) },
        { "give_obsidian_axe", new ExtraCommandSpec("give", "Obsidian Axe", new[] { "obsidian axe", "obsidian_axe" }) },
        { "give_pickaxe", new ExtraCommandSpec("give", "Pickaxe", new[] { "pickaxe", "pick axe" }) },
        { "give_spear", new ExtraCommandSpec("give", "Spear", new[] { "spear", "wooden spear", "wooden_spear", "weak spear", "weak_spear", "bamboo spear", "bamboo_spear" }, new[] { "slot", "holder", "rack", "four_pronged_spear", "fourprongedspear", "metal_spear", "metalspear" }) },
        { "give_stone_axe", new ExtraCommandSpec("give", "Stone Axe", new[] { "stone axe", "stone_axe" }) },
        { "give_stone_knife", new ExtraCommandSpec("give", "Stone Knife", new[] { "stone knife", "stone_knife", "stone blade", "stone_blade", "blade stone", "blade_stone" }) },
        { "give_stone_spear_alt", new ExtraCommandSpec("give", "Stone Spear Alt", new[] { "four pronged spear", "four_pronged_spear", "fourprongedspear", "stone spear alt", "stone_spear_alt" }, new[] { "slot", "holder", "rack" }) },
        { "give_stone_spear", new ExtraCommandSpec("give", "Stone Spear", new[] { "stone spear", "stone_spear", "stonespear" }, new[] { "slot", "holder", "rack" }) },
        { "give_torch", new ExtraCommandSpec("give", "Torch", new[] { "torch" }) },
        { "give_wooden_club", new ExtraCommandSpec("give", "Wooden Club", new[] { "wooden club", "wooden_club", "woodenclub", "club" }, new[] { "fence", "wall", "building", "weak" }) },
    };

    private bool TryHandleExtraCommand(string command)
    {

        if (string.Equals(command, "debug_greenhell_spawn_help", StringComparison.OrdinalIgnoreCase))
        {
            DumpGreenHellSpawnHelpToLog();
            Toast("Debug spawn/help salvo no LogOutput.log");
            return true;
        }

        ExtraCommandSpec spec;
        if (!ExtraCommands.TryGetValue(command, out spec)) return false;

        bool ok = string.Equals(spec.Kind, "spawn", StringComparison.OrdinalIgnoreCase)
            ? TrySpawnStrictObject(spec)
            : TryGiveStrictItem(spec);

        Toast(ok ? spec.Label + " executado" : spec.Label + " nÃ£o encontrado com seguranÃ§a");
        return true;
    }


    private bool TryGiveItemByGreenHellConsole(ExtraCommandSpec spec)
    {
        string[] names = ConsoleGiveNamesFor(spec.Label);
        foreach (string name in names)
        {
            string command = "spawn Get " + name + " 1";
            if (TryRunDebugConsoleCommand(command))
            {
                Logger.LogInfo("give console/debug: " + spec.Label + " executado via " + command);
                return true;
            }
        }
        return false;
    }

    private static string[] ConsoleGiveNamesFor(string label)
    {
        string key = NormalizeForCompare(label);
        switch (key)
        {
            case "woodenclub":
                return new[] { "Aztec_Club", "Wooden_Club", "Club" };
            case "stonespear":
                return new[] { "Stone_Spear" };
            case "stonespearalt":
                return new[] { "Four_Pronged_Spear" };
            case "stoneknife":
                return new[] { "Stone_Blade", "Stone_Knife" };
            case "stonehatchet":
                return new[] { "Stone_Axe", "Stone_Hatchet" };
            case "stoneaxe":
                return new[] { "Stone_Axe" };
            case "spear":
                return new[] { "Weak_Spear", "Spear" };
            default:
                return new string[0];
        }
    }
    private bool TryGiveStrictItem(ExtraCommandSpec spec)
    {
        // 0.1.16: caminho correto primeiro.
        // Os prefabs clonados muitas vezes nao possuem componente Item. Entao criamos Item real
        // via ItemsManager.CreateItem(Enums.ItemID, ...) e depois tentamos inserir no InventoryBackpack.
        if (TryGiveByItemsManagerFactory(spec)) return true;

        if (TryAddItemByInventoryReflection(spec)) return true;
        if (TryDropStrictObjectAtPlayer(spec, true)) return true;
        Logger.LogWarning("give estrito falhou para " + spec.Label + ". Nenhuma API ItemsManager/inventÃ¡rio ou prefab/item exato foi encontrado.");
        return false;
    }

    private bool TrySpawnStrictObject(ExtraCommandSpec spec)
    {
        if (TryRunDebugConsoleCommand("spawn Get " + spec.Label + " 1"))
        {
            Logger.LogInfo("spawn estrito via console/debug: " + spec.Label);
            return true;
        }
        if (TryDropStrictObjectAtPlayer(spec, false)) return true;
        Logger.LogWarning("spawn estrito falhou para " + spec.Label + ". Nenhuma entidade/prefab exata foi encontrada.");
        return false;
    }


    private bool TryGiveByItemsManagerFactory(ExtraCommandSpec spec)
    {
        try
        {
            Type itemIdType = FindTypeByFullOrSimpleName("Enums.ItemID");
            if (itemIdType == null) itemIdType = FindTypeBySimpleName("ItemID");

            if (itemIdType == null || !itemIdType.IsEnum)
            {
                Logger.LogWarning("give/itemsmanager: Enums.ItemID nao encontrado.");
                return false;
            }

            object itemId = ResolveItemIdEnumValue(itemIdType, spec);
            if (itemId == null)
            {
                Logger.LogWarning("give/itemsmanager: ItemID nao encontrado para " + spec.Label);
                return false;
            }

            Type itemsManagerType = FindTypeBySimpleName("ItemsManager");
            if (itemsManagerType == null)
            {
                Logger.LogWarning("give/itemsmanager: ItemsManager nao encontrado.");
                return false;
            }

            object createdItem = TryCreateItemWithItemsManager(itemsManagerType, itemId, spec);
            if (createdItem == null)
            {
                Logger.LogWarning("give/itemsmanager: CreateItem retornou nulo para " + spec.Label + " id=" + itemId);
                return false;
            }

            Component itemComponent = createdItem as Component;
            if (itemComponent == null)
            {
                Logger.LogWarning("give/itemsmanager: CreateItem nao retornou Component/Item para " + spec.Label + ". Tipo=" + createdItem.GetType().FullName);
                return false;
            }

            if (TryInsertItemComponentIntoBackpack(itemComponent, spec))
            {
                Logger.LogInfo("give/itemsmanager: " + spec.Label + " criado por ItemsManager e inserido no backpack. id=" + itemId);
                return true;
            }

            Logger.LogWarning("give/itemsmanager: Item real criado, mas nao entrou no backpack para " + spec.Label + ". id=" + itemId);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning("give/itemsmanager: erro geral para " + spec.Label + ": " + ex);
            return false;
        }
    }

    private object ResolveItemIdEnumValue(Type itemIdType, ExtraCommandSpec spec)
    {
        string[] candidates = ItemIdCandidatesFor(spec.Label, spec.Aliases);
        string[] names = Enum.GetNames(itemIdType);

        foreach (string candidate in candidates)
        {
            foreach (string name in names)
            {
                if (string.Equals(name, candidate, StringComparison.OrdinalIgnoreCase))
                {
                    return Enum.Parse(itemIdType, name);
                }
            }
        }

        foreach (string candidate in candidates)
        {
            string candidateNorm = NormalizeForCompare(candidate);
            foreach (string name in names)
            {
                string nameNorm = NormalizeForCompare(name);
                if (nameNorm == candidateNorm)
                {
                    return Enum.Parse(itemIdType, name);
                }
            }
        }

        foreach (string candidate in candidates)
        {
            string candidateNorm = NormalizeForCompare(candidate);
            foreach (string name in names)
            {
                string nameNorm = NormalizeForCompare(name);
                if (nameNorm.Contains(candidateNorm) || candidateNorm.Contains(nameNorm))
                {
                    Logger.LogInfo("give/itemsmanager: ItemID fuzzy " + spec.Label + " -> " + name + " por " + candidate);
                    return Enum.Parse(itemIdType, name);
                }
            }
        }

        Logger.LogWarning("give/itemsmanager: candidatos ItemID para " + spec.Label + " nao bateram. Tentados=" + string.Join(",", candidates));
        return null;
    }

    private string[] ItemIdCandidatesFor(string label, string[] aliases)
    {
        string key = NormalizeForCompare(label);
        List<string> list = new List<string>();

        Action<string> add = v => { if (!string.IsNullOrWhiteSpace(v) && !list.Contains(v)) list.Add(v); };

        switch (key)
        {
            case "woodenclub":
                add("Aztec_Club"); add("AztecClub"); add("Wooden_Club"); add("WoodenClub"); add("Club");
                break;
            case "stonespear":
                add("Stone_Spear"); add("StoneSpear");
                break;
            case "stonespearalt":
                add("Four_Pronged_Spear"); add("FourProngedSpear");
                break;
            case "stoneknife":
                add("Stone_Blade"); add("StoneBlade"); add("Stone_Knife"); add("StoneKnife");
                break;
            case "stonehatchet":
                add("Stone_Axe"); add("StoneAxe"); add("Stone_Hatchet"); add("StoneHatchet");
                break;
            case "stoneaxe":
                add("Stone_Axe"); add("StoneAxe");
                break;
            case "spear":
                add("Weak_Spear"); add("WeakSpear"); add("Spear");
                break;
            case "torch":
                add("Torch");
                break;
        }

        add(label);
        add(label.Replace(" ", "_"));
        add(label.Replace(" ", ""));
        foreach (string alias in aliases ?? new string[0])
        {
            add(alias);
            add(alias.Replace(" ", "_"));
            add(alias.Replace(" ", ""));
        }

        return list.ToArray();
    }

    private object TryCreateItemWithItemsManager(Type itemsManagerType, object itemId, ExtraCommandSpec spec)
    {
        Transform player = FindPlayerTransform();
        Vector3 pos = ResolveSpawnPositionNearPlayer(player, 0.9f);
        Quaternion rot = player != null ? player.rotation : Quaternion.identity;
        Type itemIdType = itemId.GetType();

        foreach (MethodInfo method in itemsManagerType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
        {
            if (!string.Equals(method.Name, "CreateItem", StringComparison.OrdinalIgnoreCase)) continue;
            ParameterInfo[] p = method.GetParameters();

            object[] args = null;

            if (p.Length == 2 && p[0].ParameterType == itemIdType && p[1].ParameterType == typeof(bool))
            {
                args = new object[] { itemId, true };
            }
            else if (p.Length == 3 && p[0].ParameterType == itemIdType && p[1].ParameterType == typeof(bool) && typeof(Transform).IsAssignableFrom(p[2].ParameterType))
            {
                args = new object[] { itemId, true, player };
            }
            else if (p.Length == 5 && p[0].ParameterType == itemIdType && p[1].ParameterType == typeof(bool) && p[2].ParameterType == typeof(Vector3) && p[3].ParameterType == typeof(Quaternion) && p[4].ParameterType == typeof(bool))
            {
                args = new object[] { itemId, true, pos, rot, true };
            }

            if (args == null) continue;

            try
            {
                object target = method.IsStatic ? null : FindLiveObjectOfType(itemsManagerType);
                object result = method.Invoke(target, args);
                Logger.LogInfo("give/itemsmanager: chamado " + itemsManagerType.Name + "." + method.Name + "(" + p.Length + ") para " + spec.Label + " result=" + (result != null ? result.GetType().Name : "null"));
                if (result != null) return result;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("give/itemsmanager: CreateItem(" + p.Length + ") erro para " + spec.Label + ": " + ex.Message);
            }
        }

        return null;
    }

    private object FindLiveObjectOfType(Type type)
    {
        try
        {
            foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour != null && type.IsAssignableFrom(behaviour.GetType())) return behaviour;
            }
        }
        catch { }
        return null;
    }

    private Type FindTypeByFullOrSimpleName(string name)
    {
        try
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
                    if (string.Equals(type.FullName, name, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return type;
                    }
                }
            }
        }
        catch { }
        return null;
    }

    private bool TryInsertItemComponentIntoBackpack(Component itemComponent, ExtraCommandSpec spec)
    {
        try
        {
            Type backpackType = FindTypeBySimpleName("InventoryBackpack");
            if (backpackType == null)
            {
                Logger.LogWarning("give/itemsmanager: InventoryBackpack nao encontrado.");
                return false;
            }

            object backpack = null;
            MethodInfo get = backpackType.GetMethod("Get", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (get != null)
            {
                try { backpack = get.Invoke(null, null); } catch { }
            }

            if (backpack == null) backpack = FindLiveObjectOfType(backpackType);
            if (backpack == null)
            {
                Logger.LogWarning("give/itemsmanager: instancia InventoryBackpack nao encontrada.");
                return false;
            }

            object slot = TryFindFreeBackpackSlot(backpack, itemComponent);
            if (slot == null)
            {
                Logger.LogWarning("give/itemsmanager: sem slot livre para " + spec.Label);
            }

            if (TryInvokeBackpackInsert(backpack, itemComponent, slot))
            {
                MarkItemAsInventoryItem(itemComponent, true);
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("give/itemsmanager: insert backpack erro para " + spec.Label + ": " + ex.Message);
        }
        return false;
    }
    private bool TryAddItemByInventoryReflection(ExtraCommandSpec spec)
    {
        try
        {
            foreach (var behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null) continue;
                Type type = behaviour.GetType();
                string typeName = (type.FullName ?? type.Name ?? string.Empty).ToLowerInvariant();
                if (!typeName.Contains("inventory") && !typeName.Contains("backpack") && !typeName.Contains("item") && !typeName.Contains("manager")) continue;

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    string methodName = method.Name.ToLowerInvariant();
                    if (!methodName.Contains("add") && !methodName.Contains("create") && !methodName.Contains("give")) continue;
                    if (!methodName.Contains("item") && !methodName.Contains("inventory")) continue;
                    var parameters = method.GetParameters();
                    if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string)) continue;

                    foreach (string alias in BuildAliasAttempts(spec))
                    {
                        try
                        {
                            object target = method.IsStatic ? null : behaviour;
                            object result = method.Invoke(target, new object[] { alias });
                            if (method.ReturnType == typeof(bool) && result is bool && !(bool)result) continue;
                            Logger.LogInfo("give: " + spec.Label + " por API " + type.Name + "." + method.Name + "(" + alias + ")");
                            return true;
                        }
                        catch { }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("give por inventory reflection falhou para " + spec.Label + ": " + ex.Message);
        }
        return false;
    }

    private bool TryDropStrictObjectAtPlayer(ExtraCommandSpec spec, bool itemMode)
    {
        try
        {
            GameObject candidate = FindStrictObjectCandidate(spec, itemMode);
            if (candidate == null) return false;

            Transform player = FindPlayerTransform();
            Vector3 position = itemMode
                ? ResolveItemDropPositionNearPlayer(player)
                : ResolveAnimalSpawnPositionNearPlayer(player, spec);

            Quaternion rotation = player != null ? Quaternion.LookRotation(player.forward, Vector3.up) : Quaternion.identity;

            GameObject clone = Instantiate(candidate, position, rotation);
            if (clone == null) return false;
            clone.name = "LivePlay_" + NormalizeForCompare(spec.Label) + (itemMode ? "_Give" : "_Spawn");
            SetActiveDeep(clone, true);
            clone.transform.position = position;
            clone.transform.rotation = rotation;

            if (itemMode) StabilizeDroppedItem(clone, position);
            else StabilizeSpawnedObject(clone, position);

            if (itemMode && TryInsertCloneIntoBackpackByReflection(clone, spec))
            {
                Logger.LogInfo("give/inventory: " + spec.Label + " inserido no backpack.");
                return true;
            }

            Logger.LogInfo((itemMode ? "give" : "spawn") + ": " + spec.Label + " criado no jogador a partir de candidato estrito: " + candidate.name + " path=" + SafeGameObjectPath(candidate) + " em " + position);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning((itemMode ? "give" : "spawn") + " estrito falhou para " + spec.Label + ": " + ex.Message);
            return false;
        }
    }


    private Vector3 ResolveAnimalSpawnPositionNearPlayer(Transform player, ExtraCommandSpec spec)
    {
        float distance = 12f;

        try
        {
            string label = NormalizeForCompare(spec != null ? spec.Label : string.Empty);
            if (label == "snake") distance = 5.5f;
            else if (label == "jaguar" || label == "puma") distance = 12f;
        }
        catch { }

        Vector3 basePosition = ResolveSpawnPositionNearPlayer(player, distance);

        try
        {
            if (player != null)
            {
                Vector3 right = player.right;
                Vector3 forward = player.forward;
                if (right.sqrMagnitude < 0.01f) right = Vector3.right;
                if (forward.sqrMagnitude < 0.01f) forward = Vector3.forward;

                basePosition += right.normalized * UnityEngine.Random.Range(-5.5f, 5.5f);
                basePosition += forward.normalized * UnityEngine.Random.Range(-2.0f, 2.0f);
            }

            RaycastHit hit;
            if (Physics.Raycast(basePosition + Vector3.up * 10f, Vector3.down, out hit, 50f))
            {
                basePosition = hit.point + Vector3.up * 0.08f;
            }
        }
        catch { }

        return basePosition;
    }


    private Vector3 ResolveItemDropPositionNearPlayer(Transform player)
    {
        Vector3 basePosition = Vector3.zero;
        Vector3 forward = Vector3.forward;

        if (player != null)
        {
            basePosition = player.position;
            forward = player.forward;
            if (forward.sqrMagnitude < 0.01f) forward = Vector3.forward;
        }

        Vector3 position = basePosition + forward.normalized * 1.15f + Vector3.up * 0.25f;
        try
        {
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 3f, Vector3.down, out hit, 8f))
            {
                position = hit.point + Vector3.up * 0.18f;
            }
        }
        catch { }
        return position;
    }

    private static void SetActiveDeep(GameObject obj, bool active)
    {
        if (obj == null) return;
        try { obj.SetActive(active); } catch { }
        try
        {
            foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.gameObject != null) child.gameObject.SetActive(active);
            }
        }
        catch { }
    }

    private void StabilizeDroppedItem(GameObject clone, Vector3 position)
    {
        try { clone.transform.position = position; } catch { }
        try { clone.transform.rotation = Quaternion.Euler(0f, clone.transform.rotation.eulerAngles.y, 0f); } catch { }

        try
        {
            foreach (var component in clone.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue;
                Type type = component.GetType();
                TrySetBoolProperty(component, type, "enabled", true);
                TrySetBoolProperty(component, type, "CanBePickedUp", true);
                TrySetBoolProperty(component, type, "canBePickedUp", true);
                TrySetBoolProperty(component, type, "Pickable", true);
                TrySetBoolProperty(component, type, "pickable", true);
                TrySetBoolProperty(component, type, "m_CanBePickedUp", true);
                TrySetBoolProperty(component, type, "m_Pickable", true);
            }
        }
        catch { }

        bool hadCollider = false;
        try
        {
            foreach (var collider in clone.GetComponentsInChildren<Collider>(true))
            {
                if (collider == null) continue;
                collider.enabled = true;
                hadCollider = true;
            }
        }
        catch { }

        try
        {
            foreach (var renderer in clone.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer != null) renderer.enabled = true;
            }
        }
        catch { }

        try
        {
            Rigidbody rb = clone.GetComponentInChildren<Rigidbody>(true);
            if (rb == null && hadCollider) rb = clone.AddComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.WakeUp();
            }
        }
        catch { }
    }

    private static void TrySetBoolProperty(object target, Type type, string propertyName, bool value)
    {
        try
        {
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanWrite && property.PropertyType == typeof(bool))
            {
                property.SetValue(target, value, null);
                return;
            }
        }
        catch { }

        try
        {
            FieldInfo field = type.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(bool)) field.SetValue(target, value);
        }
        catch { }
    }

    private GameObject FindStrictObjectCandidate(ExtraCommandSpec spec, bool itemMode)
    {
        string cacheKey = BuildStrictCandidateCacheKey(spec, itemMode);
        GameObject cached;
        float cachedAt;

        if (_strictCandidateCache.TryGetValue(cacheKey, out cached) &&
            _strictCandidateCacheAt.TryGetValue(cacheKey, out cachedAt) &&
            cached != null &&
            Time.realtimeSinceStartup - cachedAt <= StrictCandidateCacheSeconds &&
            ScoreStrictCandidate(cached, spec, itemMode, false) >= 100)
        {
            Logger.LogInfo("candidato estrito cache para " + spec.Label + ": " + cached.name + " path=" + SafeGameObjectPath(cached));
            return cached;
        }

        GameObject best = null;
        int bestScore = int.MinValue;
        HashSet<int> seen = new HashSet<int>();

        Action<GameObject, bool> scan = (obj, deepCheck) =>
        {
            if (obj == null) return;
            try
            {
                int id = obj.GetInstanceID();
                if (!seen.Add(id)) return;
            }
            catch { }

            int score = ScoreStrictCandidate(obj, spec, itemMode, deepCheck);
            if (score > bestScore)
            {
                best = obj;
                bestScore = score;
            }
        };

        try
        {
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                scan(obj, true);
            }
        }
        catch { }

        if (best != null && bestScore >= 100)
        {
            _strictCandidateCache[cacheKey] = best;
            _strictCandidateCacheAt[cacheKey] = Time.realtimeSinceStartup;
            Logger.LogInfo("candidato estrito selecionado para " + spec.Label + ": " + best.name + " score=" + bestScore + " path=" + SafeGameObjectPath(best));
            return best;
        }

        // Resources.FindObjectsOfTypeAll pode ser caro. Usa só como fallback quando a cena atual não achou candidato.
        try
        {
            foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                scan(obj, false);
            }
        }
        catch { }

        if (best != null && bestScore >= 100)
        {
            _strictCandidateCache[cacheKey] = best;
            _strictCandidateCacheAt[cacheKey] = Time.realtimeSinceStartup;
            Logger.LogInfo("candidato estrito selecionado para " + spec.Label + ": " + best.name + " score=" + bestScore + " path=" + SafeGameObjectPath(best));
            return best;
        }

        if (best != null)
        {
            Logger.LogWarning("melhor candidato para " + spec.Label + " rejeitado por score baixo: " + best.name + " score=" + bestScore);
        }
        return null;
    }

    private static string BuildStrictCandidateCacheKey(ExtraCommandSpec spec, bool itemMode)
    {
        return (itemMode ? "item:" : "spawn:") + NormalizeForCompare(spec != null ? spec.Label : string.Empty);
    }

    private static bool IsLivePlaySpawnedObjectOrChild(GameObject obj)
    {
        try
        {
            if (obj == null) return false;
            Transform current = obj.transform;
            int guard = 0;
            while (current != null && guard++ < 32)
            {
                string n = current.name ?? string.Empty;
                string lower = n.ToLowerInvariant();
                if (lower.StartsWith("liveplay_") || lower.Contains("liveplay_")) return true;
                current = current.parent;
            }
        }
        catch { }
        return false;
    }


    private int ScoreStrictCandidate(GameObject obj, ExtraCommandSpec spec, bool itemMode, bool deepComponentCheck = true)
    {
        string name = obj.name ?? string.Empty;
        string lower = name.ToLowerInvariant();
        string normalizedName = NormalizeForCompare(name);
        if (string.IsNullOrWhiteSpace(normalizedName)) return int.MinValue;

        if (IsLivePlaySpawnedObjectOrChild(obj)) return int.MinValue;

        string[] commonReject = itemMode
            ? new[] { "liveplay", "player", "camera", "audio", "ui", "canvas", "manager", "marker", "locator", "trigger", "zone", "group", "spawner", "spawnpoint", "slot", "holder", "rack" }
            : new[] { "liveplay", "player", "camera", "audio", "ui", "canvas", "manager", "marker", "locator", "trigger", "zone", "group", "spawner", "spawnpoint" };
        foreach (string reject in commonReject)
        {
            if (lower.Contains(reject)) return int.MinValue;
        }
        foreach (string reject in spec.Reject)
        {
            string rejectNorm = NormalizeForCompare(reject);
            if (normalizedName.Contains(rejectNorm) || lower.Contains(reject.ToLowerInvariant())) return int.MinValue;
        }

        int score = 0;
        foreach (string alias in spec.Aliases)
        {
            string normalizedAlias = NormalizeForCompare(alias);
            if (normalizedName == normalizedAlias) score = Math.Max(score, 180);
            else if (normalizedName == normalizedAlias + "item" || normalizedName == normalizedAlias + "prefab") score = Math.Max(score, 160);
            else if (normalizedName.StartsWith(normalizedAlias) || normalizedName.EndsWith(normalizedAlias)) score = Math.Max(score, 120);
            else if (normalizedName.Contains(normalizedAlias)) score = Math.Max(score, 80);
        }
        if (score <= 0) return int.MinValue;

        bool hasUsefulComponent = deepComponentCheck ? HasUsefulComponent(obj, itemMode) : HasUsefulComponentShallow(obj, itemMode);
        if (hasUsefulComponent) score += 35;
        else if (deepComponentCheck) score -= 50;
        else score -= 15;

        if (itemMode && (lower.Contains("bed") || lower.Contains("leaf") || lower.Contains("fence") || lower.Contains("wall") || lower.Contains("shell"))) return int.MinValue;
        if (!itemMode && (lower.Contains("shell") || lower.Contains("corpse") || lower.Contains("dead") || lower.Contains("ragdoll"))) return int.MinValue;

        return score;
    }

    private bool HasUsefulComponentShallow(GameObject obj, bool itemMode)
    {
        try
        {
            foreach (var component in obj.GetComponents<Component>())
            {
                if (component == null) continue;
                string typeName = (component.GetType().FullName ?? component.GetType().Name ?? string.Empty).ToLowerInvariant();
                if (itemMode)
                {
                    if (typeName.Contains("item") || typeName.Contains("pickup") || typeName.Contains("inventory") || typeName.Contains("weapon") || typeName.Contains("tool") || typeName.Contains("edible") || typeName.Contains("food") || typeName.Contains("bandage") || typeName.Contains("dressing")) return true;
                }
                else
                {
                    if (typeName.Contains("animal") || typeName.Contains("creature") || typeName.Contains("ai") || typeName.Contains("enemy") || typeName.Contains("locomotion") || typeName.Contains("movement")) return true;
                }
            }
        }
        catch { }
        return false;
    }

    private bool HasUsefulComponent(GameObject obj, bool itemMode)
    {
        try
        {
            foreach (var component in obj.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue;
                string typeName = (component.GetType().FullName ?? component.GetType().Name ?? string.Empty).ToLowerInvariant();
                if (itemMode)
                {
                    if (typeName.Contains("item") || typeName.Contains("pickup") || typeName.Contains("inventory") || typeName.Contains("weapon") || typeName.Contains("tool") || typeName.Contains("edible") || typeName.Contains("food") || typeName.Contains("bandage") || typeName.Contains("dressing")) return true;
                }
                else
                {
                    if (typeName.Contains("animal") || typeName.Contains("creature") || typeName.Contains("ai") || typeName.Contains("enemy") || typeName.Contains("locomotion") || typeName.Contains("movement")) return true;
                }
            }
        }
        catch { }
        return false;
    }

    private IEnumerable<string> BuildAliasAttempts(ExtraCommandSpec spec)
    {
        HashSet<string> values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Action<string> add = value => { if (!string.IsNullOrWhiteSpace(value)) values.Add(value.Trim()); };
        add(spec.Label);
        add(spec.Label.Replace(" ", "_"));
        add(spec.Label.Replace(" ", string.Empty));
        foreach (string alias in spec.Aliases)
        {
            add(alias);
            add(alias.Replace(" ", "_"));
            add(alias.Replace(" ", string.Empty));
            add(NormalizeForCompare(alias));
        }
        return values;
    }

    private static string NormalizeForCompare(string value)
    {
        if (value == null) return string.Empty;
        return Regex.Replace(value.ToLowerInvariant(), "[^a-z0-9]+", string.Empty);
    }

private void DumpGreenHellSpawnHelpToLog()
    {
        try
        {
            Logger.LogInfo("=== LivePlay Green Hell spawn/help debug start ===");

            string[] helpCommands = new[]
            {
                "spawn help",
                "spawn ?",
                "spawn",
                "help spawn",
                "item help",
                "items help",
                "give help",
                "help item",
                "help items",
                "help give"
            };

            foreach (string cmd in helpCommands)
            {
                bool ok = TryRunDebugConsoleCommand(cmd);
                Logger.LogInfo("[SPAWN-HELP-CMD] " + cmd + " ok=" + ok);
            }

            int methodCount = 0;
            foreach (Type type in SafeAllTypes())
            {
                string typeName = type.FullName ?? type.Name ?? string.Empty;
                string lowerType = typeName.ToLowerInvariant();

                if (!(lowerType.Contains("console") || lowerType.Contains("debug") || lowerType.Contains("cheat") ||
                      lowerType.Contains("command") || lowerType.Contains("spawn") || lowerType.Contains("item")))
                {
                    continue;
                }

                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    string lowerMethod = method.Name.ToLowerInvariant();
                    if (!(lowerMethod.Contains("spawn") || lowerMethod.Contains("command") || lowerMethod.Contains("execute") ||
                          lowerMethod.Contains("run") || lowerMethod.Contains("process") || lowerMethod.Contains("give") ||
                          lowerMethod.Contains("create") || lowerMethod.Contains("item") || lowerMethod.Contains("help")))
                    {
                        continue;
                    }

                    Logger.LogInfo("[SPAWN-METHOD] " + typeName + "." + MethodSignature(method));
                    methodCount++;
                    if (methodCount >= 500) break;
                }

                if (methodCount >= 500) break;
            }

            int candidateCount = 0;
            foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (obj == null || string.IsNullOrWhiteSpace(obj.name)) continue;
                string lower = obj.name.ToLowerInvariant();
                if (!(lower.Contains("stone") || lower.Contains("spear") || lower.Contains("blade") || lower.Contains("knife") ||
                      lower.Contains("hatchet") || lower.Contains("axe") || lower.Contains("club") || lower.Contains("torch") ||
                      lower.Contains("bow") || lower.Contains("arrow") || lower.Contains("inventory") || lower.Contains("backpack") ||
                      lower.Contains("pickup") || lower.Contains("item")))
                {
                    continue;
                }

                Logger.LogInfo("[SPAWN-CANDIDATE] " + obj.name + " path=" + SafeGameObjectPath(obj) + " components=" + SafeComponentNames(obj));
                candidateCount++;
                if (candidateCount >= 400) break;
            }

            Logger.LogInfo("=== LivePlay Green Hell spawn/help debug end. methods=" + methodCount + " candidates=" + candidateCount + " ===");
        }
        catch (Exception ex)
        {
            Logger.LogWarning("debug spawn/help falhou: " + ex);
        }
    }

    private static bool LooksRelevantInventoryType(string lowerType)
    {
        return lowerType.Contains("inventory") ||
               lowerType.Contains("backpack") ||
               lowerType.Contains("item") ||
               lowerType.Contains("items") ||
               lowerType.Contains("pickup") ||
               lowerType.Contains("pickable") ||
               lowerType.Contains("storage") ||
               lowerType.Contains("equipment") ||
               lowerType.Contains("craft") ||
               lowerType.Contains("weapon") ||
               lowerType.Contains("tool");
    }

    private static bool LooksRelevantInventoryMethod(string name)
    {
        string lower = (name ?? string.Empty).ToLowerInvariant();
        return lower.Contains("add") ||
               lower.Contains("create") ||
               lower.Contains("give") ||
               lower.Contains("insert") ||
               lower.Contains("pickup") ||
               lower.Contains("pick") ||
               lower.Contains("take") ||
               lower.Contains("spawn") ||
               lower.Contains("drop") ||
               lower.Contains("get") ||
               lower.Contains("find") ||
               lower.Contains("equip") ||
               lower.Contains("place");
    }

    private static bool LooksRelevantInventoryMember(string name)
    {
        string lower = (name ?? string.Empty).ToLowerInvariant();
        return lower.Contains("item") ||
               lower.Contains("inventory") ||
               lower.Contains("backpack") ||
               lower.Contains("pickup") ||
               lower.Contains("pickable") ||
               lower.Contains("id") ||
               lower.Contains("name") ||
               lower.Contains("prefab") ||
               lower.Contains("weapon") ||
               lower.Contains("tool");
    }

    private static IEnumerable<Type> SafeAllTypes()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types = new Type[0];
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }
            catch { }

            foreach (Type type in types)
            {
                if (type != null) yield return type;
            }
        }
    }

    private static string MethodSignature(MethodInfo method)
    {
        try
        {
            string parameters = string.Join(", ", method.GetParameters().Select(p => SafeTypeName(p.ParameterType) + " " + p.Name).ToArray());
            return method.Name + "(" + parameters + ") : " + SafeTypeName(method.ReturnType) + (method.IsStatic ? " static" : "");
        }
        catch
        {
            return method != null ? method.Name : "?";
        }
    }

    private static string SafeTypeName(Type type)
    {
        try { return type == null ? "void" : (type.FullName ?? type.Name ?? "?"); }
        catch { return "?"; }
    }

    private static string SafeGameObjectPath(GameObject obj)
    {
        try
        {
            if (obj == null) return "?";
            List<string> names = new List<string>();
            Transform current = obj.transform;
            int guard = 0;
            while (current != null && guard++ < 24)
            {
                names.Add(current.name);
                current = current.parent;
            }
            names.Reverse();
            return string.Join("/", names.ToArray());
        }
        catch { return obj != null ? obj.name : "?"; }
    }

    private bool TryInsertCloneIntoBackpackByReflection(GameObject clone, ExtraCommandSpec spec)
    {
        try
        {
            if (clone == null) return false;

            Component itemComponent = FindItemComponentOnClone(clone);
            if (itemComponent == null)
            {
                Logger.LogWarning("give/inventory: " + spec.Label + " nao tem componente Item no clone " + clone.name);
                return false;
            }

            Type backpackType = FindTypeBySimpleName("InventoryBackpack");
            if (backpackType == null)
            {
                Logger.LogWarning("give/inventory: InventoryBackpack nao encontrado.");
                return false;
            }

            object backpack = null;

            MethodInfo getMethod = backpackType.GetMethod("Get", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (getMethod != null)
            {
                try { backpack = getMethod.Invoke(null, null); } catch (Exception ex) { Logger.LogWarning("give/inventory: InventoryBackpack.Get erro: " + ex.Message); }
            }

            if (backpack == null)
            {
                foreach (MonoBehaviour behaviour in FindObjectsOfType<MonoBehaviour>())
                {
                    if (behaviour != null && backpackType.IsAssignableFrom(behaviour.GetType()))
                    {
                        backpack = behaviour;
                        break;
                    }
                }
            }

            if (backpack == null)
            {
                Logger.LogWarning("give/inventory: instancia InventoryBackpack nao encontrada.");
                return false;
            }

            object slot = TryFindFreeBackpackSlot(backpack, itemComponent);
            if (slot == null)
            {
                Logger.LogWarning("give/inventory: FindFreeSlot nao retornou slot para " + spec.Label);
            }

            if (TryInvokeBackpackInsert(backpack, itemComponent, slot))
            {
                MarkItemAsInventoryItem(itemComponent, true);
                TryHideDroppedCloneAfterInventoryInsert(clone);
                return true;
            }

            Logger.LogWarning("give/inventory: InsertItem falhou para " + spec.Label);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("give/inventory: erro ao inserir " + spec.Label + " no backpack: " + ex);
        }
        return false;
    }

    private Component FindItemComponentOnClone(GameObject clone)
    {
        try
        {
            foreach (Component component in clone.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue;
                string name = component.GetType().Name ?? string.Empty;
                if (name == "Item") return component;
            }

            foreach (Component component in clone.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue;
                string name = component.GetType().Name ?? string.Empty;
                if (name.EndsWith("Item", StringComparison.OrdinalIgnoreCase) && !name.Contains("Info"))
                {
                    return component;
                }
            }
        }
        catch { }
        return null;
    }

    private Type FindTypeBySimpleName(string simpleName)
    {
        try
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
                    if (string.Equals(type.Name, simpleName, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(type.FullName, simpleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return type;
                    }
                }
            }
        }
        catch { }
        return null;
    }

    private object TryFindFreeBackpackSlot(object backpack, Component itemComponent)
    {
        try
        {
            Type backpackType = backpack.GetType();
            foreach (MethodInfo method in backpackType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!string.Equals(method.Name, "FindFreeSlot", StringComparison.OrdinalIgnoreCase)) continue;
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length != 1) continue;

                Type p0 = parameters[0].ParameterType;
                if (!p0.IsAssignableFrom(itemComponent.GetType()) && !itemComponent.GetType().IsAssignableFrom(p0)) continue;

                try
                {
                    object result = method.Invoke(backpack, new object[] { itemComponent });
                    Logger.LogInfo("give/inventory: FindFreeSlot result=" + (result != null ? result.GetType().Name : "null"));
                    if (result != null) return result;
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("give/inventory: FindFreeSlot erro: " + ex.Message);
                }
            }
        }
        catch { }
        return null;
    }

    private bool TryInvokeBackpackInsert(object backpack, Component itemComponent, object slot)
    {
        Type backpackType = backpack.GetType();

        foreach (MethodInfo method in backpackType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (!string.Equals(method.Name, "InsertItem", StringComparison.OrdinalIgnoreCase)) continue;

            ParameterInfo[] p = method.GetParameters();
            object[] args = new object[p.Length];
            bool canBuild = true;

            for (int i = 0; i < p.Length; i++)
            {
                Type t = p[i].ParameterType;
                string pname = (p[i].Name ?? string.Empty).ToLowerInvariant();

                if (t.IsAssignableFrom(itemComponent.GetType()))
                {
                    args[i] = itemComponent;
                }
                else if (slot != null && t.IsAssignableFrom(slot.GetType()))
                {
                    args[i] = slot;
                }
                else if (t == typeof(bool))
                {
                    if (pname.Contains("drop")) args[i] = false;
                    else if (pname.Contains("notify")) args[i] = true;
                    else if (pname.Contains("auto")) args[i] = true;
                    else if (pname.Contains("pocket")) args[i] = false;
                    else args[i] = true;
                }
                else if (!t.IsValueType)
                {
                    args[i] = null;
                }
                else
                {
                    canBuild = false;
                    break;
                }
            }

            if (!canBuild) continue;

            try
            {
                object result = method.Invoke(backpack, args);
                Logger.LogInfo("give/inventory: " + method.Name + "(" + p.Length + ") result=" + (result != null ? result.ToString() : "null"));
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("give/inventory: " + method.Name + "(" + p.Length + ") erro: " + ex.Message);
            }
        }

        foreach (string methodName in new[] { "InsertItemTop", "InsertItemLeft", "InsertItemRight" })
        {
            foreach (MethodInfo method in backpackType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!string.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase)) continue;
                ParameterInfo[] p = method.GetParameters();
                if (p.Length != 2) continue;
                if (!p[0].ParameterType.IsAssignableFrom(itemComponent.GetType())) continue;
                if (slot == null || !p[1].ParameterType.IsAssignableFrom(slot.GetType())) continue;

                try
                {
                    object result = method.Invoke(backpack, new object[] { itemComponent, slot });
                    Logger.LogInfo("give/inventory: " + methodName + " result=" + (result != null ? result.ToString() : "null"));
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("give/inventory: " + methodName + " erro: " + ex.Message);
                }
            }
        }

        return false;
    }

    private void MarkItemAsInventoryItem(Component itemComponent, bool inInventory)
    {
        try
        {
            Type type = itemComponent.GetType();

            foreach (string fieldName in new[] { "m_InInventory", "m_InInventoryProp", "m_InInventory_Repl", "m_ShownInInventory" })
            {
                try
                {
                    FieldInfo f = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (f != null && f.FieldType == typeof(bool)) f.SetValue(itemComponent, inInventory);
                }
                catch { }
            }

            foreach (string propName in new[] { "m_InInventory", "m_InInventoryProp", "m_ShownInInventory" })
            {
                try
                {
                    PropertyInfo p = type.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (p != null && p.CanWrite && p.PropertyType == typeof(bool)) p.SetValue(itemComponent, inInventory, null);
                }
                catch { }
            }

            foreach (string methodName in new[] { "OnAddToInventory", "ItemsManagerRegister" })
            {
                try
                {
                    MethodInfo m = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (m == null) continue;
                    ParameterInfo[] ps = m.GetParameters();
                    if (ps.Length == 0) m.Invoke(itemComponent, null);
                    else if (ps.Length == 1 && ps[0].ParameterType == typeof(bool)) m.Invoke(itemComponent, new object[] { true });
                }
                catch { }
            }
        }
        catch { }
    }

    private void TryHideDroppedCloneAfterInventoryInsert(GameObject clone)
    {
        try
        {
            // Se o jogo realmente registrou o item no backpack, evitamos uma copia visual duplicada no chÃ£o.
            foreach (Renderer renderer in clone.GetComponentsInChildren<Renderer>(true))
            {
                try { renderer.enabled = false; } catch { }
            }
            foreach (Collider collider in clone.GetComponentsInChildren<Collider>(true))
            {
                try { collider.enabled = false; } catch { }
            }
        }
        catch { }
    }

private static string SafeComponentNames(GameObject obj)
    {
        try
        {
            return string.Join(",", obj.GetComponentsInChildren<Component>(true).Where(c => c != null).Select(c => c.GetType().Name).Distinct().Take(12).ToArray());
        }
        catch { return "?"; }
    }

    private static bool TryInvokeMethod(object target, string methodName, object[] args)
    {
        foreach (var method in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (!string.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase)) continue;
            if (method.GetParameters().Length != args.Length) continue;
            try
            {
                method.Invoke(target, args);
                return true;
            }
            catch {}
        }
        return false;
    }
}


