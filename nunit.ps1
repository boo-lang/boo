$ErrorActionPreference = "Stop"

$SCRIPT_PATH=$MyInvocation.MyCommand.Path
$SCRIPT_DIR=Split-Path $SCRIPT_PATH

. ${SCRIPT_DIR}\build-tools\.config-build.ps1

$ADDITIONAL_ARGS = $args

Exec-At tests {
    $NUNIT_CONSOLE_PATH = "${TOOLS_DIR}\NUnit-${NUNIT_VERSION}\bin\nunit-console.exe"
    Verify-Bootstrap $NUNIT_CONSOLE_PATH

    # Replace the nunit framework dll with the downloaded version
    Copy-Item -Force "${TOOLS_DIR}\NUnit-${NUNIT_VERSION}\bin\nunit.framework.dll" build\
    Exec-At build {
        $TEST_DLLS = Get-Item *.Tests.dll | Select -ExpandProperty Name
        & $NUNIT_CONSOLE_PATH $TEST_DLLS /framework=4.5 /stoponerror /nologo /timeout=10000 /noresult /output=stdout.txt $ADDITIONAL_ARGS
    }
}
