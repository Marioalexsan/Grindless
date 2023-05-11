namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._LevelLoading_DoStuff_Arcadia))]
static class _LevelLoading_DoStuff_Arcadia
{
    static void Prefix()
    {
        foreach (Mod mod in ModManager.Mods)
            mod.OnArcadiaLoad();

        // Just in case it didn't get set before; submitting modded runs is not a good idea
        Globals.Game.xGameSessionData.xRogueLikeSession.bTemporaryHighScoreBlock = true;
    }
}
