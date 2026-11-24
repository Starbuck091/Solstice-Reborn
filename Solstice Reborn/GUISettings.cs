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

        [Name("Cycle Length")]
        [Description("The length of a full cycle in in-game time.")]
        [Choice("2 Months", "4 Months", "6 Months", "12 Months")]
        public int CycleLength = 1;

        [Name("Start Day")]
        [Description("Where in the cycle the game starts.\nWinter: days are short and cold\nSpring: days are getting longer and warmer\nSummer: days are long and warm\nAutumn: days are getting shorter and colder")]
        public StartOffset StartDay = StartOffset.Random;

        [Name("Winter wildlife die-off")]
        [Description("If enabled, wildlife becomes scarce in late autumn (about 70% gone) and almost disappears in winter (about 99% gone). Scripted animals are not affected.")]
        public bool WildlifeDieOff = true;

        [Section("Season lengths")]
        [Name("Custom season lengths")]
        [Description("Use the sliders below instead of equal seasons. The values are scaled to fit the selected cycle length.")]
        public bool CustomSeasons = false;

        [Name("Winter")]
        [Description("Length of winter in months (0-12). Scaled to the selected cycle length.")]
        [Slider(0, 12)]
        public int WinterLength = 3;

        [Name("Spring")]
        [Description("Length of spring in months (0-12). Scaled to the selected cycle length.")]
        [Slider(0, 12)]
        public int SpringLength = 3;

        [Name("Summer")]
        [Description("Length of summer in months (0-12). Scaled to the selected cycle length.")]
        [Slider(0, 12)]
        public int SummerLength = 3;

        [Name("Autumn")]
        [Description("Length of autumn in months (0-12). Scaled to the selected cycle length.")]
        [Slider(0, 12)]
        public int AutumnLength = 3;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == nameof(Enabled))
            {
                SyncVisibility();
            }
            else if (field.Name == nameof(CustomSeasons))
            {
                SyncVisibility();
            }
        }

        public void SyncVisibility()
        {
            SetFieldVisible(nameof(CycleLength), Enabled);
            SetFieldVisible(nameof(StartDay), Enabled);

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
    }

    public enum StartOffset
    {
        Random = 0,
        Winter = 1,
        Spring = 2,
        Summer = 3,
        Autumn = 4,
    }
}