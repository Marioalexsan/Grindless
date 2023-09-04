using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace Grindless;

public abstract partial class Mod
{
    public AudioEntry CreateAudio() => Entries.Audio.Create(this, "");

    public CommandEntry CreateCommands() => Entries.Commands.Create(this, "");

    public CurseEntry CreateCurse(string modID) => Entries.Curses.Create(this, modID);

    public EnemyEntry CreateEnemy(string modID) => Entries.Enemies.Create(this, modID);

    public EquipmentEffectEntry CreateEquipmentEffect(string modID) => Entries.EquipmentEffects.Create(this, modID);

    public ItemEntry CreateItem(string modID) => Entries.Items.Create(this, modID);

    public LevelEntry CreateLevel(string modID) => Entries.Levels.Create(this, modID);

    public NetworkEntry CreateNetwork() => Entries.Network.Create(this, "");

    public PerkEntry CreatePerk(string modID) => Entries.Perks.Create(this, modID);

    public PinEntry CreatePin(string modID) => Entries.Pins.Create(this, modID);

    public QuestEntry CreateQuest(string modID) => Entries.Quests.Create(this, modID);

    public SpellEntry CreateSpell(string modID) => Entries.Spells.Create(this, modID);

    public StatusEffectEntry CreateStatusEffect(string modID) => Entries.StatusEffects.Create(this, modID);

    public WorldRegionEntry CreateWorldRegion(string modID) => Entries.WorldRegions.Create(this, modID);

    /// <summary>
    /// Gets an active mod using its nameID.
    /// Returns null if the mod isn't currently loaded.
    /// </summary>
    /// <param name="nameID">The NameID of the mod.</param>
    /// <returns></returns>
    public Mod GetMod(string nameID)
    {
        return ModManager.Mods.FirstOrDefault(x => x.Name == nameID);
    }

    public AudioEntry GetAudio() => Entries.Audio.Get(this, "");

    public CommandEntry GetCommands() => Entries.Commands.Get(this, "");

    public CurseEntry GetCurse(string modID) => Entries.Curses.Get(this, modID);

    public EnemyEntry GetEnemy(string modID) => Entries.Enemies.Get(this, modID);

    public EquipmentEffectEntry GetEquipmentEffect(string modID) => Entries.EquipmentEffects.Get(this, modID);

    public ItemEntry GetItem(string modID) => Entries.Items.Get(this, modID);

    public LevelEntry GetLevel(string modID) => Entries.Levels.Get(this, modID);

    public NetworkEntry GetNetwork() => Entries.Network.Get(this, "");

    public PerkEntry GetPerk(string modID) => Entries.Perks.Get(this, modID);

    public PinEntry GetPin(string modID) => Entries.Pins.Get(this, modID);

    public QuestEntry GetQuest(string modID) => Entries.Quests.Get(this, modID);

    public SpellEntry GetSpell(string modID) => Entries.Spells.Get(this, modID);

    public StatusEffectEntry GetStatusEffect(string modID) => Entries.StatusEffects.Get(this, modID);

    public WorldRegionEntry GetWorldRegion(string modID) => Entries.WorldRegions.Get(this, modID);

    public void AddCraftingRecipe(ItemCodex.ItemTypes result, Dictionary<ItemCodex.ItemTypes, ushort> ingredients)
    {
        if (ModManager.CurrentlyLoadingMod != null)
            throw new InvalidOperationException(ErrorHelper.UseThisAfterLoad);

        if (ingredients == null)
            throw new ArgumentNullException(nameof(ingredients));

        if (!Crafting.CraftSystem.RecipeCollection.ContainsKey(result))
        {
            var kvps = new KeyValuePair<ItemDescription, ushort>[ingredients.Count];

            int index = 0;
            foreach (var kvp in ingredients)
                kvps[index++] = new KeyValuePair<ItemDescription, ushort>(ItemCodex.GetItemDescription(kvp.Key), kvp.Value);

            ItemDescription description = ItemCodex.GetItemDescription(result);
            Crafting.CraftSystem.RecipeCollection.Add(result, new Crafting.CraftSystem.CraftingRecipe(description, kvps));
        }

        Program.Logger.LogInformation("Added recipe for item {result}!", result);
    }
}
