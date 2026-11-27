using HarmonyLib;
using System;
using Il2Cpp;
using ModSettings;
using System.Reflection;

namespace Solstice
{
    // ModSettings читает атрибуты в GUIBuilder.AddSettings при каждом построении меню.
    // Здесь мы подменяем английские подписи на нужный язык в момент чтения.
    [HarmonyPatch]
    internal static class ModSettingsAttributesPatch
    {
        private static MethodBase TargetMethod()
        {
            var attributesType = AccessTools.TypeByName("ModSettings.Attributes");
            if (attributesType == null)
            {
                return null;
            }

            return attributesType.GetMethod("GetAttributes", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }

        private static void Postfix(
            FieldInfo field,
            ref SectionAttribute section,
            ref NameAttribute name,
            ref DescriptionAttribute description,
            ref ChoiceAttribute choice)
        {
            if (field.DeclaringType != typeof(GUISettings))
            {
                return;
            }

            try
            {
                if (field.Name == nameof(GUISettings.Enabled))
                {
                    Implementation.Log($"Solstice: страница настроек строится, меню {(Localization.IsRussian() ? "ru" : "en")}, {Localization.Describe()}");
                }

                if (section != null)
                {
                    string sectionKey = GetSectionKey(section.Title);
                    if (sectionKey != null)
                    {
                        section = new SectionAttribute(Localization.GetRaw(sectionKey));
                    }
                }

                if (name != null)
                {
                    string nameKey = GetNameKey(field.Name);
                    if (nameKey != null)
                    {
                        name = new NameAttribute(Localization.GetRaw(nameKey));
                    }
                }

                if (description != null)
                {
                    string descriptionKey = GetDescriptionKey(field.Name);
                    if (descriptionKey != null)
                    {
                        description = new DescriptionAttribute(Localization.GetRaw(descriptionKey));
                    }
                }

                if (choice != null)
                {
                    string[] keys = GetChoiceKeys(field.Name, choice.Names.Length);
                    if (keys != null)
                    {
                        string[] values = new string[keys.Length];
                        for (int i = 0; i < keys.Length; i++)
                        {
                            values[i] = Localization.GetRaw(keys[i]);
                        }

                        choice = new ChoiceAttribute(values);
                    }
                }
            }
            catch
            {
                // Любая ошибка локализации не должна ломать меню: остаются английские подписи.
            }
        }

        private static string GetSectionKey(string title)
        {
            switch (title)
            {
                case "Season lengths":
                    return "SEASONS_SECTION";
                case "Global temperature decline":
                    return "GLOBAL_SECTION";
                default:
                    return "SECTION";
            }
        }

        private static string GetNameKey(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(GUISettings.Enabled):
                    return "ENABLED";
                case nameof(GUISettings.ShowSeasonHud):
                    return "SHOW_HUD";
                case nameof(GUISettings.SeasonalTemperature):
                    return "SEASONAL_TEMP";
                case nameof(GUISettings.CycleLength):
                    return "CYCLE";
                case nameof(GUISettings.StartDay):
                    return "START";
                case nameof(GUISettings.EnableGlobalTemperatureDecline):
                    return "GLOBAL_ENABLED";
                case nameof(GUISettings.Preset):
                    return "PRESET";
                case nameof(GUISettings.InitialDrop):
                    return "INITIAL_DROP";
                case nameof(GUISettings.DeclineStartDay):
                    return "DECLINE_START";
                case nameof(GUISettings.MaxDrop):
                    return "MAX_DROP";
                case nameof(GUISettings.DeclineEndDay):
                    return "DECLINE_END";
                case nameof(GUISettings.WildlifeDieOff):
                    return "WILDLIFE";
                case nameof(GUISettings.CustomSeasons):
                    return "CUSTOM";
                case nameof(GUISettings.WinterLength):
                    return "WINTER";
                case nameof(GUISettings.SpringLength):
                    return "SPRING";
                case nameof(GUISettings.SummerLength):
                    return "SUMMER";
                case nameof(GUISettings.AutumnLength):
                    return "AUTUMN";
                default:
                    return null;
            }
        }

