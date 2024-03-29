﻿namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1.EquipmentSpecialEffectAdded))]
internal class EquipmentSpecialEffectAdded
{
    static bool Prefix(EquipmentInfo.SpecialEffect enEffect, PlayerView xView)
    {
        var entry = Entries.EquipmentEffects.GetRequired(enEffect);

        if (entry.OnEquip == null && entry.IsVanilla)
            return true;  // Use vanilla equip add

        entry?.OnEquip(xView);
        return false;
    }

}
