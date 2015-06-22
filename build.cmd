@echo off

net session >nul 2>&1
if %errorLevel% == 0 (

cd %~dp0

SET CACHED_DNVM=%USERPROFILE%\.dnx\bin\dnvm.cmd

echo Downloading latest version of DNVM...
IF NOT EXIST %USERPROFILE%\.dnx md %USERPROFILE%\.dnx
IF NOT EXIST %USERPROFILE%\.dnx\bin md %USERPROFILE%\.dnx\bin
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}"

%USERPROFILE%\.dnx\bin\dnvm install 1.0.0-beta6-12100 -arch x86 -r clr -unstable
dnu restore --quiet
IF EXIST src\Compose\bin\Release DEL src\Compose\bin\Release /Q
dnu pack src\Compose --configuration Release --quiet
dnx tests\Compose.Tests\ test

IF NOT EXIST artifacts md artifacts
IF NOT EXIST artifacts\packages md artifacts\packages
XCOPY src\Compose\bin\Release\*.nupkg artifacts\packages /y

) else (
echo Unable to complete. Insufficient privileges.
EXIT /B 1
)