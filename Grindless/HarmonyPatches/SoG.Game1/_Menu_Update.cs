using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Menu_Update))]
    static class _Menu_Update
    {
        static void Postfix()
        {
            MainMenuWorker.MenuUpdate();
        }
    }
}
