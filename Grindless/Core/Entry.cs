namespace Grindless
{
    /// <summary>
    /// Represents a modded game object.
    /// </summary>
    public abstract class Entry<IDType> where IDType : struct, Enum
    {
        public Entry()
        {

        }

        public Entry(Mod mod, string modID, IDType gameID)
        {
            Mod = mod;
            ModID = modID;
            GameID = gameID;
        }

        /// <summary>
        /// Gets the mod that created this entry.
        /// </summary>
        public Mod Mod { get; internal set; }

        /// <summary>
        /// Gets the mod ID of this entry.
        /// </summary>
        public string ModID { get; internal set; }

        /// <summary>
        /// Gets the vanilla ID of this entry.
        /// </summary>
        public IDType GameID { get; internal set; }

        public bool Ready { get; internal set; } = false;

        public bool IsVanilla => Mod.GetType() == typeof(VanillaMod);

        public bool IsModded => !IsVanilla && Mod != null;

        public bool IsUnknown => !(IsVanilla || IsModded);

        internal void InitializeEntry()
        {
            if (!Ready)
            {
                Initialize();
                Ready = true;
            }
        }

        internal void CleanupEntry()
        {
            if (Ready)
            {
                Cleanup();
                Ready = false;
            }
        }

        protected abstract void Initialize();

        protected abstract void Cleanup();
    }
}