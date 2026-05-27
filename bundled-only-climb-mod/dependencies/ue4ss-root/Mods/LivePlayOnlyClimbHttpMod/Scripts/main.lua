local MOD_NAME = "LivePlayOnlyClimbHttpMod"
local VERSION = "1.7.0-no-ragdoll"
local QUEUE_FILES = {
    "Mods/LivePlayOnlyClimbHttpMod/liveplay-commands.queue",
    "./Mods/LivePlayOnlyClimbHttpMod/liveplay-commands.queue"
}
local HEARTBEAT_FILES = {
    "Mods/LivePlayOnlyClimbHttpMod/liveplay-bridge-heartbeat.json",
    "./Mods/LivePlayOnlyClimbHttpMod/liveplay-bridge-heartbeat.json"
}
local ACK_FILES = {
    "Mods/LivePlayOnlyClimbHttpMod/liveplay-bridge-ack.json",
    "./Mods/LivePlayOnlyClimbHttpMod/liveplay-bridge-ack.json"
}
local DEBUG_FILES = {
    "Mods/LivePlayOnlyClimbHttpMod/liveplay-debug.log",
    "./Mods/LivePlayOnlyClimbHttpMod/liveplay-debug.log"
}

local UEHelpers = nil
pcall(function()
    UEHelpers = require("UEHelpers")
end)

local active_reset_at = 0
local active_spin_until = 0
local active_spin_step = 35
local active_wind_until = 0
local active_wind_x = 0
local active_wind_y = 0
local active_shake_until = 0
local active_shake_strength = 420
local active_bounce_until = 0
local active_bounce_strength = 900
local active_reverse_until = 0
local active_slippery_until = 0
local active_no_jump_until = 0
local active_input_delay_until = 0
local active_original_scale = nil
local last_heartbeat_at = 0
-- Checkpoint/backup removido nesta versão para não interferir nos comandos novos.
math.randomseed(os.time())

local last_command_second = {}
local COMMAND_MIN_INTERVAL = {
    random_teleport = 1,
    push_left = 1,
    push_right = 1,
    push_forward = 1,
    push_back = 1,
    wind_forward = 2,
    wind_left = 2,
    wind_right = 2,
    wind_back = 2,
    bounce = 2,
    earthquake = 2,    random_teleport = 1,
    slippery_floor = 2,
    reverse_controls = 2,
    input_delay = 2,
    no_jump = 2,
}

local function cleanup_old_backup_files()
    -- Backup/checkpoint antigo desativado.
    -- Não chama processos externos para evitar janela preta piscando ao abrir o jogo.
    local files = {
        "Mods/LivePlayOnlyClimbHttpMod/liveplay-checkpoint.json",
        "Mods/LivePlayOnlyClimbHttpMod/liveplay-backup.json",
        "Mods/LivePlayOnlyClimbHttpMod/liveplay-position-backup.json",
        "./Mods/LivePlayOnlyClimbHttpMod/liveplay-checkpoint.json",
        "./Mods/LivePlayOnlyClimbHttpMod/liveplay-backup.json",
        "./Mods/LivePlayOnlyClimbHttpMod/liveplay-position-backup.json"
    }
    for _, file_path in ipairs(files) do
        pcall(function() os.remove(file_path) end)
    end
end

local function now_seconds()
    return os.time()
end

local function trim(value)
    return tostring(value or ""):gsub("^%s+", ""):gsub("%s+$", "")
end

local function first_writable_path(paths)
    for _, file_path in ipairs(paths) do
        local file = io.open(file_path, "a")
        if file then
            file:close()
            return file_path
        end
    end
    return paths[1]
end

local HEARTBEAT_FILE = first_writable_path(HEARTBEAT_FILES)
local ACK_FILE = first_writable_path(ACK_FILES)
local DEBUG_FILE = first_writable_path(DEBUG_FILES)

local function write_text(file_path, content)
    local file = io.open(file_path, "w")
    if file then
        file:write(content)
        file:close()
        return true
    end
    return false
end

local function append_text(file_path, content)
    local file = io.open(file_path, "a")
    if file then
        file:write(content)
        file:close()
        return true
    end
    return false
end

