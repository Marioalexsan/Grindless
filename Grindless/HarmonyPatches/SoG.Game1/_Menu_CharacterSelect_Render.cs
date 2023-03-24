namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Menu_CharacterSelect_Render))]
    static class _Menu_CharacterSelect_Render
    {
        static void Postfix()
        {
            MainMenuWorker.CheckStorySaveCompatibility();
        }
    }
}
