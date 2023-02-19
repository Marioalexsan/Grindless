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
    [HarmonyPatch(typeof(SoundSystem), nameof(SoundSystem.ReadySongInCue))]
    static class ReadySongInCue
    {
        // Also used by PlaySong Transpiler
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
        {
            var codeList = code.ToList();

            // (local1 = GetMusicSoundBank(sCueName)) != null ? local1.GetCue(sCueName) : soundBank.GetCue(sCueName)

            Label skipVanillaBank = gen.DefineLabel();
            Label doVanillaBank = gen.DefineLabel();
            LocalBuilder modBank = gen.DeclareLocal(typeof(SoundBank));

            MethodInfo target = typeof(SoundBank).GetMethod("GetCue", new Type[] { typeof(string) });

            var insertBefore = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => _Helper.GetMusicSoundBank(null))),
                new CodeInstruction(OpCodes.Stloc_S, modBank.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, modBank.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, doVanillaBank),
                new CodeInstruction(OpCodes.Ldloc_S, modBank.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => _Helper.GetCueName(null))),
                new CodeInstruction(OpCodes.Call, target),
                new CodeInstruction(OpCodes.Br, skipVanillaBank),
                new CodeInstruction(OpCodes.Nop).WithLabels(doVanillaBank)
            };

            var insertAfter = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Nop).WithLabels(skipVanillaBank)
            };

            return codeList.InsertAroundMethod(target, insertBefore, insertAfter, editsReturnValue: true);
        }

    }
}