local function log(message)
    local line = string.format("[%s] %s", MOD_NAME, tostring(message))
    print(line)
    append_text(DEBUG_FILE, os.date("[%Y-%m-%d %H:%M:%S] ") .. line .. "\n")
end

cleanup_old_backup_files()

local function json_safe(value)
    return tostring(value or "")
        :gsub("\\", "\\\\")
        :gsub('"', "'")
        :gsub("\r", " ")
        :gsub("\n", " ")
end

local function append_ack(command, ok, message)
    local file = io.open(ACK_FILE, "w")
    if not file then return end
    file:write(string.format('{"ok":%s,"command":"%s","message":"%s","version":"%s","at":%d}',
        ok and "true" or "false",
        json_safe(command),
        json_safe(message),
        VERSION,
        now_seconds()
    ))
    file:close()
end

local function heartbeat(force)
    local current = now_seconds()
    if not force and current == last_heartbeat_at then return end
    last_heartbeat_at = current
    write_text(HEARTBEAT_FILE, string.format('{"ok":true,"game":"OnlyClimb","bridge":"ue4ss-file","version":"%s","at":%d}', VERSION, current))
end

local function execute_console(command)
    command = trim(command)
    if command == "" then return false, "console command vazio" end
    if not UEHelpers then return false, "UEHelpers não carregou" end

    local ok, err = pcall(function()
        local system = UEHelpers.GetKismetSystemLibrary()
        local world_context = UEHelpers.GetWorldContextObject()
        local controller = UEHelpers.GetPlayerController()
        system:ExecuteConsoleCommand(world_context, command, controller)
    end)

    if ok then return true, "console: " .. command end
    return false, tostring(err)
end

local function get_player_controller()
    if not UEHelpers then return nil end
    local ok, controller = pcall(function()
        return UEHelpers.GetPlayerController()
    end)
    if ok and controller and controller:IsValid() then return controller end
    return nil
end

local function get_player_pawn()
    local controller = get_player_controller()
    if not controller then return nil end
    local ok, pawn = pcall(function()
        if controller.Pawn and controller.Pawn:IsValid() then return controller.Pawn end
        if controller.GetPawn then
            local p = controller:GetPawn()
            if p and p:IsValid() then return p end
        end
        return nil
    end)
    if ok then return pawn end
    return nil
end

local function vec(x, y, z)
    return { X = tonumber(x) or 0, Y = tonumber(y) or 0, Z = tonumber(z) or 0 }
end

local function rot(pitch, yaw, roll)
    return { Pitch = tonumber(pitch) or 0, Yaw = tonumber(yaw) or 0, Roll = tonumber(roll) or 0 }
end

local function get_actor_location(actor)
    if not actor then return nil end
    local ok, loc = pcall(function()
        if actor.K2_GetActorLocation then return actor:K2_GetActorLocation() end
        if actor.GetActorLocation then return actor:GetActorLocation() end
        if actor.RootComponent and actor.RootComponent:IsValid() then
            return actor.RootComponent.RelativeLocation
        end
        return nil
    end)
    if ok and loc then
        return vec(loc.X or 0, loc.Y or 0, loc.Z or 0)
    end
    return nil
end

local function get_actor_rotation(actor)
    if not actor then return nil end
    local ok, value = pcall(function()
        if actor.K2_GetActorRotation then return actor:K2_GetActorRotation() end
        if actor.GetActorRotation then return actor:GetActorRotation() end
        return nil
    end)
    if ok and value then
        return rot(value.Pitch or 0, value.Yaw or 0, value.Roll or 0)
    end
    return nil
end

local function get_forward_vector(actor)
    if not actor then return nil end
    local ok, value = pcall(function()
        if actor.GetActorForwardVector then return actor:GetActorForwardVector() end
        return nil
    end)
    if ok and value then return vec(value.X or 0, value.Y or 0, value.Z or 0) end
    local r = get_actor_rotation(actor)
    if not r then return vec(1, 0, 0) end
    local rad = (r.Yaw or 0) * math.pi / 180
    return vec(math.cos(rad), math.sin(rad), 0)
end

local function set_actor_location(actor, location)
    if not actor or not location then return false, "actor/location inválido" end
    local attempts = {
        function() return actor:K2_SetActorLocation(location, false, {}, true) end,
        function() return actor:K2_SetActorLocation(location, false, {}, false) end,
        function() return actor:SetActorLocation(location, false, {}, true) end,
        function() return actor:SetActorLocation(location) end,
    }
    for _, attempt in ipairs(attempts) do
        local ok, err = pcall(attempt)
        if ok then return true, "posição aplicada" end
    end
    return false, "falha ao aplicar posição"
