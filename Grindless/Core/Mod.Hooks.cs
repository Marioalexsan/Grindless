using Microsoft.Xna.Framework.Graphics;

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

    public class GameEvent
    {
        public bool WillExecute { get; private set; } = true;

        public void Prevent() => WillExecute = false;
    }

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
        public int Damage { get; init; }
        public byte Type { get; init; }
    }

    public virtual void PostEntityDamage(PostEntityDamageData data) { }

    public class OnEntityDeathData
    {
        public IEntity Entity { get; init; }
        public AttackPhase AttackPhase { get; init; }
        public bool WithEffect { get; set; }
    }

    public virtual void OnEntityDeath(OnEntityDeathData data) { }

    public class PostLevelLoadData
    {
        public Level.ZoneEnum Level { get; init; }
        public Level.WorldRegion Region { get; init; }
        public bool StaticOnly { get; init; }
    }

    public virtual void PostLevelLoad(PostLevelLoadData data) { }

    public class PostRenderTopGUIData
    {
        public SpriteBatch SpriteBatch { get; init; }
    }

    public virtual void PostRenderTopGUI(PostRenderTopGUIData data) { }
}
