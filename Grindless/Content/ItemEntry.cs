using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Stat = SoG.EquipmentInfo.StatEnum;

namespace Grindless;

/// <summary>
/// Represents a modded enemy, and defines ways to create it.
/// </summary>
/// <remarks> 
/// Equipment data is only used if the item acts as an equipment.
/// For example, setting weapon data isn't needed for facegear. <para/>
/// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
/// </remarks>
[ModEntry(700000)]
public class ItemEntry : Entry<ItemCodex.ItemTypes>
{
    internal ItemEntry() { }

    /// <summary>
    /// Provides methods to configure a hat's visual set.
    /// The arrays in this class are arranged based on direction: Up, Right, Down and Left.
    /// </summary>
    public class VisualSetConfig
    {
        internal ItemEntry entry;
        internal ItemCodex.ItemTypes comboItem;
        internal HatInfo.VisualSet visualSet;

        /// <summary>
        /// Gets an array of booleans that toggle hair overlap for each direction.
        /// </summary>
        public bool[] HatUnderHair => visualSet.abUnderHair;

        /// <summary>
        /// Gets an array of booleans that toggle player overlap for each direction.
        /// </summary>
        public bool[] HatBehindPlayer => visualSet.abBehindCharacter;

        /// <summary>
        /// Gets an array of four vectors that define the sprite displacement for each direction.
        /// </summary>
        public Vector2[] HatOffsets => visualSet.av2RenderOffsets;

        /// <summary>
        /// Gets an array of four booleans that toggle hat overlap for hair "sides", for each direction.
        /// </summary>
        public bool ObstructHairSides
        {
            get => visualSet.bObstructsSides;
            set => visualSet.bObstructsSides = value;
        }

        /// <summary>
        /// Gets an array of four booleans that toggle hat overlap for hair "tops", for each direction.
        /// </summary>
        public bool ObstructHairTop
        {
            get => visualSet.bObstructsSides;
            set => visualSet.bObstructsSides = value;
        }

        /// <summary>
        /// Gets an array of four booleans that toggle hat overlap for hair "bottoms", for each direction.
        /// </summary>
        public bool ObstructHairBottom
        {
            get => visualSet.bObstructsBottom;
            set => visualSet.bObstructsBottom = value;
        }

        /// <summary>
        /// Gets or sets the resource path of the visual set. <para/>
        /// For alternate sets, the path is relative to <see cref="ItemEntry.EquipResourcePath"/>.
        /// </summary>
        public string Resource
        {
            get => entry.hatAltSetResourcePaths[comboItem];
            set => entry.hatAltSetResourcePaths[comboItem] = value;
        }

        /// <summary>
        /// Helper method to set hair overlaps for each direction.
        /// </summary>
        public void SetHatUnderHair(bool up, bool right, bool down, bool left)
        {
            HatUnderHair[0] = up;
            HatUnderHair[1] = right;
            HatUnderHair[2] = down;
            HatUnderHair[3] = left;
        }

        /// <summary>
        /// Helper method to set player overlaps for each direction.
        /// </summary>
        public void SetHatBehindPlayer(bool up, bool right, bool down, bool left)
        {
            HatBehindPlayer[0] = up;
            HatBehindPlayer[1] = right;
            HatBehindPlayer[2] = down;
            HatBehindPlayer[3] = left;
        }

        /// <summary>
        /// Helper method to set sprite offsets for each direction.
        /// </summary>
        public void SetHatOffsets(Vector2 up, Vector2 right, Vector2 down, Vector2 left)
        {
            HatOffsets[0] = up;
            HatOffsets[1] = right;
            HatOffsets[2] = down;
            HatOffsets[3] = left;
        }

        /// <summary>
        /// Helper method to set hair obstructions for each direction.
        /// </summary>
        public void SetHatHairObstruction(bool sides, bool top, bool bottom)
        {
            ObstructHairSides = sides;
            ObstructHairTop = top;
            ObstructHairBottom = bottom;
        }
    }

    internal Dictionary<Stat, int> stats = new();

    internal HashSet<EquipmentInfo.SpecialEffect> effects = new();

    internal HatInfo.VisualSet defaultSet = new();

    internal Dictionary<ItemCodex.ItemTypes, HatInfo.VisualSet> altSets = new();

    internal Dictionary<ItemCodex.ItemTypes, string> hatAltSetResourcePaths = new();

    internal ItemDescription vanillaItem = new();

    internal EquipmentInfo vanillaEquip;

    /// <summary>
    /// Gets or sets the display name of the item.
    /// </summary>
    public string Name
    {
        get => vanillaItem.sFullName;
        set => vanillaItem.sFullName = value;
    }

    /// <summary>
    /// Gets or sets the description of the item.
    /// </summary>
    public string Description
    {
        get => vanillaItem.sDescription;
        set => vanillaItem.sDescription = value;
    }

