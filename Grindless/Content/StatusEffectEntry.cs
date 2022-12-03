using SoG;

namespace Grindless
{
    /// <summary>
    /// Represents a modded status effect.
    /// </summary>
    /// <remarks> 
    /// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
    /// </remarks>
    public class StatusEffectEntry : Entry<BaseStats.StatusEffectSource>
    {
        internal static EntryManager<BaseStats.StatusEffectSource, StatusEffectEntry> Entries { get; }
            = new EntryManager<BaseStats.StatusEffectSource, StatusEffectEntry>(1000);

        /// <summary>
        /// Gets or sets the icon's texture path. The texture path is relative to "Config/".
        /// </summary>
        public string TexturePath { get; set; }

        internal StatusEffectEntry()
        {
        }

        internal StatusEffectEntry(Mod mod, string modID, BaseStats.StatusEffectSource gameID)
            : base(mod, modID, gameID) { }

        protected override void Initialize()
        {
            // Nothing, texture is loaded on demand
        }

        protected override void Cleanup()
        {
            if (ModUtils.IsModContentPath(TexturePath))
            {
                AssetUtils.UnloadAsset(Globals.Game.Content, TexturePath);
            }
        }
    }
}
