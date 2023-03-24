using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Audio;
using System.Reflection;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(SoundSystem), nameof(SoundSystem.ChangeSongRegionIfNecessary))]
    static class ChangeSongRegionIfNecessary
    {
        class SoundSystemWrapper
        {
            static readonly FieldInfo s_musicWaveBank = AccessTools.Field(typeof(SoundSystem), "musicWaveBank");
            static readonly FieldInfo s_loadedMusicWaveBank = AccessTools.Field(typeof(SoundSystem), "loadedMusicWaveBank");
            static readonly FieldInfo s_standbyWaveBanks = AccessTools.Field(typeof(SoundSystem), "dsxStandbyWaveBanks");
            static readonly FieldInfo s_songRegionMap = AccessTools.Field(typeof(SoundSystem), "dssSongRegionMap");
            static readonly FieldInfo s_universalMusic = AccessTools.Field(typeof(SoundSystem), "universalMusicWaveBank");
            static readonly FieldInfo s_audioEngine = AccessTools.Field(typeof(SoundSystem), "audioEngine");
            static readonly MethodInfo s_checkStandbyBanks = AccessTools.Method(typeof(SoundSystem), "CheckStandbyBanks");

            public SoundSystem System { get; }

            public SoundSystemWrapper(SoundSystem system)
            {
                System = system;
            }

            public WaveBank MusicWaveBank
            {
                get => s_musicWaveBank.GetValue(System) as WaveBank;
                set => s_musicWaveBank.SetValue(System, value);
            }

            public WaveBank LoadedMusicWaveBank
            {
                get => s_loadedMusicWaveBank.GetValue(System) as WaveBank;
                set => s_loadedMusicWaveBank.SetValue(System, value);
            }

            public Dictionary<string, WaveBank> StandbyWaveBanks
            {
                get => s_standbyWaveBanks.GetValue(System) as Dictionary<string, WaveBank>;
                set => s_standbyWaveBanks.SetValue(System, value);
            }

            public Dictionary<string, string> SongRegionMap
            {
                get => s_songRegionMap.GetValue(System) as Dictionary<string, string>;
                set => s_songRegionMap.SetValue(System, value);
            }
            
            public WaveBank UniversalMusic
            {
                get => s_universalMusic.GetValue(System) as WaveBank;
                set => s_universalMusic.SetValue(System, value);
            }

            public AudioEngine AudioEngine
            {
                get => s_audioEngine.GetValue(System) as AudioEngine;
                set => s_audioEngine.SetValue(System, value);
            }

            public void CheckStandbyBanks(string nextBankName)
            {
                s_checkStandbyBanks.Invoke(System, new object[] { nextBankName });
            }
        }

        static bool Prefix(ref SoundSystem __instance, string sSongName)
        {
            SoundSystemWrapper wSystem = new(__instance);

            AudioEntry.GSAudioID audioID = default;
            bool currentIsModded = AudioEntry.GSAudioID.TryParse(sSongName, out audioID);

            AudioEntry entry = null;

            if (currentIsModded)
            {
                if (!audioID.IsMusic)
                {
                    Program.Logger.LogWarning("Trying to play modded audio as music, but the audio isn't music! ID: {sSongName}", sSongName);
                    return false;
                }

                entry = AudioEntry.Entries.Get((GrindlessID.AudioID)audioID.ModIndex);

                if (!entry.IDToCue.ContainsKey(audioID))
                {
                    Program.Logger.LogWarning("Trying to play unknown mod music! ID: {sSongName}", sSongName);
                    return false;
                }
            }

            string nextBankName = currentIsModded ? entry.CueToWaveBank[entry.IDToCue[audioID]] : wSystem.SongRegionMap[sSongName];

            WaveBank currentMusicBank = wSystem.MusicWaveBank;

            if (_Helper.IsUniversalMusicBank(nextBankName))
            {
                if (currentIsModded && entry.MusicWaveBank == null)
                {
                    Program.Logger.LogError("{sSongName} requested modded UniversalMusic bank, but the bank does not exist!", sSongName);
                    return false;
                }

                if (currentMusicBank != null && !_Helper.IsUniversalMusicBank(currentMusicBank))
                    wSystem.System.SetStandbyBank(wSystem.System.sCurrentMusicWaveBank, currentMusicBank);

                wSystem.MusicWaveBank = currentIsModded ? entry.MusicWaveBank : wSystem.UniversalMusic;
            }
            else if (wSystem.System.sCurrentMusicWaveBank != nextBankName)
            {
                if (currentMusicBank != null && !_Helper.IsUniversalMusicBank(currentMusicBank) && !currentMusicBank.IsDisposed)
                    wSystem.System.SetStandbyBank(wSystem.System.sCurrentMusicWaveBank, currentMusicBank);

                wSystem.System.sCurrentMusicWaveBank = nextBankName;

                if (wSystem.StandbyWaveBanks.ContainsKey(nextBankName))
                {
                    wSystem.MusicWaveBank = wSystem.StandbyWaveBanks[nextBankName];
                    wSystem.StandbyWaveBanks.Remove(nextBankName);
                }
                else
                {
                    string root = Path.Combine(Globals.Game.Content.RootDirectory, currentIsModded ? entry.Mod.AssetPath : "");

                    wSystem.LoadedMusicWaveBank = new WaveBank(wSystem.AudioEngine, Path.Combine(root, "Sound", $"{nextBankName}.xwb"));
                    wSystem.MusicWaveBank = null;
                }
                wSystem.System.xMusicVolumeMods.iMusicCueRetries = 0;
                wSystem.System.xMusicVolumeMods.sSongInWait = sSongName;

                wSystem.CheckStandbyBanks(nextBankName);
            }
            else if (_Helper.IsUniversalMusicBank(currentMusicBank))
            {
                if (wSystem.StandbyWaveBanks.ContainsKey(wSystem.System.sCurrentMusicWaveBank))
                {
                    wSystem.MusicWaveBank = wSystem.StandbyWaveBanks[wSystem.System.sCurrentMusicWaveBank];
                    wSystem.StandbyWaveBanks.Remove(wSystem.System.sCurrentMusicWaveBank);
                    return false;
                }

                string root = Path.Combine(Globals.Game.Content.RootDirectory, currentIsModded ? entry.Mod.AssetPath : "");
                string bankToUse = currentIsModded ? nextBankName : wSystem.System.sCurrentMusicWaveBank;

                wSystem.LoadedMusicWaveBank = new WaveBank(wSystem.AudioEngine, Path.Combine(root, "Sound", bankToUse + ".xwb"));
                wSystem.MusicWaveBank = null;

                wSystem.System.xMusicVolumeMods.iMusicCueRetries = 0;
                wSystem.System.xMusicVolumeMods.sSongInWait = sSongName;
            }

            return false; // Never returns control to original
        }
    }
}
