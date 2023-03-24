namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Enemy_AdjustForDifficulty))]
    static class _Enemy_AdjustForDifficulty
    {
        // Currently this patch does not apply beastmode to modded enemies
        // Nor does it force boss enemies to ignore pets
        static bool Prefix(Enemy xEn)
        {
            var entry = EnemyEntry.Entries.GetRequired(xEn.enType);

            if (entry.DifficultyScaler == null && entry.IsVanilla)
                return true;  // No replacement found, run vanilla code

            entry.DifficultyScaler.Invoke(xEn);
            return false;
        }
    }
}
