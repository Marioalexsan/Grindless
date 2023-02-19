using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using SoG;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(HudRenderComponent))]
    static class SoG_HudRenderComponent
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(HudRenderComponent.GetBuffTexture))]
        static bool GetBuffTexture_Prefix(ref Texture2D __result, BaseStats.StatusEffectSource en)
        {
            var entry = StatusEffectEntry.Entries.GetRequired(en);

            if (entry.TexturePath == null && entry.IsVanilla)
                return true;

            __result = Globals.Game.Content.TryLoad<Texture2D>(entry.TexturePath);
            return false;
        }
    }
}
