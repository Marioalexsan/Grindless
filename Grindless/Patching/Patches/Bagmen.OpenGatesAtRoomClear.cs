using Bagmen;
using HarmonyLib;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(OpenGatesAtRoomClear))]
    static class Bagmen_OpenGatesAtRoomClear
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(OpenGatesAtRoomClear.OpenBlockades))]
        static void OpenBlockades_Postfix()
        {
            foreach (Mod mod in ModManager.Mods)
                mod.PostArcadeRoomComplete();
        }
    }
}
