using Quests;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace Grindless;

public class ModInfo
{
    public string Name { get; set; }
    public Version Version { get; set; }

    public Dictionary<string, ItemCodex.ItemTypes> ItemIDMap { get; set; } = new Dictionary<string, ItemCodex.ItemTypes>();
    public Dictionary<string, EnemyCodex.EnemyTypes> EnemyIDMap { get; set; } = new Dictionary<string, EnemyCodex.EnemyTypes>();
    public Dictionary<string, QuestCodex.QuestID> QuestIDMap { get; set; } = new Dictionary<string, QuestCodex.QuestID>();
    public Dictionary<string, RogueLikeMode.Perks> PerkIDMap { get; set; } = new Dictionary<string, RogueLikeMode.Perks>();
    public Dictionary<string, RogueLikeMode.TreatsCurses> CurseIDMap { get; set; } = new Dictionary<string, RogueLikeMode.TreatsCurses>();
}

public class MetadataFile
{
    public Version GrindlessVersion { get; set; }
    public string GrindeaVersion { get; set; }

    public List<ModInfo> Mods { get; set; } = new List<ModInfo>();
}

public struct ModShortInfo
{
    public string Name { get; set; }
    public Version Version { get; set; }
}

public class SaveCompatibility
{
    public bool IsCompatible
    {
        get
        {
            return
                ModsAdded.Count == 0 &&
                ModsRemoved.Count == 0 &&
                LastGrindlessVersion.Major == Globals.GrindlessVersion.Major &&
                LastGrindeaVersion == Globals.GrindeaVersion;
        }
    }

    public Version LastGrindlessVersion { get; set; }

    public string LastGrindeaVersion { get; set; }

    public List<ModShortInfo> ModsAdded { get; set; }

    public List<ModShortInfo> ModsRemoved { get; set; }

    public List<ModShortInfo> ModsUpdated { get; set; }

    public string GetCompatibilityText()
    {
        if (IsCompatible)
            return "Save is compatible.";

        var builder = new StringBuilder();

        builder.AppendLine("Save is not compatible:");
        builder.AppendLine();

        foreach (var added in ModsAdded)
        {
            builder.Append("[+] ");
            builder.Append(added.Name);
            builder.Append(' ');
            builder.AppendLine(added.Version.ToString());
        }

        foreach (var updated in ModsUpdated)
        {
            builder.Append("[~] ");
            builder.Append(updated.Name);
            builder.Append(' ');
            builder.AppendLine(updated.Version.ToString());
        }

        foreach (var removed in ModsRemoved)
        {
            builder.Append("[-] ");
            builder.Append(removed.Name);
            builder.Append(' ');
            builder.AppendLine(removed.Version.ToString());
        }

        return builder.ToString();
    }
}

internal static class ModSaving
{
    public const string SaveFileExtension = ".gs";

    public static SaveCompatibility CheckCompatibility(Stream stream)
    {
        MetadataFile file = LoadMetadataFile(stream);

        var loadedMods = ModManager.Mods.Where(x => !x.IsBuiltin)
            .Select(x => new ModShortInfo() { Name = x.Name, Version = x.Version })
            .ToList();

        var saveMods = file.Mods;

        var missingMods = saveMods
            .Where(x => !loadedMods.Any(y => y.Name == x.Name))
            .Select(x => new ModShortInfo() { Name = x.Name, Version = x.Version })
            .ToList();

        var newMods = loadedMods
            .Where(x => !saveMods.Any(y => y.Name == x.Name))
            .ToList();

        var updatedMods = loadedMods
            .Where(x => saveMods.Any(y => y.Name == x.Name && y.Version.Major != x.Version.Major))
            .ToList();

        return new SaveCompatibility()
        {
            LastGrindlessVersion = file.GrindlessVersion,
            LastGrindeaVersion = file.GrindeaVersion,
            ModsAdded = newMods,
            ModsRemoved = missingMods,
            ModsUpdated = updatedMods
        };
    }

