namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Player_TakeDamage))]
    static class _Player_TakeDamage
    {
        static void Prefix(PlayerView xView, ref int iInDamage, ref byte byType)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.OnPlayerDamaged(xView, ref iInDamage, ref byType);
        }
    }
}
