using Microsoft.Xna.Framework.Content;

namespace Grindless;

/// <summary>
/// Represents a modded world region.
/// </summary>
[ModEntry(650)]
public class WorldRegionEntry : Entry<Level.WorldRegion>
{
    internal WorldRegionEntry() { }

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
