namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._NPC_TakeDamage))]
static class _NPC_TakeDamage
{
    static void Prefix(NPC xEnemy, ref int iDamage, ref byte byType)
    {
        Mod.OnEntityDamageData data = new()
        {
            Entity = xEnemy,
            Damage = iDamage,
            Type = byType
        };

        foreach (Mod mod in ModManager.Mods)
        {
            mod.OnEntityDamage(data);
        }

        iDamage = data.Damage;
        byType = data.Type;
    }

    static void Postfix(NPC xEnemy, int iDamage, byte byType)
    {
        Mod.PostEntityDamageData data = new()
        {
            Entity = xEnemy,
            Damage = iDamage,
            Type = byType
        };

        foreach (Mod mod in ModManager.Mods)
        {
            mod.PostEntityDamage(data);
        }
    }
}
