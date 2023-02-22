using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Grindless
{
    public static class GrindlessResources
    {
        public static void ReloadResources()
        {
            NullTexture?.Dispose();
            NullTexture = null;

            ModList?.Dispose();
            ModList = null;

            ModMenu?.Dispose();
            ModMenu = null;

            ReloadMods?.Dispose();
            ReloadMods = null;

            using (MemoryStream stream = new(Resources.Resources.NullTexGS))
            {
                NullTexture = Texture2D.FromStream(Globals.Game.GraphicsDevice, stream)
                    ?? throw new InvalidOperationException("Failed to load a resource.");
            }

            using (MemoryStream stream = new(Resources.Resources.ModList))
            {
                ModList = Texture2D.FromStream(Globals.Game.GraphicsDevice, stream)
                    ?? throw new InvalidOperationException("Failed to load a resource.");
            }
            
            using (MemoryStream stream = new(Resources.Resources.ModMenu))
            {
                ModMenu = Texture2D.FromStream(Globals.Game.GraphicsDevice, stream)
                    ?? throw new InvalidOperationException("Failed to load a resource.");
            }

            using (MemoryStream stream = new(Resources.Resources.ReloadMods))
            {
                ReloadMods = Texture2D.FromStream(Globals.Game.GraphicsDevice, stream)
                    ?? throw new InvalidOperationException("Failed to load a resource.");
            }
        }

        public static Texture2D NullTexture { get; private set; }
        public static Texture2D ModList { get; private set; }
        public static Texture2D ModMenu { get; private set; }
        public static Texture2D ReloadMods { get; private set; }
    }
}
