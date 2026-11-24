using Il2Cpp;
using MelonLoader;
using ModSettings;
using System;
using System.Reflection;
using UnityEngine;

namespace Solstice
{
    internal class Implementation
    {
        private const float INTERPOLATOR_STEPS = 12;

        private static readonly Interpolator interpolator = new Interpolator();
        private static readonly InterpolatedValue sunAngle = new InterpolatedValue(30, 47);
        private static readonly InterpolatedValue dawn = new InterpolatedValue(5, -7);
        private static readonly InterpolatedValue sunrise = new InterpolatedValue(6, -6.333f);
        private static readonly InterpolatedValue sunset = new InterpolatedValue(18, 6.333f);
        private static readonly InterpolatedValue dusk = new InterpolatedValue(19, 6.8f);

        private static readonly GUISettings guiSettings = new GUISettings();
        private static readonly Settings settings = new Settings();

        private static float[] originalKeyframeTimes;
        private static float originalMasterTimeKeyOffset;
        private static int forcedDay = -1;
        private static float strength = 1;

        private static bool customSeasons;
        private static bool wildlifeDieOff;
        private static int[] seasonStarts = { 0, 0, 0, 0 };
        private static int[] seasonLengths = { 0, 0, 0, 0 };

        internal static float BrightnessMultiplier { get; private set; }

        internal static float TemperatureOffset { get; private set; }

        internal static float WildlifeReduction { get; private set; }

        internal static int CycleLength
        {
            get => settings.cycleLength;
            private set => settings.cycleLength = value;
        }

        internal static int CycleOffset
        {
            get => settings.cycleOffset;
            private set => settings.cycleOffset = value;
        }

        internal static bool Enabled
        {
            get => settings.enabled;
            private set => settings.enabled = value;
        }

        public static void OnLoad()
        {
            Log("Version {0}", Assembly.GetExecutingAssembly().GetName().Version);

            guiSettings.AddToModSettings("Solstice");
            guiSettings.SyncVisibility();

            interpolator.Set(0, -0.492f);
            interpolator.Set(1, -0.415f);
            interpolator.Set(2, -0.216f);
            interpolator.Set(3, 0.008f);
            interpolator.Set(4, 0.264f);
            interpolator.Set(5, 0.44f);
            interpolator.Set(6, 0.5f);
            interpolator.Set(7, 0.443f);
            interpolator.Set(8, 0.264f);
            interpolator.Set(9, 0.02f);
            interpolator.Set(10, -0.225f);
            interpolator.Set(11, -0.419f);
            interpolator.Set(12, -0.492f);

            uConsole.RegisterCommand("solstice-day", new Action(SolsticeDay));
        }

        internal static void ApplySettings(UniStormWeatherSystem uniStorm = null)
        {
            Enabled = guiSettings.Enabled;

            switch (guiSettings.CycleLength)
            {
                case 0:
                    CycleLength = 60;
                    break;
                case 1:
                    CycleLength = 120;
                    break;
                case 2:
                    CycleLength = 180;
                    break;
                default:
                    CycleLength = 360;
                    break;
            }

            customSeasons = guiSettings.CustomSeasons;
            wildlifeDieOff = guiSettings.WildlifeDieOff;
            RecalculateSeasons();

            switch (guiSettings.StartDay)
            {
                case StartOffset.Random:
                    CycleOffset = -1;
                    break;
                case StartOffset.Winter:
                    CycleOffset = seasonStarts[0];
                    break;
                case StartOffset.Spring:
                    CycleOffset = seasonStarts[1];
                    break;
                case StartOffset.Summer:
                    CycleOffset = seasonStarts[2];
                    break;
                case StartOffset.Autumn:
                    CycleOffset = seasonStarts[3];
                    break;
            }

            Log("Enabled: " + Enabled + ", Cycle Length: " + CycleLength + ", Cycle Offset: " + CycleOffset);

            var storm = uniStorm ?? GameManager.GetUniStorm();
            if (storm == null)
            {
                return;
            }

            RestoreKeyframeTimes(storm);
            Update(storm);
        }

