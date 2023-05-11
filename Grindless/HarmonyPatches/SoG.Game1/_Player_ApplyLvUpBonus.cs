namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Player_ApplyLvUpBonus))]
static class _Player_ApplyLvUpBonus
{
    static void Postfix(PlayerView xView)
    {
        foreach (Mod mod in ModManager.Mods)
            mod.PostPlayerLevelUp(xView);
    }
}
