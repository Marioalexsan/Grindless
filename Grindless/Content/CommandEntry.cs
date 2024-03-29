﻿using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Grindless;

/// <summary>
/// Defines custom commands that can be entered from the in-game chat. <para/>
/// All modded commands are called by using the "/{<see cref="Mod.Name"/>}:{Command} [args] format. <para/>
/// For instance, you can use "/Grindless:Help" to invoke the mod tool's help command.
/// </summary>
/// <remarks> 
/// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
/// </remarks>
[ModEntry(0)]
public class CommandEntry : Entry<GrindlessID.CommandID>
{
    internal CommandEntry() { }

    public Dictionary<string, CommandParser> Commands = new();

    /// <summary>
    /// Name alias to use for this mod.
    /// If set to a value other than null or undefined, the mod commands will 
    /// also be callable using the "/(Alias):(Command) (args)" format.
    /// If multiple mods set the same alias, only one of them will be able to use the alias.
    /// Ideally, you want this to be a short and easy to use name.
    /// </summary>
    public string Alias { get; set; }

    public void AutoAddModCommands(string alias = null)
    {
        if (Mod is JavaScriptMod)
            throw new InvalidOperationException("Auto adding mod commands is possible only for C# mods.");

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

        Alias = alias;
    }

    internal static Mod MatchModByTarget(string target)
    {
        static bool FuzzyMatch(string name, string target) => string.Equals(name, target, StringComparison.InvariantCultureIgnoreCase);

        return ModManager.Mods.FirstOrDefault(x => x.Name == target)
            ?? ModManager.Mods.FirstOrDefault(x => FuzzyMatch(x.Name, target))
            ?? Entries.Commands.FirstOrDefault(x => x.Alias == target)?.Mod
            ?? Entries.Commands.FirstOrDefault(x => FuzzyMatch(x.Alias, target))?.Mod;
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