    public static void SaveModCharacter(Stream file)
    {
        var metadata = new MetadataFile()
        {
            GrindlessVersion = GrindlessMod.Instance.Version,
            GrindeaVersion = Globals.GrindeaVersion,
            Mods = ModManager.Mods.Where(mod => !mod.IsBuiltin).Select(mod => new ModInfo()
            {
                Name = mod.Name,
                Version = mod.Version,
                ItemIDMap = ItemEntry.Entries.Where(x => x.Mod == mod).ToDictionary(x => x.ModID, x => x.GameID),
                EnemyIDMap = EnemyEntry.Entries.Where(x => x.Mod == mod).ToDictionary(x => x.ModID, x => x.GameID),
                QuestIDMap = QuestEntry.Entries.Where(x => x.Mod == mod).ToDictionary(x => x.ModID, x => x.GameID)
            }).ToList()
        };

        SaveMetadataFile(file, metadata);
    }

    public static void LoadModCharacter(Stream file)
    {
        ApplyMetadataFile(LoadMetadataFile(file));
    }

    public static void SaveModWorld(Stream file)
    {
        var metadata = new MetadataFile()
        {
            GrindlessVersion = GrindlessMod.Instance.Version,
            GrindeaVersion = Globals.GrindeaVersion,
            Mods = ModManager.Mods.Where(mod => !mod.IsBuiltin).Select(mod => new ModInfo()
            {
                Name = mod.Name,
                Version = mod.Version,
                QuestIDMap = QuestEntry.Entries.Where(x => x.Mod == mod).ToDictionary(x => x.ModID, x => x.GameID)
            }).ToList()
        };

        SaveMetadataFile(file, metadata);
    }

    public static void LoadModWorld(Stream file)
    {
        ApplyMetadataFile(LoadMetadataFile(file));
    }

    public static void SaveModArcade(Stream file)
    {
        var metadata = new MetadataFile()
        {
            GrindlessVersion = GrindlessMod.Instance.Version,
            GrindeaVersion = Globals.GrindeaVersion,
            Mods = ModManager.Mods.Where(mod => !mod.IsBuiltin).Select(mod => new ModInfo()
            {
                Name = mod.Name,
                Version = mod.Version,
                ItemIDMap = ItemEntry.Entries.Where(x => x.Mod == mod).ToDictionary(x => x.ModID, x => x.GameID),
                PerkIDMap = PerkEntry.Entries.Where(x => x.Mod == mod).ToDictionary(x => x.ModID, x => x.GameID),
                CurseIDMap = CurseEntry.Entries.Where(x => x.Mod == mod).ToDictionary(x => x.ModID, x => x.GameID),
                EnemyIDMap = EnemyEntry.Entries.Where(x => x.Mod == mod).ToDictionary(x => x.ModID, x => x.GameID)
            }).ToList()
        };

        SaveMetadataFile(file, metadata);
    }

    public static void LoadModArcade(Stream file)
    {
        ApplyMetadataFile(LoadMetadataFile(file));
    }

    private static void ApplyMetadataFile(MetadataFile file)
    {
        ItemCodex.ItemTypes itemShuffleID = ItemEntry.Entries.End;
        EnemyCodex.EnemyTypes enemyShuffleID = EnemyEntry.Entries.End;
        QuestCodex.QuestID questShuffleID = QuestEntry.Entries.End;
        RogueLikeMode.Perks perkShuffleID = PerkEntry.Entries.End;
        RogueLikeMode.TreatsCurses curseShuffleID = CurseEntry.Entries.End;

        foreach (var mod in file.Mods.Where(mod => ModManager.Mods.FirstOrDefault(x => x.Name == mod.Name)?.IsBuiltin ?? true))
        {
            UpdateIDs(ItemEntry.Entries, mod.ItemIDMap, ref itemShuffleID, UpdateItemIDs);
            UpdateIDs(EnemyEntry.Entries, mod.EnemyIDMap, ref enemyShuffleID, UpdateEnemyIDs);
            UpdateIDs(QuestEntry.Entries, mod.QuestIDMap, ref questShuffleID, UpdateQuestIDs);
            UpdateIDs(PerkEntry.Entries, mod.PerkIDMap, ref perkShuffleID, UpdatePerkIDs);
            UpdateIDs(CurseEntry.Entries, mod.CurseIDMap, ref curseShuffleID, UpdateCurseIDs);
        }
    }

