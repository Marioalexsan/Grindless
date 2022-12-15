using HarmonyLib;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(GlobalData.MainMenu))]
    static class SoG_GlobalData_MainMenu
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GlobalData.MainMenu.Transition))]
        static void Transition_Prefix(GlobalData.MainMenu.MenuLevel enTarget)
        {
            if (enTarget == GlobalData.MainMenu.MenuLevel.CharacterSelect)
            {
                MainMenuWorker.UpdateStorySaveCompatibility();
            }
            else if (enTarget == GlobalData.MainMenu.MenuLevel.TopMenu)
            {
                MainMenuWorker.UpdateArcadeSaveCompatibility();
            }
        }
    }
}
