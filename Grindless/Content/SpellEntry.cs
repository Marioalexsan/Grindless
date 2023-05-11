namespace Grindless;

/// <summary>
/// Represents a modded entity of type ISpellInstance.
/// Some spells can act as player spells, and have additional information associated with them.
/// </summary>
/// <remarks> 
/// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
/// </remarks>
public class SpellEntry : Entry<SpellCodex.SpellTypes>
{
    internal static EntryManager<SpellCodex.SpellTypes, SpellEntry> Entries { get; }
        = new EntryManager<SpellCodex.SpellTypes, SpellEntry>(10000);

    /// <summary>
    /// Gets or sets the builder of the spell instance.
    /// The builder is called when an instance of this spell must be made.
    /// Use this to create a subclass of ISpellInstance, initialize it, and return it.
    /// </summary>
    public SpellBuilder Builder { get; set; }

    /// <summary>
    /// Gets or sets whenever this spell is a player magical skill.
    /// </summary>
    public bool IsMagicSkill { get; set; }

    /// <summary>
    /// Gets or sets whenever this spell is an player utility skill.
    /// </summary>
    public bool IsUtilitySkill { get; set; }

    /// <summary>
    /// Gets or sets whenever this spell is a player melee skill.
    /// </summary>
    public bool IsMeleeSkill { get; set; }

    internal SpellEntry() { }

    internal SpellEntry(Mod mod, string modID, SpellCodex.SpellTypes gameID)
        : base(mod, modID, gameID) { }

    protected override void Initialize()
    {
        // Nothing for now
    }

    protected override void Cleanup()
    {
        // Nothing for now
    }
}
