using System;
using System.Collections.Concurrent;
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
    private readonly ConcurrentQueue<string> _commands = new ConcurrentQueue<string>();
    private TcpListener _listener;
    private Thread _serverThread;
    private bool _running;
    private int _port = 35951;

    public LivePlayGtaBridge()
    {
        Interval = 50;
        Tick += OnTick;
        Aborted += OnAborted;
        LoadConfig();
        StartServer();
        WriteLog("Bridge iniciado na porta " + _port);
        Notify("~g~LivePlay GTA Bridge conectado na porta " + _port);
    }

    private void LoadConfig()
    {
        try
        {
            string configPath = Path.Combine("scripts", "LivePlayGtaBridge.json");
            if (!File.Exists(configPath)) return;
            string raw = File.ReadAllText(configPath);
            Match port = Regex.Match(raw, "\\\"bridgePort\\\"\\s*:\\s*(\\d+)");
            if (port.Success && int.TryParse(port.Groups[1].Value, out int parsed) && parsed > 0) _port = parsed;
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
                ThreadPool.QueueUserWorkItem(_ => HandleClient(client));
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

                _commands.Enqueue(command.Trim());
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
        int limit = 10;
        while (limit-- > 0 && _commands.TryDequeue(out string command)) ExecuteLivePlayCommand(command);
    }

    private void ExecuteLivePlayCommand(string command)
    {
        string normalized = (command ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized)) return;

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
        if (normalized.StartsWith("explode")) { ExplodeFront(); return; }
        if (normalized.StartsWith("spawn_moto_cops")) { SpawnMotoGroup("s_m_y_cop_01", true, 2, "LivePlay: moto cops"); return; }
        if (normalized.StartsWith("spawn_moto_bandits")) { SpawnMotoGroup("g_m_y_lost_01", true, 2, "LivePlay: moto bandidos"); return; }
        if (normalized.StartsWith("spawn_vehicle")) { SpawnVehicle(Arg(command, 1, "adder")); return; }
        if (normalized.StartsWith("spawn_ped")) { SpawnPed(Arg(command, 1, "s_m_y_cop_01")); return; }
        if (normalized.StartsWith("repair_vehicle")) { RepairVehicle(); return; }
        if (normalized.StartsWith("boost_vehicle")) { BoostVehicle(); return; }
        if (normalized.StartsWith("flip_vehicle")) { FlipVehicle(); return; }
        if (normalized.StartsWith("break_vehicle")) { BreakVehicle(); return; }
        if (normalized.StartsWith("weather")) { SetWeather(Arg(command, 1, "THUNDER")); return; }
        if (normalized.StartsWith("time")) { SetTime(ParseInt(Arg(command, 1, "12"), 12)); return; }
        if (normalized.StartsWith("blackout_on")) { Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, true); Notify("LivePlay: blackout on"); return; }
        if (normalized.StartsWith("blackout_off")) { Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, false); Notify("LivePlay: blackout off"); return; }

        Notify("LivePlay comando desconhecido: " + command);
    }

    private static string Arg(string command, int index, string fallback)
    {
        string[] parts = (command ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > index ? parts[index] : fallback;
    }

    private static int ParseInt(string value, int fallback) { return int.TryParse(value, out int parsed) ? parsed : fallback; }
    private static int Clamp(int value, int min, int max) { if (value < min) return min; if (value > max) return max; return value; }

    private void SpawnVehicle(string modelName)
    {
        Model model = new Model(modelName);
        model.Request(1200);
        if (!model.IsLoaded) { Notify("LivePlay: veículo não carregou " + modelName); return; }
        Ped player = Game.Player.Character;
        Vector3 pos = player.Position + player.ForwardVector * 6f + new Vector3(0f, 0f, 0.5f);
        Vehicle vehicle = World.CreateVehicle(model, pos, player.Heading);
        if (vehicle != null) { vehicle.PlaceOnGround(); vehicle.IsEngineRunning = true; }
        model.MarkAsNoLongerNeeded();
        Notify("LivePlay: veículo " + modelName);
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
            ped.Weapons.Give(WeaponHash.Pistol, 120, true, true);
            ped.Task.FightAgainst(player);
        }
        model.MarkAsNoLongerNeeded();
        Notify("LivePlay: ped " + modelName);
    }

    private void Earthquake()
    {
        Ped player = Game.Player.Character;
        Vector3 origin = player.Position;
        World.AddExplosion(origin + new Vector3(0f, 0f, -1f), ExplosionType.Grenade, 0.15f, 0.4f);
        Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "LARGE_EXPLOSION_SHAKE", 1.15f);
        player.ApplyForce(new Vector3(0f, 0f, 7.5f));
        Notify("LivePlay: terremoto");
    }

    private void Drunk()
    {
        Function.Call(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character.Handle, 1800, 2600, 0, true, true, false);
        Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "DRUNK_SHAKE", 0.85f);
        Notify("LivePlay: bêbado");
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
            Model bikeModel = new Model("bati");
            Model pedModel = new Model(pedModelName);
            bikeModel.Request(1200);
            pedModel.Request(1200);
            if (!bikeModel.IsLoaded || !pedModel.IsLoaded) continue;
            Vector3 offset = player.ForwardVector * (8f + i * 2f) + player.RightVector * (i % 2 == 0 ? 3f : -3f);
            Vehicle bike = World.CreateVehicle(bikeModel, player.Position + offset, player.Heading + 180f);
            Ped ped = World.CreatePed(pedModel, player.Position + offset + new Vector3(0f, 0f, 1f), player.Heading + 180f);
            if (ped != null)
            {
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


    private void ExecuteLivePlayEffectSlug(string slug)
    {
        slug = NormalizeSlug(slug);
        if (string.IsNullOrWhiteSpace(slug)) return;

        // Todos os comandos vindos do seletor chegam aqui como "lp <slug>".
        // Os efeitos abaixo cobrem por família de comando. Assim nenhum item do seletor fica morto.
        if (slug.Contains("nothing") || slug == "afk" || slug.Contains("pause") || slug.Contains("no_chaos")) { Notify("LivePlay GTA: " + Pretty(slug)); return; }

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
        if (slug.Contains("jump") || slug.Contains("super_jump") || slug.Contains("rocket_man") || slug.Contains("skydive")) { LaunchPlayer(); return; }
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
        Notify("LivePlay GTA: invencível temporário");
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
        if (slug.Contains("rpg") || slug.Contains("rocket")) GiveWeapon("rpg");
        else if (slug.Contains("sniper")) GiveWeapon("sniper");
        else if (slug.Contains("minigun")) GiveWeapon("minigun");
        else if (slug.Contains("shotgun")) GiveWeapon("shotgun");
        else if (slug.Contains("railgun")) GiveWeapon("railgun");
        else GiveWeapon("carbine");
    }

    private void ArmNearbyPeds(string slug)
    {
        Ped player = Game.Player.Character;
        Ped[] peds = World.GetNearbyPeds(player, 45f);
        foreach (Ped ped in peds)
        {
            if (ped == null || !ped.Exists() || ped == player) continue;
            ped.Weapons.Give(slug.Contains("rpg") ? WeaponHash.RPG : WeaponHash.CarbineRifle, 300, true, true);
            if (!slug.Contains("friendly")) ped.Task.FightAgainst(player);
        }
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void SpawnEffect(string slug)
    {
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

        if (slug.Contains("repair")) { RepairVehicle(); return; }
        if (slug.Contains("boost") || slug.Contains("speed") || slug.Contains("acceleration") || slug.Contains("nitro") || slug.Contains("need_for_speed")) { BoostVehicle(); return; }
        if (slug.Contains("flip") || slug.Contains("turtle")) { FlipNearbyVehicles(); return; }
        if (slug.Contains("delete") || slug.Contains("remove_current") || slug.Contains("remove_spawned") || slug.Contains("remove_current_vehicle")) { if (current != null) current.Delete(); Notify("LivePlay GTA: veículo removido"); return; }
        if (slug.Contains("explode") || slug.Contains("detonate")) { if (current != null) World.AddExplosion(current.Position, ExplosionType.Grenade, 5f, 1f); else ExplodeFront(); Notify("LivePlay GTA: veículo explodiu"); return; }
        if (slug.Contains("break") || slug.Contains("engine")) { BreakVehicle(); return; }
        if (slug.Contains("tire") || slug.Contains("wheel")) { PopVehicleTires(); return; }
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

    private void PopVehicleTires()
    {
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null) { Notify("LivePlay GTA: sem veículo"); return; }
        for (int i = 0; i < 8; i++) Function.Call(Hash.SET_VEHICLE_TYRE_BURST, vehicle.Handle, i, true, 1000f);
        Notify("LivePlay GTA: pneus furados");
    }

    private void FlipNearbyVehicles()
    {
        foreach (Vehicle v in World.GetNearbyVehicles(Game.Player.Character, 45f))
        {
            if (v == null || !v.Exists()) continue;
            v.Rotation = new Vector3(180f, 0f, v.Rotation.Z);
        }
        Notify("LivePlay GTA: veículos virados");
    }

    private void LaunchNearbyVehicles(string slug)
    {
        foreach (Vehicle v in World.GetNearbyVehicles(Game.Player.Character, 55f))
        {
            if (v == null || !v.Exists()) continue;
            v.ApplyForce(new Vector3(0f, 0f, slug.Contains("low") ? 4f : 20f));
        }
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void WorldEffect(string slug)
    {
        if (slug.Contains("blackout")) { Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, !slug.Contains("off")); Notify("LivePlay GTA: blackout"); return; }
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
        if (slug.Contains("up") || slug.Contains("heaven")) target = player.Position + new Vector3(0f, 0f, 120f);
        else if (slug.Contains("airport")) target = new Vector3(-1034f, -2733f, 20f);
        else if (slug.Contains("maze")) target = new Vector3(-75f, -818f, 326f);
        else if (slug.Contains("zancudo")) target = new Vector3(-2047f, 3132f, 32f);
        else if (slug.Contains("chiliad")) target = new Vector3(501f, 5604f, 797f);
        else if (slug.Contains("random")) target = player.Position + new Vector3(RandomFloat(-400f, 400f), RandomFloat(-400f, 400f), 60f);
        player.Position = target;
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private static float RandomFloat(float min, float max)
    {
        return (float)(new Random().NextDouble() * (max - min) + min);
    }

    private void VisualEffect(string slug)
    {
        if (slug.Contains("hud") || slug.Contains("radar")) Function.Call(Hash.DISPLAY_RADAR, false);
        if (slug.Contains("night_vision")) Function.Call(Hash.SET_NIGHTVISION, true);
        if (slug.Contains("heat_vision")) Function.Call(Hash.SET_SEETHROUGH, true);
        Function.Call(Hash.SHAKE_GAMEPLAY_CAM, slug.Contains("spinning") ? "DRUNK_SHAKE" : "SMALL_EXPLOSION_SHAKE", 0.8f);
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void ArenaEffect(string slug)
    {
        if (slug.Contains("super_jump") || slug.Contains("add_life") || slug.Contains("add_time")) { LaunchPlayer(); return; }
        ExplosionRing();
        Notify("LivePlay GTA: " + Pretty(slug));
    }

    private void MeteorShower()
    {
        Ped player = Game.Player.Character;
        for (int i = 0; i < 5; i++)
        {
            Vector3 pos = player.Position + new Vector3(RandomFloat(-18f, 18f), RandomFloat(-18f, 18f), 0f);
            World.AddExplosion(pos, ExplosionType.Grenade, 3f, 1f);
        }
        Notify("LivePlay GTA: meteor shower");
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

    private void ExplosiveChaos(string slug)
    {
        if (slug.Contains("all") || slug.Contains("nearby") || slug.Contains("ring")) ExplosionRing();
        else ExplodeFront();
    }

    private void FireChaos(string slug)
    {
        Ped player = Game.Player.Character;
        World.AddExplosion(player.Position + player.ForwardVector * 3f, ExplosionType.Molotov1, 1.8f, 0.4f);
        Notify("LivePlay GTA: " + Pretty(slug));
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
        try { _listener?.Stop(); } catch { }
    }
}
