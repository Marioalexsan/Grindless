using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Extensions.Logging;

namespace Grindless
{
    /// <summary>
    /// The base class for all mods.
    /// </summary>
    /// <remarks>
    /// Mod DLLs need to have at one class that is derived from <see cref="Mod"/>. That class will be constructed by Grindless when loading.
    /// </remarks>
    public abstract class Mod
    {
        internal virtual bool IsBuiltin => false;

        /// <summary>
        /// Gets the name of the mod. <para/>
        /// The name of a mod is used as an identifier, and should be unique between different mods!
        /// </summary>
        public virtual string Name => GetType().Name;

        /// <summary>
        /// Gets the version of the mod.
        /// </summary>
        public virtual Version Version => new(0, 0);

        /// <summary>
        /// Gets whenever the mod should have object creation disabled. <para/>
        /// Mods that have object creation disabled can't use methods such as <see cref="CreateItem(string)"/>. <para/>
        /// Additionally, mod information won't be sent in multiplayer or written in save files.
        /// </summary>
        public virtual bool DisableObjectCreation => false;

        /// <summary>
        /// Gets the default logger for this mod.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the path to the mod's assets, relative to the "ModContent" folder.
        /// The default value is "ModContent/{NameID}".
        /// </summary>
        public string AssetPath => Path.Combine("ModContent", Name) + "/";

        /// <summary>
        /// Gets whenever the mod is currently being loaded.
        /// </summary>
        public bool InLoad => ModManager.CurrentlyLoadingMod == this;

        public Mod()
        {
            Logger = Program.LogFactory.CreateLogger(Name);
        }

        #region Virtual Methods

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

        #endregion

        #region Game Logic Callbacks

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

        #endregion

        #region Update Callbacks

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

        #endregion

        #region Game Object Creation Methods

        public AudioEntry CreateAudio() => AudioEntry.Entries.Create(this, "");

        public CommandEntry CreateCommands() => CommandEntry.Entries.Create(this, "");

        public CurseEntry CreateCurse(string modID) => CurseEntry.Entries.Create(this, modID);

        public EnemyEntry CreateEnemy(string modID) => EnemyEntry.Entries.Create(this, modID);

        public EquipmentEffectEntry CreateEquipmentEffect(string modID) => EquipmentEffectEntry.Entries.Create(this, modID);

        public ItemEntry CreateItem(string modID) => ItemEntry.Entries.Create(this, modID);

        public LevelEntry CreateLevel(string modID) => LevelEntry.Entries.Create(this, modID);

        public NetworkEntry CreateNetwork() => NetworkEntry.Entries.Create(this, "");

        public PerkEntry CreatePerk(string modID) => PerkEntry.Entries.Create(this, modID);

        public PinEntry CreatePin(string modID) => PinEntry.Entries.Create(this, modID);

        public QuestEntry CreateQuest(string modID) => QuestEntry.Entries.Create(this, modID);

        public SpellEntry CreateSpell(string modID) => SpellEntry.Entries.Create(this, modID);

        public StatusEffectEntry CreateStatusEffect(string modID) => StatusEffectEntry.Entries.Create(this, modID);

        public WorldRegionEntry CreateWorldRegion(string modID) => WorldRegionEntry.Entries.Create(this, modID);

        #endregion

        #region Game Object Getters

        /// <summary>
        /// Gets an active mod using its nameID.
        /// Returns null if the mod isn't currently loaded.
        /// </summary>
        /// <param name="nameID">The NameID of the mod.</param>
        /// <returns></returns>
        public Mod GetMod(string nameID)
        {
            return ModManager.Mods.FirstOrDefault(x => x.Name == nameID);
        }

        public AudioEntry GetAudio() => AudioEntry.Entries.Get(this, "");

        public CommandEntry GetCommands() => CommandEntry.Entries.Get(this, "");

        public CurseEntry GetCurse(string modID) => CurseEntry.Entries.Get(this, modID);

        public EnemyEntry GetEnemy(string modID) => EnemyEntry.Entries.Get(this, modID);

        public EquipmentEffectEntry GetEquipmentEffect(string modID) => EquipmentEffectEntry.Entries.Get(this, modID);

        public ItemEntry GetItem(string modID) => ItemEntry.Entries.Get(this, modID);

