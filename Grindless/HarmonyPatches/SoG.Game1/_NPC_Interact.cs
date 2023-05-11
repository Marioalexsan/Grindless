namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._NPC_Interact))]
static class _NPC_Interact
{
    static void _NPC_Interact_Prefix(PlayerView xView, NPC xNPC)
    {
        foreach (Mod mod in ModManager.Mods)
            mod.OnNPCInteraction(xNPC);
    }
}
