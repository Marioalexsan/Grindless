using Microsoft.Xna.Framework.Content;
using SoG;

namespace Grindless
{
    /// <summary>
    /// Represents a modded world region.
    /// </summary>
    public class WorldRegionEntry : Entry<Level.WorldRegion>
    {
        internal static EntryManager<Level.WorldRegion, WorldRegionEntry> Entries { get; } 
            = new EntryManager<Level.WorldRegion, WorldRegionEntry>(650);

        internal WorldRegionEntry() { }

        internal WorldRegionEntry(Mod mod, string modID, Level.WorldRegion gameID)
            : base(mod, modID, gameID) { }

        protected override void Initialize()
        {
            var content = Globals.Game.Content;

            Globals.Game.xLevelMaster.denxRegionContent.Add(GameID, new ContentManager(content.ServiceProvider, content.RootDirectory));
        }

        protected override void Cleanup()
        {
            Globals.Game.xLevelMaster.denxRegionContent.TryGetValue(GameID, out var manager);

            manager?.Unload();

            Globals.Game.xLevelMaster.denxRegionContent.Remove(GameID);
        }
    }
}
