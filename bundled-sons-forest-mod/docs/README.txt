LivePlay Sons Of The Forest Bridge

Este pacote contém:
- BepInEx 6 IL2CPP x64 correto para Sons Of The Forest.
- LivePlaySonsForestBridge.dll compilada.
- Configuração LivePlaySonsForestBridge.json usando a porta 35952.

Instalação pelo app:
1. Abra LivePlay.
2. Vá em Modelos > Sons Of The Forest.
3. Selecione a pasta raiz do jogo, onde fica SonsOfTheForest.exe.
4. Clique em Instalar mod.
5. Abra o jogo uma vez.
6. Verifique em BepInEx/LogOutput.log se aparece: LivePlay Sons Forest Bridge ativo na porta 35952.

Estrutura esperada no jogo:
Sons Of The Forest/
  winhttp.dll
  doorstop_config.ini
  .doorstop_version
  dotnet/
  BepInEx/
    core/
    plugins/
      LivePlaySonsForestBridge.dll
    config/
      LivePlaySonsForestBridge.json

Essa integração é isolada. Não usa GTA, Minecraft, RCON ou ScriptHook.
