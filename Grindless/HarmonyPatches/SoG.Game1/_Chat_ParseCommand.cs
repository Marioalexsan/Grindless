using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Chat_ParseCommand))]
    static class _Chat_ParseCommand
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
        {
            var codeList = code.ToList();

            Label afterRet = gen.DefineLabel();

            MethodInfo target = typeof(string).GetMethod(nameof(string.ToLowerInvariant));
            MethodInfo implementerCall = SymbolExtensions.GetMethodInfo(() => ParseModCommands(default, default, default));

            var insert = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, 2),
                new CodeInstruction(OpCodes.Ldarg_S, 1),
                new CodeInstruction(OpCodes.Ldarg_S, 2),
                new CodeInstruction(OpCodes.Call, implementerCall),
                new CodeInstruction(OpCodes.Brfalse, afterRet),
                new CodeInstruction(OpCodes.Ret),
                new CodeInstruction(OpCodes.Nop).WithLabels(afterRet)
            };

            return codeList.InsertAfterMethod(target, insert);
        }

        static bool ParseModCommands(string command, string message, int connection)
        {
            string[] words = command.Split(new[] { ':' }, 2);
            if (words.Length != 2)
                return false; // Is probably a vanilla command

            string target = words[0];
            string trueCommand = words[1];

            Mod mod = ModManager.Mods.FirstOrDefault(x => x.Name == target);

            // Also try case insensitive close matches
            mod ??= ModManager.Mods.FirstOrDefault(x => x.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase));

            if (mod == null)
            {
                CAS.AddChatMessage($"[{GrindlessMod.ModName}] Unknown mod!");
                return true;
            }

            var entry = CommandEntry.Entries.Get(mod, "");

            CommandParser parser = null;

            if (entry != null && !entry.Commands.TryGetValue(trueCommand, out parser))
            {
                // Try case insensitive close matches
                parser = entry.Commands.First(x => x.Key.Equals(trueCommand, StringComparison.InvariantCultureIgnoreCase)).Value;
            }

            if (entry == null || parser == null)
            {
                if (trueCommand.Equals("help", StringComparison.InvariantCultureIgnoreCase))
                {
                    ParseModCommands($"{GrindlessMod.ModName}:Help", target, connection);
                    return true;
                }

                CAS.AddChatMessage($"[{target}] Unknown command!");
                return true;
            }

            string[] args = ModUtils.GetArgs(message);

            Program.Logger.LogDebug("Parsed command {target} : {trueCommand}, arguments: {args.Length}", target, trueCommand, args.Length);
            parser(args, connection);

            return true;
        }
    }
}
