$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$src = Join-Path $root 'src\Installer.cs'
$payload = Join-Path $root 'src\payload.zip'
$dist = Join-Path $root 'dist'
$out = Join-Path $dist 'REPO-PTBR-Contextual-Installer.exe'
$publicOut = Join-Path $root 'REPO-Traducao-PT-BR-Installer.exe'
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'

if (-not (Test-Path -LiteralPath $csc)) {
    throw "csc.exe not found: $csc"
}

if (-not (Test-Path -LiteralPath $payload)) {
    throw "payload.zip not found: $payload"
}

New-Item -ItemType Directory -Path $dist -Force | Out-Null

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
