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
    [HarmonyPatch(typeof(Game1), nameof(Game1._Saving_SaveWorldToFile))]
    static class _Saving_SaveWorldToFile
    {
        static void Prefix()
        {
            // Required so that the vanilla save holds the "SoG-only" version
            Globals.SetVersionTypeAsModded(false);
        }

        static void Postfix(int iFileSlot)
        {
            Globals.SetVersionTypeAsModded(true);
            SaveWorldMetadataFile(iFileSlot);
        }

        static void SaveWorldMetadataFile(int slot)
        {
            string ext = ModSaving.SaveFileExtension;

            PlayerView player = Globals.Game.xLocalPlayer;
            string appData = Globals.Game.sAppData;

            string backupPath = "";
            string chrFile = $"{appData}Characters/{slot}.cha{ext}";
            string wldFile = $"{appData}Worlds/{slot}.wld{ext}";

            if (File.Exists(chrFile))
            {
                if (player.sSaveableName == "")
                {
                    player.sSaveableName = player.sNetworkNickname;
                    foreach (char c in Path.GetInvalidFileNameChars())
                        player.sSaveableName = player.sSaveableName.Replace(c, ' ');
                }

                backupPath = $"{appData}Backups/{player.sSaveableName}_{player.xJournalInfo.iCollectorID}{slot}/";
                Directory.CreateDirectory(backupPath);
            }

            using (FileStream file = new($"{wldFile}.temp", FileMode.Create, FileAccess.Write))
            {
                Program.Logger.LogInformation("Saving mod world {Slot}...", slot);
                ModSaving.SaveModWorld(file);
            }

            try
            {
                File.Copy($"{wldFile}.temp", wldFile, overwrite: true);
                if (backupPath != "" && slot != 100)
                {
                    File.Copy($"{wldFile}.temp", backupPath + $"latest.wld{ext}", overwrite: true);
                }
                File.Delete($"{wldFile}.temp");
            }
            catch { }
        }

    }
}
