namespace Grindless
{
    /// <summary>
    /// Represents a modded equipment special effect.
    /// </summary>
    /// <remarks> 
    /// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
    /// </remarks>
    public class EquipmentEffectEntry : Entry<EquipmentInfo.SpecialEffect>
    {
        internal static EntryManager<EquipmentInfo.SpecialEffect, EquipmentEffectEntry> Entries { get; }
            = new EntryManager<EquipmentInfo.SpecialEffect, EquipmentEffectEntry>(700);

        /// <summary>
        /// Gets or sets the callback that will be called when an equipment with this effect is worn.
        /// You can use this callback to do non-stat things such as creating persistent spells.
        /// </summary>
        public Action<PlayerView> OnEquip { get; set; }

        /// <summary>
        /// Gets or sets the callback that will be called when an equipment with this effect is worn.
        /// You can use this callback to undo your actions in OnEquip.
        /// </summary>
        public Action<PlayerView> OnRemove { get; set; }

        internal EquipmentEffectEntry()
        {

        }

        protected override void Cleanup()
        {
            // Nothing for now
        }

        protected override void Initialize()
        {
            // Nothing for now
        }
    }
}
