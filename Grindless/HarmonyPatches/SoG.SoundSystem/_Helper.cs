using Microsoft.Xna.Framework.Audio;
using System.Reflection;

namespace Grindless.HarmonyPatches
{
    static class _Helper
    {
        internal static SoundBank GetEffectSoundBank(string audioID)
        {
            if (!AudioEntry.GSAudioID.TryParse(audioID, out var id) || id.IsMusic)
                return null;

            var entry = AudioEntry.Entries.Get((GrindlessID.AudioID)id.ModIndex);

            return entry?.EffectsSoundBank;
        }

        internal static string GetCueName(string GSID)
        {
            if (!AudioEntry.GSAudioID.TryParse(GSID, out var audioID))
                return "";

            var entry = AudioEntry.Entries.Get((GrindlessID.AudioID)audioID.ModIndex);

            return entry?.IDToCue[audioID];
        }

        internal static SoundBank GetMusicSoundBank(string audioID)
        {
            if (!AudioEntry.GSAudioID.TryParse(audioID, out var id) || !id.IsMusic)
                return null;

            var entry = AudioEntry.Entries.Get((GrindlessID.AudioID)id.ModIndex);

            return entry?.MusicSoundBank;
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

                if (entry != null && entry.MusicWaveBank == bank)
                    return true;
            }

            FieldInfo universalWaveBankField = typeof(SoundSystem).GetField("universalMusicWaveBank", BindingFlags.NonPublic | BindingFlags.Instance);
            if (bank == universalWaveBankField.GetValue(Globals.Game.xSoundSystem))
                return true;

            return false;
        }
    }
}
