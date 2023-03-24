using System.Reflection.Emit;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(SoundSystem), nameof(SoundSystem.PlayTrackableInterfaceCue))]
    static class PlayTrackableInterfaceCue
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
            => PlayCue.Transpiler(code, gen);
    }
}
