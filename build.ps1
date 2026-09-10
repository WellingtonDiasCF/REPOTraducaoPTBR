$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$src = Join-Path $root 'src\Installer.cs'
$payload = Join-Path $root 'src\payload.zip'
$payloadRoot = Join-Path $root 'payload'
$pluginProject = Join-Path $root 'src\RepoPTBRContextual\RepoPTBRContextual.csproj'
$pluginOutput = Join-Path $root 'src\RepoPTBRContextual\bin\Release\netstandard2.1\RepoPTBRContextual.dll'
$pluginTargetDir = Join-Path $payloadRoot 'BepInEx\plugins\RepoPTBRContextual'
$dist = Join-Path $root 'dist'
$out = Join-Path $dist 'REPOTraducaoPTBRInstaller.exe'
$publicOut = Join-Path $root 'REPOTraducaoPTBRInstaller.exe'
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'

if (-not (Test-Path -LiteralPath $csc)) {
    throw "csc.exe not found: $csc"
}

if (-not (Test-Path -LiteralPath $pluginProject)) {
    throw "Plugin project not found: $pluginProject"
}

New-Item -ItemType Directory -Path $dist -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $pluginTargetDir 'Localizations') -Force | Out-Null

& dotnet build $pluginProject -c Release
if ($LASTEXITCODE -ne 0) {
    throw "Plugin build failed with exit code $LASTEXITCODE"
}

Copy-Item -LiteralPath $pluginOutput -Destination (Join-Path $pluginTargetDir 'RepoPTBRContextual.dll') -Force
foreach ($table in @('Game.tsv', 'HUD.tsv', 'Menu.tsv')) {
    Copy-Item -LiteralPath (Join-Path $root "localizations\$table") -Destination (Join-Path $pluginTargetDir "Localizations\$table") -Force
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
if (Test-Path -LiteralPath $payload) {
    Remove-Item -LiteralPath $payload -Force
}
[System.IO.Compression.ZipFile]::CreateFromDirectory(
    $payloadRoot,
    $payload,
    [System.IO.Compression.CompressionLevel]::Optimal,
    $false
)

& $csc `
    /nologo `
    /target:exe `
    /optimize+ `
    "/out:$out" `
    "/resource:$payload,RepoPtbrContextualInstaller.payload.zip" `
    /reference:System.IO.Compression.dll `
    /reference:System.IO.Compression.FileSystem.dll `
    $src

Copy-Item -LiteralPath $out -Destination $publicOut -Force

Write-Host "Built $out"
Write-Host "Copied $publicOut"
