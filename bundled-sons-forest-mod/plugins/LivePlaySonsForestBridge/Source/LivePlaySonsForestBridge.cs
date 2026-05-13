using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

[BepInPlugin("br.liveplay.sonsforest.bridge", "LivePlay Sons Forest Bridge", "0.2.0")]
public sealed class LivePlaySonsForestBridge : BasePlugin
{
    private TcpListener? _listener;
    private Thread? _serverThread;
    private volatile bool _running;
    private int _port = 35952;
    private ManualLogSource? _log;

    public override void Load()
    {
        _log = Log;
        LoadConfig();
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
                ExecuteCommand(command.Trim());
                WriteJson(stream, 200, "{\"ok\":true,\"executed\":true,\"bridge\":\"sons-forest\",\"version\":\"0.2.0\"}");
                return;
            }

            WriteJson(stream, 200, "{\"ok\":true,\"bridge\":\"sons-forest\",\"version\":\"0.2.0\"}");
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

    private static void WriteJson(NetworkStream stream, int status, string body)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
        string statusText = status == 200 ? "OK" : "Not Found";
        string head = $"HTTP/1.1 {status} {statusText}\r\nContent-Type: application/json; charset=utf-8\r\nContent-Length: {bodyBytes.Length}\r\nConnection: close\r\n\r\n";
        byte[] headBytes = Encoding.ASCII.GetBytes(head);
        stream.Write(headBytes, 0, headBytes.Length);
        stream.Write(bodyBytes, 0, bodyBytes.Length);
    }

    private void ExecuteCommand(string raw)
    {
        string command = (raw ?? string.Empty).Trim().ToLowerInvariant();
        if (command.Length == 0) return;

        _log?.LogInfo("LivePlay command: " + command);

        switch (command)
        {
            case "ping":
            case "app-open":
                _log?.LogInfo("LivePlay ping recebido.");
                break;

            case "set_time_day":
            case "set_time_night":
            case "heal_player":
            case "damage_player":
            case "give_item":
            case "spawn_cannibal":
            case "spawn_mutant":
            case "toggle_storm":
            case "play_scare":
            case "clear_enemies":
                _log?.LogWarning("Comando recebido pelo bridge, mas ainda sem binding interno do Sons Of The Forest: " + command);
                break;

            default:
                _log?.LogWarning("Comando não suportado pelo bridge Sons Forest: " + command);
                break;
        }
    }
}
