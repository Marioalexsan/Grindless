using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), "Initialize")]
    static class Initialize
    {
        static void Prefix()
        {
            Globals.InitializeGlobals();
        }
    }
}
