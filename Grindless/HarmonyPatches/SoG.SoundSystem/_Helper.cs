using HarmonyLib;
using Microsoft.Xna.Framework.Audio;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    static class _Helper
    {
        internal static SoundBank GetEffectSoundBank(string audioID)
        {
            bool success = ModUtils.SplitAudioID(audioID, out int entryID, out bool isMusic, out _);
            if (!(success && !isMusic))
                return null;

            var entry = AudioEntry.Entries.Get((GrindlessID.AudioID)entryID);

            return entry?.effectsSB;
        }

        internal static string GetCueName(string GSID)
        {
            if (!ModUtils.SplitAudioID(GSID, out int entryID, out bool isMusic, out int cueID))
                return "";

            var entry = AudioEntry.Entries.Get((GrindlessID.AudioID)entryID);

            if (entry == null)
            {
                return "";
            }

            return isMusic ? entry.indexedMusicCues[cueID] : entry.indexedEffectCues[cueID];
        }

        internal static SoundBank GetMusicSoundBank(string audioID)
        {
            bool success = ModUtils.SplitAudioID(audioID, out int entryID, out bool isMusic, out _);

            if (!(success && isMusic))
                return null;

            var entry = AudioEntry.Entries.Get((GrindlessID.AudioID)entryID);

            return entry?.musicSB;
        }

        internal static bool IsUniversalMusicBank(string bank)
        {
            if (bank == "UniversalMusic")
                return true;

            foreach (var mod in ModManager.Mods)
            {
                if (mod.Name == bank)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whenever the given WaveBank is persistent. <para/>
        /// Persistent WaveBanks are never unloaded.
        /// </summary>
        internal static bool IsUniversalMusicBank(WaveBank bank)
        {
            if (bank == null)
                return false;

            foreach (var mod in ModManager.Mods)
            {
                var entry = AudioEntry.Entries.Get(mod, "");

                if (entry != null && entry.universalWB == bank)
                    return true;
            }

            FieldInfo universalWaveBankField = typeof(SoundSystem).GetTypeInfo().GetField("universalMusicWaveBank", BindingFlags.NonPublic | BindingFlags.Instance);
            if (bank == universalWaveBankField.GetValue(Globals.Game.xSoundSystem))
                return true;

            return false;
        }
    }
}
