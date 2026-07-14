# Traducción al portugués de Brasil para R.E.P.O.

[Português](README.md) · [English](README.en.md) · **Español**

[![Versión 1.0.0](https://img.shields.io/badge/versión-1.0.0-7a5cff)](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases/latest)
[![Windows](https://img.shields.io/badge/Windows-Steam-1673b6?logo=steam)](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases/latest)
[![Licencia MIT](https://img.shields.io/badge/licencia-MIT-e59a36)](LICENSE)

Traducción contextual de R.E.P.O. al portugués de Brasil, con instalador automático y copia de seguridad de los archivos originales.

> Este proyecto instala una traducción en portugués. La página en español explica cómo instalarla y restaurar el juego.

## Descargar

[**Descargar el instalador de la traducción**](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases/latest/download/REPOTraducaoPTBRInstaller.exe)

- Archivo: `REPOTraducaoPTBRInstaller.exe`
- Versión actual: 1.0.0
- SHA-256: `BF8F6BB205167D2F68B7469740C387A26C441A3E94A6E6DDFD9C8B7321BE39A4`

## Instalación

1. Cierra R.E.P.O.
2. Descarga el instalador con el botón anterior.
3. Ejecuta `REPOTraducaoPTBRInstaller.exe`.
4. El instalador intentará localizar el juego mediante Steam y mostrará la carpeta detectada.
5. Si solicita una ruta, selecciona la carpeta que contiene `REPO.exe`.
6. Cuando aparezca el mensaje de finalización, abre el juego desde Steam.

Ruta habitual:

```text
C:\Program Files (x86)\Steam\steamapps\common\REPO
```

## Copia de seguridad y eliminación

Antes de modificar archivos, el instalador crea una copia en:

```text
<carpeta del juego>\REPO_PTBR_Backups\backup_YYYY_MM_DD_HH_MM_SS
```

Para quitar la traducción, cierra el juego y copia el contenido de la copia más reciente a la carpeta de R.E.P.O., permitiendo que Windows reemplace los archivos modificados.

## Después de una actualización

Una actualización de R.E.P.O. puede restaurar los textos originales o cambiar las tablas de localización. Si ocurre, vuelve a ejecutar el instalador. Consulta primero la [página de versiones](https://github.com/WellingtonDiasCF/REPOTraducaoPTBR/releases) si faltan textos o aparecen fuera de contexto.

## Aviso de Windows

SmartScreen puede mostrar un aviso porque el instalador no posee una firma digital comercial y todavía tiene pocas descargas. Confirma que el archivo procede de este repositorio y compara su valor SHA-256 antes de ejecutarlo.

## Cómo funciona

El instalador añade BepInEx y un complemento contextual, y después actualiza las tablas `Game.tsv`, `HUD.tsv` y `Menu.tsv`. El locale permanece intencionadamente como `en-US`: la traducción sustituye la tabla base para evitar el prefijo `(pt-BR)` mostrado por el juego.

<details>
<summary>Instalación manual</summary>

1. Cierra R.E.P.O.
2. Guarda una copia de `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `BepInEx` y las tablas de `REPO_Data\StreamingAssets\Localizations`.
3. Copia todo el contenido de la carpeta `payload` de este repositorio a la carpeta del juego.
4. Abre `%USERPROFILE%\AppData\LocalLow\semiwork\Repo`.
5. Crea o edita `CurrentLocale.es3` con:

```json
{
  "Locale": {
    "__type": "string",
    "value": "en-US"
  }
}
```

6. Abre el juego desde Steam.

</details>

<details>
<summary>Compilar el instalador</summary>

Ejecuta en Windows:

```powershell
.\build.ps1
```

El ejecutable se genera en `dist\REPOTraducaoPTBRInstaller.exe` y se copia a la raíz del repositorio.

</details>

## Créditos

Traducción contextual, organización e instalador por **Wellington Dias**. BepInEx y sus dependencias pertenecen a sus respectivos autores.

Distribuido bajo la [licencia MIT](LICENSE).
