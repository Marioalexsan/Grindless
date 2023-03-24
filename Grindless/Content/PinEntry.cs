namespace Grindless
{
    /// <summary>
    /// Represents a modded pin from Arcade Mode.
    /// </summary>
    /// <remarks> 
    /// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
    /// </remarks>
    public class PinEntry : Entry<PinCodex.PinType>
    {
        internal static EntryManager<PinCodex.PinType, PinEntry> Entries { get; }
            = new EntryManager<PinCodex.PinType, PinEntry>(35000);

        /// <summary>
        /// Enumerates the available pin symbols.
        /// </summary>
        public enum Symbol
        {
            Bow = 0,
            Sword = 1,
            Star = 2,
            Shield = 3,
            UpArrow = 4,
            Potion = 5,
            Exclamation = 6
        }

        /// <summary>
        /// Enumerates the available pin shapes.
        /// </summary>
        public enum Shape
        {
            Circle = 0,
            Square = 1,
            Plus = 2,
            Tablet = 3,
            Diamond = 4
        }

        /// <summary>
        /// Enumerates the available pin colors.
        /// </summary>
        public enum Color
        {
            YellowOrange = 0,
            Seagull = 1,
            Coral = 2,
            Conifer = 3,
            BilobaFlower = 4,

            /// <summary>
            /// This color usually represents sticky pins.
            /// </summary>
            White = 1337,
        }

        /// <summary>
        /// Gets or sets the pin's symbol.
        /// </summary>
        public Symbol PinSymbol { get; set; }

        /// <summary>
        /// Gets or sets the pin's shape.
        /// </summary>
        public Shape PinShape { get; set; }

        /// <summary>
        /// Gets or sets the pin's color.
        /// </summary>
        public Color PinColor { get; set; }

        /// <summary>
        /// Gets or sets whenever this pin is sticky. <para/>
        /// Sticky pins appear as white, and cannot be unequipped.
        /// </summary>
        public bool IsSticky { get; set; }

        /// <summary>
        /// Gets or sets whenever this pin is broken. <para/>
        /// Broken pins have a cracked appearance.
        /// </summary>
        public bool IsBroken { get; set; }

        /// <summary>
        /// Gets or sets the pin's in-game description.
        /// </summary>
        public string Description { get; set; } = "Some modded pin that isn't very descriptive!";

        /// <summary>
        /// Gets or sets the condition for the pin to drop. <para/>
        /// If the method is not empty, and its return value is false,
        /// then pins of this type will be skipped when selecting a pin drop.
        /// For example, in vanilla, the three red smash balls pin can drop only if you have a two handed weapon equipped.
        /// </summary>
        public Func<bool> ConditionToDrop { get; set; }

        /// <summary>
        /// Gets or sets the action to run when this pin is equipped.
        /// </summary>
        public Action<PlayerView> EquipAction { get; set; }

        /// <summary>
        /// Gets or sets the action to run when this pin is unequipped.
        /// </summary>
        public Action<PlayerView> UnequipAction { get; set; }

        /// <summary>
        /// Gets or sets whenever to create a pin display in Traveller's pin collection.
        /// This is set to true by default.
        /// </summary>
        public bool CreateCollectionEntry { get; set; }

        internal PinEntry() { }

        internal PinEntry(Mod mod, string modID, PinCodex.PinType gameID)
            : base(mod, modID, gameID) { }

        protected override void Initialize()
        {
            if (CreateCollectionEntry)
            {
                PinCodex.SortedPinEntries.Add(GameID);
            }
        }

        protected override void Cleanup()
        {
            if (CreateCollectionEntry)
            {
                PinCodex.SortedPinEntries.Remove(GameID);
            }
        }
    }
}
