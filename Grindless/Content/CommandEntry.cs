using System.Collections.Generic;

namespace Grindless
{
    /// <summary>
    /// Defines custom commands that can be entered from the in-game chat. <para/>
    /// All modded commands are called by using the "/{<see cref="Mod.Name"/>}:{Command} [args] format. <para/>
    /// For instance, you can use "/Grindless:Help" to invoke the mod tool's help command.
    /// </summary>
    /// <remarks> 
    /// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
    /// </remarks>
    public class CommandEntry : Entry<GrindlessID.CommandID>
    {
        internal static EntryManager<GrindlessID.CommandID, CommandEntry> Entries { get; }
            = new EntryManager<GrindlessID.CommandID, CommandEntry>(0);

        public Dictionary<string, CommandParser> Commands = new Dictionary<string, CommandParser>();

        internal CommandEntry() { }

        protected override void Initialize()
        {
            // Nothing to do
        }

        protected override void Cleanup()
        {
            // Nothing to do
        }
    }
}
