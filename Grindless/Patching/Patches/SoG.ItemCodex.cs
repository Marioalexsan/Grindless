using HarmonyLib;
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
                    AssetUtils.TryLoadTexture(entry.iconPath, Globals.Game.Content, out __result.txDisplayImage);
                }
                else
                {
                    if (__result.txDisplayImage == null)
                    {
                        __result.txDisplayImage = GrindlessResources.NullTexture;
                    }
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

            if (entry.iconPath != null)
            {
                AssetUtils.TryLoadTexture(entry.iconPath, Globals.Game.xLevelMaster.contRegionContent, out __result.xRenderComponent.txTexture);
            }
            else
            {
                if (__result.xRenderComponent.txTexture == null)
                {
                    __result.xRenderComponent.txTexture = entry.vanillaItem.txDisplayImage;
                }
            }

            if (entry.shadowPath != null)
            {
                AssetUtils.TryLoadTexture(entry.shadowPath, Globals.Game.xLevelMaster.contRegionContent, out __result.xRenderComponent.txShadowTexture);
            }
            else
            {
                if (__result.xRenderComponent.txShadowTexture == null)
                {
                    AssetUtils.TryLoadTexture("Items/DropAppearance/hartass02", Globals.Game.xLevelMaster.contRegionContent, out __result.xRenderComponent.txShadowTexture);
                }
            }
        }
    }
}