end

local function set_actor_rotation(actor, rotation)
    if not actor or not rotation then return false, "actor/rotation inválido" end
    local attempts = {
        function() return actor:K2_SetActorRotation(rotation, false) end,
        function() return actor:SetActorRotation(rotation) end,
    }
    for _, attempt in ipairs(attempts) do
        local ok, err = pcall(attempt)
        if ok then return true, "rotação aplicada" end
    end
    return false, "falha ao aplicar rotação"
end

local function launch_player(x, y, z)
    local pawn = get_player_pawn()
    if not pawn then return false, "player pawn não encontrado" end

    local ok, err = pcall(function()
        if pawn.LaunchCharacter then
            pawn:LaunchCharacter(vec(x, y, z), false, true)
        elseif pawn.CharacterMovement and pawn.CharacterMovement:IsValid() then
            pawn.CharacterMovement.Velocity = vec(x, y, z)
        else
            error("LaunchCharacter/CharacterMovement indisponível")
        end
    end)

    if ok then return true, "impulso aplicado" end
    return false, tostring(err)
end

local function push_relative(forward_strength, side_strength, up_strength)
    local pawn = get_player_pawn()
    if not pawn then return false, "player pawn não encontrado" end
    local f = get_forward_vector(pawn) or vec(1, 0, 0)
    local right = vec(f.Y, -f.X, 0)
    local x = (f.X * (forward_strength or 0)) + (right.X * (side_strength or 0))
    local y = (f.Y * (forward_strength or 0)) + (right.Y * (side_strength or 0))
    return launch_player(x, y, up_strength or 250)
end

local function teleport_relative(forward_distance, side_distance, up_distance)
    local pawn = get_player_pawn()
    if not pawn then return false, "player pawn não encontrado" end
    local loc = get_actor_location(pawn)
    if not loc then return false, "posição atual não encontrada" end
    local f = get_forward_vector(pawn) or vec(1, 0, 0)
    local right = vec(f.Y, -f.X, 0)
    local target = vec(
        loc.X + f.X * (forward_distance or 0) + right.X * (side_distance or 0),
        loc.Y + f.Y * (forward_distance or 0) + right.Y * (side_distance or 0),
        loc.Z + (up_distance or 0)
    )
    return set_actor_location(pawn, target)
end


local function random_teleport(distance)
    local pawn = get_player_pawn()
    if not pawn then return false, "player pawn não encontrado" end
    local loc = get_actor_location(pawn)
    if not loc then return false, "posição atual não encontrada" end
    local amount = math.max(150, math.min(2500, tonumber(distance) or 800))
    local target = vec(
        loc.X + math.random(-amount, amount),
        loc.Y + math.random(-amount, amount),
        loc.Z + math.random(80, math.floor(amount / 2))
    )
    return set_actor_location(pawn, target)
end

local function teleport_vertical(z)
    local pawn = get_player_pawn()
    if not pawn then return false, "player pawn não encontrado" end
    local loc = get_actor_location(pawn)
    if not loc then return false, "posição atual não encontrada" end
    return set_actor_location(pawn, vec(loc.X, loc.Y, loc.Z + (tonumber(z) or 0)))
end

local function start_bounce(seconds, strength)
    active_bounce_until = now_seconds() + math.max(2, math.min(15, tonumber(seconds) or 8))
    active_bounce_strength = math.max(350, math.min(1800, tonumber(strength) or 900))
    return true, "quique iniciado"
end

local function set_slomo(scale, duration)
    local ok, msg = execute_console("slomo " .. tostring(scale))
    if ok and duration and tonumber(duration) and tonumber(duration) > 0 then
        active_reset_at = now_seconds() + math.floor(tonumber(duration))
    end
    return ok, msg
end

