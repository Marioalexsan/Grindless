using HarmonyLib;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(WeaponCodex))]
    internal static class SoG_WeaponCodex
    {
        /// <summary>
        /// Retrieves the WeaponInfo of an entry.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(WeaponCodex.GetWeaponInfo))]
        internal static bool GetWeaponInfo_Prefix(ref WeaponInfo __result, ItemCodex.ItemTypes enType)
        {
            var entry = ItemEntry.Entries.Get(enType);;

            __result = entry?.vanillaEquip as WeaponInfo;

            return false;
        }
    }
}
