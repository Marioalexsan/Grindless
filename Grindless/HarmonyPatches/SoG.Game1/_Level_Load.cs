namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Level_Load))]
static class _Level_Load
{
    static void Postfix(LevelBlueprint xBP, bool bStaticOnly)
    {
        //foreach (Mod mod in ModManager.Mods)
        //    mod.PostLevelLoad(xBP.enZone, xBP.enRegion, bStaticOnly);
    }

}
