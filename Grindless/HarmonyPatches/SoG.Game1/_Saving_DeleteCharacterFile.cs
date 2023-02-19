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
    [HarmonyPatch(typeof(Game1), nameof(Game1._Saving_DeleteCharacterFile))]
    static class _Saving_DeleteCharacterFile
    {
        static void Prefix(int iFileSlot)
        {
            File.Delete($"{Globals.AppDataPath}Characters/{iFileSlot}.cha.gs");
        }
    }
}
