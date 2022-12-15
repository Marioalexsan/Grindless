using HarmonyLib;
using System.IO;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(HatCodex))]
    static class SoG_HatCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(HatCodex.GetHatInfo))]
        static bool GetHatInfo_Prefix(ref HatInfo __result, ItemCodex.ItemTypes enType)
        {
            var entry = ItemEntry.Entries.Get(enType);;

            __result = null;

            if (entry != null && entry.vanillaEquip is HatInfo info)
            {
                __result = info;

                string path = entry.equipResourcePath;

                string[] directions = new string[] { "Up", "Right", "Down", "Left" };

                int index = -1;

                while (++index < 4)
                {
                    if (__result.xDefaultSet.atxTextures[index] == null)
                    {
                        if (path != null)
                        {
                            AssetUtils.TryLoadTexture(Path.Combine(path, directions[index]), Globals.Game.Content, out __result.xDefaultSet.atxTextures[index]);
                        }
                        else
                        {
                            __result.xDefaultSet.atxTextures[index] = GrindlessResources.NullTexture;
                        }
                    }
                }

                foreach (var kvp in entry.hatAltSetResourcePaths)
                {
                    index = -1;

                    while (++index < 4)
                    {
                        var altSet = __result.denxAlternateVisualSets[kvp.Key];

                        if (altSet.atxTextures[index] == null)
                        {
                            if (path != null && kvp.Value != null)
                            {
                                string altPath = Path.Combine(path, kvp.Value);
                                AssetUtils.TryLoadTexture(Path.Combine(altPath, directions[index]), Globals.Game.Content, out altSet.atxTextures[index]);
                            }
                            else
                            {
                                altSet.atxTextures[index] = GrindlessResources.NullTexture;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
