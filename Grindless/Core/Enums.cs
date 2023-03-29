namespace Grindless
{
    /// <summary>
    /// Defines all of the available equipment types.
    /// </summary>
    public enum EquipmentType
    {
        None = -1,
        Weapon = ItemCodex.ItemCategories.Weapon,
        Shield = ItemCodex.ItemCategories.Shield,
        Armor = ItemCodex.ItemCategories.Armor,
        Hat = ItemCodex.ItemCategories.Hat,
        Accessory = ItemCodex.ItemCategories.Accessory,
        Shoes = ItemCodex.ItemCategories.Shoes,
        Facegear = ItemCodex.ItemCategories.Facegear
    }

    /// <summary>
    /// Defines the directions used by the game.
    /// </summary>
    public static class Directions
    {
        public const byte Up = 0;
        public const byte Right = 1;
        public const byte Down = 2;
        public const byte Left = 3;
    }

    /// <summary>
    /// Defines damage types used by the game.
    /// </summary>
    public static class DamageTypes
    {
        // Shield related
        public const byte Type1_ShieldDamage = 1;
        public const byte Type2_ShieldDamage_PerfectGuard = 2;
        public const byte Type101_BreakShield = 101;

        // Barrier related
        public const byte Type10_BarrierDamage = 10;
        public const byte Type12_BreakBarrier = 12;

        // Chicken mode related
        public const byte Type11_BlockedByChicken = 11;

        // Take damage
        public const byte Type0_HealthDamage_Stun_TriggerInvuln = 0;
        public const byte Type9_HealthDamage_ShortStun_TriggerInvuln = 9;
        public const byte Type7_HealthDamage_Stun = 7;
        public const byte Type8_HealthDamage_ShortStun = 8;
        public const byte Type3_HealthDamage = 3;
        public const byte Type5_HealthDamage_NoInvulnerability = 5;

        public const byte Type103_HealthDamage = 103;

        // Other
        public const byte Type4_PerfectGuard = 4;
        public const byte Type6_Stun = 6;
        public const byte Type100_BlockedByUltimateGuard = 100;
        public const byte Type102_BlockedByDodge = 102;
    }
}


