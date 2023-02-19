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
    [HarmonyPatch(typeof(Game1), nameof(Game1._Saving_SaveCharacterToFile))]
    static class _Saving_SaveCharacterToFile
    {
        static void Prefix()
        {
            // Required so that the vanilla save holds the "SoG-only" version
            Globals.SetVersionTypeAsModded(false);
        }

        static void Postfix(int iFileSlot)
        {
            Globals.SetVersionTypeAsModded(true);
            SaveCharacterMetadataFile(iFileSlot);
        }

        static void SaveCharacterMetadataFile(int slot)
        {
            string ext = ModSaving.SaveFileExtension;

            PlayerView player = Globals.Game.xLocalPlayer;
            string appData = Globals.Game.sAppData;

            int carousel = player.iSaveCarousel - 1;
            if (carousel < 0)
                carousel += 5;

            string backupPath = "";

            string chrFile = $"{appData}Characters/{slot}.cha{ext}";

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

                File.Copy(chrFile, backupPath + $"auto{carousel}.cha{ext}", overwrite: true);

                string wldFile = $"{appData}Worlds/{slot}.wld{ext}";
                if (File.Exists(wldFile))
                {
                    File.Copy(wldFile, backupPath + $"auto{carousel}.wld{ext}", overwrite: true);
                }
            }

            using (FileStream file = new FileStream($"{chrFile}.temp", FileMode.Create, FileAccess.Write))
            {
                Program.Logger.LogInformation("Saving mod character {Slot}...", slot);
                ModSaving.SaveModCharacter(file);
            }

            try
            {
                File.Copy($"{chrFile}.temp", chrFile, overwrite: true);
                if (backupPath != "")
                {
                    File.Copy($"{chrFile}.temp", backupPath + $"latest.cha{ext}", overwrite: true);
                }
                File.Delete($"{chrFile}.temp");
            }
            catch { }
        }

    }
}
