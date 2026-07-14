# Brazilian Portuguese translation for R.E.P.O.

[Português](README.md) · **English** · [Español](README.es.md)

[![Version 1.0.0](https://img.shields.io/badge/version-1.0.0-7a5cff)](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases/latest)
[![Windows](https://img.shields.io/badge/Windows-Steam-1673b6?logo=steam)](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases/latest)
[![MIT License](https://img.shields.io/badge/license-MIT-e59a36)](LICENSE)

A contextual Brazilian Portuguese translation for R.E.P.O., with an automatic installer and backups of the original files.

> This project installs a Portuguese translation. The English page is provided to explain installation and recovery.

## Download

[**Download the translation installer**](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases/latest/download/REPOTraducaoPTBRInstaller.exe)

- File: `REPOTraducaoPTBRInstaller.exe`
- Current version: 1.0.0
- SHA-256: `BF8F6BB205167D2F68B7469740C387A26C441A3E94A6E6DDFD9C8B7321BE39A4`

## Installation

1. Close R.E.P.O.
2. Download the installer using the button above.
3. Run `REPOTraducaoPTBRInstaller.exe`.
4. The installer will try to locate the game through Steam and display the detected folder.
5. If it asks for a path, select the folder containing `REPO.exe`.
6. After the completion message, launch the game through Steam.

Common path:

```text
C:\Program Files (x86)\Steam\steamapps\common\REPO
```

## Backup and removal

Before changing any files, the installer creates a backup at:

```text
<game folder>\REPO_PTBR_Backups\backup_YYYY_MM_DD_HH_MM_SS
```

To remove the translation, close the game and copy the contents of the newest backup back into the R.E.P.O. folder, allowing Windows to replace the modified files.

## After a game update

A R.E.P.O. update may restore the original text or change localization tables. If that happens, run the installer again. Check the [Releases page](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases) first if any text is missing or appears in the wrong context.

## Windows warning

SmartScreen may display a warning because the installer does not have a commercial digital signature and has a limited download history. Confirm that the file came from this repository and compare its SHA-256 value before running it.

## How it works

The installer adds BepInEx and a contextual plugin, then updates the `Game.tsv`, `HUD.tsv`, and `Menu.tsv` tables. The locale intentionally remains set to `en-US`: the translation replaces the base table to avoid the `(pt-BR)` prefix displayed by the game.

<details>
<summary>Manual installation</summary>

1. Close R.E.P.O.
2. Back up `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `BepInEx`, and the tables in `REPO_Data\StreamingAssets\Localizations`.
3. Copy everything from this repository's `payload` folder into the game folder.
4. Open `%USERPROFILE%\AppData\LocalLow\semiwork\Repo`.
5. Create or edit `CurrentLocale.es3` with:

```json
{
  "Locale": {
    "__type": "string",
    "value": "en-US"
  }
}
```

6. Launch the game through Steam.

</details>

<details>
<summary>Build the installer</summary>

Run on Windows:

```powershell
.\build.ps1
```

The executable is generated at `dist\REPOTraducaoPTBRInstaller.exe` and copied to the repository root.

</details>

## Credits

Contextual translation, packaging, and installer by **Wellington Dias**. BepInEx and its dependencies belong to their respective authors.

Distributed under the [MIT License](LICENSE).
