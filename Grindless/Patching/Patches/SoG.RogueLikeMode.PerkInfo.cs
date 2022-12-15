using HarmonyLib;
using System.Linq;
using static SoG.HitEffectMap;
using PerkInfo = SoG.RogueLikeMode.PerkInfo;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(PerkInfo))]
    static class SoG_RogueLikeMode_PerkInfo
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PerkInfo.Init))]
        static bool InitPrefix()
        {
            PerkInfo.lxAllPerks.Clear();
            PerkInfo.lxAllPerks.AddRange(PerkEntry.Entries.Where(x => x.UnlockCondition?.Invoke() ?? true).Select(x => new PerkInfo(x.GameID, x.EssenceCost, x.TextEntry)));
            return false;
        }
    }
}
