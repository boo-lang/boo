@echo off
rem Compiles Boo source and prints the IL that came out of it.
rem Set BOO_CONFIGURATION=Release to use that build of booc.
setlocal
if "%BOO_CONFIGURATION%"=="" (set _cfg=Debug) else (set _cfg=%BOO_CONFIGURATION%)
set _booc=%~dp0src\booc\bin\%_cfg%\net10.0\booc.exe

if "%~1"=="" (
	echo usage: il [booc options] ^<source.boo^> 1>&2
	exit /b 2
)
if not exist "%_booc%" (
	echo il: booc is not built. Run: dotnet build Boo.slnx 1>&2
	exit /b 1
)

set _work=%TEMP%\boo-il-%RANDOM%
mkdir "%_work%"
"%_booc%" -target:library -out:"%_work%\il.dll" %* 1>&2
if errorlevel 1 (
	rmdir /s /q "%_work%"
	exit /b 1
)

pushd "%~dp0"
dotnet tool restore >nul 2>&1
if errorlevel 1 (
	echo il: could not restore ilspycmd from .config/dotnet-tools.json 1>&2
	popd
	rmdir /s /q "%_work%"
	exit /b 1
)
dotnet ilspycmd -il "%_work%\il.dll"
set _rc=%errorlevel%
popd
rmdir /s /q "%_work%"
exit /b %_rc%
