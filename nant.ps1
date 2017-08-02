$ErrorActionPreference = "Stop"

$SCRIPT_PATH=$MyInvocation.MyCommand.Path
$SCRIPT_DIR=Split-Path $SCRIPT_PATH

. ${SCRIPT_DIR}\build-tools\.config-build.ps1

$NANT_PATH = "${TOOLS_DIR}\nant\NAnt.exe"
Verify-Bootstrap $NANT_PATH

$ADDITIONAL_ARGS = $args

& $NANT_PATH -t:net-4.5 -D:skip.antlr=true $ADDITIONAL_ARGS
