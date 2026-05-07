@echo off
setlocal
set SCRIPT_DIR=%~dp0
set GTA_DIR=%SCRIPT_DIR%..\..
set SCRIPTS_DIR=%GTA_DIR%\scripts
set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
"%CSC%" /nologo /target:library /optimize+ /out:"%SCRIPTS_DIR%\LivePlayGtaBridge.dll" /reference:"%SCRIPTS_DIR%\ScriptHookVDotNet3.dll" "%SCRIPTS_DIR%\LivePlayGtaBridge.cs"
pause