        internal static void Disable()
        {
            if (!Enabled)
            {
                return;
            }

            Enabled = false;
            WildlifeReduction = 0f;

            var storm = GameManager.GetUniStorm();
            if (storm != null)
            {
                RestoreKeyframeTimes(storm);
            }
        }

        private static void RecalculateSeasons()
        {
            if (!customSeasons)
            {
                int quarter = CycleLength / 4;
                for (int i = 0; i < 4; i++)
                {
                    seasonLengths[i] = quarter;
                }

                seasonLengths[0] += CycleLength - quarter * 4;
            }
            else
            {
                int[] months =
                {
                    guiSettings.WinterLength,
                    guiSettings.SpringLength,
                    guiSettings.SummerLength,
                    guiSettings.AutumnLength
                };

                int total = 0;
                for (int i = 0; i < 4; i++)
                {
                    total += months[i];
                }

                if (total <= 0)
                {
                    int quarter = CycleLength / 4;
                    for (int i = 0; i < 4; i++)
                    {
                        seasonLengths[i] = quarter;
                    }

                    seasonLengths[0] += CycleLength - quarter * 4;
                }
                else
                {
                    float[] exact = new float[4];
                    int assigned = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        exact[i] = CycleLength * (float)months[i] / total;
                        seasonLengths[i] = (int)exact[i];
                        assigned += seasonLengths[i];
                    }

                    for (int remaining = CycleLength - assigned; remaining > 0; remaining--)
                    {
                        int best = 0;
                        float bestFraction = -1f;

                        for (int i = 0; i < 4; i++)
                        {
                            float fraction = exact[i] - seasonLengths[i];
                            if (months[i] > 0 && fraction > bestFraction)
                            {
                                bestFraction = fraction;
                                best = i;
                            }
                        }

                        seasonLengths[best]++;
                    }
                }
            }

            int start = 0;
            for (int i = 0; i < 4; i++)
            {
                seasonStarts[i] = start;
                start += seasonLengths[i];
            }
        }

        private static int GetSeason(int cycleDay, out float position)
        {
            int season = 0;
            int day = cycleDay;

            while (season < 3)
            {
                int length = seasonLengths[season];
                if (length > 0 && day < length)
                {
                    break;
                }

                if (length > 0)
                {
                    day -= length;
                }

                season++;
            }

            int seasonLength = seasonLengths[season];
            position = seasonLength > 0 ? day / (float)seasonLength : 0f;
            return season;
        }

        private static float GetStrength(int cycleDay, int season, float position)
        {
            if (!customSeasons)
            {
                return interpolator.GetValue(INTERPOLATOR_STEPS * cycleDay / CycleLength);
            }

            float winterStrength = interpolator.GetValue(0);
            float summerStrength = interpolator.GetValue(6);

            switch (season)
            {
                case 0:
                    return winterStrength;
                case 1:
                    return Mathf.Lerp(winterStrength, summerStrength, position);
                case 2:
                    return summerStrength;
                default:
                    return Mathf.Lerp(summerStrength, winterStrength, position);
            }
        }

        private static float GetWildlifeReduction(int season, float position)
        {
            if (!wildlifeDieOff)
            {
                return 0f;
            }

            switch (season)
            {
                case 0:
                    return Mathf.Lerp(0.70f, 0.99f, Mathf.Clamp01(position / 0.1f));
                case 1:
                    return Mathf.Lerp(0.99f, 0f, Mathf.Clamp01(position / 0.1f));
                case 3:
                    return 0.70f * Mathf.Clamp01(position);
                default:
                    return 0f;
            }
        }

