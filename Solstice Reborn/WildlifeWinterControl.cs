using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace Solstice
{
    internal static class WildlifeWinterControl
    {
        private static bool logged;

        internal static void Enforce(float reduction)
        {
            if (reduction <= 0f)
            {
                return;
            }

            var manager = GameManager.GetSpawnRegionManager();
            if (manager == null)
            {
                return;
            }

            var regions = manager.m_SpawnRegions;
            if (regions == null)
            {
                return;
            }

            int affected = 0;

            for (int i = 0; i < regions.Count; i++)
            {
                var region = regions[i];
                if (region == null || !region.m_ControlledByRandomSpawner)
                {
                    continue;
                }

                if (IsScriptedDen(region))
                {
                    continue;
                }

                affected++;

                int target = region.CalculateTargetPopulation();
                int active = region.GetNumActiveSpawns();

                if (active > target)
                {
                    region.RemoveActiveSpawns(active - target, WildlifeMode.Normal, false);
                }
            }

            if (!logged)
            {
                logged = true;
                Implementation.Log("Wildlife winter control: " + affected + " random spawn regions");
            }
        }

        private static bool IsScriptedDen(SpawnRegion region)
        {
            var dens = WolfDen.s_WolfDenObjects;
            if (dens == null)
            {
                return false;
            }

            for (int i = 0; i < dens.Count; i++)
            {
                var den = dens[i];
                if (den == null)
                {
                    continue;
                }

                if (den.m_DenSpawnRegion == region ||
                    den.m_WanderSpawnRegion == region ||
                    den.m_AlphaSpawnRegion == region)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.CalculateTargetPopulation))]
    internal class SpawnRegion_CalculateTargetPopulation
    {
        internal static void Postfix(ref int __result)
        {
            float reduction = Implementation.WildlifeReduction;
            if (reduction > 0f)
            {
                __result = Mathf.FloorToInt(__result * (1f - reduction));
            }
        }
    }

    [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.RespawnAllowed))]
    internal class SpawnRegion_RespawnAllowed
    {
        internal static void Postfix(ref bool __result)
        {
            if (Implementation.WildlifeReduction >= 0.98f)
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(SpawnRegion), nameof(SpawnRegion.MaybeSpawnPendingSerializedRespawn))]
    internal class SpawnRegion_MaybeSpawnPendingSerializedRespawn
    {
        internal static bool Prefix(ref BaseAi __result)
        {
            if (Implementation.WildlifeReduction >= 0.98f)
            {
                __result = null;
                return false;
            }

            return true;
        }
    }
}