namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Menu_Update))]
    static class _Menu_Update
    {
        static void Postfix()
        {
            MainMenuWorker.MenuUpdate();
        }
    }
}