local function reset_effects()
    active_reset_at = 0
    active_spin_until = 0
    active_wind_until = 0
    active_shake_until = 0
    active_bounce_until = 0
    active_reverse_until = 0
    active_slippery_until = 0
    active_no_jump_until = 0
    active_input_delay_until = 0
    execute_console("slomo 1")
    execute_console("r.TonemapperGamma 2.2")
    execute_console("r.Color.Mid 0.5")
    execute_console("r.Color.Max 1")
    local pawn = get_player_pawn()
    if pawn and active_original_scale then
        pcall(function()
            if pawn.SetActorScale3D then pawn:SetActorScale3D(active_original_scale) end
        end)
    end
    active_original_scale = nil
    local controller = get_player_controller()
    if controller then
        pcall(function()
            if controller.ClientSetCameraFade then controller:ClientSetCameraFade(false, { R = 0, G = 0, B = 0, A = 0 }, { R = 0, G = 0, B = 0, A = 0 }, 0.15, false, false) end
        end)
    end
    return true, "efeitos resetados"
end

-- Sistema antigo de checkpoint/backup removido.

local function spin_once(amount)
    local pawn = get_player_pawn()
    if not pawn then return false, "player pawn não encontrado" end
    local r = get_actor_rotation(pawn)
    if not r then return false, "rotação do player não encontrada" end
    r.Yaw = (r.Yaw or 0) + (tonumber(amount) or 180)
    return set_actor_rotation(pawn, r)
end

local function start_spin(total_degrees, seconds, direction)
    active_spin_until = now_seconds() + math.max(2, math.min(10, tonumber(seconds) or 4))
    local sign = direction == "reverse" and -1 or 1
    active_spin_step = sign * math.max(18, math.min(120, math.floor((tonumber(total_degrees) or 720) / 12)))
    local ok, msg = spin_once(active_spin_step)
    return ok, ok and "giro iniciado" or msg
end

local function start_wind(name, seconds)
    local duration = math.max(2, math.min(20, tonumber(seconds) or 8))
    active_wind_until = now_seconds() + duration
    if name == "wind_left" then
        active_wind_x, active_wind_y = 0, -650
    elseif name == "wind_right" then
        active_wind_x, active_wind_y = 0, 650
    elseif name == "wind_back" then
        active_wind_x, active_wind_y = -850, 0
    else
        active_wind_x, active_wind_y = 900, 0
    end
    return true, "vento iniciado por " .. tostring(duration) .. "s"
end

local function start_shake(seconds, strength)
    active_shake_until = now_seconds() + math.max(2, math.min(15, tonumber(seconds) or 6))
    active_shake_strength = math.max(180, math.min(950, tonumber(strength) or 420))
    return true, "tremedeira iniciada"
end

local function set_player_scale(scale, seconds)
    local pawn = get_player_pawn()
    if not pawn then return false, "player pawn não encontrado" end
    scale = math.max(0.35, math.min(2.5, tonumber(scale) or 1))
    if not active_original_scale then
        local ok, current = pcall(function()
            if pawn.GetActorScale3D then return pawn:GetActorScale3D() end
            return { X = 1, Y = 1, Z = 1 }
        end)
        if ok and current then active_original_scale = current else active_original_scale = { X = 1, Y = 1, Z = 1 } end
    end
    local ok, err = pcall(function()
        if pawn.SetActorScale3D then pawn:SetActorScale3D(vec(scale, scale, scale)) else error("SetActorScale3D indisponível") end
    end)
    if ok then
        active_reset_at = now_seconds() + math.max(3, math.min(25, tonumber(seconds) or 12))
        return true, "escala aplicada: " .. tostring(scale)
    end
    return false, tostring(err)
end

local function start_reverse_controls(seconds)
    active_reverse_until = now_seconds() + math.max(3, math.min(20, tonumber(seconds) or 10))
    return true, "controle reverso simulado iniciado"
end

local function start_slippery_floor(seconds)
    active_slippery_until = now_seconds() + math.max(3, math.min(20, tonumber(seconds) or 10))
    return true, "piso escorregadio iniciado"
end

local function start_no_jump(seconds)
    active_no_jump_until = now_seconds() + math.max(3, math.min(20, tonumber(seconds) or 10))
    return true, "anti-pulo iniciado"
end

local function start_input_delay(seconds)
    active_input_delay_until = now_seconds() + math.max(3, math.min(20, tonumber(seconds) or 8))
    return set_slomo(0.38, seconds or 8)
end

-- ragdoll removido do Only Climb.
-- fake_victory removido do Only Climb.

