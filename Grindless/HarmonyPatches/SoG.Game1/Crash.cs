namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), "Crash")]
    static class Crash
    {
        static bool Prefix()
        {
            Globals.Game.bExitingFromCrash = true;
            if (CAS.RecordingReplay != null)
            {
                CAS.RecordingReplay.RecordManualRunEnd();
                CAS.RecordingReplay.RecordPlayerDeath();
                Globals.Game._RogueLike_Replays_EndRecordingReplay();
                Globals.Game._RogueLike_Replays_SaveCrashReplay(CAS.RecordingReplay);
            }

            // Straight up rethrow errors to bubble exceptions up
            throw AccessTools.Field(typeof(Game1), "exceptionFatalError").GetValue(Globals.Game) as Exception;
        }
    }
}
