using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using WeaponAssets;
using SoG;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(WeaponContentManager))]
    static class WeaponAssets_WeaponContentManager
    {
        /// <summary>
        /// Loads weapon assets for a mod entry.
        /// For entries that do not use the vanilal resource format, a shortened folder structure is used.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("LoadBatch", typeof(Dictionary<ushort, string>))] // Protected Method
        static bool LoadBatch_Prefix(ref Dictionary<ushort, string> dis, WeaponContentManager __instance)
        {
            var entry = ItemEntry.Entries.Get(__instance.enType);

            ErrorHelper.Assert(entry != null, ErrorHelper.UnknownEntry);

            bool oneHanded = (entry.vanillaEquip as WeaponInfo).enWeaponCategory == WeaponInfo.WeaponCategory.OneHanded;

            string resourcePath = entry.equipResourcePath;

            foreach (KeyValuePair<ushort, string> kvp in dis)
            {
                string texPath = kvp.Value;

                if (!entry.useVanillaResourceFormat)
                {
                    texPath = texPath.Replace($"Weapons/{resourcePath}/", "");

                    if (oneHanded)
                    {
                        texPath = texPath
                            .Replace("Sprites/Heroes/OneHanded/", resourcePath + "/")
                            .Replace("Sprites/Heroes/Charge/OneHand/", resourcePath + "/1HCharge/");
                    }
                    else
                    {
                        texPath = texPath
                            .Replace("Sprites/Heroes/TwoHanded/", resourcePath + "/")
                            .Replace("Sprites/Heroes/Charge/TwoHand/", resourcePath + "/2HCharge/");
                    }
                }

                __instance.ditxWeaponTextures.Add(kvp.Key, __instance.contWeaponContent.TryLoad<Texture2D>(texPath));
            }

            return false;
        }
    }
}
