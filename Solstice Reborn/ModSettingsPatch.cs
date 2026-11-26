using HarmonyLib;
using ModSettings;
using System.Reflection;

namespace Solstice
{
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

            if (section != null)
            {
                string key;
                switch (section.Title)
                {
                    case "Season lengths":
                        key = "SEASONS_SECTION";
                        break;
                    case "Global temperature decline":
                        key = "GLOBAL_SECTION";
                        break;
                    default:
                        key = "SECTION";
                        break;
                }

                section = new SectionAttribute(Localization.GetRaw(key));
            }

            if (name != null)
            {
                name = new NameAttribute(Localization.GetRaw(GetNameKey(field.Name)));
            }

            if (description != null)
            {
                description = new DescriptionAttribute(Localization.GetRaw(GetDescriptionKey(field.Name)));
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
                case nameof(GUISettings.CycleLength):
                    return "CYCLE";
                case nameof(GUISettings.StartDay):
                    return "START";
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
                    return "ENABLED";
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
                case nameof(GUISettings.CycleLength):
                    return "CYCLE_DESC";
                case nameof(GUISettings.StartDay):
                    return "START_DESC";
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
}