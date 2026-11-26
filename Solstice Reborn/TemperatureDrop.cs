using HarmonyLib;
using Il2Cpp;

namespace Solstice
{
    [HarmonyPatch(typeof(ExperienceModeManager), nameof(ExperienceModeManager.GetOutdoorTempDropCelcius))]
    internal class ExperienceModeManager_GetOutdoorTempDropCelcius
    {
        private static void Postfix(ref float __result, float numDays)
        {
            if (!Implementation.GlobalTemperatureDeclineEnabled)
            {
                return;
            }

            __result = Implementation.CalculateGlobalTemperatureDrop(numDays);
        }
    }
}