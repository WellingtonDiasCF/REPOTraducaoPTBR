# R.E.P.O. PT-BR Contextual Installer

Instalador Windows para a tradução contextual PT-BR de R.E.P.O.

O instalador:

- detecta a pasta do R.E.P.O. na Steam;
- aborta se o jogo estiver aberto;
- cria backup antes de sobrescrever arquivos;
- instala BepInExPack e o plugin `RepoPTBRContextual`;
- instala os arquivos `Game.tsv`, `HUD.tsv` e `Menu.tsv`;
- salva o locale como `en-US` para evitar o fallback visual `(pt-BR) texto em inglês`, usando a tabela base sobrescrita em português.

## Como Usar

Baixe e execute:

```text
REPO-PTBR-Contextual-Installer.exe
```

Se o instalador não encontrar a pasta automaticamente, cole o caminho da pasta que contém `REPO.exe`.

## Backup

Antes de instalar, o programa cria um backup em:

```text
<pasta do jogo>\REPO_PTBR_Backups\backup_YYYY_MM_DD_HH_MM_SS
```

Para desfazer, copie os arquivos desse backup de volta para a pasta do jogo.

## Arquivos Instalados

```text
winhttp.dll
doorstop_config.ini
.doorstop_version
BepInEx\core\...
BepInEx\config\BepInEx.cfg
BepInEx\plugins\RepoPTBRContextual\RepoPTBRContextual.dll
BepInEx\plugins\RepoPTBRContextual\runtime.tsv
REPO_Data\StreamingAssets\Localizations\Game.tsv
REPO_Data\StreamingAssets\Localizations\HUD.tsv
REPO_Data\StreamingAssets\Localizations\Menu.tsv
```

Também grava:

```text
%USERPROFILE%\AppData\LocalLow\semiwork\Repo\CurrentLocale.es3
```

com `en-US`, por causa do comportamento de fallback do jogo.

## Build

No PowerShell:

```powershell
.\build.ps1
```

O executável sai em:

```text
dist\REPO-PTBR-Contextual-Installer.exe
```

## Créditos

Este projeto empacota BepInExPack para carregar o plugin no R.E.P.O. O BepInEx e suas dependências pertencem aos seus respectivos autores.
