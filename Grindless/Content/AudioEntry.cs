using HarmonyLib;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.IO;
using SoG;
using Microsoft.Extensions.Logging;
using static SoG.HitEffectMap;
using System;
using System.Linq;

namespace Grindless
{
    /// <summary>
    /// Contains data for playing custom audio files inside the game.
    /// Each mod can have at most one audio entry. <para/>
    /// Audio files are loaded from a preset "Sound" folder inside the mod's content path. <para/>
    /// You can use XACT3 to generate wave banks and sound banks to use in your mods. <para/>
    /// </summary>
    /// <remarks>
    /// Grindless uses preset wave bank and sound bank names to load audio data.
    /// Depending on what you do, you will need the following wave banks in the "Sound" folder:
    /// <list type="bullet">
    ///     <item>
    ///         <term>{<see cref="Mod.Name"/>}Effects.xwb</term>
    ///         <description>The wave bank used for effects</description>
    ///     </item>
    ///     <item>
    ///         <term>{<see cref="Mod.Name"/>}Effects.xsb</term>
    ///         <description>The sound bank used for effects</description>
    ///     </item>
    ///     <item>
    ///         <term>{<see cref="Mod.Name"/>}.xwb</term>
    ///         <description>The universal ("never unload") music wave bank</description>
    ///     </item>
    ///     <item>
    ///         <term>{<see cref="Mod.Name"/>}Music.xsb</term>
    ///         <description>The sound bank used for music</description>
    ///     </item>
    ///     <item>
    ///         <term>Region wave banks</term>
    ///         <description>
    ///             These have the same name as the wave bank names specified in <see cref="AddMusic"/>.
    ///             Each wave bank represents a distinct region that is loaded and unloaded on demand.
    ///         </description>
    ///     </item>
    /// </list> 
    /// </remarks>
    /// <remarks> 
    /// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
    /// </remarks>
    public class AudioEntry : Entry<GrindlessID.AudioID>
    {
        public readonly struct GSAudioID
        {
            public GSAudioID(int modIndex, int cueID, bool isMusic)
            {
                ModIndex = modIndex;
                AudioIndex = cueID;
                IsMusic = isMusic;
            }

            public readonly int ModIndex;
            public readonly int AudioIndex;
            public readonly bool IsMusic;

            public static bool TryParse(string str, out GSAudioID audioID)
            {
                audioID = default;

                if (!str.StartsWith("GS_"))
                    return false;

                string[] words = str.Remove(0, 3).Split('_');

                if (words.Length != 2 || !(words[1][0] == 'M' || words[1][0] == 'S'))
                    return false;

                if (!int.TryParse(words[0], out int modIndex))
                    return false;

                if (!int.TryParse(words[1].Substring(1), out int cueID))
                    return false;

                audioID = new GSAudioID(modIndex, cueID, words[1][0] == 'M');
                return true;
            }

            public override string ToString() => $"GS_{ModIndex}_{(IsMusic ? 'M' : 'S')}{AudioIndex}";
        }

        internal static EntryManager<GrindlessID.AudioID, AudioEntry> Entries { get; }
            = new EntryManager<GrindlessID.AudioID, AudioEntry>(0);

        internal static Dictionary<string, string> VanillaMusicRedirects { get; } = new Dictionary<string, string>();

        internal SoundBank EffectsSoundBank; // "<Mod>Effects.xsb"
        internal WaveBank EffectsWaveBank; // "<Mod>Music.xwb"
        internal SoundBank MusicSoundBank; //"<Mod>Music.xsb"
        internal WaveBank MusicWaveBank; // "<Mod>.xwb", never unloaded

        internal Dictionary<GSAudioID, string> IDToCue { get; } = new();
        internal Dictionary<string, string> CueToWaveBank { get; } = new();
        internal int NextID = 0;

        /// <summary>
        /// Adds effect cues for this mod. <para/>
        /// </summary>
        /// <param name="effects"> A list of effect names to add. </param>
        public void AddEffects(params string[] effects)
        {
            ErrorHelper.ThrowIfNotLoading(Mod);

            foreach (var audio in effects)
                IDToCue[new GSAudioID((int)GameID, NextID++, false)] = audio;
        }

