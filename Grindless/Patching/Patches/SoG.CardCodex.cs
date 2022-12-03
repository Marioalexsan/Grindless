using HarmonyLib;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(CardCodex))]
    internal static class SoG_CardCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CardCodex.GetIllustrationPath))]
        public static bool GetIllustrationPath_Prefix(ref string __result, EnemyCodex.EnemyTypes enEnemy)
        {
            var entry = EnemyEntry.Entries.Get(enEnemy);

            if (entry != null)
            {
                __result = entry.cardIllustrationPath;
            }
            else
            {
                __result = "";
            }

            return false;
        }

    }
}
