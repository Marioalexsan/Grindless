namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Leaderboards_UploadEntryToSteamLeaderboards))]
static class _Leaderboards_UploadEntryToSteamLeaderboards
{
    [HarmonyPriority(Priority.First)]
    static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}
