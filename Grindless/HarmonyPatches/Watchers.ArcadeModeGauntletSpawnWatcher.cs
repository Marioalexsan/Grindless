using System.Reflection.Emit;
using Watchers;
using CodeList = System.Collections.Generic.IEnumerable<HarmonyLib.CodeInstruction>;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(ArcadeModeGauntletSpawnWatcher))]
    static class Watchers_ArcadeModeGauntletSpawnWatcher
    {
        [HarmonyPatch(nameof(ArcadeModeGauntletSpawnWatcher.Update))]
        [HarmonyTranspiler]
        static CodeList Update_Transpiler(CodeList code, ILGenerator gen)
        {
            List<CodeInstruction> codeList = code.ToList();

            int position = codeList.FindPosition((list, index) => list[index].opcode == OpCodes.Stfld && list[index + 1].opcode == OpCodes.Ret);

            var insert = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, 5),
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => GauntletEnemySpawned(null))),
            };

            return codeList.InsertAt(position, insert);
        }

        static void GauntletEnemySpawned(Enemy enemy)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.PostArcadeGauntletEnemySpawned(enemy);
        }
    }
}
