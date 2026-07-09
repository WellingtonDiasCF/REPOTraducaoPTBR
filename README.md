# R.E.P.O tradução PT-BR

Tradução contextual em português do Brasil para **R.E.P.O**, criada e organizada por **Wellington Dias**.

## Download Fácil

Baixe o instalador pronto aqui:

**[REPO-Traducao-PT-BR-Installer.exe](https://github.com/WellingtonDiasCF/REPO-traducao-PT-BR/releases/latest/download/REPO-Traducao-PT-BR-Installer.exe)**

Também há uma cópia do `.exe` direto na raiz deste repositório:

**[REPO-Traducao-PT-BR-Installer.exe](./REPO-Traducao-PT-BR-Installer.exe)**

## Instalação pelo EXE

1. Feche o R.E.P.O. se ele estiver aberto.
2. Baixe `REPO-Traducao-PT-BR-Installer.exe`.
3. Execute o instalador.
4. Se o Windows mostrar SmartScreen, clique em **Mais informações** e depois em **Executar assim mesmo**.
5. O instalador tenta encontrar a pasta do jogo automaticamente pela Steam.
6. Se ele pedir o caminho, cole a pasta que contém `REPO.exe`, por exemplo:

```text
C:\Program Files (x86)\Steam\steamapps\common\REPO
```

O instalador faz backup antes de sobrescrever qualquer arquivo.

## Instalação Manual

Use este método se não quiser usar o `.exe`.

1. Feche o R.E.P.O.
2. Abra a pasta do jogo na Steam:

```text
C:\Program Files (x86)\Steam\steamapps\common\REPO
```

3. Faça backup manual dos arquivos/pastas abaixo, se já existirem:

```text
winhttp.dll
doorstop_config.ini
.doorstop_version
BepInEx\
REPO_Data\StreamingAssets\Localizations\Game.tsv
REPO_Data\StreamingAssets\Localizations\HUD.tsv
REPO_Data\StreamingAssets\Localizations\Menu.tsv
```

4. Copie tudo da pasta `payload` deste projeto para dentro da pasta do jogo.
5. Confirme a substituição dos arquivos.
6. Abra a pasta de save/configuração do jogo:

```text
%USERPROFILE%\AppData\LocalLow\semiwork\Repo
```

7. Crie ou edite o arquivo `CurrentLocale.es3` com este conteúdo:

```json
{
	"Locale" : {
		"__type" : "string",
		"value" : "en-US"
	}
}
```

8. Abra o jogo pela Steam.

## O Que Este Projeto Instala

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

O locale fica como `en-US` de propósito. O jogo usa fallback visual estranho quando seleciona `pt-BR`, então a tradução sobrescreve a tabela base para evitar textos com prefixo como `(pt-BR)`.

## Backup Automático

Antes de instalar, o `.exe` cria um backup em:

```text
<pasta do jogo>\REPO_PTBR_Backups\backup_YYYY_MM_DD_HH_MM_SS
```

Para desfazer, copie os arquivos desse backup de volta para a pasta do jogo.

## Build

Para compilar o instalador:

```powershell
.\build.ps1
```

O executável sai em dois lugares:

```text
dist\REPO-PTBR-Contextual-Installer.exe
REPO-Traducao-PT-BR-Installer.exe
```

## Créditos

Projeto, organização, empacotamento e tradução contextual por **Wellington Dias**.

Este projeto empacota BepInEx para carregar o plugin no R.E.P.O. O BepInEx e suas dependências pertencem aos seus respectivos autores.
