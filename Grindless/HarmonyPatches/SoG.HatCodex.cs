using HarmonyLib;
using System.IO;
using SoG;
using Microsoft.Xna.Framework.Graphics;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(HatCodex))]
    static class SoG_HatCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(HatCodex.GetHatInfo))]
        static bool GetHatInfo_Prefix(ref HatInfo __result, ItemCodex.ItemTypes enType)
        {
            var entry = ItemEntry.Entries.Get(enType); ;

            __result = null;

            if (entry != null && entry.vanillaEquip is HatInfo info)
            {
                __result = info;

                string path = entry.EquipResourcePath;

                string[] directions = new string[] { "Up", "Right", "Down", "Left" };

                int index = -1;

                while (++index < 4)
                {
                    if (__result.xDefaultSet.atxTextures[index] == null)
                    {
                        if (path != null)
                        {
                            __result.xDefaultSet.atxTextures[index] = Globals.Game.Content.TryLoad<Texture2D>(Path.Combine(path, directions[index]));
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
                                altSet.atxTextures[index] = Globals.Game.Content.TryLoad<Texture2D>(Path.Combine(path, kvp.Value, directions[index]));
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
