using HarmonyLib;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(CardCodex))]
    static class SoG_CardCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CardCodex.GetIllustrationPath))]
        static bool GetIllustrationPath_Prefix(ref string __result, EnemyCodex.EnemyTypes enEnemy)
        {
            var entry = EnemyEntry.Entries.Get(enEnemy);

            if (entry != null)
            {
                __result = entry.CardIllustrationPath;
            }
            else
            {
                __result = "";
            }

            return false;
        }

    }
}
