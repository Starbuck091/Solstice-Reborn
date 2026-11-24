using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace Solstice
{
    [HarmonyPatch(typeof(UniStormWeatherSystem), nameof(UniStormWeatherSystem.Init))]
    internal class UniStormWeatherSystem_Init
    {
        internal static void Postfix(UniStormWeatherSystem __instance)
        {
            Implementation.Log("UniStorm Init, custom mode: " + GameManager.InCustomMode());
            Implementation.Init(__instance);
            Implementation.ApplySettings(__instance);
        }
    }

    [HarmonyPatch(typeof(UniStormWeatherSystem), nameof(UniStormWeatherSystem.Update))]
    internal class UniStormWeatherSystem_Update
    {
        private static int lastDay = int.MinValue;

        internal static void Prefix(UniStormWeatherSystem __instance)
        {
            if (__instance.m_DayCounter == lastDay)
            {
                return;
            }

            lastDay = __instance.m_DayCounter;
            Implementation.Update(__instance);
        }
    }

    [HarmonyPatch(typeof(TODStateData), nameof(TODStateData.SetBlended))]
    internal class TODStateData_SetBlended
    {
        internal static void Postfix(TODStateData __instance)
        {
            if (!Implementation.Enabled)
            {
                return;
            }

            var storm = GameManager.GetUniStorm();
            if (storm == null || storm.IsNightOrNightBlend())
            {
                return;
            }

            float brightnessMultiplier = Implementation.BrightnessMultiplier;

            __instance.m_SunLightIntensity *= brightnessMultiplier;
            __instance.m_BloomIntensity *= brightnessMultiplier;
            __instance.m_SkyFogColor *= brightnessMultiplier;
            __instance.m_FogColor *= brightnessMultiplier;
        }
    }

    [HarmonyPatch(typeof(UniStormWeatherSystem), nameof(UniStormWeatherSystem.UpdateSunTransform))]
    internal class UniStormWeatherSystem_UpdateSunTransform
    {
        private static readonly float REFERENCE_ANGLE = 45;
        private static readonly float REFERENCE_HEIGHT = Mathf.Sin(Mathf.Deg2Rad * REFERENCE_ANGLE);

        internal static bool Prefix(UniStormWeatherSystem __instance)
        {
            if (!Implementation.Enabled || __instance.m_SunLight == null)
            {
                return true;
            }

            Transform transform = __instance.m_SunLight.transform;

            transform.forward = new Vector3(0, -REFERENCE_HEIGHT, REFERENCE_HEIGHT);
            transform.Rotate(Vector3.up, __instance.m_NormalizedTime * 360f - 180f);

            float offset = REFERENCE_HEIGHT - Mathf.Sin(Mathf.Deg2Rad * __instance.m_SunAngle);
            transform.LookAt(new Vector3(transform.forward.x, transform.forward.y + offset, transform.forward.z));

            return false;
        }
    }

    [HarmonyPatch(typeof(Weather), "GenerateTempHigh")]
    internal class Weather_GenerateTempHigh
    {
        internal static void Postfix(Weather __instance)
        {
            if (Implementation.Enabled)
            {
                __instance.m_TempHigh += Implementation.TemperatureOffset;
            }
        }
    }

    [HarmonyPatch(typeof(Weather), "GenerateTempLow")]
    internal class Weather_GenerateTempLow
    {
        internal static void Postfix(Weather __instance)
        {
            if (Implementation.Enabled)
            {
                __instance.m_TempLow += Implementation.TemperatureOffset;
            }
        }
    }
}