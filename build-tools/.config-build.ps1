$ErrorActionPreference = "Stop"

$SCRIPT_PATH=$MyInvocation.MyCommand.Path
$SCRIPT_DIR=Split-Path $SCRIPT_PATH

Set-Variable -Name TOOLS_DIR -Value ${SCRIPT_DIR}\tools

Get-Content $SCRIPT_DIR\versions | ForEach-Object {
    $var,$val=$_.Split("{=}")
    Set-Variable -Name $var -Value $val
}

function Exec-At {
    param($location, $block)

    Push-Location $location
    Try {
        Invoke-Command $block
    } Finally {
        Pop-Location
    }
}

function Verify-Bootstrap {
    param($file)

    if(-Not (Test-Path $file)) {
        Throw "Run 'powershell .\build-tools\bootstrap' from a Developer Command Prompt prior to building"
    }
}