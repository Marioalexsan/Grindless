using HarmonyLib;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(GlobalData.MainMenu))]
    internal static class SoG_GlobalData_MainMenu
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GlobalData.MainMenu.Transition))]
        internal static void Transition_Prefix(GlobalData.MainMenu.MenuLevel enTarget)
        {
            if (enTarget == GlobalData.MainMenu.MenuLevel.CharacterSelect)
            {
                PatchHelper.MainMenuWorker.UpdateStorySaveCompatibility();
            }
            else if (enTarget == GlobalData.MainMenu.MenuLevel.TopMenu)
            {
                PatchHelper.MainMenuWorker.UpdateArcadeSaveCompatibility();
            }
        }
    }
}
