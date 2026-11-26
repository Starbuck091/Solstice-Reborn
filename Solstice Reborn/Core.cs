using MelonLoader;

[assembly: MelonInfo(typeof(Solstice.Core), "Solstice", "3.2.0", "WulfMarius, Starbuck097", null)]
[assembly: MelonGame("Hinterland", "TheLongDark")]

namespace Solstice
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            HarmonyInstance.PatchAll();
            Implementation.OnLoad();
            Implementation.Log("Initialized.");
        }
    }
}