    private static MetadataFile LoadMetadataFile(Stream stream)
    {
        return JsonSerializer.Deserialize<MetadataFile>(stream);
    }

    private static void SaveMetadataFile(Stream stream, MetadataFile file)
    {
        JsonSerializer.Serialize(stream, file);
    }

    private static void UpdateIDs<IDType>(IEnumerable<Entry<IDType>> entries, Dictionary<string, IDType> oldIDs, ref IDType temporaryID, Action<IDType, IDType> updater) where IDType : struct, Enum
    {
        var previousConflictingID = new Dictionary<string, IDType>();
        var remainingIDs = new Dictionary<string, IDType>(oldIDs);

        while (remainingIDs.Count > 0)
        {
            var pair = remainingIDs.First();

            IDType oldGameID = pair.Value;
            string modID = pair.Key;

            Entry<IDType> entry = entries.FirstOrDefault(x => x.ModID == modID);

            if (entry == null)
            {
                Program.Logger.LogWarning("Failed to find {name}:{modID}. Game object was removed or has changed ModID.", typeof(IDType).Name, modID);
                remainingIDs.Remove(modID);
                continue;
            }

            IDType newGameID = entry.GameID;

            if (oldGameID.Equals(newGameID))
            {
                remainingIDs.Remove(modID);
                continue;
            }

            bool directSwapIsDangerous = remainingIDs.ContainsValue(newGameID);

            if (directSwapIsDangerous)
            {
                string conflictModID = remainingIDs.First(x => x.Value.Equals(newGameID)).Key;

                updater.Invoke(newGameID, temporaryID);

                previousConflictingID[conflictModID] = newGameID;
                remainingIDs[conflictModID] = temporaryID;

                temporaryID = (IDType)Enum.ToObject(typeof(IDType), Convert.ToInt64(temporaryID) + 1);
            }

            updater.Invoke(entry.GameID, newGameID);
            remainingIDs.Remove(modID);

            IDType oldGameID_actual = oldGameID;

            if (previousConflictingID.ContainsKey(modID))
            {
                oldGameID_actual = previousConflictingID[modID];
            }

            Program.Logger.LogDebug("Updated ID {name}:{modID}. GameID change: {oldGameID_actual} -> {newGameID}.", typeof(IDType).Name, modID, oldGameID_actual, newGameID);
        }
    }

    private static void UpdateItemIDs(ItemCodex.ItemTypes from, ItemCodex.ItemTypes to)
    {
        Inventory inventory = Globals.Game.xLocalPlayer.xInventory;
        Journal journal = Globals.Game.xLocalPlayer.xJournalInfo;

        // Shuffle inventory, discovered items, crafted items, and fishes

        if (inventory.denxInventory.ContainsKey(from))
        {
            inventory.denxInventory[to] = new Inventory.DisplayItem(inventory.denxInventory[from].iAmount, inventory.denxInventory[from].iPickupNumber, ItemCodex.GetItemDescription(to));
            inventory.denxInventory.Remove(from);
        }

        if (journal.henUniqueDiscoveredItems.Contains(from))
        {
            journal.henUniqueDiscoveredItems.Remove(from);
            journal.henUniqueDiscoveredItems.Add(to);
        }

        if (journal.henUniqueCraftedItems.Contains(from))
        {
            journal.henUniqueCraftedItems.Remove(from);
            journal.henUniqueCraftedItems.Add(to);
        }

        if (journal.henUniqueFishies.Contains(from))
        {
            journal.henUniqueFishies.Remove(from);
            journal.henUniqueFishies.Add(to);
        }
    }

