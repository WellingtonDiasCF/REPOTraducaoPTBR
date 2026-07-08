using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

namespace RepoPTBRContextual
{
    [BepInPlugin("local.repo.ptbrcontextual", "R.E.P.O. PT-BR Contextual", "1.0.1")]
    public sealed class RepoPTBRContextualPlugin : BaseUnityPlugin
    {
        private Harmony harmony;

        public void Awake()
        {
            PTBRTranslator.Load(Paths.PluginPath);
            harmony = new Harmony("local.repo.ptbrcontextual");
            harmony.PatchAll(typeof(RepoPTBRContextualPlugin).Assembly);
            Logger.LogInfo("PT-BR contextual translation loaded.");
        }

        public void OnDestroy()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }
    }

    internal static class PTBRTranslator
    {
        private static readonly Dictionary<string, string> Exact = new Dictionary<string, string>();
        private static readonly HashSet<string> Missing = new HashSet<string>();
        private static readonly object MissingLock = new object();
        private static string missingPath;
        private static DateTime nextOverrideApplyAt = DateTime.MinValue;

        public static void Load(string pluginPath)
        {
            string ownDir = Path.Combine(pluginPath, "RepoPTBRContextual");
            string runtimePath = Path.Combine(ownDir, "runtime.tsv");
            missingPath = Path.Combine(Paths.ConfigPath, "RepoPTBRContextual.missing.tsv");

            Exact.Clear();
            if (!File.Exists(runtimePath))
            {
                LoadOfficialLocalizationPairs();
                return;
            }

            foreach (string line in File.ReadAllLines(runtimePath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                string[] parts = line.Split(new[] { '\t' }, 2);
                if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
                {
                    continue;
                }

                Exact[parts[0].Replace("\\n", "\n")] = parts[1].Replace("\\n", "\n");
            }

            LoadOfficialLocalizationPairs();
        }

        public static bool ShouldApplyOverridesNow()
        {
            DateTime now = DateTime.UtcNow;
            if (now < nextOverrideApplyAt)
            {
                return false;
            }

            nextOverrideApplyAt = now.AddSeconds(1);
            return true;
        }

        public static string Translate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            string translated;
            const string ptbrPrefix = "(PT-BR) ";
            if (value.StartsWith(ptbrPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string stripped = value.Substring(ptbrPrefix.Length);
                if (TryTranslate(stripped, out translated))
                {
                    return translated;
                }

                return stripped;
            }

            if (TryTranslate(value, out translated))
            {
                return translated;
            }

            LogMissing(value);
            return value;
        }

        private static bool TryTranslate(string value, out string translated)
        {
            if (Exact.TryGetValue(value, out translated))
            {
                return true;
            }

            string trimmed = value.Trim();
            if (trimmed.Length != value.Length && Exact.TryGetValue(trimmed, out translated))
            {
                int leading = value.Length - value.TrimStart().Length;
                int trailing = value.Length - value.TrimEnd().Length;
                translated = value.Substring(0, leading) + translated + value.Substring(value.Length - trailing);
                return true;
            }

            return false;
        }

        private static void LoadOfficialLocalizationPairs()
        {
            try
            {
                string localizationDir = Path.Combine(Application.streamingAssetsPath, "Localizations");
                string defaultDir = Path.Combine(localizationDir, "Default");
                string[] files = { "HUD.tsv", "Menu.tsv", "Game.tsv" };

                for (int i = 0; i < files.Length; i++)
                {
                    Dictionary<string, string> english = ReadTsv(Path.Combine(defaultDir, files[i]));
                    Dictionary<string, string> ptbr = ReadTsv(Path.Combine(localizationDir, files[i]));
                    foreach (KeyValuePair<string, string> pair in english)
                    {
                        string translated;
                        if (ptbr.TryGetValue(pair.Key, out translated) && !Exact.ContainsKey(pair.Value))
                        {
                            Exact[pair.Value] = translated;
                            string upper = pair.Value.ToUpperInvariant();
                            if (!Exact.ContainsKey(upper))
                            {
                                Exact[upper] = translated;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private static Dictionary<string, string> ReadTsv(string path)
        {
            Dictionary<string, string> entries = new Dictionary<string, string>();
            if (!File.Exists(path))
            {
                return entries;
            }

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(new[] { '\t' }, 2);
                if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
                {
                    entries[parts[0]] = parts[1].Replace("\\n", "\n");
                }
            }

            return entries;
        }

        private static void LogMissing(string value)
        {
            if (!LooksUserFacingEnglish(value))
            {
                return;
            }

            lock (MissingLock)
            {
                if (!Missing.Add(value))
                {
                    return;
                }

                File.AppendAllText(missingPath, value.Replace("\r", "\\r").Replace("\n", "\\n") + "\t\r\n", Encoding.UTF8);
            }
        }

        private static bool LooksUserFacingEnglish(string value)
        {
            string text = value.Trim();
            if (text.Length < 2 || text.Length > 180)
            {
                return false;
            }

            if (text.Contains("/") || text.Contains("\\") || text.Contains(".dll") || text.Contains(".asset"))
            {
                return false;
            }

            bool hasLetter = false;
            bool hasSpaceOrPunctuation = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    hasLetter = true;
                }
                else if (char.IsWhiteSpace(c) || ",.!?:;()[]{}'\"-".IndexOf(c) >= 0)
                {
                    hasSpaceOrPunctuation = true;
                }
            }

            if (!hasLetter)
            {
                return false;
            }

            if (!hasSpaceOrPunctuation && text.ToUpperInvariant() == text && text.Contains("."))
            {
                return false;
            }

            return true;
        }
    }

    internal static class LocaleTools
    {
        public static void ForcePtBr(object manager)
        {
            try
            {
                if (manager == null)
                {
                    return;
                }

                Type type = manager.GetType();
                FieldInfo availableField = AccessTools.Field(type, "AvailableLocales");
                MethodInfo switchLocale = AccessTools.Method(type, "SwitchLocale");
                IEnumerable locales = availableField == null ? null : availableField.GetValue(manager) as IEnumerable;
                if (locales == null || switchLocale == null)
                {
                    return;
                }

                foreach (object localeAsset in locales)
                {
                    string code = GetLocaleCode(localeAsset);
                    if (string.Equals(code, "pt-BR", StringComparison.OrdinalIgnoreCase))
                    {
                        switchLocale.Invoke(manager, new[] { localeAsset, (object)true });
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("[RepoPTBRContextual] Failed to apply localization overrides: " + ex);
            }
        }

        private static string GetLocaleCode(object localeAsset)
        {
            if (localeAsset == null)
            {
                return null;
            }

            FieldInfo localeReferenceField = AccessTools.Field(localeAsset.GetType(), "LocaleReference");
            object localeReference = localeReferenceField == null ? null : localeReferenceField.GetValue(localeAsset);
            if (localeReference == null)
            {
                return null;
            }

            object identifier = AccessTools.Property(localeReference.GetType(), "Identifier").GetValue(localeReference, null);
            return identifier == null ? null : (string)AccessTools.Property(identifier.GetType(), "Code").GetValue(identifier, null);
        }
    }

    internal static class OverrideTools
    {
        public static void ApplyOverridesToActiveLocales()
        {
            try
            {
                string dir = Path.Combine(Application.streamingAssetsPath, "Localizations");
                string[] tables = { "HUD", "Menu", "Game" };
                List<Locale> locales = GetTargetLocales();
                int applied = 0;

                for (int i = 0; i < tables.Length; i++)
                {
                    string tableName = tables[i];
                    string path = Path.Combine(dir, tableName + ".tsv");
                    if (!File.Exists(path))
                    {
                        continue;
                    }

                    List<KeyValuePair<string, string>> entries = ReadEntries(path);
                    for (int j = 0; j < locales.Count; j++)
                    {
                        applied += ApplyTable(tableName, locales[j], entries);
                    }
                }

                UnityEngine.Debug.Log("[RepoPTBRContextual] Applied " + applied + " localization override entries to " + locales.Count + " locale(s).");
            }
            catch
            {
            }
        }

        private static List<Locale> GetTargetLocales()
        {
            List<Locale> locales = new List<Locale>();
            AddLocale(locales, LocalizationSettings.SelectedLocale);
            AddLocale(locales, LocalizationSettings.ProjectLocale);

            try
            {
                if (LocalizationSettings.AvailableLocales != null)
                {
                    foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
                    {
                        if (IsPtBr(locale))
                        {
                            AddLocale(locales, locale);
                        }
                    }
                }
            }
            catch
            {
            }

            return locales;
        }

        private static void AddLocale(List<Locale> locales, Locale locale)
        {
            if (locale == null)
            {
                return;
            }

            for (int i = 0; i < locales.Count; i++)
            {
                if (locales[i] == locale)
                {
                    return;
                }
            }

            locales.Add(locale);
        }

        private static bool IsPtBr(Locale locale)
        {
            if (locale == null)
            {
                return false;
            }

            return string.Equals(locale.Identifier.Code, "pt-BR", StringComparison.OrdinalIgnoreCase);
        }

        private static List<KeyValuePair<string, string>> ReadEntries(string path)
        {
            List<KeyValuePair<string, string>> entries = new List<KeyValuePair<string, string>>();
            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(new[] { '\t' }, 2);
                if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
                {
                    continue;
                }

                entries.Add(new KeyValuePair<string, string>(parts[0], parts[1].Replace("\\n", "\n")));
            }

            return entries;
        }

        private static int ApplyTable(string tableName, Locale locale, List<KeyValuePair<string, string>> entries)
        {
            StringTable table = LocalizationSettings.StringDatabase.GetTable(tableName, locale);
            if (table == null)
            {
                return 0;
            }

            int applied = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                KeyValuePair<string, string> pair = entries[i];
                StringTableEntry entry = table.GetEntry(pair.Key);
                if (entry != null)
                {
                    entry.Value = pair.Value;
                }
                else
                {
                    table.AddEntry(pair.Key, pair.Value);
                }

                applied++;
            }

            return applied;
        }
    }

    [HarmonyPatch(typeof(LocalizationManager), "Start")]
    internal static class LocalizationManagerStartPatch
    {
        private static void Postfix(LocalizationManager __instance)
        {
            OverrideTools.ApplyOverridesToActiveLocales();
            MethodInfo notify = AccessTools.Method(__instance.GetType(), "NotifyLocalizationChanged");
            if (notify != null)
            {
                notify.Invoke(__instance, null);
            }
        }
    }

    [HarmonyPatch(typeof(LocalizationChangedEvent), "OnLocaleChange")]
    internal static class LocalizationChangedEventPatch
    {
        private static void Prefix()
        {
            if (PTBRTranslator.ShouldApplyOverridesNow())
            {
                OverrideTools.ApplyOverridesToActiveLocales();
            }
        }
    }

    [HarmonyPatch(typeof(TMP_Text), "set_text")]
    internal static class TMPTextPatch
    {
        private static void Prefix(ref string value)
        {
            value = PTBRTranslator.Translate(value);
        }
    }

    [HarmonyPatch]
    internal static class TMPSetTextPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            return typeof(TMP_Text)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.Name == "SetText"
                    && method.GetParameters().Length > 0
                    && method.GetParameters()[0].ParameterType == typeof(string));
        }

        private static void Prefix(ref string sourceText)
        {
            sourceText = PTBRTranslator.Translate(sourceText);
        }
    }

    [HarmonyPatch(typeof(Text), "set_text")]
    internal static class UITextPatch
    {
        private static void Prefix(ref string value)
        {
            value = PTBRTranslator.Translate(value);
        }
    }
}
