namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Skill_ActivateSkill))]
static class _Skill_ActivateSkill
{
    static void Postfix(PlayerView xView, ISpellActivation xact, SpellCodex.SpellTypes enType, int iBoostState)
    {
        foreach (Mod mod in ModManager.Mods)
            mod.PostSpellActivation(xView, xact, enType, iBoostState);
    }
}