    private static void UpdatePerkIDs(RogueLikeMode.Perks from, RogueLikeMode.Perks to)
    {
        var session = Globals.Game.xGlobalData.xLocalRoguelikeData;

        if (session.enPerkSlot01 == from)
            session.enPerkSlot01 = to;

        if (session.enPerkSlot02 == from)
            session.enPerkSlot02 = to;

        if (session.enPerkSlot03 == from)
            session.enPerkSlot03 = to;

        for (int i = 0; i < session.lenPerksOwned.Count; i++)
        {
            if (session.lenPerksOwned[i] == from)
                session.lenPerksOwned[i] = to;
        }
    }

    private static void UpdateCurseIDs(RogueLikeMode.TreatsCurses from, RogueLikeMode.TreatsCurses to)
    {
        var session = Globals.Game.xGlobalData.xLocalRoguelikeData;

        if (session.enCurseTreatSlot01 == from)
            session.enCurseTreatSlot01 = to;

        if (session.enCurseTreatSlot02 == from)
            session.enCurseTreatSlot02 = to;

        if (session.enCurseTreatSlot03 == from)
            session.enCurseTreatSlot03 = to;
    }

    private static void UpdateEnemyIDs(EnemyCodex.EnemyTypes from, EnemyCodex.EnemyTypes to)
    {
        var cards = Globals.Game.xLocalPlayer.xJournalInfo.henCardAlbum;
        if (cards.Remove(from))
        {
            cards.Add(to);
        }

        var knownEnemies = Globals.Game.xLocalPlayer.xJournalInfo.henKnownEnemies;
        if (knownEnemies.Remove(from))
        {
            knownEnemies.Add(to);
        }

        var killedEnemies = Globals.Game.xLocalPlayer.xJournalInfo.deniMonstersKilled;
        if (killedEnemies.ContainsKey(from))
        {
            killedEnemies.Add(to, killedEnemies[from]);
            killedEnemies.Remove(from);
        }
    }

    private static void UpdateQuestIDs(QuestCodex.QuestID from, QuestCodex.QuestID to)
    {
        UpdateQuestIDs_Arcade(from, to);
        UpdateQuestIDs_Character(from, to);
        UpdateQuestIDs_World(from, to);
    }

    private static void UpdateQuestIDs_World(QuestCodex.QuestID from, QuestCodex.QuestID to)
    {
        QuestLog log = Globals.Game.xLocalPlayer.xJournalInfo.xQuestLog;

        List<Quest> activeQuests = log.lxActiveQuests;
        List<Quest> completedQuests = log.lxCompletedQuests;

        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].enQuestID == from)
                activeQuests[i].enQuestID = to;
        }

        for (int i = 0; i < completedQuests.Count; i++)
        {
            if (completedQuests[i].enQuestID == from)
                completedQuests[i].enQuestID = to;
        }

        Quest trackedQuest = Globals.Game.xHUD.xTrackedQuest;

        if (trackedQuest.enQuestID == from)
            trackedQuest.enQuestID = to;
    }

    private static void UpdateQuestIDs_Character(QuestCodex.QuestID from, QuestCodex.QuestID to)
    {
        QuestLog log = Globals.Game.xLocalPlayer.xJournalInfo.xQuestLog;

        HashSet<QuestCodex.QuestID> completedQuests = log.henQuestsCompletedOnThisCharacter;

        if (completedQuests.Remove(from))
            completedQuests.Add(to);
    }

    private static void UpdateQuestIDs_Arcade(QuestCodex.QuestID from, QuestCodex.QuestID to)
    {
        var data = Globals.Game.xGlobalData.xLocalRoguelikeData;

        if (data.xSavedQuestInstance.enQuestID == from)
            data.xSavedQuestInstance.enQuestID = to;

        if (data._enActiveQuest == from)
            data._enActiveQuest = to;

        List<QuestCodex.QuestID> completedQuests = data.lenCompletedQuests;
        List<QuestCodex.QuestID> remoteQuests = data.lenRemoteCompletedQuests;

        for (int i = 0; i < completedQuests.Count; i++)
        {
            if (completedQuests[i] == from)
                completedQuests[i] = to;
        }

        for (int i = 0; i < remoteQuests.Count; i++)
        {
            if (remoteQuests[i] == from)
                remoteQuests[i] = to;
        }
    }
}
