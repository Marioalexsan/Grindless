using Microsoft.Extensions.Logging;
using SoG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.Patching
{
    internal static class PatchImplementer
    {
        public static bool ParseModCommands(string command, string message, int connection)
        {
            string[] words = command.Split(new[] { ':' }, 2);
            if (words.Length != 2)
                return false; // Is probably a vanilla command

            string target = words[0];
            string trueCommand = words[1];

            Mod mod = ModManager.Mods.FirstOrDefault(x => x.Name == target);

            // Also try case insensitive close matches
            mod ??= ModManager.Mods.FirstOrDefault(x => x.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase));

            if (mod == null)
            {
                CAS.AddChatMessage($"[{GrindlessMod.ModName}] Unknown mod!");
                return true;
            }

            var entry = CommandEntry.Entries.Get(mod, "");

            CommandParser parser = null;

            if (entry != null && !entry.Commands.TryGetValue(trueCommand, out parser))
            {
                // Try case insensitive close matches
                parser = entry.Commands.First(x => x.Key.Equals(trueCommand, StringComparison.InvariantCultureIgnoreCase)).Value;
            }

            if (entry == null || parser == null)
            {
                if (trueCommand.Equals("help", StringComparison.InvariantCultureIgnoreCase))
                {
                    ParseModCommands($"{GrindlessMod.ModName}:Help", target, connection);
                    return true;
                }

                CAS.AddChatMessage($"[{target}] Unknown command!");
                return true;
            }

            string[] args = ModUtils.GetArgs(message);

            Program.Logger.LogDebug("Parsed command {target} : {trueCommand}, arguments: {args.Length}", target, trueCommand, args.Length);
            parser(args, connection);

            return true;
        }

        public static void SaveCharacterMetadataFile(int slot)
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

        public static void SaveWorldMetadataFile(int slot)
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

            using (FileStream file = new FileStream($"{wldFile}.temp", FileMode.Create, FileAccess.Write))
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

        public static void SaveArcadeMetadataFile()
        {
            string ext = ModSaving.SaveFileExtension;

            bool other = CAS.IsDebugFlagSet("OtherArcadeMode");
            string savFile = Globals.Game.sAppData + $"arcademode{(other ? "_other" : "")}.sav{ext}";

            using (FileStream file = new FileStream($"{savFile}.temp", FileMode.Create, FileAccess.Write))
            {
                Program.Logger.LogInformation("Saving mod arcade...");
                ModSaving.SaveModArcade(file);
            }

            File.Copy($"{savFile}.temp", savFile, overwrite: true);
            File.Delete($"{savFile}.temp");
        }

        public static void LoadCharacterMetadataFile(int slot)
        {
            string ext = ModSaving.SaveFileExtension;

            string chrFile = Globals.Game.sAppData + "Characters/" + $"{slot}.cha{ext}";

            if (!File.Exists(chrFile)) return;

            using var file = File.OpenRead(chrFile);

            Program.Logger.LogInformation("Loading mod character {Slot}...", slot);
            ModSaving.LoadModCharacter(file);
        }

        public static void LoadWorldMetadataFile(int slot)
        {
            string ext = ModSaving.SaveFileExtension;

            string wldFile = Path.Combine(Globals.AppDataPath, "Worlds", $"{slot}.wld{ext}");

            if (!File.Exists(wldFile)) 
                return;

            using var file = File.OpenRead(wldFile);

            Program.Logger.LogInformation("Loading mod world {Slot}...", slot);
            ModSaving.LoadModWorld(file);
        }

        public static void LoadArcadeMetadataFile()
        {
            string ext = ModSaving.SaveFileExtension;

            if (RogueLikeMode.LockedOutDueToHigherVersionSaveFile) return;

            bool other = CAS.IsDebugFlagSet("OtherArcadeMode");
            string savFile = Globals.Game.sAppData + $"arcademode{(other ? "_other" : "")}.sav{ext}";

            if (!File.Exists(savFile)) return;

            using var file = new FileStream(savFile, FileMode.Open, FileAccess.Read);
            Program.Logger.LogInformation("Loading mod arcade...");
            ModSaving.LoadModArcade(file);
        }

        public static void PrepareModLoader()
        {
            GrindlessResources.ReloadResources();
            ModManager.Reload();

            MainMenuWorker.UpdateStorySaveCompatibility();
            MainMenuWorker.UpdateArcadeSaveCompatibility();
        }
    }
}
