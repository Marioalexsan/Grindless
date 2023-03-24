namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._LevelLoading_DoStuff))]
    static class _LevelLoading_DoStuff
    {
        static bool Prefix(Game1 __instance, Level.ZoneEnum enLevel, bool bStaticOnly)
        {
            var entry = LevelEntry.Entries.Get(enLevel);

            if (entry.Loader == null && entry.IsVanilla)
                OriginalMethods._LevelLoading_DoStuff(__instance, enLevel, bStaticOnly);

            else EditedMethods._LevelLoading_DoStuff(__instance, enLevel, bStaticOnly);
            return false;
        }
    }
}
