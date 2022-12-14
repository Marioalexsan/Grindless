using Microsoft.Xna.Framework.Graphics;
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

            using (MemoryStream stream = new MemoryStream(Resources.Resources.NullTexGS))
            {
                NullTexture = Texture2D.FromStream(Globals.Game.GraphicsDevice, stream);
            }

            using (MemoryStream stream = new MemoryStream(Resources.Resources.ModList))
            {
                ModList = Texture2D.FromStream(Globals.Game.GraphicsDevice, stream);
            }
            
            using (MemoryStream stream = new MemoryStream(Resources.Resources.ModMenu))
            {
                ModMenu = Texture2D.FromStream(Globals.Game.GraphicsDevice, stream);
            }

            using (MemoryStream stream = new MemoryStream(Resources.Resources.ReloadMods))
            {
                ReloadMods = Texture2D.FromStream(Globals.Game.GraphicsDevice, stream);
            }
        }

        public static Texture2D NullTexture { get; private set; }
        public static Texture2D ModList { get; private set; }
        public static Texture2D ModMenu { get; private set; }
        public static Texture2D ReloadMods { get; private set; }
    }
}
