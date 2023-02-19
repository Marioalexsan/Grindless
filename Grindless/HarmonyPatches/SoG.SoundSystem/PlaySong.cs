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
    [HarmonyPatch(typeof(SoundSystem), nameof(SoundSystem.PlaySong))]
    static class PlaySong
    {
        static void Prefix(ref string sSongName, bool bFadeIn)
        {
            var redirects = AudioEntry.VanillaMusicRedirects;
            string audioIDToUse = sSongName;

            if (!audioIDToUse.StartsWith("GS_") && redirects.ContainsKey(audioIDToUse))
                audioIDToUse = redirects[audioIDToUse];

            sSongName = audioIDToUse;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
            => ReadySongInCue.Transpiler(code, gen);
    }
}
