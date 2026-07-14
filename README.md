# Tradução PT-BR para R.E.P.O.

**Português** · [English](README.en.md) · [Español](README.es.md)

[![Versão 1.0.0](https://img.shields.io/badge/versão-1.0.0-7a5cff)](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases/latest)
[![Windows](https://img.shields.io/badge/Windows-Steam-1673b6?logo=steam)](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases/latest)
[![Licença MIT](https://img.shields.io/badge/licença-MIT-e59a36)](LICENSE)

Tradução contextual do R.E.P.O. para português do Brasil, com instalador automático e backup dos arquivos originais.

## Download

[**Baixar o instalador da tradução**](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases/latest/download/REPOTraducaoPTBRInstaller.exe)

- Arquivo: `REPOTraducaoPTBRInstaller.exe`
- Versão atual: 1.0.0
- SHA-256: `BF8F6BB205167D2F68B7469740C387A26C441A3E94A6E6DDFD9C8B7321BE39A4`

## Como instalar

1. Feche o R.E.P.O.
2. Baixe o instalador pelo botão acima.
3. Execute `REPOTraducaoPTBRInstaller.exe`.
4. O instalador tentará localizar o jogo pela Steam e mostrará a pasta encontrada.
5. Se ele pedir um caminho, informe a pasta que contém `REPO.exe`.
6. Depois da mensagem de conclusão, abra o jogo pela Steam.

Caminho comum:

```text
C:\Program Files (x86)\Steam\steamapps\common\REPO
```

## Backup e remoção

Antes de alterar qualquer arquivo, o instalador cria um backup em:

```text
<pasta do jogo>\REPO_PTBR_Backups\backup_YYYY_MM_DD_HH_MM_SS
```

Para remover a tradução, feche o jogo e copie o conteúdo do backup mais recente de volta para a pasta do R.E.P.O., confirmando a substituição.

## Depois de uma atualização do jogo

Uma atualização do R.E.P.O. pode restaurar os textos originais ou mudar as tabelas de localização. Se isso acontecer, execute novamente o instalador. Caso a tradução apresente textos faltando ou fora de contexto, consulte a [página de versões](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases) antes de reinstalar.

## Aviso do Windows

O SmartScreen pode alertar porque o instalador não possui assinatura digital comercial e ainda tem poucos downloads. Confirme que o arquivo veio deste repositório e compare o SHA-256 acima antes de executá-lo.

## Como a tradução funciona

O instalador adiciona BepInEx e um plugin contextual, além de atualizar as tabelas `Game.tsv`, `HUD.tsv` e `Menu.tsv`. O locale permanece como `en-US` de propósito: a tradução substitui a tabela base para evitar o prefixo visual `(pt-BR)` mostrado pelo jogo.

<details>
<summary>Instalação manual</summary>

1. Feche o R.E.P.O.
2. Faça backup de `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `BepInEx` e das tabelas em `REPO_Data\StreamingAssets\Localizations`.
3. Copie todo o conteúdo da pasta `payload` deste repositório para a pasta do jogo.
4. Abra `%USERPROFILE%\AppData\LocalLow\semiwork\Repo`.
5. Crie ou edite `CurrentLocale.es3` com:

```json
{
  "Locale": {
    "__type": "string",
    "value": "en-US"
  }
}
```

6. Abra o jogo pela Steam.

</details>

<details>
<summary>Compilar o instalador</summary>

No Windows, execute:

```powershell
.\build.ps1
```

O executável será criado em `dist\REPOTraducaoPTBRInstaller.exe` e copiado para a raiz do projeto.

</details>

## Créditos

Tradução contextual, organização e instalador por **Wellington Dias**. BepInEx e suas dependências pertencem aos respectivos autores.

Distribuído sob a [licença MIT](LICENSE).
