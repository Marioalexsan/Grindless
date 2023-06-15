using Quests;
using System.Reflection;
using static SoG.IEntity;

namespace Grindless;

/// <summary>
/// Represents a modded game object.
/// </summary>
public abstract class Entry<IDType> where IDType : struct, Enum
{
    internal Entry() { }

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

internal static class Entries
{
    private static void CheckInit()
    {
        if (_managers.Count == 0)
        {
            Type[] _managerConstructorTypes = new[]
            {
                typeof(long),
                typeof(long)
            };

            // Create managers for all registered entries in the assembly
            var entryTypes = typeof(ModManager).Assembly.DefinedTypes
                .Select(x => (x, x.GetCustomAttribute<ModEntryAttribute>()))
                .Where(x => x.Item2 != null);

            foreach ((var entryType, var attr) in entryTypes)
            {
                var type = typeof(EntryManager<,>).MakeGenericType(entryType.BaseType.GenericTypeArguments[0], entryType);

                _managers[type] = (IEntryManager)type.GetConstructor(_managerConstructorTypes).Invoke(new object[] { attr.Start, attr.Count });
            }
        }

    }

    private static readonly Dictionary<Type, IEntryManager> _managers = new();

    internal static EntryManager<IDType, EntryType> Manager<IDType, EntryType>()
        where IDType : struct, Enum
        where EntryType : Entry<IDType>
    {
        CheckInit();
        return _managers[typeof(EntryManager<IDType, EntryType>)] as EntryManager<IDType, EntryType>;
    }

    internal static IEnumerable<IEntryManager> Managers()
    {
        CheckInit();
        return _managers.Values;
    }

    public static EntryManager<GrindlessID.AudioID, AudioEntry> Audio
        => Manager<GrindlessID.AudioID, AudioEntry>();

    public static EntryManager<GrindlessID.CommandID, CommandEntry> Commands
        => Manager<GrindlessID.CommandID, CommandEntry>();

    public static EntryManager<RogueLikeMode.TreatsCurses, CurseEntry> Curses
        => Manager<RogueLikeMode.TreatsCurses, CurseEntry>();

    public static EntryManager<EnemyCodex.EnemyTypes, EnemyEntry> Enemies
        => Manager<EnemyCodex.EnemyTypes, EnemyEntry>();

    public static EntryManager<EquipmentInfo.SpecialEffect, EquipmentEffectEntry> EquipmentEffects
        => Manager<EquipmentInfo.SpecialEffect, EquipmentEffectEntry>();

    public static EntryManager<ItemCodex.ItemTypes, ItemEntry> Items
        => Manager<ItemCodex.ItemTypes, ItemEntry>();

    public static EntryManager<Level.ZoneEnum, LevelEntry> Levels
        => Manager<Level.ZoneEnum, LevelEntry>();

    public static EntryManager<GrindlessID.NetworkID, NetworkEntry> Network
        => Manager<GrindlessID.NetworkID, NetworkEntry>();

    public static EntryManager<RogueLikeMode.Perks, PerkEntry> Perks
        => Manager<RogueLikeMode.Perks, PerkEntry>();

    public static EntryManager<PinCodex.PinType, PinEntry> Pins
        => Manager<PinCodex.PinType, PinEntry>();

    public static EntryManager<QuestCodex.QuestID, QuestEntry> Quests
        => Manager<QuestCodex.QuestID, QuestEntry>();

    public static EntryManager<SpellCodex.SpellTypes, SpellEntry> Spells
        => Manager<SpellCodex.SpellTypes, SpellEntry>();

    public static EntryManager<BaseStats.StatusEffectSource, StatusEffectEntry> StatusEffects
        => Manager<BaseStats.StatusEffectSource, StatusEffectEntry>();

    public static EntryManager<Level.WorldRegion, WorldRegionEntry> WorldRegions
        => Manager<Level.WorldRegion, WorldRegionEntry>();
}
