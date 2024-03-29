﻿using Microsoft.Xna.Framework.Graphics;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(FacegearCodex))]
static class SoG_FacegearCodex
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(FacegearCodex.GetHatInfo))]
    static bool GetHatInfo_Prefix(ref FacegearInfo __result, ItemCodex.ItemTypes enType)
    {
        var entry = Entries.Items.Get(enType);

        __result = entry?.vanillaEquip as FacegearInfo;

        if (__result != null)
        {
            string path = entry.EquipResourcePath;

            if (entry.UseVanillaResourceFormat)
            {
                path = Path.Combine("Sprites/Equipment/Facegear/", path);
            }

            string[] directions = new string[] { "Up", "Right", "Down", "Left" };

            int index = -1;
            while (++index < 4)
            {
                if (__result.atxTextures[index] == null)
                {
                    if (!entry.FacegearSides[index])
                    {
                        __result.atxTextures[index] = RenderMaster.txNullTex;
                    }
                    else if (path != null)
                    {
                        __result.atxTextures[index] = Globals.Game.Content.TryLoad<Texture2D>(Path.Combine(path, directions[index]));
                    }
                    else if (__result.atxTextures[index] == null)
                    {
                        __result.atxTextures[index] = GrindlessResources.NullTexture;
                    }
                }
            }
        }
        else if (entry?.vanillaEquip is HatInfo hat)
        {
            // Hacky way from Teddy to render double slotted masks
            __result = new FacegearInfo(enType)
            {
                xItemDescription = hat.xItemDescription,
                atxTextures = new Texture2D[]
                {
                    RenderMaster.txNullTex,
                    RenderMaster.txNullTex,
                    RenderMaster.txNullTex,
                    RenderMaster.txNullTex,
                }
            };
        }

        return false;
    }
}
