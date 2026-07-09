using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace RepoPtbrContextualInstaller
{
    internal static class Installer
    {
        private const string AppId = "3241660";
        private const string GameExe = "REPO.exe";
        private static bool noPause;

        private static int Main(string[] args)
        {
            noPause = HasArg(args, "--no-pause") || HasArg(args, "/quiet") || Console.IsInputRedirected;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "R.E.P.O traducao PT-BR - Wellington Dias";
            Console.WriteLine("R.E.P.O traducao PT-BR");
            Console.WriteLine("Projeto por Wellington Dias");
            Console.WriteLine();

            try
            {
                if (IsRepoRunning())
                {
                    Fail("O R.E.P.O. está aberto. Feche o jogo antes de instalar.");
                    return 2;
                }

                string gameDir = ResolveGameDir(args);
                if (string.IsNullOrEmpty(gameDir))
                {
                    Fail("Não consegui encontrar a pasta do R.E.P.O.");
                    return 3;
                }

                Console.WriteLine("R.E.P.O. encontrado em:");
                Console.WriteLine(gameDir);
                Console.WriteLine();

                string backupDir = CreateBackup(gameDir);
                Console.WriteLine("Backup criado em:");
                Console.WriteLine(backupDir);
                Console.WriteLine();

                InstallPayload(gameDir);
                WriteLocalePreference();
                Console.WriteLine("R.E.P.O traducao PT-BR por Wellington Dias.");

                Console.WriteLine("Instalação concluída.");
                Console.WriteLine("Abra o jogo pela Steam. A tradução usa a tabela base sobrescrita em PT-BR para evitar o prefixo (pt-BR).");
                Console.WriteLine();
                Console.WriteLine("Se quiser desfazer, restaure os arquivos do backup acima.");
                PauseIfInteractive();
                return 0;
            }
            catch (Exception ex)
            {
                Fail(ex.ToString());
                return 1;
            }
        }

        private static void Fail(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            PauseIfInteractive();
        }

        private static void PauseIfInteractive()
        {
            if (!noPause && Environment.UserInteractive)
            {
                Console.WriteLine("Pressione Enter para sair...");
                Console.ReadLine();
            }
        }

        private static bool HasArg(string[] args, string expected)
        {
            if (args == null)
            {
                return false;
            }

            foreach (string arg in args)
            {
                if (string.Equals(arg, expected, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsRepoRunning()
        {
            return Process.GetProcessesByName("REPO").Length > 0;
        }

        private static string ResolveGameDir(string[] args)
        {
            if (args != null)
            {
                foreach (string arg in args)
                {
                    if (string.IsNullOrWhiteSpace(arg) || arg.StartsWith("-", StringComparison.Ordinal) || arg.StartsWith("/", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    string fromArg = NormalizeGameDir(arg);
                    if (IsGameDir(fromArg))
                    {
                        return fromArg;
                    }
                }
            }

            foreach (string candidate in GetCandidateGameDirs())
            {
                string normalized = NormalizeGameDir(candidate);
                if (IsGameDir(normalized))
                {
                    return normalized;
                }
            }

            Console.WriteLine("Digite ou cole o caminho da pasta do R.E.P.O. (a pasta que contém REPO.exe):");
            string typed = Console.ReadLine();
            string fromInput = NormalizeGameDir(typed);
            return IsGameDir(fromInput) ? fromInput : null;
        }

        private static IEnumerable<string> GetCandidateGameDirs()
        {
            yield return @"C:\Program Files (x86)\Steam\steamapps\common\REPO";
            yield return @"C:\Program Files\Steam\steamapps\common\REPO";

            foreach (string steamPath in GetSteamPaths())
            {
                if (string.IsNullOrEmpty(steamPath))
                {
                    continue;
                }

                string manifestDir = Path.Combine(steamPath, "steamapps");
                string fromManifest = TryResolveFromManifest(manifestDir);
                if (!string.IsNullOrEmpty(fromManifest))
                {
                    yield return fromManifest;
                }

                foreach (string library in GetSteamLibraries(steamPath))
                {
                    fromManifest = TryResolveFromManifest(Path.Combine(library, "steamapps"));
                    if (!string.IsNullOrEmpty(fromManifest))
                    {
                        yield return fromManifest;
                    }
                }
            }
        }

        private static IEnumerable<string> GetSteamPaths()
        {
            string[] keys =
            {
                @"HKEY_CURRENT_USER\Software\Valve\Steam",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam"
            };

            foreach (string key in keys)
            {
                object value = Registry.GetValue(key, "SteamPath", null) ?? Registry.GetValue(key, "InstallPath", null);
                if (value != null)
                {
                    yield return value.ToString().Replace('/', '\\');
                }
            }
        }

        private static IEnumerable<string> GetSteamLibraries(string steamPath)
        {
            string vdf = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(vdf))
            {
                yield break;
            }

            foreach (string line in File.ReadAllLines(vdf))
            {
                string trimmed = line.Trim();
                if (!trimmed.Contains("\"path\""))
                {
                    continue;
                }

                string[] parts = trimmed.Split('"');
                if (parts.Length >= 4)
                {
                    yield return parts[3].Replace(@"\\", @"\");
                }
            }
        }

        private static string TryResolveFromManifest(string steamAppsDir)
        {
            string manifest = Path.Combine(steamAppsDir, "appmanifest_" + AppId + ".acf");
            if (!File.Exists(manifest))
            {
                return null;
            }

            string installDir = "REPO";
            foreach (string line in File.ReadAllLines(manifest))
            {
                string trimmed = line.Trim();
                if (!trimmed.StartsWith("\"installdir\"", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string[] parts = trimmed.Split('"');
                if (parts.Length >= 4)
                {
                    installDir = parts[3];
                }
            }

            return Path.Combine(steamAppsDir, "common", installDir);
        }

        private static string NormalizeGameDir(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            path = path.Trim().Trim('"');
            if (File.Exists(path) && string.Equals(Path.GetFileName(path), GameExe, StringComparison.OrdinalIgnoreCase))
            {
                path = Path.GetDirectoryName(path);
            }

            return Path.GetFullPath(path);
        }

        private static bool IsGameDir(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(Path.Combine(path, GameExe));
        }

        private static string CreateBackup(string gameDir)
        {
            string stamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            string backupDir = Path.Combine(gameDir, "REPO_PTBR_Backups", "backup_" + stamp);
            Directory.CreateDirectory(backupDir);

            BackupPath(gameDir, backupDir, "winhttp.dll");
            BackupPath(gameDir, backupDir, "doorstop_config.ini");
            BackupPath(gameDir, backupDir, ".doorstop_version");
            BackupPath(gameDir, backupDir, Path.Combine("BepInEx", "config", "BepInEx.cfg"));
            BackupPath(gameDir, backupDir, Path.Combine("BepInEx", "core"));
            BackupPath(gameDir, backupDir, Path.Combine("BepInEx", "plugins", "RepoPTBRContextual"));
            BackupPath(gameDir, backupDir, Path.Combine("REPO_Data", "StreamingAssets", "Localizations", "Game.tsv"));
            BackupPath(gameDir, backupDir, Path.Combine("REPO_Data", "StreamingAssets", "Localizations", "HUD.tsv"));
            BackupPath(gameDir, backupDir, Path.Combine("REPO_Data", "StreamingAssets", "Localizations", "Menu.tsv"));

            string locale = GetLocalePath();
            if (File.Exists(locale))
            {
                string dest = Path.Combine(backupDir, "LocalLow", "semiwork", "Repo", "CurrentLocale.es3");
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.Copy(locale, dest, true);
            }

            File.WriteAllText(Path.Combine(backupDir, "README_RESTORE.txt"),
                "Backup criado antes de instalar R.E.P.O. PT-BR Contextual.\r\n" +
                "Copie estes arquivos de volta para a pasta do jogo se quiser desfazer.\r\n",
                Encoding.UTF8);

            return backupDir;
        }

        private static void BackupPath(string gameDir, string backupDir, string relativePath)
        {
            string source = Path.Combine(gameDir, relativePath);
            if (!File.Exists(source) && !Directory.Exists(source))
            {
                return;
            }

            string dest = Path.Combine(backupDir, relativePath);
            if (File.Exists(source))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.Copy(source, dest, true);
                return;
            }

            CopyDirectory(source, dest);
        }

        private static void InstallPayload(string gameDir)
        {
            string temp = Path.Combine(Path.GetTempPath(), "repo-ptbr-contextual-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temp);

            try
            {
                string zip = Path.Combine(temp, "payload.zip");
                using (Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RepoPtbrContextualInstaller.payload.zip"))
                {
                    if (resource == null)
                    {
                        throw new InvalidOperationException("Payload embutido não encontrado.");
                    }

                    using (FileStream output = File.Create(zip))
                    {
                        resource.CopyTo(output);
                    }
                }

                string extracted = Path.Combine(temp, "payload");
                Directory.CreateDirectory(extracted);
                ZipFile.ExtractToDirectory(zip, extracted);
                CopyDirectory(extracted, gameDir);
            }
            finally
            {
                try
                {
                    Directory.Delete(temp, true);
                }
                catch
                {
                }
            }
        }

        private static void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                string relative = dir.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                Directory.CreateDirectory(Path.Combine(destination, relative));
            }

            foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                string relative = file.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string dest = Path.Combine(destination, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.Copy(file, dest, true);
            }
        }

        private static void WriteLocalePreference()
        {
            string locale = GetLocalePath();
            Directory.CreateDirectory(Path.GetDirectoryName(locale));
            string text =
                "{\r\n" +
                "\t\"Locale\" : {\r\n" +
                "\t\t\"__type\" : \"string\",\r\n" +
                "\t\t\"value\" : \"en-US\"\r\n" +
                "\t}\r\n" +
                "}\r\n";

            File.WriteAllText(locale, text, new UTF8Encoding(false));
        }

        private static string GetLocalePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "AppData",
                "LocalLow",
                "semiwork",
                "Repo",
                "CurrentLocale.es3");
        }
    }
}
