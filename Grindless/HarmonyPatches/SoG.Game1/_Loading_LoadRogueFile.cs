using Microsoft.Extensions.Logging;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Loading_LoadRogueFile))]
    internal class _Loading_LoadRogueFile
    {
        static void Prefix()
        {
            // Required so that the vanilla save loads the "SoG-only" version
            Globals.SetVersionTypeAsModded(false);
        }

        static void Postfix()
        {
            Globals.SetVersionTypeAsModded(true);
            LoadArcadeMetadataFile();
        }

        static void LoadArcadeMetadataFile()
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
    }
}