    /// <summary> 
    /// Gets or sets the path to the item's icon. The texture path is relative to "Content/".
    /// </summary>
    public string IconPath { get; set; } = "";

    /// <summary> 
    /// Gets or sets the path to the item's shadow texture. The texture path is relative to "Content/".
    /// </summary>
    public string ShadowPath { get; set; } = "";

    /// <summary> 
    /// Gets or sets whenever to use the SoG resource format for loading.
    /// By default, this value is set to false, and custom paths are used.
    /// </summary>
    public bool UseVanillaResourceFormat { get; set; } = false;

    /// <summary>
    /// Gets or sets the gold value of the item.
    /// Buy price is equal to the gold value, the sell price is halved, and buyback is doubled.
    /// </summary>
    public int Value
    {
        get => vanillaItem.iValue;
        set => vanillaItem.iValue = value;
    }

    /// <summary>
    /// Gets or sets the health cost of this item, when bought from the Shadier Merchant in Arcade.
    /// A value of 0 will cause the game to calculate the blood cost from the item's gold price.
    /// </summary>
    public int BloodValue
    {
        get => vanillaItem.iOverrideBloodValue;
        set => vanillaItem.iOverrideBloodValue = value;
    }

    /// <summary>
    /// Gets or sets the value modifier for this item in Arcade Mode.
    /// A value of 0.5 would make the item have half the gold value in Arcade Mode.
    /// </summary>
    public float ArcadeValueModifier
    {
        get => vanillaItem.fArcadeModeCostModifier;
        set => vanillaItem.fArcadeModeCostModifier = value;
    }

    /// <summary>
    /// Gets or sets the sort value of this item.
    /// Items with a higher value will appear first when sorting the inventory using the "Best" filter.
    /// </summary>
    public ushort SortingValue
    {
        get => vanillaItem.iInternalLevel;
        set => vanillaItem.iInternalLevel = value;
    }

    /// <summary> 
    /// Gets or sets the special effect around item drops of this type.
    /// Valid values are 1 (none), 2 (silver ring) and 3 (gold ring). 
    /// </summary>
    public byte Fancyness
    {
        get => vanillaItem.byFancyness;
        set => vanillaItem.byFancyness = (byte)MathHelper.Clamp(value, 1, 3);
    }

    /// <summary>
    /// Adds a category for this item. <para/>
    /// Some categories have a special meaning.
    /// For example, items with <see cref="ItemCodex.ItemCategories.Usable"/> can be quickslotted and activated.
    /// Grindless automatically assigns certain categories for items that are also equipments. 
    /// </summary>
    /// <param name="category"> The category to add. </param>
    public void AddCategory(ItemCodex.ItemCategories category)
    {
        vanillaItem.lenCategory.Add(category);
    }

    /// <summary>
    /// Removes a category for this item.
    /// </summary>
    /// <param name="category"> The category to remove. </param>
    public void RemoveCategory(ItemCodex.ItemCategories category)
    {
        vanillaItem.lenCategory.Add(category);
    }

    /// <summary>
    /// Gets or sets the equipment's resource path. The resource path is relative to "Content/".
    /// For equipment, textures are loaded using specific file names, all relative to this resource path.
    /// </summary>
    public string EquipResourcePath { get; set; } = "";

    /// <summary>
    /// Gets or sets an equipment stat.
    /// <para/> ShldHP stat works even on non-shield items.
    /// <para/> ShldRegen stat isn't visible in the equipment display, however it still works,
    /// and will boost up the shield's health regen amount.
    /// </summary>
    public int this[Stat stat]
    {
        get => stats.TryGetValue(stat, out int value) ? value : 0;
        set => stats[Stat.HP] = value;
    }

    /// <summary>
    /// Gets or sets what equipment this item acts as (if any).
    /// Setting this to values other than <see cref="EquipmentType.None"/>
    /// will make this item into an equipment of that type.
    /// </summary>
    public EquipmentType EquipType { get; set; } = EquipmentType.None;

    /// <summary>
    /// Adds a special effect to this equipment.
    /// Multiple effects (of different types) can be added;
    /// each of them will show up as "Special Effect" in-game.
    /// </summary>
    /// <param name="effect"> The effect to add. This can also be a modded effect. </param>
    public void AddSpecialEffect(EquipmentInfo.SpecialEffect effect)
    {
        effects.Add(effect);
    }

    /// <summary>
    /// Removes a special effect from this equipment.
    /// </summary>
    /// <param name="effect"> The effect to add. This can also be a modded effect. </param>
    public void RemoveSpecialEffect(EquipmentInfo.SpecialEffect effect)
    {
        effects.Remove(effect);
    }

