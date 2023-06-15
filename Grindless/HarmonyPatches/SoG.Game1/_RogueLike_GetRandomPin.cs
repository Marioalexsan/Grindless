using System.Reflection.Emit;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_GetRandomPin))]
static class _RogueLike_GetRandomPin
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> _RogueLike_GetRandomPin_Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
    {
        List<CodeInstruction> codeList = code.ToList();

        int start = codeList.FindIndex(x => x.opcode == OpCodes.Stloc_1) + 1;

        int newobj_pos = 2;
        int end = -1;
        while (newobj_pos-- > 0)
        {
            end = codeList.FindIndex(end + 1, x => x.opcode == OpCodes.Newobj);
        }

        List<CodeInstruction> toInsert = new()
        {
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => InGetRandomPin(null))),
        };

        codeList.RemoveRange(start, end - start);
        codeList.InsertAt(start, toInsert);

        return codeList;
    }

    static void InGetRandomPin(List<PinCodex.PinType> list)
    {
        list.AddRange(Entries.Pins.Where(x => x.ConditionToDrop?.Invoke() ?? true).Select(x => x.GameID));
    }
}
