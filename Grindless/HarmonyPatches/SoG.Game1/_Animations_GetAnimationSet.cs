using HarmonyLib;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Animations_GetAnimationSet), typeof(PlayerView), typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(bool))]
    static class _Animations_GetAnimationSet
    {
        static bool Prefix(PlayerView xPlayerView, string sAnimation, string sDirection, bool bWithWeapon, bool bWithShield, bool bWeaponOnTop, ref PlayerAnimationTextureSet __result)
        {
            ContentManager VanillaContent = RenderMaster.contPlayerStuff;

            __result = new PlayerAnimationTextureSet
            {
                bWeaponOnTop = bWeaponOnTop,
                txBase = VanillaContent.TryLoad<Texture2D>($"Sprites/Heroes/{sAnimation}/{sDirection}")
            };

            if (bWithShield)
            {
                EquipmentInfo shield = xPlayerView.xEquipment.DisplayShield;
                ItemEntry entry = null;

                if (shield != null)
                {
                    entry = ItemEntry.Entries.Get(shield.enItemType);
                }

                if (entry == null)
                {
                    __result.txShield = GrindlessResources.NullTexture;
                }
                else
                {
                    var pathToUse = entry.UseVanillaResourceFormat ?
                        $"Sprites/Heroes/{sAnimation}/Shields/{shield.sResourceName}/{sDirection}" :
                        $"{shield.sResourceName}/{sAnimation}/{sDirection}";

                    __result.txShield = VanillaContent.TryLoad<Texture2D>(pathToUse);
                }
            }

            if (bWithWeapon)
                __result.txWeapon = RenderMaster.txNullTex;

            return false; // Never executes the original
        }
    }
}
