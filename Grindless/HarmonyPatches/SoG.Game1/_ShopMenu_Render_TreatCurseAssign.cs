using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._ShopMenu_Render_TreatCurseAssign))]
    static class _ShopMenu_Render_TreatCurseAssign
    {
        /// <summary>
        /// Implements an updated interface for Treat and Curse shops in Arcade.
        /// The new menus support viewing more than 10 entries at a time.
        /// </summary>
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
        {
            var codeList = code.ToList();

            const string ErrorMessage = "ShopMenu_Render_TreatCurseAssign transpiler is invalid!";
            Debug.Assert(codeList[457].opcode == OpCodes.Ldarg_0, ErrorMessage);
            Debug.Assert(codeList[451].opcode == OpCodes.Ldarg_0, ErrorMessage);
            Debug.Assert(codeList[105].opcode == OpCodes.Ldc_I4_5, ErrorMessage);
            Debug.Assert(codeList[94].opcode == OpCodes.Ldc_I4_5, ErrorMessage);
            Debug.Assert(codeList[70].opcode == OpCodes.Ldc_I4_0, ErrorMessage);

            LocalBuilder start = gen.DeclareLocal(typeof(int));
            LocalBuilder end = gen.DeclareLocal(typeof(int));

            var firstInsert = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Call, typeof(TCMenuWorker).GetMethod(nameof(TCMenuWorker.Update))),
                new CodeInstruction(OpCodes.Call, typeof(TCMenuWorker).GetProperty(nameof(TCMenuWorker.TCListStart)).GetGetMethod()),
                new CodeInstruction(OpCodes.Stloc_S, start.LocalIndex),
                new CodeInstruction(OpCodes.Call, typeof(TCMenuWorker).GetProperty(nameof(TCMenuWorker.TCListEnd)).GetGetMethod()),
                new CodeInstruction(OpCodes.Stloc_S, end.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, start.LocalIndex),
            };

            var secondInsert = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, end.LocalIndex)
            };

            var thirdInsert = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(Globals), nameof(Globals.SpriteBatch)).GetGetMethod(true)),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, typeof(TCMenuWorker).GetMethod(nameof(TCMenuWorker.DrawScroller))),
            };

            var offsetInsert = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, start.LocalIndex),
                new CodeInstruction(OpCodes.Sub)
            };

            return codeList
                .InsertAt(457, thirdInsert)
                .ReplaceAt(451, 5, secondInsert)
                .InsertAt(105, offsetInsert)
                .InsertAt(94, offsetInsert)
                .ReplaceAt(70, 1, firstInsert);
        }

    }
}
