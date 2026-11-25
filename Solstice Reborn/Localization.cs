using System;
using System.Collections.Generic;
using GameLocalization = Il2Cpp.Localization;

namespace Solstice
{
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
        };

        internal static bool IsRussian()
        {
            string language = GameLocalization.s_Language;
            return string.Equals(language, "Russian", StringComparison.OrdinalIgnoreCase)
                || string.Equals(language, "ru", StringComparison.OrdinalIgnoreCase);
        }

        internal static string GetRaw(string key)
        {
            if (!texts.TryGetValue(key, out string[] pair))
            {
                return key;
            }

            return IsRussian() ? pair[1] : pair[0];
        }
    }
}