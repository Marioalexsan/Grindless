namespace Grindless;

/// <summary>
/// Contains data for sending custom packets between clients and servers.
/// </summary>
/// <remarks> 
/// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
/// </remarks>
public class NetworkEntry : Entry<GrindlessID.NetworkID>
{
    internal static EntryManager<GrindlessID.NetworkID, NetworkEntry> Entries { get; }
        = new EntryManager<GrindlessID.NetworkID, NetworkEntry>(0);

    public Dictionary<ushort, ServerSideParser> ServerSide { get; } = new Dictionary<ushort, ServerSideParser>();

    public Dictionary<ushort, ClientSideParser> ClientSide { get; } = new Dictionary<ushort, ClientSideParser>();

    internal NetworkEntry() { }

    protected override void Initialize()
    {
        // Nothing to do
    }

    protected override void Cleanup()
    {
        // Nothing to do
    }

}
