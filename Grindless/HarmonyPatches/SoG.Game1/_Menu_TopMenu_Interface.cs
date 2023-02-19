using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Menu_TopMenu_Interface))]
    static class _Menu_TopMenu_Interface
    {
        static void Postfix()
        {
            MainMenuWorker.PostTopMenuInterface();
        }
    }
}
