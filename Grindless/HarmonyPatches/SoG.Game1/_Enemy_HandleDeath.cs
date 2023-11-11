namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Enemy_HandleDeath))]
static class _Enemy_HandleDeath
{
    static void Postfix(Enemy xEnemy, AttackPhase xAttackPhaseThatHit)
    {
        Mod.OnEntityDeathData data = new()
        {
            Entity = xEnemy,
            WithEffect = false,
            AttackPhase = xAttackPhaseThatHit
        };

        foreach (Mod mod in ModManager.Mods)
            mod.OnEntityDeath(data);
    }
}
