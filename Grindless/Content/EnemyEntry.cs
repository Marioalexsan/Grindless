using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Grindless
{
    /// <summary>
    /// Represents a modded enemy, and defines ways to create it.
    /// </summary>
    /// <remarks> 
    /// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
    /// </remarks>
    public class EnemyEntry : Entry<EnemyCodex.EnemyTypes>
    {
        internal static EntryManager<EnemyCodex.EnemyTypes, EnemyEntry> Entries { get; }
            = new EntryManager<EnemyCodex.EnemyTypes, EnemyEntry>(400000);

        /// <summary>
        /// Holds the item and drop chance pair inside enemy loot tables.
        /// </summary>
        public struct Drop
        {
            public float Chance;

            public ItemCodex.ItemTypes Item;

            public Drop(float chance, ItemCodex.ItemTypes item)
            {
                Chance = chance;
                Item = item;
            }
        }

        internal EnemyDescription Vanilla { get; set; } = new EnemyDescription(EnemyCodex.EnemyTypes.Null, "", 1, 100);

        public string Name
        {
            get => Vanilla.sFullName;
            set => Vanilla.sFullName = value;
        }

        /// <summary>
        /// Displayed when selecting an enemy description in the enemy menu.
        /// </summary>
        public string ShortDescription
        {
            get => Vanilla.sFlavorText;
            set => Vanilla.sFlavorText = value;
        }

        /// <summary>
        /// Displayed when reading an enemy's details from the enemy menu.
        /// </summary>
        public string LongDescription
        {
            get => Vanilla.sDetailedDescription;
            set => Vanilla.sDetailedDescription = value;
        }

        /// <summary>
        /// The base health represents the health of the monster on difficulty 0
        /// (Story's Normal, Arcade's "No Catalysts"), as a non-elite enemy.
        /// </summary>
        public int BaseHealth
        {
            get => Vanilla.iMaxHealth;
            set => Vanilla.iMaxHealth = value;
        }

        /// <summary>
        /// Players gain experience based on the player - enemy level difference.
        /// This is also displayed in the enemy codex when viewing an enemy's details.
        /// </summary>
        public int Level
        {
            get => Vanilla.iLevel;
            set => Vanilla.iLevel = value;
        }

        /// <summary>
        /// Effect name of the audio (if any) that this enemy emits on hit.
        /// </summary>
        public string OnHitSound
        {
            get => Vanilla.sOnHitSound;
            set => Vanilla.sOnHitSound = value;
        }

        /// <summary>
        /// The effect name of the audio (if any) that this enemy emits on death.
        /// </summary>
        public string OnDeathSound
        {
            get => Vanilla.sOnDeathSound;
            set => Vanilla.sOnDeathSound = value;
        }

        /// <summary>
        /// If set to true, an entry will appear for this monster inside the enemy menu.
        /// </summary>
        public bool CreateJournalEntry { get; set; }

        /// <summary>
        /// The callback should return an animation to use when viewing the enemy's details.
        /// </summary>
        public Func<ContentManager, Animation> DefaultAnimation { get; set; }

        /// <summary>
        /// The texture is used as background for the enemy animation when reading the enemy's details.
        /// </summary>
        public string DisplayBackgroundPath { get; set; }

        /// <summary>
        /// The texture is used as the icon for the enemy entry.
        /// </summary>
        public string DisplayIconPath { get; set; }

        /// <summary>
        /// Some game elements (such as Guardian shields) use this field to position themselves relative to
        /// the enemy center.
        /// </summary>
        public Vector2 ApproximateCenter
        {
            get => Vanilla.v2ApproximateOffsetToMid;
            set => Vanilla.v2ApproximateOffsetToMid = value;
        }

        /// <summary>
        /// Some game elements (such as Guardian shields) use this field to position themselves based on the
        /// enemy's size.
        /// </summary>
        public Vector2 ApproximateSize
        {
            get => Vanilla.v2ApproximateSize;
            set => Vanilla.v2ApproximateSize = value;
        }

        /// <summary>
        /// Setting this to anything other than Null will cause the enemy to drop that card 
        /// instead, and will remove the enemy's entry from the card menu.
        /// </summary>
        public EnemyCodex.EnemyTypes CardDropOverride
        {
            get => Vanilla.enCardTypeOverride;
            set => Vanilla.enCardTypeOverride = value;
        }

        /// <summary>
        /// A value of 100f corresponds to 100%.
        /// A value of 0f disables card drops. It will also remove the enemy's entry from the card menu.
        /// </summary>
        public float CardDropChance
        {
            get => Vanilla.iCardDropChance != 0 ? (int)(100f / Vanilla.iCardDropChance) : 0f;
            set => Vanilla.iCardDropChance = value != 0f ? (int)(100f / value) : 0;
        }

        /// <summary>
        /// Gets or sets the description of the card's effects.
        /// </summary>
        public string CardInfo
        {
            get => Vanilla.sCardDescription;
            set => Vanilla.sCardDescription = value;
        }

        /// <summary>
        /// This is the artwork that gets displayed when viewing a card.
        /// </summary>
        public string CardIllustrationPath { get; set; } = "GUI/InGameMenu/Journal/CardAlbum/Cards/placeholder";

        /// <summary>
        /// You can edit the list to add new drops.
        /// </summary>
        public List<Drop> LootTable { get; } = new List<Drop>();

        /// <summary>
        /// The constructor is used to initialize an enemy once it has been created.
        /// </summary>
        public EnemyBuilder Constructor { get; set; }

        /// <summary>
        /// The constructor is used to update the enemy's stats or behavior when the difficulty changes.
        /// This method is also called after the enemy is spawned.
        /// </summary>
        public EnemyBuilder DifficultyScaler { get; set; }

        /// <summary>
        /// The constructor is used to update the enemy's stats or behavior when it becomes elite.
        /// This method is also called after the enemy is spawned.
        /// </summary>
        public EnemyBuilder EliteScaler { get; set; }

        /// <summary>
        /// Sets if this enemy is regular, miniboss or a boss enemy.
        /// </summary>
        public EnemyDescription.Category Category
        {
            get => Vanilla.enCategory;
            set => Vanilla.enCategory = value;
        }

        internal EnemyEntry() { }

        protected override void Initialize()
        {
            Vanilla.enType = GameID;

            // Add a Card entry in the Journal
            if (Vanilla.iCardDropChance != 0)
            {
                if (Vanilla.enCardTypeOverride == EnemyCodex.EnemyTypes.Null)
                {
                    EnemyCodex.lxSortedCardEntries.Add(Vanilla);
                }
                else
                {
                    EnemyDescription desc = EnemyCodex.GetEnemyDescription(Vanilla.enCardTypeOverride);

                    if (!EnemyCodex.lxSortedCardEntries.Contains(desc))
                    {
                        EnemyCodex.lxSortedCardEntries.Add(desc);
                    }
                }
            }

            // Add an Enemy entry in the Journal
            if (CreateJournalEntry)
            {
                EnemyCodex.lxSortedDescriptions.Add(Vanilla);
            }

            // Add drops
            Vanilla.lxLootTable.AddRange(LootTable.Select(x => new DropChance((int)(x.Chance * 1000f), x.Item, 1)));

            Globals.Game.EXT_AddMiscText("Enemies", Vanilla.sNameLibraryHandle, Vanilla.sFullName);
            Globals.Game.EXT_AddMiscText("Enemies", Vanilla.sFlavorLibraryHandle, Vanilla.sFlavorText);
            Globals.Game.EXT_AddMiscText("Enemies", Vanilla.sCardDescriptionLibraryHandle, Vanilla.sCardDescription);
            Globals.Game.EXT_AddMiscText("Enemies", Vanilla.sDetailedDescriptionLibraryHandle, Vanilla.sDetailedDescription);
        }

        protected override void Cleanup()
        {
            // Enemy instances have their assets cleared due to using the world region content manager

            EnemyCodex.lxSortedCardEntries.Remove(Vanilla);
            EnemyCodex.lxSortedDescriptions.Remove(Vanilla);

            Globals.Game.EXT_RemoveMiscText("Enemies", Vanilla.sNameLibraryHandle);
            Globals.Game.EXT_RemoveMiscText("Enemies", Vanilla.sFlavorLibraryHandle);
            Globals.Game.EXT_RemoveMiscText("Enemies", Vanilla.sCardDescriptionLibraryHandle);
            Globals.Game.EXT_RemoveMiscText("Enemies", Vanilla.sDetailedDescriptionLibraryHandle);

            // Enemy Codex textures only load into InGameMenu.contTempAssetManager
            // We unload contTempAssetManager as part of mod reloading procedure
        }
    }
}