local function process_command(raw_command)
    local command = trim(raw_command)
    if command == "" then return end
    local lower = string.lower(command)
    lower = lower:gsub("^oc:", "")
    lower = lower:gsub("^onlyclimb:", "")
    lower = trim(lower)

    log("Comando LivePlay recebido: " .. command)

    local name, value = lower:match("^(%S+)%s*(.*)$")
    value = trim(value)
    local number_value = tonumber(value)

    local min_interval = COMMAND_MIN_INTERVAL[name]
    if min_interval then
        local current = now_seconds()
        local last = last_command_second[name] or 0
        if current - last < min_interval then
            append_ack(command, true, "ignorado por anti-spam: " .. tostring(name))
            log("OK: ignorado por anti-spam: " .. tostring(name))
            return
        end
        last_command_second[name] = current
    end

    local ok = false
    local msg = "comando não tratado"

    if name == "ping" then
        ok, msg = true, "pong"
    elseif name == "reset_effects" or name == "normal_gravity" or name == "reset_slomo" then
        ok, msg = reset_effects()
    elseif name == "jump_boost" then
        ok, msg = launch_player(0, 0, number_value and (number_value * 150) or 1200)
        if not ok then ok, msg = execute_console("jump") end
    elseif name == "launch_down" then
        ok, msg = launch_player(0, 0, number_value and (number_value * -160) or -1200)
    elseif name == "push_forward" then
        ok, msg = push_relative(number_value and (number_value * 120) or 1050, 0, 300)
    elseif name == "launch_forward" then
        ok, msg = push_relative(number_value and (number_value * 170) or 1500, 0, 650)
    elseif name == "launch_back" then
        ok, msg = push_relative(number_value and (number_value * -170) or -1400, 0, 550)
    elseif name == "push_back" then
        ok, msg = push_relative(number_value and (number_value * -120) or -900, 0, 260)
    elseif name == "push_left" then
        ok, msg = push_relative(0, number_value and (number_value * -120) or -850, 260)
    elseif name == "push_right" then
        ok, msg = push_relative(0, number_value and (number_value * 120) or 850, 260)
    elseif name == "teleport_forward" then
        ok, msg = teleport_relative(number_value and (number_value * 100) or 700, 0, 80)
    elseif name == "teleport_up" then
        ok, msg = teleport_vertical(number_value and (number_value * 100) or 600)
    elseif name == "teleport_down" then
        ok, msg = teleport_vertical(number_value and (number_value * -100) or -500)
    elseif name == "random_teleport" then
        ok, msg = random_teleport(number_value and (number_value * 120) or 900)
    elseif name == "teleport_back" then
        ok, msg = teleport_relative(number_value and (number_value * -100) or -600, 0, 80)
    elseif name == "low_gravity" or name == "zero_gravity" then
        ok, msg = set_slomo(0.65, number_value or 12)
    elseif name == "moon_gravity" then
        ok, msg = set_slomo(0.45, number_value or 15)
    elseif name == "freeze_player" then
        ok, msg = set_slomo(0.18, number_value or 4)
    elseif name == "unfreeze_player" then
        ok, msg = reset_effects()
    elseif name == "speed_up" or name == "speed_up_1_50x" then
        ok, msg = set_slomo(1.5, number_value or 10)
    elseif name == "turbo" then
        ok, msg = set_slomo(2.0, number_value or 6)
    elseif name == "slow_down" or name == "slow_down_0_60x" then
        ok, msg = set_slomo(0.6, number_value or 10)
    elseif name == "slomo" then
        ok, msg = set_slomo(number_value or 1, nil)
    elseif name == "checkpoint" or name == "save_checkpoint" or name == "teleport_checkpoint" or name == "back_checkpoint" then
        ok, msg = false, "checkpoint/backup removido nesta versão"
    elseif name == "spin_player" then
        ok, msg = start_spin(number_value or 720, 4, "normal")
    elseif name == "drunk_camera" then
        ok, msg = start_spin(number_value or 540, 7, "reverse")
    elseif name == "camera_shake" then
        ok, msg = start_shake(number_value or 6, 320)
    elseif name == "earthquake" then
        ok, msg = start_shake(number_value or 10, 780)
    elseif name == "bounce" or name == "bouncy_player" then
        ok, msg = start_bounce(number_value or 8, 900)
    elseif name == "wind_forward" or name == "wind_left" or name == "wind_right" or name == "wind_back" then
        ok, msg = start_wind(name, number_value or 8)
    elseif name == "ragdoll" then
        ok, msg = false, "ragdoll removido do Only Climb"
    elseif name == "reverse_controls" then
        ok, msg = start_reverse_controls(number_value or 10)
    elseif name == "slippery_floor" then
        ok, msg = start_slippery_floor(number_value or 10)
    elseif name == "no_jump" then
        ok, msg = start_no_jump(number_value or 10)
    elseif name == "input_delay" then
        ok, msg = start_input_delay(number_value or 8)
    elseif name == "blackout" then
        ok, msg = false, "blackout removido do template"
    elseif name == "mini_player" then
        ok, msg = set_player_scale(0.55, number_value or 12)
    elseif name == "giant_player" then
        ok, msg = set_player_scale(1.65, number_value or 12)
    elseif name == "fake_victory" then
        ok, msg = false, "fake_victory removido do template"
    elseif name == "chat" then
        ok, msg = false, "chat por comando removido; será integrado pelo sistema de chat do app"
    else
        ok, msg = execute_console(lower)
    end

    append_ack(command, ok, msg)
    log((ok and "OK: " or "FALHA: ") .. tostring(msg))
