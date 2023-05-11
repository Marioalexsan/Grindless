namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Item_Use), new Type[] { typeof(ItemCodex.ItemTypes), typeof(PlayerView), typeof(bool) })]
static class _Item_Use
{
    static void Prefix(ItemCodex.ItemTypes enItem, PlayerView xView, ref bool bSend)
    {
        if (xView.xViewStats.bIsDead)
            return;

        foreach (Mod mod in ModManager.Mods)
            mod.OnItemUse(enItem, xView, ref bSend);
    }
}
