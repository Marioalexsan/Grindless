namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), "Initialize")]
static class Initialize
{
    static void Prefix()
    {
        Globals.InitializeGlobals();
    }
}
