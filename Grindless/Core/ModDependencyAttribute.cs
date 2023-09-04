namespace Grindless;

/// <summary>
/// Defines a dependency on another mod.
/// Mods that have dependencies require all of them to be present and loaded before they are.
/// If a dependency is missing or disabled, the mod will fail to load.
/// Mod version dependencies can be specified in npm syntax.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ModDependencyAttribute : Attribute
{
    /// <summary>
    /// Gets the dependency's identifier.
    /// </summary>
    public string NameID { get; }

    /// <summary>
    /// Gets the required dependency version.
    /// </summary>
    public string ModVersion { get; }

    public ModDependencyAttribute(string NameID, string ModVersion = "*")
    {
        this.NameID = NameID;
        this.ModVersion = ModVersion;
    }
}
