@echo off
rem Runs the booc built from source. Set BOO_CONFIGURATION=Release for that build.
setlocal
if "%BOO_CONFIGURATION%"=="" (set _cfg=Debug) else (set _cfg=%BOO_CONFIGURATION%)
set _exe=%~dp0src\booc\bin\%_cfg%\net10.0\booc.exe
if not exist "%_exe%" (
	echo booc is not built. Run: dotnet build Boo.slnx 1>&2
	exit /b 1
)
"%_exe%" %*
