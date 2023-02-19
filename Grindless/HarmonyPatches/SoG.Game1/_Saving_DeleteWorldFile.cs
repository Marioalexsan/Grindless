using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Saving_DeleteWorldFile))]
    static class _Saving_DeleteWorldFile
    {
        static void Postfix(int iFileSlot)
        {
            File.Delete($"{Globals.AppDataPath}Worlds/{iFileSlot}.wld.gs");
        }
    }
}
