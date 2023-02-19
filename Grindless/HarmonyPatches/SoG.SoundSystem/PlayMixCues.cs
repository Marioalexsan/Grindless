using HarmonyLib;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(SoundSystem), nameof(SoundSystem.PlayMixCues))]
    static class PlayMixCues
    {
        static IEnumerable<CodeInstruction> PlayMixCues_Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
        {
            var codeList = code.ToList();

            Label skipVanillaBank_one = gen.DefineLabel();
            Label doVanillaBank_one = gen.DefineLabel();
            LocalBuilder modBank_one = gen.DeclareLocal(typeof(SoundBank));

            Label skipVanillaBank_two = gen.DefineLabel();
            Label doVanillaBank_two = gen.DefineLabel();
            LocalBuilder modBank_two = gen.DeclareLocal(typeof(SoundBank));

            MethodInfo target = typeof(SoundBank).GetMethod("GetCue", new Type[] { typeof(string) });

            var insertBefore_one = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => _Helper.GetMusicSoundBank(null))),
                new CodeInstruction(OpCodes.Stloc_S, modBank_one.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, modBank_one.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, doVanillaBank_one),
                new CodeInstruction(OpCodes.Ldloc_S, modBank_one.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => _Helper.GetCueName(null))),
                new CodeInstruction(OpCodes.Call, target),
                new CodeInstruction(OpCodes.Br, skipVanillaBank_one),
                new CodeInstruction(OpCodes.Nop).WithLabels(doVanillaBank_one)
            };

            var insertAfter_one = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Nop).WithLabels(skipVanillaBank_one)
            };

            var insertBefore_two = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => _Helper.GetMusicSoundBank(null))),
                new CodeInstruction(OpCodes.Stloc_S, modBank_two.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, modBank_two.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, doVanillaBank_two),
                new CodeInstruction(OpCodes.Ldloc_S, modBank_two.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => _Helper.GetCueName(null))),
                new CodeInstruction(OpCodes.Call, target),
                new CodeInstruction(OpCodes.Br, skipVanillaBank_two),
                new CodeInstruction(OpCodes.Nop).WithLabels(doVanillaBank_two)
            };

            var insertAfter_two = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Nop).WithLabels(skipVanillaBank_two)
            };

            // Patch both methods
            return codeList
                .InsertAroundMethod(target, insertBefore_two, insertAfter_two, methodIndex: 1, editsReturnValue: true)
                .InsertAroundMethod(target, insertBefore_one, insertAfter_one, editsReturnValue: true);
        }

    }
}
