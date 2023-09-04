namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(BaseStats))]
static class SoG_BaseStats
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(BaseStats.Update))]
    static void Update_Prefix(BaseStats __instance)
    {
        //foreach (Mod mod in ModManager.Mods)
        //    mod.OnBaseStatsUpdate(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(BaseStats.Update))]
    static void Update_Postfix(BaseStats __instance)
    {
        //foreach (Mod mod in ModManager.Mods)
        //    mod.PostBaseStatsUpdate(__instance);
    }
}
