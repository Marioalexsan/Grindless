namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Menu_TopMenu_Interface))]
static class _Menu_TopMenu_Interface
{
    static void Postfix()
    {
        MainMenuWorker.PostTopMenuInterface();
    }
}
