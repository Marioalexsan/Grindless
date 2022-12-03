using HarmonyLib;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.IO;
using SoG;
using Microsoft.Extensions.Logging;

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
        internal static EntryManager<GrindlessID.AudioID, AudioEntry> Entries { get; }
            = new EntryManager<GrindlessID.AudioID, AudioEntry>(0);

        internal static Dictionary<string, string> VanillaMusicRedirects { get; } = new Dictionary<string, string>();

        internal HashSet<string> effectCueNames = new HashSet<string>();

        internal Dictionary<string, HashSet<string>> musicCueNames = new Dictionary<string, HashSet<string>>();

        internal SoundBank effectsSB; // "<Mod>Effects.xsb"

        internal WaveBank effectsWB; // "<Mod>Music.xwb"

        internal SoundBank musicSB; //"<Mod>Music.xsb"

        internal WaveBank universalWB; // "<Mod>.xwb", never unloaded

        internal List<string> indexedEffectCues = new List<string>();

        internal List<string> indexedMusicCues = new List<string>();

        internal List<string> indexedMusicBanks = new List<string>();

        /// <summary>
        /// Adds effect cues for this mod. <para/>
        /// </summary>
        /// <param name="effects"> A list of effect names to add. </param>
        public void AddEffects(params string[] effects)
        {
            ErrorHelper.ThrowIfNotLoading(Mod);

            foreach (var audio in effects)
                effectCueNames.Add(audio);
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

            var setToUpdate = musicCueNames.TryGetValue(bankName, out var set) ? set : musicCueNames[bankName] = new HashSet<string>();

            foreach (var audio in music)
                setToUpdate.Add(audio);
        }

        /// <summary>
        /// Removes effect cues from this mod.
        /// </summary>
        /// <remarks>
        /// This method can only be used inside <see cref="Mod.Load"/>.
        /// </remarks>
        /// <param name="effects"> A list of effect names to remove. </param>
        public void RemoveEffects(params string[] effects)
        {
            ErrorHelper.ThrowIfNotLoading(Mod);

            foreach (var audio in effects)
                effectCueNames.Remove(audio);
        }

        /// <summary>
        /// Removes music cues from this mod.
        /// </summary>
        /// <remarks>
        /// This method can only be used inside <see cref="Mod.Load"/>.
        /// </remarks>
        /// <param name="bankName"> The wave bank name containing the music (without the ".xnb" extension). </param>
        /// <param name="music"> A list of music names to remove. </param>
        public void RemoveMusic(string bankName, params string[] music)
        {
            ErrorHelper.ThrowIfNotLoading(Mod);

            if (!musicCueNames.TryGetValue(bankName, out var set))
            {
                return;
            }

            foreach (var audio in music)
                set.Remove(audio);

            musicCueNames.Remove(bankName);
        }


        /// <summary>
        /// Gets the ID of the effect that has the given name. <para/>
        /// This ID can be used to play effects with methods such as <see cref="SoundSystem.PlayCue"/>.
        /// </summary>
        /// <returns> An identifier that can be used to play the effect using vanilla methods. </returns>
        public string GetEffectID(string effectName)
        {
            ErrorHelper.ThrowIfLoading(Mod);

            for (int i = 0; i < indexedEffectCues.Count; i++)
            {
                if (indexedEffectCues[i] == effectName)
                {
                    return $"GS_{(int)GameID}_S{i}";
                }
            }

            return "";
        }

        /// <summary>
        /// Gets the ID of the music that has the given cue name. <para/>
        /// This ID can be used to play music with <see cref="SoundSystem.PlaySong"/>.
        /// </summary>
        /// <returns> An identifier that can be used to play music using vanilla methods. </returns>
        public string GetMusicID(string musicName)
        {
            ErrorHelper.ThrowIfLoading(Mod);

            for (int i = 0; i < indexedMusicCues.Count; i++)
            {
                if (indexedMusicCues[i] == musicName)
                    return $"GS_{(int)GameID}_M{i}";
            }

            return "";
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

            if (!songRegionMap.ContainsKey(vanillaName))
            {
                Program.Logger.LogWarning("Invalid music redirect {vanillaName} -> {modID}.", vanillaName, musicName);
                return;
            }

            bool isModded = ModUtils.SplitAudioID(musicName, out int entryID, out bool isMusic, out int cueID);

            var entry = Entries.Get((GrindlessID.AudioID)entryID);

            string cueName = null;

            if (entry != null && cueID >= 0 && cueID < entry.indexedMusicCues.Count)
            {
                cueName = entry.indexedMusicCues[cueID];
            }

            if (!(musicName == "" || isModded && isMusic && cueName != null))
            {
                Program.Logger.LogWarning("Invalid music redirect {vanillaName} -> {modID}.", vanillaName, musicName);
                return;
            }

            if (musicName == "")
            {
                Program.Logger.LogWarning("Removed music redirect for {vanillaName}.", vanillaName);
                VanillaMusicRedirects.Remove(vanillaName);
            }
            else
            {
                Program.Logger.LogWarning("Set music redirect {vanillaName} -> {modID} ({cueName})", vanillaName, musicName, cueName);
                VanillaMusicRedirects[vanillaName] = musicName;
            }
        }

        internal AudioEntry() { }

        protected override void Initialize()
        {
            AudioEngine audioEngine = AccessTools.Field(typeof(SoundSystem), "audioEngine").GetValue(Globals.Game.xSoundSystem) as AudioEngine;

            indexedEffectCues.AddRange(effectCueNames);

            foreach (var kvp in musicCueNames)
            {
                string bankName = kvp.Key;

                foreach (var music in kvp.Value)
                {
                    indexedMusicBanks.Add(bankName);
                    indexedMusicCues.Add(music);
                }
            }

            string root = Path.Combine(Globals.Game.Content.RootDirectory, Mod.AssetPath);

            // Non-unique sound / wave banks will cause audio conflicts
            // This is why the file paths are set in stone
            AssetUtils.TryLoadWaveBank(Path.Combine(root, "Sound", Mod.Name + "Effects.xwb"), audioEngine, out effectsWB);
            AssetUtils.TryLoadSoundBank(Path.Combine(root, "Sound", Mod.Name + "Effects.xsb"), audioEngine, out effectsSB);
            AssetUtils.TryLoadSoundBank(Path.Combine(root, "Sound", Mod.Name + "Music.xsb"), audioEngine, out musicSB);
            AssetUtils.TryLoadWaveBank(Path.Combine(root, "Sound", Mod.Name + ".xwb"), audioEngine, out universalWB);
        }

        protected override void Cleanup()
        {
            effectsSB?.Dispose();

            effectsWB?.Dispose();

            musicSB?.Dispose();

            universalWB?.Dispose();
        }
    }
}
