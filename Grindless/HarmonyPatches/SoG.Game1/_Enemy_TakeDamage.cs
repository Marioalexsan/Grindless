namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Enemy_TakeDamage))]
    static class _Enemy_TakeDamage
    {
        static void Prefix(Enemy xEnemy, ref int iDamage, ref byte byType)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.OnEnemyDamaged(xEnemy, ref iDamage, ref byType);
        }
    }
}
