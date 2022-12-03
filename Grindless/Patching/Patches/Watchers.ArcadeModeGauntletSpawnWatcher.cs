using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Watchers;
using CodeList = System.Collections.Generic.IEnumerable<HarmonyLib.CodeInstruction>;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(ArcadeModeGauntletSpawnWatcher))]
    internal static class Watchers_ArcadeModeGauntletSpawnWatcher
    {
        [HarmonyPatch(nameof(ArcadeModeGauntletSpawnWatcher.Update))]
        [HarmonyTranspiler]
        internal static CodeList Update_Transpiler(CodeList code, ILGenerator gen)
        {
            List<CodeInstruction> codeList = code.ToList();

            int position = codeList.FindPosition((list, index) => list[index].opcode == OpCodes.Stfld && list[index + 1].opcode == OpCodes.Ret);

            var insert = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, 5),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GauntletEnemySpawned))),
            };

            return codeList.InsertAt(position, insert);
        }
    }
}