        internal static void Init(UniStormWeatherSystem uniStormWeatherSystem)
        {
            var times = uniStormWeatherSystem.m_TODKeyframeTimes;
            if (times == null)
            {
                return;
            }

            originalMasterTimeKeyOffset = uniStormWeatherSystem.m_MasterTimeKeyOffset;
            originalKeyframeTimes = new float[times.Length];

            for (int i = 0; i < times.Length; i++)
            {
                originalKeyframeTimes[i] = times[i];
            }
        }

        internal static void Log(string message)
        {
            MelonLogger.Msg("[Solstice] " + message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            Log(string.Format(message, parameters));
        }

        internal static void SolsticeDay()
        {
            if (CycleLength <= 0)
            {
                return;
            }

            if (uConsole.GetNumParameters() == 1)
            {
                forcedDay = uConsole.GetInt() % CycleLength;
                Update(GameManager.GetUniStorm());
            }
        }

        internal static void Update(UniStormWeatherSystem uniStormWeatherSystem)
        {
            if (!Enabled || uniStormWeatherSystem == null || CycleLength <= 0)
            {
                return;
            }

            int offset = CycleOffset;
            if (offset < 0)
            {
                uint gameId = SaveGameSystem.m_CurrentGameId;
                offset = gameId != 0 ? (int)(gameId % (uint)CycleLength) : 0;
            }

            int cycleDay = (uniStormWeatherSystem.m_DayCounter + offset) % CycleLength;
            if (forcedDay >= 0)
            {
                cycleDay = forcedDay;
            }

            int season = GetSeason(cycleDay, out float seasonPosition);
            strength = GetStrength(cycleDay, season, seasonPosition);
            WildlifeReduction = GetWildlifeReduction(season, seasonPosition);

            BrightnessMultiplier = Mathf.Clamp(1.05f + strength, 0, 1);
            TemperatureOffset = Mathf.Clamp(10 * strength, float.NegativeInfinity, 2);
            ConfigureKeyframeTimes(uniStormWeatherSystem);
            WildlifeWinterControl.Enforce(WildlifeReduction);

            Log("Day: " + cycleDay + " / " + CycleLength + "; strength: " + strength + "; wildlife reduction: " + WildlifeReduction);
        }

        private static void ConfigureKeyframeTimes(UniStormWeatherSystem uniStormWeatherSystem)
        {
            var keyframeTimes = uniStormWeatherSystem.m_TODKeyframeTimes;
            if (keyframeTimes == null || keyframeTimes.Length < 7)
            {
                Log("Unexpected m_TODKeyframeTimes length");
                return;
            }

            uniStormWeatherSystem.m_MasterTimeKeyOffset = 0;

            keyframeTimes[0] = dawn.Calculate(strength);
            keyframeTimes[1] = sunrise.Calculate(strength);
            keyframeTimes[2] = keyframeTimes[1] + 1;
            keyframeTimes[3] = 12;
            keyframeTimes[6] = dusk.Calculate(strength);
            keyframeTimes[5] = sunset.Calculate(strength);
            keyframeTimes[4] = keyframeTimes[5] - 1;

            uniStormWeatherSystem.m_SunAngle = sunAngle.Calculate(strength);

            TimeWidgetUpdater.SetTimes(keyframeTimes[1], keyframeTimes[5]);
        }

        private static void RestoreKeyframeTimes(UniStormWeatherSystem uniStormWeatherSystem)
        {
            var keyframeTimes = uniStormWeatherSystem.m_TODKeyframeTimes;
            if (keyframeTimes == null || originalKeyframeTimes == null)
            {
                return;
            }

            int count = Math.Min(keyframeTimes.Length, originalKeyframeTimes.Length);
            for (int i = 0; i < count; i++)
            {
                keyframeTimes[i] = originalKeyframeTimes[i];
            }

            uniStormWeatherSystem.m_MasterTimeKeyOffset = originalMasterTimeKeyOffset;
        }
    }

    internal class Settings
    {
        public int cycleLength;
        public int cycleOffset;
        public bool enabled;
    }
}