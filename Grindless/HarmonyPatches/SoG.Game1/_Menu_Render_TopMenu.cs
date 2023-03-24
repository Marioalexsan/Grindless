namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Menu_Render_TopMenu))]
    static class _Menu_Render_TopMenu
    {
        static void Postfix()
        {
            MainMenuWorker.CheckArcadeSaveCompatiblity();
            MainMenuWorker.RenderModMenuButton();
        }
    }
}
