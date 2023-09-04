using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Extensions.Logging;

namespace Grindless;

/// <summary>
/// The base class for all mods.
/// </summary>
/// <remarks>
/// Mod DLLs need to have at one class that is derived from <see cref="Mod"/>. That class will be constructed by Grindless when loading.
/// </remarks>
public abstract partial class Mod
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
    public ILogger Logger { get; protected set; } = Program.LogFactory.CreateLogger("UnknownMod");

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
        if (Name != null)
            Logger = Program.LogFactory.CreateLogger(Name);
    }
}