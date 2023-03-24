namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.EquipmentSpecialEffectRemoved))]
    static class EquipmentSpecialEffectRemoved
    {
        static bool Prefix(EquipmentInfo.SpecialEffect enEffect, PlayerView xView)
        {
            var entry = EquipmentEffectEntry.Entries.GetRequired(enEffect);

            if (entry.OnRemove == null && entry.IsVanilla)
                return true;  // Use vanilla equip add

            entry?.OnRemove(xView);
            return false;
        }
    }
}
