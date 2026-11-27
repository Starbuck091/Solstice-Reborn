using System;
using System.Collections.Generic;
using System.Globalization;
using Il2Cpp;
using UnityEngine;
using GameLocalization = Il2Cpp.Localization;

namespace Solstice
{
    // Все подписи меню в одном месте: [0] — английский, [1] — русский.
    // Язык берётся из самой игры, а уже созданные подписи можно переписать
    // при смене языка без пересборки страницы настроек.
    internal static class Localization
    {
        private static readonly Dictionary<string, string[]> texts = new Dictionary<string, string[]>
        {
            { "SECTION",         new[] { "Solstice", "Solstice" } },
            { "ENABLED",         new[] { "Enabled", "Включено" } },
            { "ENABLED_DESC",    new[] { "If enabled the length of the day will cycle from short to long and back to short.", "Если включено, длина светового дня будет меняться от короткого к длинному и обратно." } },
            { "CYCLE",           new[] { "Cycle Length", "Длина цикла" } },
            { "CYCLE_DESC",      new[] { "The length of a full cycle in in-game time.", "Длительность полного цикла в игровом времени." } },
            { "CYCLE_0",         new[] { "2 Months", "2 месяца" } },
            { "CYCLE_1",         new[] { "4 Months", "4 месяца" } },
            { "CYCLE_2",         new[] { "6 Months", "6 месяцев" } },
            { "CYCLE_3",         new[] { "12 Months", "12 месяцев" } },
            { "START",           new[] { "Start Day", "Стартовый сезон" } },
            { "START_DESC",      new[] { "Where in the cycle the game starts.\nWinter: days are short and cold\nSpring: days are getting longer and warmer\nSummer: days are long and warm\nAutumn: days are getting shorter and colder", "С какого места цикла начинается игра.\nЗима: дни короткие и холодные\nВесна: дни становятся длиннее и теплее\nЛето: дни длинные и тёплые\nОсень: дни становятся короче и холоднее" } },
            { "START_0",         new[] { "Random", "Случайно" } },
            { "START_1",         new[] { "Winter", "Зима" } },
            { "START_2",         new[] { "Spring", "Весна" } },
            { "START_3",         new[] { "Summer", "Лето" } },
            { "START_4",         new[] { "Autumn", "Осень" } },
            { "WILDLIFE",        new[] { "Winter wildlife die-off", "Зимнее сокращение животных" } },
            { "WILDLIFE_DESC",   new[] { "If enabled, wildlife becomes scarce in late autumn (about 70% gone) and almost disappears in winter (about 99% gone). Scripted animals are not affected.", "Если включено, к концу осени животных становится меньше примерно на 70%, а зимой они почти исчезают (около 99%). Именные и скриптовые животные не затрагиваются." } },
            { "SEASONS_SECTION", new[] { "Season lengths", "Длины сезонов" } },
            { "CUSTOM",          new[] { "Custom season lengths", "Свои длины сезонов" } },
            { "CUSTOM_DESC",     new[] { "Use the sliders below instead of equal seasons. The values are scaled to fit the selected cycle length.", "Использовать ползунки ниже вместо равных сезонов. Значения пересчитываются под выбранную длину цикла." } },
            { "WINTER",          new[] { "Winter", "Зима" } },
            { "SPRING",          new[] { "Spring", "Весна" } },
            { "SUMMER",          new[] { "Summer", "Лето" } },
            { "AUTUMN",          new[] { "Autumn", "Осень" } },
            { "LENGTH_DESC",     new[] { "Length of the season in months (0-12). Scaled to the selected cycle length.", "Длина сезона в месяцах (0-12). Пересчитывается под выбранную длину цикла." } },
            { "HUD_0",           new[] { "Winter, day {0}/{1}", "Зима, день {0}/{1}" } },
            { "HUD_1",           new[] { "Spring, day {0}/{1}", "Весна, день {0}/{1}" } },
            { "HUD_2",           new[] { "Summer, day {0}/{1}", "Лето, день {0}/{1}" } },
            { "HUD_3",           new[] { "Autumn, day {0}/{1}", "Осень, день {0}/{1}" } },
            { "SHOW_HUD",        new[] { "Show season counter", "Показывать счётчик сезона" } },
            { "SHOW_HUD_DESC",   new[] { "Show the season and day counter under the clock while the HUD is visible.", "Показывать сезон и день сезона под часами, когда HUD виден." } },
            { "SEASONAL_TEMP",   new[] { "Seasonal temperature", "Сезонная температура" } },
            { "SEASONAL_TEMP_DESC", new[] { "Apply the seasonal temperature offset. Disable this to use only the global temperature decline.", "Учитывать сезонную поправку температуры. Отключите, если хотите использовать только глобальное снижение." } },
            { "GLOBAL_SECTION",  new[] { "Global temperature decline", "Глобальное снижение температуры" } },
            { "GLOBAL_ENABLED",  new[] { "Enable global temperature decline", "Включить глобальное снижение температуры" } },
            { "GLOBAL_ENABLED_DESC", new[] { "Enable a global temperature drop over survival days, similar to Extreme Temperature Drop.", "Включить глобальное снижение температуры по дням выживания, как в Extreme Temperature Drop." } },
            { "PRESET",          new[] { "Preset", "Пресет" } },
            { "PRESET_DESC",     new[] { "Preset values from Extreme Temperature Drop. Custom lets you set the values manually.", "Готовые значения из Extreme Temperature Drop. «Пользовательский» позволяет задать всё вручную." } },
            { "PRESET_0",        new[] { "Custom", "Пользовательский" } },
            { "PRESET_1",        new[] { "Pilgram", "Пилигрим" } },
            { "PRESET_2",        new[] { "Voyager", "Скиталец" } },
            { "PRESET_3",        new[] { "Stalker", "Сталкер" } },
            { "PRESET_4",        new[] { "Interloper", "Незваный гость" } },
            { "INITIAL_DROP",    new[] { "Initial Temperature Drop", "Начальное снижение температуры" } },
            { "INITIAL_DROP_DESC", new[] { "The global temperature drop on day 0.", "Глобальное снижение температуры в день 0." } },
            { "DECLINE_START",   new[] { "Decline Starting Day", "День начала снижения" } },
            { "DECLINE_START_DESC", new[] { "The day at which global temperatures start dropping.", "День, с которого начинает действовать глобальное снижение температуры." } },
            { "MAX_DROP",        new[] { "Maximum Temperature Drop", "Максимальное снижение температуры" } },
            { "MAX_DROP_DESC",   new[] { "The maximum decrease in global temperature.", "Максимальная величина глобального снижения температуры." } },
            { "DECLINE_END",     new[] { "Decline Ending Day", "День окончания снижения" } },
            { "DECLINE_END_DESC", new[] { "The day at which global temperature stops dropping.", "День, на котором глобальное снижение температуры достигает максимума." } },
        };

