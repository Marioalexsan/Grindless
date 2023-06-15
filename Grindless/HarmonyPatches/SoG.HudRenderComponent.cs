using Microsoft.Xna.Framework.Graphics;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(HudRenderComponent))]
static class SoG_HudRenderComponent
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(HudRenderComponent.GetBuffTexture))]
    static bool GetBuffTexture_Prefix(ref Texture2D __result, BaseStats.StatusEffectSource en)
    {
        var entry = Entries.StatusEffects.GetRequired(en);

        if (entry.TexturePath == null && entry.IsVanilla)
            return true;

        __result = string.IsNullOrEmpty(entry.TexturePath) ? RenderMaster.txNullTex : Globals.Game.Content.TryLoad<Texture2D>(entry.TexturePath);
        return false;
    }
}
