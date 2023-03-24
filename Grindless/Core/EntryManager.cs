using System.Collections;

namespace Grindless
{
    internal interface IEntryManager
    {
        int Count { get; }
        void Reset();
        void InitializeEntries(Mod specificMod);
        void CleanupEntries(Mod specificMod);
    }

    internal class EntryManager<IDType, EntryType> : IEntryManager, IEnumerable<EntryType>
        where IDType : struct, Enum
        where EntryType : Entry<IDType>
    {
        private readonly Dictionary<IDType, EntryType> _entries = new();

        private IDType _next;

        public IDType Start { get; }
        public IDType End { get; }

        public int Count => _entries.Count;

        public EntryManager(long start) : this(start, 1000) { }

        public EntryManager(long start, long count)
        {
            Start = (IDType)Enum.ToObject(typeof(IDType), start);
            End = (IDType)Enum.ToObject(typeof(IDType), start + count);
            _next = Start;
        }

        public EntryType Create(Mod mod, string modID)
        {
            if (_next.Equals(End))
                throw new InvalidOperationException("Cannot register any more objects of type " + typeof(EntryType).Name);

            if (Get(mod, modID) != null)
                throw new InvalidOperationException(ErrorHelper.DuplicateModID);

            if (!mod.InLoad)
                throw new InvalidOperationException(ErrorHelper.UseThisDuringLoad);

            if (mod.DisableObjectCreation)
                throw new InvalidOperationException(ErrorHelper.ObjectCreationDisabled);

            var gameID = _next;
            _next = (IDType)Enum.ToObject(typeof(IDType), Convert.ToInt64(_next) + 1);

            if (Get(_next) != null)
                throw new InvalidOperationException("Object allocation failed: an object with ID " + _next + " already exists.");

            var entry = (EntryType)Activator.CreateInstance(typeof(EntryType), true);

            entry.Mod = mod;
            entry.ModID = modID;
            entry.GameID = gameID;

            return _entries[gameID] = entry;
        }

        public EntryType CreateFromExisting(EntryType entry)
        {
            if (Get(entry.GameID) != null)
                throw new InvalidOperationException("An entry with id " + entry.GameID + " already exists.");

            if (Get(entry.Mod, entry.ModID) != null)
                throw new InvalidOperationException("An entry from mod " + entry.Mod.Name + " with modID " + entry.ModID + " already exists.");

            return _entries[entry.GameID] = entry;
        }

        public EntryType Get(IDType gameID)
        {
            _entries.TryGetValue(gameID, out var value);
            return value;
        }

        public EntryType Get(Mod mod, string modID)
        {
            return _entries.Values.FirstOrDefault(x => x.Mod == mod && x.ModID == modID);
        }

        public EntryType GetRequired(IDType gameID) => Get(gameID) ?? throw new InvalidOperationException($"Couldn't retrieve entry {gameID}");

        public EntryType GetRequired(Mod mod, string modID) => Get(mod, modID) ?? throw new InvalidOperationException($"Couldn't retrieve entry {mod.Name}:{modID}");

        public void Reset()
        {
            CleanupEntries(null);
            _entries.Clear();
            _next = Start;
        }

        public IEnumerator<EntryType> GetEnumerator() => _entries.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _entries.Values.GetEnumerator();

        public void InitializeEntries(Mod specificMod)
        {
            var entries = _entries.Values.AsEnumerable();

            if (specificMod != null)
                entries = entries.Where(x => x.Mod == specificMod);

            foreach (var entry in entries)
                entry.InitializeEntry();
        }

        public void CleanupEntries(Mod specificMod)
        {
            var entries = _entries.Values.AsEnumerable();

            if (specificMod != null)
                entries = entries.Where(x => x.Mod == specificMod);

            foreach (var entry in entries)
                entry.CleanupEntry();
        }
    }
}
