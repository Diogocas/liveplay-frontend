@echo off
setlocal
set SCRIPT_DIR=%~dp0
set GTA_DIR=%SCRIPT_DIR%..\..
set SCRIPTS_DIR=%GTA_DIR%\scripts
set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
set FRAMEWORK_DIR=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319
if not exist "%CSC%" (
  set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
  set FRAMEWORK_DIR=%WINDIR%\Microsoft.NET\Framework\v4.0.30319
)
if not exist "%CSC%" (
  echo Nao encontrei o compilador C# do .NET Framework 4.x.
  pause
  exit /b 1
)
if not exist "%GTA_DIR%\ScriptHookVDotNet3.dll" (
  echo Nao encontrei ScriptHookVDotNet3.dll na raiz do GTA.
  pause
  exit /b 1
)
"%CSC%" /nologo /target:library /optimize+ /out:"%SCRIPTS_DIR%\LivePlayGtaBridge.dll" /reference:"%FRAMEWORK_DIR%\System.dll" /reference:"%GTA_DIR%\ScriptHookVDotNet3.dll" "%SCRIPTS_DIR%\LivePlayGtaBridge.3.cs"
pause