        private static Dictionary<string, string> textToKey;
        private static string lastReport;
        private static Transform panelRoot;

        internal static bool IsRussian()
        {
            string language = DetectLanguage(out string source);
            bool russian = LooksRussian(language);

            Report(language, source, russian);
            return russian;
        }

        // Главный источник — язык самой игры (статическое поле s_Language).
        // PlayerPrefs и язык системы — только запасные варианты.
        private static string DetectLanguage(out string source)
        {
            string game = SafeGameLanguage();

            if (!string.IsNullOrEmpty(game))
            {
                source = "язык игры s_Language";
                return game;
            }

            string prefs = SafePrefsLanguage();

            if (!string.IsNullOrEmpty(prefs))
            {
                source = "PlayerPrefs Language";
                return prefs;
            }

            source = "язык системы";
            return SafeSystemLanguage();
        }

        internal static string SafeGameLanguage()
        {
            try
            {
                return GameLocalization.s_Language ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static string SafePrefsLanguage()
        {
            try
            {
                return PlayerPrefs.GetString("Language", string.Empty) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static string SafeSystemLanguage()
        {
            try
            {
                return Application.systemLanguage.ToString();
            }
            catch
            {
            }

            try
            {
                return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static string Describe()
        {
            return $"s_Language=«{SafeGameLanguage()}», PlayerPrefs=«{SafePrefsLanguage()}», система=«{SafeSystemLanguage()}»";
        }

        private static bool LooksRussian(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                return false;
            }

            // Покрывает ru, RU, ru-RU, Russian, Русский.
            return language.StartsWith("ru", StringComparison.OrdinalIgnoreCase)
                || language.IndexOf("russian", StringComparison.OrdinalIgnoreCase) >= 0
                || language.IndexOf("русск", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void Report(string language, string source, bool russian)
        {
            string key = (language ?? string.Empty) + "|" + source + "|" + russian;

            if (key == lastReport)
            {
                return;
            }

            lastReport = key;
            Implementation.Log($"Solstice: меню настроек — {(russian ? "ru" : "en")}, источник: {source}, {Describe()}");
        }

        internal static string GetRaw(string key)
        {
            if (!texts.TryGetValue(key, out string[] pair))
            {
                return key;
            }

            return IsRussian() ? pair[1] : pair[0];
        }

        // Панель настроек, чтобы потом переписать её подписи без пересборки.
        internal static void TrackPanel(Panel_OptionsMenu panel)
        {
            try
            {
                if (panel != null)
                {
                    panelRoot = panel.transform;
                }
            }
            catch
            {
            }
        }

        internal static int ApplyTracked()
        {
            return ApplyTrackedReport(out _);
        }

        internal static int ApplyTrackedReport(out string report)
        {
            return Translate(panelRoot, out report);
        }

        internal static int ApplyToUi(Transform root)
        {
            if (root == null)
            {
                return 0;
            }

            return Translate(root, out _);
        }

        internal static string ApplyToUiReport(Transform root)
        {
            if (root == null)
            {
                return "панель не найдена";
            }

            Translate(root, out string report);
            return report;
        }

        private static int Translate(Transform root, out string report)
        {
            int panelVisited = 0;
            int panelMatched = 0;
            int panelChanged = 0;

            if (root != null)
            {
                TranslateTree(root, ref panelVisited, ref panelMatched, ref panelChanged);
            }

            int sceneVisited = 0;
            int sceneMatched = 0;
            int sceneChanged = 0;

            TranslateScene(ref sceneVisited, ref sceneMatched, ref sceneChanged);

            int matched = panelMatched + sceneMatched;
            int changed = panelChanged + sceneChanged;

            report = $"панель: {panelMatched}/{panelVisited}, сцена: {sceneMatched}/{sceneVisited}, изменено: {changed}";

            if (matched == 0)
            {
                report += " | образцы: " + SampleSceneTexts();
            }

            return changed;
        }

        // Перевод одиночной строки для подписей, которые ModSettings выставляет позже
        // (описания, значения списков).
        internal static string TranslateText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            if (!ByText().TryGetValue(text.Trim(), out string key))
            {
                return null;
            }

            string value = GetRaw(key);

            return string.Equals(value, text, StringComparison.Ordinal) ? null : value;
        }

        private static void TranslateScene(ref int visited, ref int matched, ref int changed)
        {
            try
            {
                var labels = UnityEngine.Object.FindObjectsOfType<UILabel>(true);

                for (int i = 0; i < labels.Length; i++)
                {
                    UILabel label = labels[i] as UILabel;

                    if (label != null)
                    {
                        TranslateLabel(label, ref visited, ref matched, ref changed);
                    }
                }
            }
            catch
            {
            }
        }

        private static void TranslateLabel(UILabel label, ref int visited, ref int matched, ref int changed)
        {
            visited++;

            string text = label.text;

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!ByText().TryGetValue(text.Trim(), out string key))
            {
                return;
            }

            matched++;
            string value = GetRaw(key);

            if (!string.Equals(value, text, StringComparison.Ordinal))
            {
                label.text = value;
                changed++;
            }
        }

        private static string SampleSceneTexts()
        {
            var list = new List<string>();

            try
            {
                var labels = UnityEngine.Object.FindObjectsOfType<UILabel>(true);

                for (int i = 0; i < labels.Length && list.Count < 12; i++)
                {
                    UILabel label = labels[i] as UILabel;

                    if (label == null)
                    {
                        continue;
                    }

                    string text = (label.text ?? string.Empty).Trim();

                    if (text.Length == 0)
                    {
                        continue;
                    }

                    if (text.Length > 40)
                    {
                        text = text.Substring(0, 40) + "…";
                    }

                    list.Add(text);
                }
            }
            catch
            {
            }

            return list.Count == 0 ? "-" : string.Join(" | ", list);
        }

        private static void TranslateTree(Transform node, ref int visited, ref int matched, ref int changed)
        {
            try
            {
                UILabel label = node.GetComponent<UILabel>();

                if (label != null)
                {
                    TranslateLabel(label, ref visited, ref matched, ref changed);
                }

                int count = node.childCount;

                for (int i = 0; i < count; i++)
                {
                    TranslateTree(node.GetChild(i), ref visited, ref matched, ref changed);
                }
            }
            catch
            {
            }
        }

        private static Dictionary<string, string> ByText()
        {
            if (textToKey != null)
            {
                return textToKey;
            }

            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, string[]> pair in texts)
            {
                map[pair.Value[0].Trim()] = pair.Key;
                map[pair.Value[1].Trim()] = pair.Key;
            }

            textToKey = map;
            return textToKey;
        }
    }
}