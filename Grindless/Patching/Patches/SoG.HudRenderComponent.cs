using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(HudRenderComponent))]
    internal static class SoG_HudRenderComponent
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(HudRenderComponent.GetBuffTexture))]
        internal static bool GetBuffTexture_Prefix(ref Texture2D __result, BaseStats.StatusEffectSource en)
        {
            var entry = StatusEffectEntry.Entries.Get(en); ;

            if (entry == null)
            {
                __result = RenderMaster.txNullTex;  // Unknown mod entry?
                return false;
            }

            if (entry.TexturePath == null)
            {
                if (entry.IsVanilla)
                {
                    __result = OriginalMethods.GetBuffTexture(Globals.Game.xHUD, en);
                    return false;
                }

                __result = GrindlessResources.NullTexture;  // Bad texture
                return false;
            }

            AssetUtils.TryLoadTexture(entry.TexturePath, Globals.Game.Content, out __result);
            return false;
        }
    }
}