        /// <summary>
        /// Adds music cues for this mod. The cues be loaded using the given wave bank.
        /// Keep in mind that the universal music wave bank follows a "never unload" policy.
        /// </summary>
        /// <remarks>
        /// This method can only be used inside <see cref="Mod.Load"/>.
        /// </remarks>
        /// <param name="bankName"> The wave bank name containing the music (without the ".xnb" extension). </param>
        /// <param name="music"> A list of music names to add. </param>
        public void AddMusic(string bankName, params string[] music)
        {
            ErrorHelper.ThrowIfNotLoading(Mod);

            foreach (var audio in music)
            {
                IDToCue[new GSAudioID((int)GameID, NextID++, false)] = audio;
                CueToWaveBank[audio] = bankName;
            }
        }

        /// <summary>
        /// Gets the ID of the effect that has the given name. <para/>
        /// This ID can be used to play effects with methods such as <see cref="SoundSystem.PlayCue"/>.
        /// </summary>
        /// <returns> An identifier that can be used to play the effect using vanilla methods. </returns>
        public GSAudioID? GetEffectID(string effectName)
        {
            try
            {
                return IDToCue.First(x => x.Value == effectName && !x.Key.IsMusic).Key;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the ID of the music that has the given cue name. <para/>
        /// This ID can be used to play music with <see cref="SoundSystem.PlaySong"/>.
        /// </summary>
        /// <returns> An identifier that can be used to play music using vanilla methods. </returns>
        public GSAudioID? GetMusicID(string musicName)
        {
            try
            {
                return IDToCue.First(x => x.Value == musicName && x.Key.IsMusic).Key;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Redirects a vanilla music to a modded music name - whenever vanilla music would play
        /// normally, the modded music will play instead.
        /// </summary>
        /// <remarks>
        /// This method cannot be used during <see cref="Mod.Load"/>.
        /// Use it somewhere else, such as <see cref="Mod.PostLoad"/>.
        /// </remarks>
        /// <param name="vanillaName"> The vanilla music to redirect from. </param>
        /// <param name="musicName"> The modded music to redirect to. </param>
        public void RedirectVanillaMusic(string vanillaName, string musicName)
        {
            ErrorHelper.ThrowIfLoading(Mod);

            var songRegionMap = AccessTools.Field(typeof(SoundSystem), "dssSongRegionMap")
                .GetValue(Globals.Game.xSoundSystem) as Dictionary<string, string>;

            if (musicName == "")
            {
                Program.Logger.LogWarning("Removed music redirect for {vanillaName}.", vanillaName);
                VanillaMusicRedirects.Remove(vanillaName);
                return;
            }

            if (!songRegionMap.ContainsKey(vanillaName))
            {
                Program.Logger.LogWarning("Invalid music redirect {vanillaName} -> {modID}.", vanillaName, musicName);
                return;
            }

            if (!GSAudioID.TryParse(musicName, out GSAudioID id) || !id.IsMusic)
            {
                Program.Logger.LogWarning("Invalid music redirect {vanillaName} -> {modID}.", vanillaName, musicName);
                return;
            }

            var entry = Entries.Get((GrindlessID.AudioID)id.ModIndex);

            if (entry == null || !entry.IDToCue.TryGetValue(id, out string cueName))
            {
                Program.Logger.LogWarning("Invalid music redirect {vanillaName} -> {modID}.", vanillaName, musicName);
                return;
            }

            Program.Logger.LogWarning("Set music redirect {vanillaName} -> {modID} ({effectName})", vanillaName, musicName, cueName);
            VanillaMusicRedirects[vanillaName] = musicName;
        }

        internal AudioEntry() { }

        protected override void Initialize()
        {
            AudioEngine audioEngine = AccessTools.Field(typeof(SoundSystem), "audioEngine").GetValue(Globals.Game.xSoundSystem) as AudioEngine;

            string root = Path.Combine(Globals.Game.Content.RootDirectory, Mod.AssetPath, "Sound");
            string name = Mod.Name;

            // Non-unique sound / wave banks will cause audio conflicts
            // This is why the file paths are set in stone
            EffectsWaveBank = audioEngine.TryLoadWaveBank(Path.Combine(root, $"{name}Effects.xwb"));
            EffectsSoundBank = audioEngine.TryLoadSoundBank(Path.Combine(root, $"{name}Effects.xsb"));
            MusicSoundBank = audioEngine.TryLoadSoundBank(Path.Combine(root, $"{name}Music.xsb"));
            MusicWaveBank = audioEngine.TryLoadWaveBank(Path.Combine(root, $"{name}.xwb"));
        }

        protected override void Cleanup()
        {
            EffectsSoundBank?.Dispose();
            EffectsWaveBank?.Dispose();
            MusicSoundBank?.Dispose();
            MusicWaveBank?.Dispose();
        }
    }
}
