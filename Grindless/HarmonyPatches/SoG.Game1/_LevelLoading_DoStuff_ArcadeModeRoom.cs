using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._LevelLoading_DoStuff_ArcadeModeRoom))]
    static class _LevelLoading_DoStuff_ArcadeModeRoom
    {
        static void Postfix()
        {
            foreach (Mod mod in ModManager.Mods)
                mod.PostArcadeRoomStart();
        }
    }
}