end

local function drain_queue_file(queue_file)
    local file = io.open(queue_file, "r")
    if not file then return false end
    local content = file:read("*a") or ""
    file:close()
    if content == "" then return true end

    write_text(queue_file, "")
    for line in content:gmatch("[^\r\n]+") do
        process_command(line)
    end
    return true
end

log("LivePlay UE4SS file bridge carregado. Versão: " .. VERSION)
log("Fila de comandos: Mods/LivePlayOnlyClimbHttpMod/liveplay-commands.queue")
heartbeat(true)

LoopAsync(100, function()
    heartbeat(false)

    if active_reset_at > 0 and now_seconds() >= active_reset_at then
        reset_effects()
    end

    if active_spin_until > 0 and now_seconds() <= active_spin_until then
        spin_once(active_spin_step)
    elseif active_spin_until > 0 then
        active_spin_until = 0
    end

    if active_wind_until > 0 and now_seconds() <= active_wind_until then
        launch_player(active_wind_x, active_wind_y, 180)
    elseif active_wind_until > 0 then
        active_wind_until = 0
    end

    if active_shake_until > 0 and now_seconds() <= active_shake_until then
        local dir = (now_seconds() % 2 == 0) and 1 or -1
        push_relative(0, dir * active_shake_strength, math.floor(active_shake_strength / 3))
    elseif active_shake_until > 0 then
        active_shake_until = 0
    end

    if active_bounce_until > 0 and now_seconds() <= active_bounce_until then
        launch_player(0, 0, active_bounce_strength)
    elseif active_bounce_until > 0 then
        active_bounce_until = 0
    end

    if active_reverse_until > 0 and now_seconds() <= active_reverse_until then
        push_relative(-520, 0, 120)
    elseif active_reverse_until > 0 then
        active_reverse_until = 0
    end

    if active_slippery_until > 0 and now_seconds() <= active_slippery_until then
        local side = (math.random(0, 1) == 0) and -420 or 420
        push_relative(180, side, 90)
    elseif active_slippery_until > 0 then
        active_slippery_until = 0
    end


    if active_no_jump_until > 0 and now_seconds() <= active_no_jump_until then
        pcall(function()
            local pawn = get_player_pawn()
            if pawn and pawn.CharacterMovement and pawn.CharacterMovement:IsValid() then
                pawn.CharacterMovement.Velocity = vec(0, 0, -360)
            else
                launch_player(0, 0, -360)
            end
        end)
    elseif active_no_jump_until > 0 then
        active_no_jump_until = 0
    end

    if active_input_delay_until > 0 and now_seconds() > active_input_delay_until then
        active_input_delay_until = 0
        execute_console("slomo 1")
    end

    -- Blackout e Ragdoll foram removidos do Only Climb.

    for _, queue_file in ipairs(QUEUE_FILES) do
        if drain_queue_file(queue_file) then break end
    end

    return false
end)
