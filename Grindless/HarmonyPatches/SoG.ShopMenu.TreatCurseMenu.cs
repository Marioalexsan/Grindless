using TreatCurseMenu = SoG.ShopMenu.TreatCurseMenu;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(TreatCurseMenu))]
static class SoG_ShopMenu_TreatCurseMenu
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(TreatCurseMenu.FillCurseList))]
    static bool FillCurseList_Prefix(TreatCurseMenu __instance)
    {
        __instance.lenTreatCursesAvailable.Clear();
        __instance.lenTreatCursesAvailable.AddRange(
            CurseEntry.Entries.Where(x => !x.IsTreat).Select(x => x.GameID)
            );
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(TreatCurseMenu.FillTreatList))]
    static bool FillTreatList_Prefix(TreatCurseMenu __instance)
    {
        __instance.lenTreatCursesAvailable.Clear();
        __instance.lenTreatCursesAvailable.AddRange(
            CurseEntry.Entries.Where(x => x.IsTreat).Select(x => x.GameID)
            );
        return false;
    }
}
