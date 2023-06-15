namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(CardCodex))]
static class SoG_CardCodex
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CardCodex.GetIllustrationPath))]
    static bool GetIllustrationPath_Prefix(ref string __result, EnemyCodex.EnemyTypes enEnemy)
    {
        __result = Entries.Enemies.GetRequired(enEnemy).CardIllustrationPath;
        return false;
    }
}
