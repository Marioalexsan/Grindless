using Microsoft.Xna.Framework;
using static SoG.Level;

namespace Grindless;

public abstract partial class Mod
{
    /// <summary>
    /// Called when the mod is loaded. This is where all game stuff you want to use should be created.
    /// </summary>
    public abstract void Load();

    /// <summary>
    /// Called after all mods have been loaded.
    /// You can use this method to do some thing that you can't do in <see cref="Load"/>,
    /// such as getting audio IDs.
    /// </summary>
    public virtual void PostLoad() { }

    /// <summary>
    /// Called when the mod is unloaded. Use this method to clean up after your mod. <para/>
    /// For instance, you can undo Harmony patches, or revert changes to game data. <para/>
    /// Keep in mind that modded game objects such as Items are removed automatically.
    /// </summary>
    /// <remarks>
    /// Mods are unloaded in the inverse order that they were loaded in.
    /// </remarks>
    public abstract void Unload();

    public class OnEntityDamageData
    {
        public IEntity Entity { get; init; }
        public int Damage { get; set; }
        public byte Type { get; set; }
    }

    public virtual void OnEntityDamage(OnEntityDamageData data) { }

    public class PostEntityDamageData
    {
        public IEntity Entity { get; init; }
        public int Damage { get; set; }
        public byte Type { get; set; }
    }

    public virtual void PostEntityDamage(PostEntityDamageData data) { }

    public class PostLevelLoadData
    {
        public Level.ZoneEnum Level { get; init; }
        public Level.WorldRegion Region { get; init; }
        public bool StaticOnly { get; init; }
    }

    public virtual void PostLevelLoad(PostLevelLoadData data) { }
}
