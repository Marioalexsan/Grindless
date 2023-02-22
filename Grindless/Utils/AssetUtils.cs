using HarmonyLib;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SoG;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static SoG.LordOfTextures;

namespace Grindless
{
    /// <summary>
    /// Provides helper methods for loading and unloading game assets.
    /// </summary>
    public static class AssetUtils
    {
        private static readonly FieldInfo s_disposableAssetsField = AccessTools.Field(typeof(ContentManager), "disposableAssets");

        private static readonly FieldInfo s_loadedAssetsField = AccessTools.Field(typeof(ContentManager), "loadedAssets");

        private static readonly MethodInfo s_getCleanPathMethod = AccessTools.Method(AccessTools.TypeByName("Microsoft.Xna.Framework.TitleContainer"), "GetCleanPath");

        /// <summary>
        /// Unloads a single asset from the given ContentManager.
        /// If the asset is found, it is disposed of, and removed from the ContentManager.
        /// </summary>
        /// <returns>True if asset was found and unloaded, false otherwise.</returns>
        public static bool Unload(this ContentManager manager, string path)
        {
            GetContentManagerFields(manager, out List<IDisposable> disposableAssets, out Dictionary<string, object> loadedAssets);

            var cleanPath = GetContentManagerCleanPath(path);

            if (loadedAssets.ContainsKey(cleanPath))
            {
                object asset = loadedAssets[cleanPath];

                loadedAssets.Remove(cleanPath);

                if (asset is IDisposable disposable)
                {
                    disposableAssets.Remove(disposable);
                    disposable.Dispose();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool UnloadIfModded(this ContentManager manager, string path)
        {
            if (ModUtils.IsModContentPath(path))
                return manager.Unload(path);

            return false;
        }

        public static T TryLoad<T>(this ContentManager manager, string path)
        where T : class
        {
            manager.TryLoad<T>(path, out T asset);
            return asset;
        }

        public static bool TryLoad<T>(this ContentManager manager, string path, out T asset)
            where T : class
        {
            try
            {
                asset = manager.Load<T>(path);
                return true;
            }
            catch
            {
                if (typeof(T) == typeof(Texture2D))
                    asset = GrindlessResources.NullTexture as T;

                else asset = null;
                
                return false;
            }
        }

        public static WaveBank TryLoadWaveBank(this AudioEngine engine, string path)
        {
            engine.TryLoadWaveBank(path, out WaveBank asset);
            return asset;
        }

        public static bool TryLoadWaveBank(this AudioEngine engine, string path, out WaveBank result)
        {
            try
            {
                result = new WaveBank(engine, path);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static SoundBank TryLoadSoundBank(this AudioEngine engine, string path)
        {
            engine.TryLoadSoundBank(path, out SoundBank asset);
            return asset;
        }

        public static bool TryLoadSoundBank(this AudioEngine engine, string path, out SoundBank result)
        {
            try
            {
                result = new SoundBank(engine, path);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }


        /// <summary>
        /// Experimental internal method that unloads all modded assets from a manager.
        /// Modded assets are assets for which <see cref="ModUtils.IsModContentPath(string)"/> returns true.
        /// </summary>
        internal static void UnloadModContentPathAssets(this ContentManager manager)
        {
            GetContentManagerFields(manager, out List<IDisposable> disposableAssets, out Dictionary<string, object> loadedAssets);

            foreach (var kvp in loadedAssets.Where(x => ModUtils.IsModContentPath(x.Key)).ToList())
            {
                loadedAssets.Remove(kvp.Key);

                if (kvp.Value is IDisposable disposable)
                {
                    disposableAssets.Remove(disposable);
                    disposable.Dispose();
                }
            }
        }

        private static void GetContentManagerFields(ContentManager manager, out List<IDisposable> disposableAssets, out Dictionary<string, object> loadedAssets)
        {
            disposableAssets = (List<IDisposable>)s_disposableAssetsField.GetValue(manager);

            loadedAssets = (Dictionary<string, object>)s_loadedAssetsField.GetValue(manager);
        }

        private static string GetContentManagerCleanPath(string path)
        {
            return (string)s_getCleanPathMethod.Invoke(null, new object[] { path });
        }
    }
}
