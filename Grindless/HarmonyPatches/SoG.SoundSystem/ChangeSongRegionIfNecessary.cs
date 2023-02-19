using HarmonyLib;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using SoG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(SoundSystem), nameof(SoundSystem.ChangeSongRegionIfNecessary))]
    static class ChangeSongRegionIfNecessary
    {
        internal static readonly FieldInfo s_musicWaveBank = AccessTools.Field(typeof(SoundSystem), "musicWaveBank");
        internal static readonly FieldInfo s_loadedMusicWaveBank = AccessTools.Field(typeof(SoundSystem), "loadedMusicWaveBank");
        internal static readonly FieldInfo s_standbyWaveBanks = AccessTools.Field(typeof(SoundSystem), "dsxStandbyWaveBanks");
        internal static readonly FieldInfo s_songRegionMap = AccessTools.Field(typeof(SoundSystem), "dssSongRegionMap");
        internal static readonly FieldInfo s_universalMusic = AccessTools.Field(typeof(SoundSystem), "universalMusicWaveBank");
        internal static readonly FieldInfo s_audioEngine = AccessTools.Field(typeof(SoundSystem), "audioEngine");
        internal static readonly MethodInfo s_checkStandbyBanks = AccessTools.Method(typeof(SoundSystem), "CheckStandbyBanks");

        static bool Prefix(ref SoundSystem __instance, string sSongName)
        {
            SoundSystem system = __instance;

            var dsxStandbyWaveBanks = s_standbyWaveBanks.GetValue(system) as Dictionary<string, WaveBank>;
            var dssSongRegionMap = s_songRegionMap.GetValue(system) as Dictionary<string, string>;

            var universalMusic = s_universalMusic.GetValue(system) as WaveBank;
            var audioEngine = s_audioEngine.GetValue(system) as AudioEngine;

            bool currentIsModded = ModUtils.SplitAudioID(sSongName, out int entryID, out bool isMusic, out int cueID);

            if (currentIsModded && !isMusic)
                Program.Logger.LogWarning("Trying to play modded audio as music, but the audio isn't music! ID: {sSongName}", sSongName);

            Mod mod = null;
            AudioEntry entry = null;

            if (currentIsModded)
            {
                entry = AudioEntry.Entries.Get((GrindlessID.AudioID)entryID);
                mod = entry.Mod;
            }

            string nextBankName = currentIsModded ? entry.indexedMusicBanks[cueID] : dssSongRegionMap[sSongName];

            WaveBank currentMusicBank = s_musicWaveBank.GetValue(system) as WaveBank;

            if (_Helper.IsUniversalMusicBank(nextBankName))
            {
                if (currentIsModded && entry.universalWB == null)
                {
                    Program.Logger.LogError("{sSongName} requested modded UniversalMusic bank, but the bank does not exist!", sSongName);
                    return false;
                }

                if (currentMusicBank != null && !_Helper.IsUniversalMusicBank(currentMusicBank))
                    system.SetStandbyBank(system.sCurrentMusicWaveBank, currentMusicBank);

                s_musicWaveBank.SetValue(system, currentIsModded ? entry.universalWB : universalMusic);
            }
            else if (system.sCurrentMusicWaveBank != nextBankName)
            {
                if (currentMusicBank != null && !_Helper.IsUniversalMusicBank(currentMusicBank) && !currentMusicBank.IsDisposed)
                    system.SetStandbyBank(system.sCurrentMusicWaveBank, currentMusicBank);

                system.sCurrentMusicWaveBank = nextBankName;

                if (dsxStandbyWaveBanks.ContainsKey(nextBankName))
                {
                    s_musicWaveBank.SetValue(system, dsxStandbyWaveBanks[nextBankName]);
                    dsxStandbyWaveBanks.Remove(nextBankName);
                }
                else
                {
                    string root = Path.Combine(Globals.Game.Content.RootDirectory, currentIsModded ? mod.AssetPath : "");

                    s_loadedMusicWaveBank.SetValue(system, new WaveBank(audioEngine, Path.Combine(root, "Sound", $"{nextBankName}.xwb")));
                    s_musicWaveBank.SetValue(system, null);
                }
                system.xMusicVolumeMods.iMusicCueRetries = 0;
                system.xMusicVolumeMods.sSongInWait = sSongName;

                s_checkStandbyBanks.Invoke(system, new object[] { nextBankName });
            }
            else if (_Helper.IsUniversalMusicBank(currentMusicBank))
            {
                if (dsxStandbyWaveBanks.ContainsKey(system.sCurrentMusicWaveBank))
                {
                    s_musicWaveBank.SetValue(system, dsxStandbyWaveBanks[system.sCurrentMusicWaveBank]);
                    dsxStandbyWaveBanks.Remove(system.sCurrentMusicWaveBank);
                    return false;
                }

                string root = Path.Combine(Globals.Game.Content.RootDirectory, currentIsModded ? mod.AssetPath : "");
                string bankToUse = currentIsModded ? nextBankName : system.sCurrentMusicWaveBank;

                s_loadedMusicWaveBank.SetValue(system, new WaveBank(audioEngine, Path.Combine(root, "Sound", bankToUse + ".xwb")));
                s_musicWaveBank.SetValue(system, null);

                system.xMusicVolumeMods.iMusicCueRetries = 0;
                system.xMusicVolumeMods.sSongInWait = sSongName;
            }

            return false; // Never returns control to original
        }
    }
}
