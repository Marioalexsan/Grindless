using Microsoft.Extensions.Logging;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1.OutputError), typeof(string), typeof(string))]
static class OutputError
{
    static bool Prefix(string e)
    {
        // Silent errors are usually triggered manually
        // It might be worthwhile to log them
        if (CAS.IsDebugFlagSet_Release("silentsend"))
        {
            Program.Logger.LogWarning("A silent error was triggered!");
            Program.Logger.LogWarning("{e}", e);
        }

        // Don't write game errors to disk via this method
        return false;
    }
}
