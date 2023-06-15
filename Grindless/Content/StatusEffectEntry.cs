namespace Grindless;

/// <summary>
/// Represents a modded status effect.
/// </summary>
/// <remarks> 
/// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
/// </remarks>
[ModEntry(1000)]
public class StatusEffectEntry : Entry<BaseStats.StatusEffectSource>
{
    internal StatusEffectEntry() { }

    /// <summary>
    /// Gets or sets the icon's texture path. The texture path is relative to "Config/".
    /// A null or empty string will load NullTex instead.
    /// </summary>
    public string TexturePath { get; set; }

    protected override void Initialize()
    {
        // Nothing, texture is loaded on demand
    }

    protected override void Cleanup()
    {
        Globals.Game.Content.UnloadIfModded(TexturePath);
    }
}