    /// <summary>
    /// Gets an array of four booleans that toggle facegear overlap for hair, for each direction.
    /// </summary>
    public bool[] FacegearOverHair { get; } = new bool[] { true, true, true, true };

    /// <summary>
    /// Gets an array of four booleans that toggle facegear overlap for hat, for each direction.
    /// </summary>
    public bool[] FacegearOverHat { get; } = new bool[] { true, true, true, true };

    /// <summary>
    /// Gets an array of four sprite render offsets for facegear, for each direction.
    /// </summary>
    public Vector2[] FacegearOffsets { get; } = new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };

    /// <summary>
    /// A bool for each direction, defines whenever to load a texture for the facegear in that direction.
    /// All are set to true by default.
    /// </summary>
    public bool[] FacegearSides { get; } = new bool[] { true, true, true, true };

    /// <summary>
    /// Gets the default visual set of a hat. This set is used for most hair types.
    /// </summary>
    public VisualSetConfig DefaultSet => _defaultSetConfig ??= new VisualSetConfig()
    {
        entry = this,
        comboItem = ItemCodex.ItemTypes.Null,
        visualSet = defaultSet
    };
    private VisualSetConfig _defaultSetConfig;

    /// <summary>
    /// Creates a configuration for an alternate visual set. <para/>
    /// Alternate visual sets are used when the player has certain hair styles.
    /// Keep in mind that the hair styles are defined as items in <see cref="ItemCodex.ItemTypes"/>.
    /// </summary>
    /// <param name="comboItem"> The hair to create the alternate visual set for. </param>
    /// <returns> The config for the alternate set. </returns>
    public VisualSetConfig ConfigureAltSet(ItemCodex.ItemTypes comboItem)
    {
        if (!altSets.TryGetValue(comboItem, out HatInfo.VisualSet set))
        {
            set = altSets[comboItem] = new HatInfo.VisualSet();
        }

        return new VisualSetConfig()
        {
            entry = this,
            comboItem = comboItem,
            visualSet = set
        };
    }

    /// <summary>
    /// Gets or sets whenever hats occupy one slot (hat) or two (hat + facegear => mask).
    /// </summary>
    public bool HatDoubleSlot { get; set; } = false;

    /// <summary>
    /// Gets or sets the equipment's weapon type.
    /// </summary>
    public WeaponInfo.WeaponCategory WeaponType { get; set; } = WeaponInfo.WeaponCategory.OneHanded;

    /// <summary>
    /// Gets or sets whenever the weapon is magical or not. <para/>
    /// Basic attacks with physical weapons deal 100% ATK as damage. <para/>
    /// For magic weapons, the damage is equal to 40% (ATK + MATK).
    /// Additionally, magic weapons can fire a projectile that deals 40% MATK as damage.
    /// </summary>
    public bool MagicWeapon { get; set; } = false;

    // Methods specific to FacegearInfo

    /// <summary>
    /// Helper method for setting facegear hair overlaps for all directions.
    /// </summary>
    public void SetFacegearOverHair(bool up, bool right, bool down, bool left)
    {
        FacegearOverHair[0] = up;
        FacegearOverHair[1] = right;
        FacegearOverHair[2] = down;
        FacegearOverHair[3] = left;
    }

    /// <summary>
    /// Helper method for setting facegear hat overlaps for all directions.
    /// </summary>
    public void SetFacegearOverHat(bool up, bool right, bool down, bool left)
    {
        FacegearOverHat[0] = up;
        FacegearOverHat[1] = right;
        FacegearOverHat[2] = down;
        FacegearOverHat[3] = left;
    }

    /// <summary>
    /// Helper method for setting facegear offsets for all directions.
    /// </summary>
    public void SetFacegearOffsets(Vector2 up, Vector2 right, Vector2 down, Vector2 left)
    {
        FacegearOffsets[0] = up;
        FacegearOffsets[1] = right;
        FacegearOffsets[2] = down;
        FacegearOffsets[3] = left;
    }

    protected override void Initialize()
    {
        vanillaItem.enType = GameID;

        if (vanillaEquip != null)
        {
            vanillaEquip.enItemType = GameID;
            vanillaEquip.xItemDescription = vanillaItem;
        }

        vanillaItem.sNameLibraryHandle = $"Item_{(int)GameID}_Name";
        vanillaItem.sDescriptionLibraryHandle = $"Item_{(int)GameID}_Description";
        vanillaItem.sCategory = "";

        EquipmentType typeToUse = Enum.IsDefined(typeof(EquipmentType), EquipType) ? EquipType : EquipmentType.None;

        EquipmentInfo equipData = null;
        switch (typeToUse)
        {
            case EquipmentType.None:
                break;
            case EquipmentType.Facegear:
                FacegearInfo faceData = (equipData = new FacegearInfo(GameID)) as FacegearInfo;

                Array.Copy(FacegearOverHair, faceData.abOverHair, 4);
                Array.Copy(FacegearOverHat, faceData.abOverHat, 4);
                Array.Copy(FacegearOffsets, faceData.av2RenderOffsets, 4);

                break;
            case EquipmentType.Hat:
                HatInfo hatData = (equipData = new HatInfo(GameID) { bDoubleSlot = HatDoubleSlot }) as HatInfo;

                hatData.xDefaultSet = defaultSet;
                hatData.denxAlternateVisualSets = altSets;

                break;
            case EquipmentType.Weapon:
                WeaponInfo weaponData = new(EquipResourcePath, GameID, WeaponType)
                {
                    enWeaponCategory = WeaponType,
                    enAutoAttackSpell = WeaponInfo.AutoAttackSpell.None
                };
                equipData = weaponData;

                if (WeaponType == WeaponInfo.WeaponCategory.OneHanded)
                {
                    weaponData.iDamageMultiplier = 90;
                    if (MagicWeapon)
                        weaponData.enAutoAttackSpell = WeaponInfo.AutoAttackSpell.Generic1H;
                }
                else if (WeaponType == WeaponInfo.WeaponCategory.TwoHanded)
                {
                    weaponData.iDamageMultiplier = 125;
                    if (MagicWeapon)
                        weaponData.enAutoAttackSpell = WeaponInfo.AutoAttackSpell.Generic2H;
                }
                break;
            default:
                equipData = new EquipmentInfo(EquipResourcePath, GameID);
                break;
        }

        if (EquipType != EquipmentType.None)
        {
            equipData.deniStatChanges = new Dictionary<Stat, int>(stats);
            equipData.lenSpecialEffects.AddRange(effects);
        }

        vanillaEquip = equipData;

        HashSet<ItemCodex.ItemCategories> toSanitize = new()
        {
            ItemCodex.ItemCategories.OneHandedWeapon,
            ItemCodex.ItemCategories.TwoHandedWeapon,
            ItemCodex.ItemCategories.Weapon,
            ItemCodex.ItemCategories.Shield,
            ItemCodex.ItemCategories.Armor,
            ItemCodex.ItemCategories.Hat,
            ItemCodex.ItemCategories.Accessory,
            ItemCodex.ItemCategories.Shoes,
            ItemCodex.ItemCategories.Facegear
        };

        vanillaItem.lenCategory.ExceptWith(toSanitize);

        if (equipData != null)
        {
            ItemCodex.ItemCategories type = (ItemCodex.ItemCategories)EquipType;
            vanillaItem.lenCategory.Add(type);

            if (type == ItemCodex.ItemCategories.Weapon)
            {
                switch ((equipData as WeaponInfo).enWeaponCategory)
                {
                    case WeaponInfo.WeaponCategory.OneHanded:
                        vanillaItem.lenCategory.Add(ItemCodex.ItemCategories.OneHandedWeapon); break;
                    case WeaponInfo.WeaponCategory.TwoHanded:
                        vanillaItem.lenCategory.Add(ItemCodex.ItemCategories.TwoHandedWeapon); break;
                }
            }
        }

        Globals.Game.EXT_AddMiscText("Items", vanillaItem.sNameLibraryHandle, vanillaItem.sFullName);
        Globals.Game.EXT_AddMiscText("Items", vanillaItem.sDescriptionLibraryHandle, vanillaItem.sDescription);

        // Textures are loaded on demand
    }

    protected override void Cleanup()
    {
        Globals.Game.EXT_RemoveMiscText("Items", vanillaItem.sNameLibraryHandle);
        Globals.Game.EXT_RemoveMiscText("Items", vanillaItem.sDescriptionLibraryHandle);

        // TODO: Set texture references to null since they're disposed

        // Weapon assets are automatically disposed of by the game
        // Same goes for dropped item's textures

        ContentManager manager = Globals.Game.Content;

        manager.UnloadIfModded(IconPath);

        string[] directions = new string[]
        {
            "Up", "Right", "Down", "Left"
        };

        if (vanillaEquip is HatInfo hatData)
        {
            string basePath = EquipResourcePath;

            if (basePath != null)
            {
                foreach (var dir in directions)
                    manager.UnloadIfModded(Path.Combine(basePath, dir));

                foreach (var key in hatData.denxAlternateVisualSets.Keys)
                {
                    if (hatAltSetResourcePaths[key] != null)
                    {
                        foreach (var dir in directions)
                            manager.UnloadIfModded(Path.Combine(basePath, hatAltSetResourcePaths[key], dir));
                    }
                }
            }
        }
        else if (vanillaEquip is FacegearInfo)
        {
            string path = EquipResourcePath;

            if (path != null)
            {
                foreach (var dir in directions)
                    manager.Unload(Path.Combine(path, dir));
            }
        }
    }
}
