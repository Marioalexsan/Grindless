using Microsoft.Xna.Framework;

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

    /// <summary>
    /// Called when character files (".cha") need saving.
    /// You can write your Story Mode character-related information into the given stream.
    /// </summary>
    /// <param name="stream">A stream where you write the mod data.</param>
    public virtual void SaveCharacterData(BinaryWriter stream) { }

    /// <summary>
    /// Called when character files (".cha") need loading.
    /// You can load your previously written data from this stream.
    /// The method must also handle cases where the stream is empty, or incomplete.
    /// </summary>
    /// <param name="stream">A stream that contains the mod data.</param>
    /// <param name="saveVersion">The version that the mod had when the data was saved. You can use this to parse data based on mod version.</param>
    public virtual void LoadCharacterData(BinaryReader stream, Version saveVersion) { }

    /// <summary>
    /// Called when world files (".wld") need saving.
    /// You can write your Story Mode world-related information into the given stream.
    /// </summary>
    /// <param name="stream">A stream where you write the mod data.</param>
    public virtual void SaveWorldData(BinaryWriter stream) { }

    /// <summary>
    /// Called when world files (".wld") need loading.
    /// You can load your previously written data from this stream.
    /// The method must also handle cases where the stream is empty, or incomplete.
    /// </summary>
    /// <param name="stream">A stream that contains the mod data.</param>
    /// <param name="saveVersion">The version that the mod had when the data was saved. You can use this to parse data based on mod version.</param>
    public virtual void LoadWorldData(BinaryReader stream, Version saveVersion) { }

    /// <summary>
    /// Called when arcade files (".sav") need saving.
    /// You can write your Arcade-related information into the given stream.
    /// </summary>
    /// <param name="stream">A stream where you write the mod data.</param>
    public virtual void SaveArcadeData(BinaryWriter stream) { }

    /// <summary>
    /// Called when arcade files (".sav") need loading.
    /// You can load your previously written data from this stream.
    /// The method must also handle cases where the stream is empty, or incomplete.
    /// </summary>
    /// <param name="stream">A stream that contains the mod data.</param>
    /// <param name="saveVersion">The version that the mod had when the data was saved. You can use this to parse data based on mod version.</param>
    public virtual void LoadArcadeData(BinaryReader stream, Version saveVersion) { }

    /// <summary>
    /// Called when a player is damaged by something.
    /// </summary>
    public virtual void OnPlayerDamaged(PlayerView player, ref int damage, ref byte type) { }

    /// <summary>
    /// Called when a player dies.
    /// </summary>
    public virtual void OnPlayerKilled(PlayerView player) { }

    /// <summary>
    /// Called after a player levels up.
    /// During save file loading, this method is called multiple times to initialize the player's stats to their level.
    /// </summary>
    public virtual void PostPlayerLevelUp(PlayerView player) { }

    /// <summary>
    /// Called when an enemy is damaged by something.
    /// </summary>
    public virtual void OnEnemyDamaged(Enemy enemy, ref int damage, ref byte type) { }

    /// <summary>
    /// Called when an Enemy dies.
    /// </summary>
    public virtual void PostEnemyKilled(Enemy enemy, AttackPhase killer) { }

    /// <summary>
    /// Called when an NPC is damaged by something.
    /// </summary>
    public virtual void OnNPCDamaged(NPC enemy, ref int damage, ref byte type) { }

    /// <summary>
    /// Called when a player interacts with an NPC
    /// </summary>
    public virtual void OnNPCInteraction(NPC npc) { }

    /// <summary>
    /// Called when the game loads Arcadia's level.
    /// </summary>
    public virtual void OnArcadiaLoad() { }

    /// <summary>
    /// Called when a new Arcade room is entered, after it has been prepared by the game
    /// (i.e. enemies have been spawned, traps laid out, etc.)
    /// </summary>
    public virtual void PostArcadeRoomStart() { }

    /// <summary>
    /// Called when an Arcade room has completed (if applicable).
    /// </summary>
    public virtual void PostArcadeRoomComplete() { }

    /// <summary>
    /// Called when an Enemy has been spawned as part of an Arcade Gauntlet.
    /// </summary>
    public virtual void PostArcadeGauntletEnemySpawned(Enemy enemy) { }

    /// <summary>
    /// Called when a player uses an item. This method can be used to implement behavior for usable items.
    /// Items can be used if they have the "Usable" item category.
    /// </summary>
    public virtual void OnItemUse(ItemCodex.ItemTypes enItem, PlayerView xView, ref bool bSend) { }

    /// <summary>
    /// Called after a spell charge started.
    /// </summary>
    public virtual void PostSpellActivation(PlayerView xView, ISpellActivation xact, SpellCodex.SpellTypes enType, int iBoostState) { }

    /// <summary>
    /// Called after a level was loaded.
    /// </summary>
    public virtual void PostLevelLoad(Level.ZoneEnum level, Level.WorldRegion region, bool staticOnly) { }

    /// <summary>
    /// Called before an enemy is created. You can edit the parameters before returning.
    /// </summary>
    public virtual void OnEnemySpawn(ref EnemyCodex.EnemyTypes enemy, ref Vector2 position, ref bool isElite, ref bool dropsLoot, ref int bitLayer, ref float virtualHeight, float[] behaviourVariables) { }

    /// <summary>
    /// Called after an enemy is created. If the enemy type was changed via prefix callback, the original type can be retrieved from original.
    /// </summary>
    public virtual void PostEnemySpawn(Enemy entity, EnemyCodex.EnemyTypes enemy, EnemyCodex.EnemyTypes original, Vector2 position, bool isElite, bool dropsLoot, int bitLayer, float virtualHeight, float[] behaviourVariables) { }

    /// <summary>
    /// Called before stats are updated for an entity. 
    /// You can query the entity using <see cref="BaseStats.xOwner"/>.
    /// </summary>
    /// <param name="stats"> The entity's stats. </param>
    public virtual void OnBaseStatsUpdate(BaseStats stats) { }

    /// <summary>
    /// Called after stats are updated for an entity. 
    /// You can query the entity using <see cref="BaseStats.xOwner"/>.
    /// </summary>
    /// <param name="stats"> The entity's stats. </param>
    public virtual void PostBaseStatsUpdate(BaseStats stats) { }
}
