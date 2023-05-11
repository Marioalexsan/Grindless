namespace Grindless;

/// <summary>
/// Represents a modded treat or curse from Arcade Mode.
/// </summary>
/// <remarks> 
/// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
/// </remarks>
public class CurseEntry : Entry<RogueLikeMode.TreatsCurses>
{
    internal static EntryManager<RogueLikeMode.TreatsCurses, CurseEntry> Entries { get; }
        = new EntryManager<RogueLikeMode.TreatsCurses, CurseEntry>(3500);

    internal string nameHandle = "";

    internal string descriptionHandle = "";

    /// <summary>
    /// Gets or sets the object as being a treat or curse. <para/>
    /// The only difference between a treat and curse is the NPC shop in which they appear.
    /// </summary>
    public bool IsTreat { get; set; }

    /// <summary>
    /// Gets or sets the object as being a treat or curse. <para/>
    /// The only difference between a treat and curse is the NPC shop in which they appear.
    /// </summary>
    public bool IsCurse
    {
        get => !IsTreat;
        set => IsTreat = !value;
    }

    /// <summary>
    /// Gets or sets the texture path of the treat or curse's icon. This path is relative to the "Content" folder.
    /// </summary>
    public string TexturePath { get; set; }

    /// <summary>
    /// Gets or sets the name displayed inside the game.
    /// </summary>
    public string Name { get; set; } = "Candy's Shenanigans";

    /// <summary>
    /// Gets or sets the description displayed inside the game.
    /// </summary>
    public string Description { get; set; } = "It's a mysterious treat or curse!";

    /// <summary>
    /// Gets or sets the score modifier. <para/>
    /// A score modifier of 0.2 corresponds to +20%, -0.15 to -15%, etc.
    /// </summary>
    public float ScoreModifier { get; set; }

    internal CurseEntry() { }

    protected override void Initialize()
    {
        nameHandle = $"TreatCurse_{(int)GameID}_Name";
        descriptionHandle = $"TreatCurse_{(int)GameID}_Description";

        Globals.Game.EXT_AddMiscText("Menus", nameHandle, Name);
        Globals.Game.EXT_AddMiscText("Menus", descriptionHandle, Description);

        // Texture on demand
    }

    protected override void Cleanup()
    {
        Globals.Game.EXT_RemoveMiscText("Menus", nameHandle);
        Globals.Game.EXT_RemoveMiscText("Menus", descriptionHandle);
        Globals.Game.Content.UnloadIfModded(TexturePath);
    }
}
