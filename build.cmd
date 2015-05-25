@echo off

net session >nul 2>&1
if %errorLevel% == 0 (

cd %~dp0

SETLOCAL
SET CACHED_DNVM=%USERPROFILE%\.dnx\bin\dnvm.cmd

IF EXIST %CACHED_DNVM% goto installdnx
echo Downloading latest version of DNVM...
IF NOT EXIST %USERPROFILE%\.dnx md %USERPROFILE%\.dnx
IF NOT EXIST %USERPROFILE%\.dnx\bin md %USERPROFILE%\.dnx\bin
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}"

:installdnx
%USERPROFILE%\.dnx\bin\dnvm install 1.0.0-beta4 -arch x86 -persistent -a beta4
SET CACHED_DNU=%USERPROFILE%\.dnx\runtimes\dnx-clr-win-x86.1.0.0-beta4\bin
echo Setting DNU Path to %CACHED_DNU%

:restore
%CACHED_DNU%\dnu restore

:clean
IF EXIST src\Compose\bin\Release DEL src\Compose\bin\Release /Q

:build
%CACHED_DNU%\dnu pack src\Compose --configuration Release

:artifacts
IF NOT EXIST artifacts md artifacts
IF NOT EXIST artifacts\packages md artifacts\packages
XCOPY src\Compose\bin\Release\*.nupkg artifacts\packages /y

) else (
echo Unable to complete. Insufficient privileges.
EXIT /B 1
)