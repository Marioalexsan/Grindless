using HarmonyLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public Dictionary<string, CommandParser> Commands = new();

        internal CommandEntry() { }

        public void AutoAddModCommands()
        {
            var methods = AccessTools.GetDeclaredMethods(Mod.GetType())
                .Select(x => (method: x, attrib: x.GetCustomAttribute<ModCommandAttribute>()))
                .Where(x => x.attrib != null);

            foreach (var (method, attrib) in methods)
            {
                try
                {
                    var command = (CommandParser)method.CreateDelegate(typeof(CommandParser), method.IsStatic ? null : Mod);

                    Commands[attrib.Command] = command;
                }
                catch (Exception e)
                {
                    Program.Logger.LogWarning($"Couldn't add command {attrib.Command}: {e.Message}");
                }
            }
        }

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
