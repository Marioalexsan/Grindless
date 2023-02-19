using HarmonyLib;
using SoG;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(WeaponCodex))]
    static class SoG_WeaponCodex
    {
        /// <summary>
        /// Retrieves the WeaponInfo of an entry.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(WeaponCodex.GetWeaponInfo))]
        static bool GetWeaponInfo_Prefix(ref WeaponInfo __result, ItemCodex.ItemTypes enType)
        {
            __result = ItemEntry.Entries.Get(enType)?.vanillaEquip as WeaponInfo;
            return false;
        }
    }
}
