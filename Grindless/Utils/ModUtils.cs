using System;
using System.IO;

namespace Grindless
{
    /// <summary>
    /// Provides various helper methods.
    /// </summary>
    public static class ModUtils
    {
        /// <summary>
        /// Returns true if the mod path starts with "ModContent\", false otherwise.
        /// </summary>
        public static bool IsModContentPath(string assetPath)
        {
            return assetPath != null && assetPath.Trim().Replace('/', '\\').StartsWith("ModContent\\");
        }

        /// <summary>
        /// Splits a message in words, removing any empty results.
        /// </summary>
        public static string[] GetArgs(string message)
        {
            return message == null ? new string[0] : message.Split(new char[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Finds and replaces commonly occuring game paths with shortened forms.
        /// </summary>
        public static string ShortenModPaths(string path)
        {
            return path
                .Replace('/', '\\')
                .Replace(Directory.GetCurrentDirectory() + @"\Content\ModContent", "(ModContent)")
                .Replace(Directory.GetCurrentDirectory() + @"\Content\Mods", "(Mods)")
                .Replace(Directory.GetCurrentDirectory() + @"\Content", "(Content)")
                .Replace(Directory.GetCurrentDirectory(), "(SoG)");
        }
    }
}
