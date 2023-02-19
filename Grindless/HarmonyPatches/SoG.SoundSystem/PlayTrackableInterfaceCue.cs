using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(SoundSystem), nameof(SoundSystem.PlayTrackableInterfaceCue))]
    static class PlayTrackableInterfaceCue
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
            => PlayCue.Transpiler(code, gen);
    }
}
