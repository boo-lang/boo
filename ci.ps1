$ErrorActionPreference = "Stop"

$SCRIPT_PATH=$MyInvocation.MyCommand.Path
$SCRIPT_DIR=Split-Path $SCRIPT_PATH

& ${SCRIPT_DIR}\nant.ps1 compile-tests
& ${SCRIPT_DIR}\nunit.ps1
