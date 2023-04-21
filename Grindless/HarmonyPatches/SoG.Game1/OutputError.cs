namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.OutputError), typeof(string), typeof(string))]
    static class OutputError
    {
        static bool Prefix()
        {
            // Don't write game errors
            return false;
        }
    }
}
