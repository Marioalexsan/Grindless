using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_GetTreatCurseInfo))]
    static class _RogueLike_GetTreatCurseInfo
    {
        static bool Prefix(RogueLikeMode.TreatsCurses enTreatCurse, out string sNameHandle, out string sDescriptionHandle, out float fScoreModifier)
        {
            var entry = CurseEntry.Entries.Get(enTreatCurse);

            // Null check covers the cases where the treat / curse is None
            sNameHandle = entry?.nameHandle ?? "";
            sDescriptionHandle = entry?.descriptionHandle ?? "";
            fScoreModifier = entry?.ScoreModifier ?? 0f;
            return false;
        }
    }
}
