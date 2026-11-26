using System.Reflection;
using ModSettings;

namespace Solstice
{
    public class GUISettings : JsonModSettings
    {
        public GUISettings() : base("Solstice")
        {
        }

        [Section("Solstice")]
        [Name("Enabled")]
        [Description("If enabled the length of the day will cycle from short to long and back to short.")]
        public bool Enabled = true;

        [Name("Show season counter")]
        [Description("Show the season and day counter under the clock while the HUD is visible.")]
        public bool ShowSeasonHud = true;

        [Name("Seasonal temperature")]
        [Description("Apply the seasonal temperature offset. Disable this to use only the global temperature decline.")]
        public bool SeasonalTemperature = true;

        [Name("Cycle Length")]
        [Description("The length of a full cycle in in-game time.")]
        [Choice("2 Months", "4 Months", "6 Months", "12 Months")]
        public int CycleLength = 1;

        [Name("Start Day")]
        [Description("Where in the cycle the game starts.\nWinter: days are short and cold\nSpring: days are getting longer and warmer\nSummer: days are long and warm\nAutumn: days are getting shorter and colder")]
        public StartOffset StartDay = StartOffset.Random;

        [Section("Global temperature decline")]
        [Name("Enable global temperature decline")]
        [Description("Enable a global temperature drop over survival days, similar to Extreme Temperature Drop.")]
        public bool EnableGlobalTemperatureDecline = false;

        [Name("Preset")]
        [Description("Preset values from Extreme Temperature Drop. Custom lets you set the values manually.")]
        [Choice("Custom", "Pilgram", "Voyager", "Stalker", "Interloper")]
        public TempPreset Preset = TempPreset.Pilgram;

        [Name("Initial Temperature Drop")]
        [Description("The global temperature drop on day 0.")]
        [Slider(0, 100, 101)]
        public float InitialDrop = 0f;

        [Name("Decline Starting Day")]
        [Description("The day at which global temperatures start dropping.")]
        [Slider(0, 200)]
        public int DeclineStartDay = 0;

        [Name("Maximum Temperature Drop")]
        [Description("The maximum decrease in global temperature.")]
        [Slider(0, 100, 101)]
        public float MaxDrop = 0f;

        [Name("Decline Ending Day")]
        [Description("The day at which global temperature stops dropping.")]
        [Slider(0, 200)]
        public int DeclineEndDay = 0;

        [Name("Winter wildlife die-off")]
        [Description("If enabled, wildlife becomes scarce in late autumn (about 70% gone) and almost disappears in winter (about 99% gone). Scripted animals are not affected.")]
        public bool WildlifeDieOff = true;

        [Section("Season lengths")]
        [Name("Custom season lengths")]
        [Description("Use the sliders below instead of equal seasons. The values are scaled to fit the selected cycle length.")]
        public bool CustomSeasons = false;

        [Name("Winter")]
        [Description("Length of the season in months (0-12). Scaled to the selected cycle length.")]
        [Slider(0, 12)]
        public int WinterLength = 3;

        [Name("Spring")]
        [Description("Length of the season in months (0-12). Scaled to the selected cycle length.")]
        [Slider(0, 12)]
        public int SpringLength = 3;

        [Name("Summer")]
        [Description("Length of the season in months (0-12). Scaled to the selected cycle length.")]
        [Slider(0, 12)]
        public int SummerLength = 3;

        [Name("Autumn")]
        [Description("Length of the season in months (0-12). Scaled to the selected cycle length.")]
        [Slider(0, 12)]
        public int AutumnLength = 3;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            base.OnChange(field, oldValue, newValue);

            if (field.Name == nameof(Enabled)
                || field.Name == nameof(CustomSeasons)
                || field.Name == nameof(EnableGlobalTemperatureDecline))
            {
                SyncVisibility();
            }

            if (field.Name == nameof(Preset))
            {
                UsePreset((TempPreset)Convert.ToInt32(newValue));
            }
            else if (IsGlobalTemperatureField(field.Name))
            {
                Preset = TempPreset.Custom;
            }

            if (field.Name == nameof(InitialDrop))
            {
                MaxDrop = Math.Max((float)newValue, MaxDrop);
            }
            else if (field.Name == nameof(MaxDrop))
            {
                InitialDrop = Math.Min((float)newValue, InitialDrop);
            }
            else if (field.Name == nameof(DeclineStartDay))
            {
                DeclineEndDay = Math.Max((int)newValue, DeclineEndDay);
            }
            else if (field.Name == nameof(DeclineEndDay))
            {
                DeclineStartDay = Math.Min((int)newValue, DeclineStartDay);
            }

            RefreshGUI();
        }

        public void SyncVisibility()
        {
            SetFieldVisible(nameof(ShowSeasonHud), Enabled);
            SetFieldVisible(nameof(SeasonalTemperature), Enabled);
            SetFieldVisible(nameof(CycleLength), Enabled);
            SetFieldVisible(nameof(StartDay), Enabled);
            SetFieldVisible(nameof(EnableGlobalTemperatureDecline), Enabled);

            bool globalVisible = Enabled && EnableGlobalTemperatureDecline;
            SetFieldVisible(nameof(Preset), globalVisible);
            SetFieldVisible(nameof(InitialDrop), globalVisible);
            SetFieldVisible(nameof(DeclineStartDay), globalVisible);
            SetFieldVisible(nameof(MaxDrop), globalVisible);
            SetFieldVisible(nameof(DeclineEndDay), globalVisible);

            bool seasonsVisible = Enabled && CustomSeasons;
            SetFieldVisible(nameof(WinterLength), seasonsVisible);
            SetFieldVisible(nameof(SpringLength), seasonsVisible);
            SetFieldVisible(nameof(SummerLength), seasonsVisible);
            SetFieldVisible(nameof(AutumnLength), seasonsVisible);
        }

        protected override void OnConfirm()
        {
            base.OnConfirm();
            Implementation.ApplySettings();
        }

        private static bool IsGlobalTemperatureField(string fieldName)
        {
            return fieldName == nameof(InitialDrop)
                || fieldName == nameof(DeclineStartDay)
                || fieldName == nameof(MaxDrop)
                || fieldName == nameof(DeclineEndDay);
        }

        private void UsePreset(TempPreset preset)
        {
            switch (preset)
            {
                case TempPreset.Pilgram:
                    InitialDrop = 0f;
                    DeclineStartDay = 0;
                    MaxDrop = 0f;
                    DeclineEndDay = 0;
                    break;
                case TempPreset.Voyager:
                    InitialDrop = 0f;
                    DeclineStartDay = 20;
                    MaxDrop = 5f;
                    DeclineEndDay = 200;
                    break;
                case TempPreset.Stalker:
                    InitialDrop = 0f;
                    DeclineStartDay = 20;
                    MaxDrop = 10f;
                    DeclineEndDay = 200;
                    break;
                case TempPreset.Interloper:
                    InitialDrop = 0f;
                    DeclineStartDay = 5;
                    MaxDrop = 20f;
                    DeclineEndDay = 50;
                    break;
            }
        }
    }

    public enum StartOffset
    {
        Random = 0,
        Winter = 1,
        Spring = 2,
        Summer = 3,
        Autumn = 4,
    }

    public enum TempPreset
    {
        Custom = 0,
        Pilgram = 1,
        Voyager = 2,
        Stalker = 3,
        Interloper = 4,
    }
}