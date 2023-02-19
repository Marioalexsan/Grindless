using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Leaderboards_UploadEntryToSteamLeaderboards))]
    static class _Leaderboards_UploadEntryToSteamLeaderboards
    {
        [HarmonyPriority(Priority.First)]
        static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
