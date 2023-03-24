using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

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
            if (manager.IsModContentPath(path))
                return manager.Unload(path);

            return false;
        }

        public static T TryLoad<T>(this ContentManager manager, string path)
        where T : class
        {
            manager.TryLoad(path, out T asset);
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
            catch (Exception e)
            {
                if (typeof(T) == typeof(Texture2D))
                    asset = GrindlessResources.NullTexture as T;

                else asset = null;

                Program.Logger.LogWarning("Failed to load a {ResourceType}! Path: {Path}, Reason: {e}", typeof(T).Name, path, e.Message);

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
        /// Returns true if the mod path starts with "ModContent\", false otherwise.
        /// </summary>
        public static bool IsModContentPath(this ContentManager manager, string assetPath)
        {
            return assetPath != null && assetPath.Trim().Replace('/', '\\').StartsWith("ModContent\\");
        }

        /// <summary>
        /// Experimental internal method that unloads all modded assets from a manager.
        /// Modded assets are assets for which <see cref="ModUtils.IsModContentPath(string)"/> returns true.
        /// </summary>
        internal static void UnloadModContentPathAssets(this ContentManager manager)
        {
            GetContentManagerFields(manager, out List<IDisposable> disposableAssets, out Dictionary<string, object> loadedAssets);

            foreach (var kvp in loadedAssets.Where(x => manager.IsModContentPath(x.Key)).ToList())
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