        private static string GetDescriptionKey(string fieldName)
        {
            switch (fieldName)
            {
                case nameof(GUISettings.Enabled):
                    return "ENABLED_DESC";
                case nameof(GUISettings.ShowSeasonHud):
                    return "SHOW_HUD_DESC";
                case nameof(GUISettings.SeasonalTemperature):
                    return "SEASONAL_TEMP_DESC";
                case nameof(GUISettings.CycleLength):
                    return "CYCLE_DESC";
                case nameof(GUISettings.StartDay):
                    return "START_DESC";
                case nameof(GUISettings.EnableGlobalTemperatureDecline):
                    return "GLOBAL_ENABLED_DESC";
                case nameof(GUISettings.Preset):
                    return "PRESET_DESC";
                case nameof(GUISettings.InitialDrop):
                    return "INITIAL_DROP_DESC";
                case nameof(GUISettings.DeclineStartDay):
                    return "DECLINE_START_DESC";
                case nameof(GUISettings.MaxDrop):
                    return "MAX_DROP_DESC";
                case nameof(GUISettings.DeclineEndDay):
                    return "DECLINE_END_DESC";
                case nameof(GUISettings.WildlifeDieOff):
                    return "WILDLIFE_DESC";
                case nameof(GUISettings.CustomSeasons):
                    return "CUSTOM_DESC";
                default:
                    return "LENGTH_DESC";
            }
        }

        private static string[] GetChoiceKeys(string fieldName, int count)
        {
            if (fieldName == nameof(GUISettings.CycleLength) && count == 4)
            {
                return new[] { "CYCLE_0", "CYCLE_1", "CYCLE_2", "CYCLE_3" };
            }

            if (fieldName == nameof(GUISettings.StartDay) && count == 5)
            {
                return new[] { "START_0", "START_1", "START_2", "START_3", "START_4" };
            }

            if (fieldName == nameof(GUISettings.Preset) && count == 5)
            {
                return new[] { "PRESET_0", "PRESET_1", "PRESET_2", "PRESET_3", "PRESET_4" };
            }

            return null;
        }
    }

    // Запоминаем панель настроек и переписываем подписи при смене языка.
    [HarmonyPatch(typeof(Panel_OptionsMenu))]
    internal static class LanguageChangePatch
    {
        private static bool prepared;

        private static bool Prepare()
        {
            if (!prepared)
            {
                prepared = true;
                Implementation.Log("Solstice: хуки панели настроек применены");
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("InitializeAutosaveMenuItems")]
        private static void Postfix_InitializeAutosaveMenuItems(Panel_OptionsMenu __instance)
        {
            Localization.TrackPanel(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnConfirmLanguage")]
        private static void Postfix_OnConfirmLanguage(Panel_OptionsMenu __instance)
        {
            Localization.TrackPanel(__instance);

            string report = Localization.ApplyToUiReport(__instance.transform);

            Implementation.Log($"Solstice: OnConfirmLanguage, {report}, {Localization.Describe()}");
        }

        [HarmonyPostfix]
        [HarmonyPatch("RefreshLanguage")]
        private static void Postfix_RefreshLanguage(Panel_OptionsMenu __instance)
        {
            Localization.TrackPanel(__instance);
            Localization.ApplyToUi(__instance.transform);
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetLanguage")]
        private static void Postfix_SetLanguage(Panel_OptionsMenu __instance)
        {
            Localization.TrackPanel(__instance);

            string report = Localization.ApplyToUiReport(__instance.transform);

            Implementation.Log($"Solstice: SetLanguage, {report}, {Localization.Describe()}");
        }
    }

    // Само событие смены языка игры — самый надёжный момент для перевода уже созданных подписей.
    [HarmonyPatch(typeof(Il2Cpp.Localization), "set_Language")]
    internal static class GameLanguageChangedPatch
    {
        private static void Postfix()
        {
            Localization.ApplyTrackedReport(out string report);

            Implementation.Log($"Solstice: игра сменила язык (set_Language), {report}, {Localization.Describe()}");
        }
    }

    // Некоторые пути смены языка идут через SelectLanguage, а не через сеттер свойства.
    [HarmonyPatch(typeof(Il2Cpp.Localization), "SelectLanguage")]
    internal static class GameSelectLanguagePatch
    {
        private static void Postfix()
        {
            Localization.ApplyTrackedReport(out string report);

            Implementation.Log($"Solstice: игра сменила язык (SelectLanguage), {report}, {Localization.Describe()}");
        }
    }

    // Подписи, которые ModSettings выставляет уже после сборки страницы
    // (описания, значения списков), переводим в момент присваивания текста.
    [HarmonyPatch(typeof(UILabel), "set_text")]
    internal static class LabelTextPatch
    {
        [ThreadStatic]
        private static bool busy;

        private static void Postfix(UILabel __instance)
        {
            if (busy)
            {
                return;
            }

            string text = __instance.text;
            string translated = Localization.TranslateText(text);

            if (translated == null)
            {
                return;
            }

            try
            {
                busy = true;
                __instance.text = translated;
            }
            catch
            {
            }
            finally
            {
                busy = false;
            }
        }
    }
}