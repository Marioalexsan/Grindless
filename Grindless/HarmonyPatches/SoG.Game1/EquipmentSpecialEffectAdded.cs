using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.EquipmentSpecialEffectAdded))]
    internal class EquipmentSpecialEffectAdded
    {
        static bool Prefix(EquipmentInfo.SpecialEffect enEffect, PlayerView xView)
        {
            var entry = EquipmentEffectEntry.Entries.GetRequired(enEffect);

            if (entry.OnEquip == null && entry.IsVanilla)
                return true;  // Use vanilla equip add

            entry?.OnEquip(xView);
            return false;
        }

    }
}