        public LevelEntry GetLevel(string modID) => LevelEntry.Entries.Get(this, modID);

        public NetworkEntry GetNetwork() => NetworkEntry.Entries.Get(this, "");

        public PerkEntry GetPerk(string modID) => PerkEntry.Entries.Get(this, modID);

        public PinEntry GetPin(string modID) => PinEntry.Entries.Get(this, modID);

        public QuestEntry GetQuest(string modID) => QuestEntry.Entries.Get(this, modID);

        public SpellEntry GetSpell(string modID) => SpellEntry.Entries.Get(this, modID);

        public StatusEffectEntry GetStatusEffect(string modID) => StatusEffectEntry.Entries.Get(this, modID);

        public WorldRegionEntry GetWorldRegion(string modID) => WorldRegionEntry.Entries.Get(this, modID);

        public void AddCraftingRecipe(ItemCodex.ItemTypes result, Dictionary<ItemCodex.ItemTypes, ushort> ingredients)
        {
            if (ModManager.CurrentlyLoadingMod != null)
                throw new InvalidOperationException(ErrorHelper.UseThisAfterLoad);

            if (ingredients == null)
                throw new ArgumentNullException(nameof(ingredients));

            if (!Crafting.CraftSystem.RecipeCollection.ContainsKey(result))
            {
                var kvps = new KeyValuePair<ItemDescription, ushort>[ingredients.Count];

                int index = 0;
                foreach (var kvp in ingredients)
                    kvps[index++] = new KeyValuePair<ItemDescription, ushort>(ItemCodex.GetItemDescription(kvp.Key), kvp.Value);

                ItemDescription description = ItemCodex.GetItemDescription(result);
                Crafting.CraftSystem.RecipeCollection.Add(result, new Crafting.CraftSystem.CraftingRecipe(description, kvps));
            }

            Program.Logger.LogInformation("Added recipe for item {result}!", result);
        }

        /// <summary>
        /// Sends a packet to the chosen Client if currently playing as a Server; otherwise, it does nothing.
        /// </summary>
        public void SendToClient(ushort packetID, Action<BinaryWriter> data, PlayerView receiver, SequenceChannel channel = SequenceChannel.PrioritySpells_20, NetDeliveryMethod reliability = NetDeliveryMethod.ReliableOrdered)
        {
            if (!NetUtils.IsServer)
                return;

            Globals.Game._Network_SendMessage(NetUtils.WriteModData(this, packetID, data), receiver, channel, reliability);
        }

        /// <summary>
        /// Sends a packet to all Clients, if currently playing as a Server; otherwise, it does nothing.
        /// </summary>
        public void SendToAllClients(ushort packetID, Action<BinaryWriter> data, SequenceChannel channel = SequenceChannel.PrioritySpells_20, NetDeliveryMethod reliability = NetDeliveryMethod.ReliableOrdered)
        {
            if (!NetUtils.IsServer)
                return;

            Globals.Game._Network_SendMessage(NetUtils.WriteModData(this, packetID, data), channel, reliability);
        }

        /// <summary>
        /// Sends a packet to all Clients, except one, if currently playing as a Server; otherwise, it does nothing.
        /// </summary>
        public void SendToAllClientsExcept(ushort packetID, Action<BinaryWriter> data, PlayerView excluded, SequenceChannel channel = SequenceChannel.PrioritySpells_20, NetDeliveryMethod reliability = NetDeliveryMethod.ReliableOrdered)
        {
            if (!NetUtils.IsServer)
                return;

            foreach (PlayerView view in Globals.Game.dixPlayers.Values)
            {
                if (view == excluded)
                    continue;

                Globals.Game._Network_SendMessage(NetUtils.WriteModData(this, packetID, data), channel, reliability);
            }
        }

        /// <summary>
        /// Sends a packet to the Server if currently playing as a Client; otherwise, it does nothing.
        /// </summary>
        public void SendToServer(ushort packetID, Action<BinaryWriter> data, SequenceChannel channel = SequenceChannel.PrioritySpells_20, NetDeliveryMethod reliability = NetDeliveryMethod.ReliableOrdered)
        {
            if (!NetUtils.IsClient)
                return;

            Globals.Game._Network_SendMessage(NetUtils.WriteModData(this, packetID, data), channel, reliability);
        }

        #endregion
    }
}