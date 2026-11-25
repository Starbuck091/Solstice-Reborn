using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace Solstice
{
    internal static class SeasonWidget
    {
        private static GameObject root;
        private static UILabel label;
        private static TimeWidget owner;

        internal static void TryCreate(TimeWidget timeWidget)
        {
            if (timeWidget == null)
            {
                return;
            }

            if (owner == timeWidget && root != null && label != null)
            {
                return;
            }

            if (root != null)
            {
                UnityEngine.Object.Destroy(root);
            }

            owner = timeWidget;

            var hud = InterfaceManager.GetPanel<Panel_HUD>();
            UILabel source = hud != null ? hud.m_SliderLabel_TOD : null;
            if (source == null)
            {
                source = UnityEngine.Object.FindObjectOfType<UILabel>();
            }

            if (source == null)
            {
                return;
            }

            root = NGUITools.AddChild(timeWidget.gameObject);
            root.name = "SolsticeSeasonLabel";
            root.layer = timeWidget.gameObject.layer;

            label = root.AddComponent<UILabel>();
            label.font = source.font;
            label.fontSize = 16;
            label.width = 300;
            label.height = 24;
            label.alignment = NGUIText.Alignment.Center;
            label.pivot = UIWidget.Pivot.Top;
            label.depth = source.depth + 20;
            label.color = source.color;
            label.text = string.Empty;

            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            Reposition();
            Implementation.Log("Season widget: radius=" + timeWidget.m_SunRadius + "/" + timeWidget.m_MoonRadius + ", offset=" + GetOffset(timeWidget));

            Refresh();
        }

        internal static void Refresh()
        {
            if (root == null && owner != null)
            {
                TryCreate(owner);
            }

            if (root == null || label == null)
            {
                return;
            }

            if (!Implementation.Enabled || !Implementation.ShowSeasonHud || Implementation.SeasonLength <= 0)
            {
                root.SetActive(false);
                return;
            }

            root.SetActive(true);
            label.text = string.Format(
                Localization.GetRaw("HUD_" + Implementation.CurrentSeason),
                Implementation.DayInSeason,
                Implementation.SeasonLength);
        }

        internal static void Reposition()
        {
            if (root != null && owner != null)
            {
                root.transform.localPosition = new Vector3(0f, -GetOffset(owner), 0f);
            }
        }

        private static float GetOffset(TimeWidget timeWidget)
        {
            float radius = Mathf.Max(timeWidget.m_SunRadius, timeWidget.m_MoonRadius);
            if (radius <= 1f)
            {
                radius = 40f;
            }

            return radius - 28f;
        }
    }

    [HarmonyPatch(typeof(TimeWidget), nameof(TimeWidget.Update))]
    internal class TimeWidget_Update
    {
        internal static void Postfix()
        {
            SeasonWidget.Reposition();
        }
    }

    [HarmonyPatch(typeof(TimeWidget), nameof(TimeWidget.Start))]
    internal class TimeWidget_Start
    {
        internal static void Postfix(TimeWidget __instance)
        {
            SeasonWidget.TryCreate(__instance);
        }
    }
}