using HarmonyLib;
using TreatCurseMenu = SoG.ShopMenu.TreatCurseMenu;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(TreatCurseMenu))]
    internal static class SoG_ShopMenu_TreatCurseMenu
    {
        /// <summary>
        /// Inserts custom curses in the Curse shop menu.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(TreatCurseMenu.FillCurseList))]
        internal static bool FillCurseList_Prefix(TreatCurseMenu __instance)
        {
            __instance.lenTreatCursesAvailable.Clear();

            foreach (var entry in CurseEntry.Entries)
            {
                if (!entry.IsTreat)
                    __instance.lenTreatCursesAvailable.Add(entry.GameID);
            }

            return false;
        }

        /// <summary>
        /// Inserts custom curses in the Treat shop menu.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(TreatCurseMenu.FillTreatList))]
        internal static bool FillTreatList_Prefix(TreatCurseMenu __instance)
        {
            __instance.lenTreatCursesAvailable.Clear();

            foreach (var entry in CurseEntry.Entries)
            {
                if (entry.IsTreat)
                    __instance.lenTreatCursesAvailable.Add(entry.GameID);
            }

            return false;
        }
    }
}
