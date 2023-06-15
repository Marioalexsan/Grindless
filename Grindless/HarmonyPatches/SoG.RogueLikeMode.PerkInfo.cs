using PerkInfo = SoG.RogueLikeMode.PerkInfo;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(PerkInfo))]
static class SoG_RogueLikeMode_PerkInfo
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PerkInfo.Init))]
    static bool InitPrefix()
    {
        PerkInfo.lxAllPerks.Clear();
        PerkInfo.lxAllPerks.AddRange(
            Entries.Perks
                .Where(x => x.UnlockCondition?.Invoke() ?? true)
                .Select(x => new PerkInfo(x.GameID, x.EssenceCost, x.TextEntry))
            );
        return false;
    }
}
