using HarmonyLib;
using PerkInfo = SoG.RogueLikeMode.PerkInfo;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(PerkInfo))]
    internal static class SoG_RogueLikeMode_PerkInfo
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PerkInfo.Init))]
        internal static bool InitPrefix()
        {
            PerkInfo.lxAllPerks.Clear();

            foreach (var entry in PerkEntry.Entries)
            {
                if (entry.UnlockCondition == null || entry.UnlockCondition.Invoke())
                {
                    PerkInfo.lxAllPerks.Add(new PerkInfo(entry.GameID, entry.EssenceCost, entry.TextEntry));
                }
            }

            return false;
        }
    }
}
