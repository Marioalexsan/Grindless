using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(ItemCodex))]
    static class SoG_ItemCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ItemCodex.GetItemDescription))]
        static bool GetItemDescription_Prefix(ref ItemDescription __result, ItemCodex.ItemTypes enType)
        {
            var entry = ItemEntry.Entries.Get(enType);

            if (entry != null)
            {
                __result = entry.vanillaItem;

                if (entry.iconPath != null)
                {
                    __result.txDisplayImage = Globals.Game.Content.TryLoad<Texture2D>(entry.iconPath);
                }
                else
                {
                    __result.txDisplayImage ??= GrindlessResources.NullTexture;
                }
            }
            else
            {
                __result = new ItemDescription() { enType = enType };
            }

            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ItemCodex.GetItemInstance))]
        static void GetItemInstance_Postfix(ref Item __result, ItemCodex.ItemTypes enType)
        {
            var entry = ItemEntry.Entries.Get(enType);

            __result.enType = entry.vanillaItem.enType;
            __result.sFullName = entry.vanillaItem.sFullName;
            __result.bGiveToServer = entry.vanillaItem.lenCategory.Contains(ItemCodex.ItemCategories.GrantToServer);

            var manager = Globals.Game.xLevelMaster.contRegionContent;

            if (entry.iconPath != null)
            {
                __result.xRenderComponent.txTexture = manager.TryLoad<Texture2D>(entry.iconPath);
            }
            else
            {
                __result.xRenderComponent.txTexture ??= entry.vanillaItem.txDisplayImage;
            }

            if (entry.shadowPath != null)
            {
                __result.xRenderComponent.txShadowTexture = manager.TryLoad<Texture2D>(entry.shadowPath);
            }
            else
            {
                __result.xRenderComponent.txShadowTexture ??= manager.TryLoad<Texture2D>("Items/DropAppearance/hartass02");
            }
        }
    }
}
