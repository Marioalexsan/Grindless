using Microsoft.Extensions.Logging;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Saving_SaveRogueToFile), typeof(string))]
    static class _Saving_SaveRogueToFile
    {
        static void Prefix()
        {
            // Required so that the vanilla save holds the "SoG-only" version
            Globals.SetVersionTypeAsModded(false);
        }

        static void Postfix()
        {
            Globals.SetVersionTypeAsModded(true);
            SaveArcadeMetadataFile();
        }

        static void SaveArcadeMetadataFile()
        {
            string ext = ModSaving.SaveFileExtension;

            bool other = CAS.IsDebugFlagSet("OtherArcadeMode");
            string savFile = Globals.Game.sAppData + $"arcademode{(other ? "_other" : "")}.sav{ext}";

            using (FileStream file = new($"{savFile}.temp", FileMode.Create, FileAccess.Write))
            {
                Program.Logger.LogInformation("Saving mod arcade...");
                ModSaving.SaveModArcade(file);
            }

            File.Copy($"{savFile}.temp", savFile, overwrite: true);
            File.Delete($"{savFile}.temp");
        }

    }
}
