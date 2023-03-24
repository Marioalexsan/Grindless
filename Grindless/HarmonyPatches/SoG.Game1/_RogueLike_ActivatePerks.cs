using Microsoft.Extensions.Logging;
using System.Reflection.Emit;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_ActivatePerks))]
    static class _RogueLike_ActivatePerks
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> codeList = instructions.ToList();

            int start = -1;
            int ldloc_pos = 3;
            while (ldloc_pos-- > 0)
            {
                start = codeList.FindIndex(start + 1, x => x.opcode == OpCodes.Ldloc_0);
            }

            int end = -1;
            int ldloca_pos = 2;
            while (ldloca_pos-- > 0)
            {
                end = codeList.FindIndex(end + 1, x => x.opcode == OpCodes.Ldloca_S && (x.operand as LocalBuilder).LocalIndex == 3);
            }

            Label perkProcessedSkip = generator.DefineLabel();

            codeList[end].WithLabels(perkProcessedSkip);

            List<CodeInstruction> inserted = new()
            {
                new CodeInstruction(OpCodes.Ldarg_S, 1).WithLabels(codeList[start].labels).WithBlocks(codeList[start].blocks),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => InActivatePerk(null, default))),
                new CodeInstruction(OpCodes.Brtrue, perkProcessedSkip)
            };

            codeList.InsertRange(start, inserted);

            return codeList;
        }

        // If this method returns true, the vanilla switch case is skipped
        static bool InActivatePerk(PlayerView view, RogueLikeMode.Perks perk)
        {
            var entry = PerkEntry.Entries.Get(perk);

            if (entry == null)
                return false;

            // This callback accepts vanilla perks because the vanilla activators are in a for loop
            if (entry.IsVanilla && entry.RunStartActivator == null)
                return false;

            try
            {
                entry.RunStartActivator?.Invoke(view);
            }
            catch (Exception e)
            {
                Program.Logger.LogError("Run start activator threw an exception for perk {perk}! Exception: {e}", perk, e);
            }

            return true;
        }

    }
}
