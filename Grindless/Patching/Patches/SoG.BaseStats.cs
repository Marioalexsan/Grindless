using HarmonyLib;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(BaseStats))]
    internal static class SoG_BaseStats
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BaseStats.Update))]
        public static void Update_Prefix(BaseStats __instance)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.OnBaseStatsUpdate(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseStats.Update))]
        public static void Update_Postfix(BaseStats __instance)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.PostBaseStatsUpdate(__instance);
        }
    }
}
