using HarmonyLib;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(EquipmentCodex))]
    internal static class SoG_EquipmentCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(EquipmentCodex.GetArmorInfo))]
        internal static bool GetArmorInfo_Prefix(ref EquipmentInfo __result, ItemCodex.ItemTypes enType)
        {
            var entry = ItemEntry.Entries.Get(enType);;

            __result = entry?.vanillaEquip;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(EquipmentCodex.GetAccessoryInfo))]
        internal static bool GetAccessoryInfo_Prefix(ref EquipmentInfo __result, ItemCodex.ItemTypes enType)
        {
            return GetArmorInfo_Prefix(ref __result, enType);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(EquipmentCodex.GetShieldInfo))]
        internal static bool GetShieldInfo_Prefix(ref EquipmentInfo __result, ItemCodex.ItemTypes enType)
        {
            return GetArmorInfo_Prefix(ref __result, enType);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(EquipmentCodex.GetShoesInfo))]
        internal static bool GetShoesInfo_Prefix(ref EquipmentInfo __result, ItemCodex.ItemTypes enType)
        {
            return GetArmorInfo_Prefix(ref __result, enType);
        }
    }
}
