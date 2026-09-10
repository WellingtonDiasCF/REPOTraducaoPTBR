[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$packageSource = Join-Path $repositoryRoot "thunderstore"
$manifestPath = Join-Path $packageSource "manifest.json"
$iconPath = Join-Path $packageSource "icon.png"
$pluginPath = Join-Path $packageSource "BepInEx\plugins\RepoPTBRContextual\RepoPTBRContextual.dll"
$pluginSource = Join-Path $repositoryRoot "src\RepoPTBRContextual\RepoPTBRContextual.cs"

foreach ($requiredPath in @(
    $manifestPath,
    $iconPath,
    $pluginPath,
    $pluginSource,
    (Join-Path $packageSource "README.md"),
    (Join-Path $packageSource "BepInEx\plugins\RepoPTBRContextual\runtime.tsv"),
    (Join-Path $packageSource "BepInEx\plugins\RepoPTBRContextual\Localizations\Game.tsv"),
    (Join-Path $packageSource "BepInEx\plugins\RepoPTBRContextual\Localizations\HUD.tsv"),
    (Join-Path $packageSource "BepInEx\plugins\RepoPTBRContextual\Localizations\Menu.tsv")
)) {
    if (-not (Test-Path -LiteralPath $requiredPath)) {
        throw "Required package file not found: $requiredPath"
    }
}

$manifest = Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
if ($manifest.version_number -notmatch '^\d+\.\d+\.\d+$') {
    throw "manifest.json version_number must use Major.Minor.Patch."
}

$pluginSourceText = Get-Content -LiteralPath $pluginSource -Raw -Encoding UTF8
$versionPattern = '\[BepInPlugin\([^,]+,\s*[^,]+,\s*"([^\"]+)"\)\]'
$versionMatch = [regex]::Match($pluginSourceText, $versionPattern)
if (-not $versionMatch.Success -or $versionMatch.Groups[1].Value -ne $manifest.version_number) {
    throw "The BepInPlugin version must match manifest version $($manifest.version_number)."
}

Add-Type -AssemblyName System.Drawing
$icon = [System.Drawing.Image]::FromFile($iconPath)
try {
    if ($icon.Width -ne 256 -or $icon.Height -ne 256) {
        throw "icon.png must be exactly 256x256 pixels."
    }
}
finally {
    $icon.Dispose()
}

$outputDirectory = Join-Path $repositoryRoot "artifacts\thunderstore"
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$outputPath = Join-Path $outputDirectory "REPO_PTBR_Contextual-$($manifest.version_number).zip"
Compress-Archive -Path (Join-Path $packageSource "*") -DestinationPath $outputPath -CompressionLevel Optimal -Force

Write-Host "Thunderstore package created: $outputPath"
