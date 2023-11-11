using SoG;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Player_TakeDamage))]
static class _Player_TakeDamage
{
    static void Prefix(PlayerView xView, ref int iInDamage, ref byte byType)
    {
        Mod.OnEntityDamageData data = new()
        {
            Entity = xView.xEntity,
            Damage = iInDamage,
            Type = byType
        };

        foreach (Mod mod in ModManager.Mods)
        {
            mod.OnEntityDamage(data);
        }

        iInDamage = data.Damage;
        byType = data.Type;
    }

    static void Postfix(PlayerView xView, int iInDamage, byte byType)
    {
        Mod.PostEntityDamageData data = new()
        {
            Entity = xView.xEntity,
            Damage = iInDamage,
            Type = byType
        };

        foreach (Mod mod in ModManager.Mods)
        {
            mod.PostEntityDamage(data);
        }
    }
}
