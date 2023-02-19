using HarmonyLib;
using Microsoft.Extensions.Logging;
using SoG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Loading_LoadWorldFromFile))]
    static class _Loading_LoadWorldFromFile
    {
        static void Prefix()
        {
            // Required so that the vanilla save loads the "SoG-only" version
            Globals.SetVersionTypeAsModded(false);
        }

        static void Postfix(int iFileSlot)
        {
            Globals.SetVersionTypeAsModded(true);
            LoadWorldMetadataFile(iFileSlot);
        }

        static void LoadWorldMetadataFile(int slot)
        {
            string ext = ModSaving.SaveFileExtension;

            string wldFile = Path.Combine(Globals.AppDataPath, "Worlds", $"{slot}.wld{ext}");

            if (!File.Exists(wldFile))
                return;

            using var file = File.OpenRead(wldFile);

            Program.Logger.LogInformation("Loading mod world {Slot}...", slot);
            ModSaving.LoadModWorld(file);
        }

    }
}
