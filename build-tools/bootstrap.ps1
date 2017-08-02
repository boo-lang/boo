$ErrorActionPreference = "Stop"

$SCRIPT_PATH=$MyInvocation.MyCommand.Path
$SCRIPT_DIR=Split-Path $SCRIPT_PATH

New-Item -Force -ItemType Directory ${SCRIPT_DIR}\tools > $null

. ${SCRIPT_DIR}\.config-build.ps1

function Download-If-Missing {
    param($url, $file_path)

    if(-Not (Test-Path $file_path)) {
        echo "Downloading $url to $file_path"
        Invoke-WebRequest -Uri $url -OutFile $file_path
    }
}

Exec-At $TOOLS_DIR {

    # Fetch a version of NAnt compatible with Mono 3

    $NANT_DOWNLOAD_FILE_NAME="$NANT_COMMIT.zip"
    $NANT_DOWNLOAD_FILE_PATH="$NANT_DOWNLOAD_FILE_NAME"
    $NANT_URL="https://github.com/nant/nant/archive/${NANT_DOWNLOAD_FILE_NAME}"

    Download-If-Missing "$NANT_URL" "$NANT_DOWNLOAD_FILE_PATH"
    echo "Extracting NAnt from ${NANT_DOWNLOAD_FILE_PATH}"
    Expand-Archive -Force -Path $NANT_DOWNLOAD_FILE_PATH -DestinationPath .

    If(Test-Path nant) { Remove-Item -Recurse nant }
    Exec-At "nant-${NANT_COMMIT}" {
        echo "Building NAnt"
        & nmake /f Makefile.nmake setup bootstrap/NAnt.exe bootstrap/NAnt.Core.dll bootstrap/NAnt.DotNetTasks.dll bootstrap/NAnt.Win32Tasks.dll
        Copy-Item -Recurse -Force bootstrap ../nant
    }
    Remove-Item -Recurse "nant-${NANT_COMMIT}"

    # Fetch a recent version of NUnit

    $NUNIT_DOWNLOAD_FILE_NAME = "NUnit-$NUNIT_VERSION.zip"
    $NUNIT_DOWNLOAD_FILE_PATH = "$NUNIT_DOWNLOAD_FILE_NAME"
    $NUNIT_URL = "https://launchpad.net/nunitv2/trunk/$NUNIT_VERSION/+download/$NUNIT_DOWNLOAD_FILE_NAME"

    Download-If-Missing "${NUNIT_URL}" "${NUNIT_DOWNLOAD_FILE_PATH}"
    echo "Extracting NUnit from ${NUNIT_DOWNLOAD_FILE_PATH}"
    Expand-Archive -Force -Path $NUNIT_DOWNLOAD_FILE_PATH -DestinationPath .
}