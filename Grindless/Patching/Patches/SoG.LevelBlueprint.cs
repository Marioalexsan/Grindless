using HarmonyLib;
using LevelLoading;
using System;
using SoG;
using Microsoft.Extensions.Logging;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(LevelBlueprint))]
    static class SoG_LevelBlueprint
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LevelBlueprint.GetBlueprint))]
        static bool GetBlueprint_Prefix(ref LevelBlueprint __result, Level.ZoneEnum enZoneToGet)
        {
            var entry = LevelEntry.Entries.Get(enZoneToGet);

            if (entry == null)
            {
                return true;
            }

            if (entry.IsVanilla && entry.Builder == null)
            {
                return true;  // Go with vanilla method
            }
            else if (entry.IsUnknown)
            {
                __result = new LevelBlueprint();
                __result.CheckForConsistency();

                return false;  // This will either crash the game or softlock it
            }

            LevelBlueprint blueprint = new LevelBlueprint();

            blueprint.CheckForConsistency();

            try
            {
                entry.Builder?.Invoke(blueprint);
            }
            catch (Exception e)
            {
                Program.Logger.LogError("Builder threw an exception for level {enZoneToGet}! Exception: {e}", enZoneToGet, e);
                blueprint = new LevelBlueprint();
            }

            blueprint.CheckForConsistency(true);

            // Enforce certain values

            blueprint.enRegion = entry.WorldRegion;
            blueprint.enZone = entry.GameID;
            blueprint.sDefaultMusic = ""; // TODO Custom music
            blueprint.sDialogueFiles = ""; // TODO Dialogue Files
            blueprint.sMenuBackground = "bg01_mountainvillage"; // TODO Proper custom backgrounds. Transpiling _Level_Load is a good idea.
            blueprint.sZoneName = ""; // TODO Zone titles

            // Loader setup

            Loader.afCurrentHeightLayers = new float[blueprint.aiLayerDefaultHeight.Length];
            for (int i = 0; i < blueprint.aiLayerDefaultHeight.Length; i++)
                Loader.afCurrentHeightLayers[i] = blueprint.aiLayerDefaultHeight[i];

            Loader.lxCurrentSC = blueprint.lxInvisibleWalls;

            // Return from method

            __result = blueprint;
            return false;
        }
    }
}
