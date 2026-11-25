using Il2Cpp;
using UnityEngine;

namespace Solstice
{
    internal class TimeWidgetUpdater
    {
        private static readonly Interpolator moonInterpolator = new Interpolator();
        private static readonly Interpolator sunInterpolator = new Interpolator();
        private static float moonRadius;
        private static float sunRadius;

        internal static void Initialize(TimeWidget timeWidget)
        {
            if (timeWidget == null)
            {
                return;
            }

            sunRadius = timeWidget.m_SunRadius;
            moonRadius = timeWidget.m_MoonRadius;
        }

        internal static bool Update(TimeWidget timeWidget, float angleDegrees)
        {
            if (timeWidget == null || timeWidget.m_SunSprite == null || timeWidget.m_MoonSprite == null)
            {
                return false;
            }

            var storm = GameManager.GetUniStorm();
            if (storm == null)
            {
                return false;
            }

            float normalizedTime = storm.m_NormalizedTime;
            float sunAngle = sunInterpolator.GetValue(normalizedTime);
            timeWidget.m_SunSprite.transform.localPosition = timeWidget.GetPositionOnCircle(sunRadius, sunAngle);

            float moonAngle = moonInterpolator.GetValue(normalizedTime);
            timeWidget.m_MoonSprite.transform.localPosition = timeWidget.GetPositionOnCircle(moonRadius, moonAngle);

            return true;
        }

        internal static void SetTimes(float sunrise, float sunset)
        {
            sunInterpolator.Clear();
            sunInterpolator.Set(0, -180);
            sunInterpolator.Set(sunrise / 24f, -100);
            sunInterpolator.Set(0.5f, 0);
            sunInterpolator.Set(sunset / 24f, 100);
            sunInterpolator.Set(1, 180);

            moonInterpolator.Clear();
            moonInterpolator.Set(0, 0);
            moonInterpolator.Set(sunrise / 24f, 100);
            moonInterpolator.Set(0.5f, 180);
            moonInterpolator.Set(sunset / 24f, 260);
            moonInterpolator.Set(1, 360);
        }

    }
}