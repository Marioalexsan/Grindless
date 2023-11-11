using SoG;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Player_KillPlayer), new Type[] { typeof(PlayerView), typeof(bool), typeof(bool) })]
static class _Player_KillPlayer
{
    static void Prefix(PlayerView xView, bool bWithEffect)
    {
        Mod.OnEntityDeathData data = new()
        {
            Entity = xView.xEntity,
            WithEffect = bWithEffect
        };

        foreach (Mod mod in ModManager.Mods)
            mod.OnEntityDeath(data);
    }
}
