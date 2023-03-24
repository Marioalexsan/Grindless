using Microsoft.Xna.Framework.Graphics;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(ItemCodex))]
    static class SoG_ItemCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ItemCodex.GetItemDescription))]
        static bool GetItemDescription_Prefix(ref ItemDescription __result, ItemCodex.ItemTypes enType)
        {
            var entry = ItemEntry.Entries.Get(enType);

            if (entry == null)
            {
                // Handle unknown / unloaded items
                __result = new ItemDescription() { enType = enType };
                return false;
            }

            __result = entry.vanillaItem;

            if (entry.IconPath != null)
                __result.txDisplayImage = Globals.Game.Content.TryLoad<Texture2D>(entry.IconPath);

            __result.txDisplayImage ??= GrindlessResources.NullTexture;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ItemCodex.GetItemInstance))]
        static void GetItemInstance_Postfix(ref Item __result, ItemCodex.ItemTypes enType)
        {
            var entry = ItemEntry.Entries.Get(enType);

            if (entry == null)
                return;

            __result.enType = enType;
            __result.sFullName = entry.vanillaItem.sFullName;
            __result.bGiveToServer = entry.vanillaItem.lenCategory.Contains(ItemCodex.ItemCategories.GrantToServer);

            var manager = Globals.Game.xLevelMaster.contRegionContent;

            if (entry.IconPath != null)
                __result.xRenderComponent.txTexture = manager.TryLoad<Texture2D>(entry.IconPath);

            __result.xRenderComponent.txTexture ??= entry.vanillaItem.txDisplayImage;

            if (entry.ShadowPath != null)
                __result.xRenderComponent.txShadowTexture = manager.TryLoad<Texture2D>(entry.ShadowPath);

            __result.xRenderComponent.txShadowTexture ??= manager.TryLoad<Texture2D>("Items/DropAppearance/hartass02");
        }
    }
}
