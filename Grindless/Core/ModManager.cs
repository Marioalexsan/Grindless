using Microsoft.Extensions.Logging;
using Quests;

namespace Grindless
{
    /// <summary>
    /// Provides access to the objects used for modding, and some miscellaneous functionality.
    /// </summary>
    internal static class ModManager
    {
        internal static Mod CurrentlyLoadingMod { get; private set; }

        internal static List<Mod> Mods { get; } = new List<Mod>();

        private static Harmony _modPatcher = new("Grindless.ModPatches");

        public static void Reload()
        {
            if (Globals.Game.xStateMaster.enGameState != StateMaster.GameStates.MainMenu)
            {
                Program.Logger.LogWarning("Reloading outside of the main menu!");
            }

            UnloadMods();
            ReloadSoGState();
            LoadMods(ModLoader.ObtainMods());
            PrepareSoGStatePostLoad();
        }

        private static void UnloadMods()
        {
            string currentMusic = Globals.Game.xSoundSystem.xMusicVolumeMods.sCurrentSong;
            Globals.Game.xSoundSystem.StopSong(false);

            Program.Logger.LogInformation("Unloading mods...");

            foreach (Mod mod in Mods.AsEnumerable().Reverse())
                mod.Unload();

            Program.Logger.LogInformation("Unpatching mods...");

            _modPatcher.UnpatchAll(_modPatcher.Id);

            Program.Logger.LogInformation("Clearing mod entries...");

            AudioEntry.Entries.Reset();
            CommandEntry.Entries.Reset();
            CurseEntry.Entries.Reset();
            EnemyEntry.Entries.Reset();
            EquipmentEffectEntry.Entries.Reset();
            ItemEntry.Entries.Reset();
            LevelEntry.Entries.Reset();
            NetworkEntry.Entries.Reset();
            PerkEntry.Entries.Reset();
            PinEntry.Entries.Reset();
            QuestEntry.Entries.Reset();
            SpellEntry.Entries.Reset();
            StatusEffectEntry.Entries.Reset();
            WorldRegionEntry.Entries.Reset();

            Mods.Clear();

            Globals.Game.xSoundSystem.PlaySong(currentMusic, true);
        }

        private static void ReloadSoGState()
        {
            Program.Logger.LogInformation("Reloading game state...");

            // Unloads some mod textures for enemies. Textures are always requeried, so it's allowed
            InGameMenu.contTempAssetManager?.Unload();

            // Experimental / Risky. Unloads all mod assets
            AssetUtils.UnloadModContentPathAssets(RenderMaster.contPlayerStuff);

            // Reloads the english localization
            Globals.Game.xDialogueGod_Default = DialogueGod.ReadFile("Content/Data/Dialogue/defaultEnglish.dlf");
            Globals.Game.xMiscTextGod_Default = MiscTextGod.ReadFile("Content/Data/Text/defaultEnglish.vtf");

            // Reloads enemy descriptions
            EnemyCodex.denxDescriptionDict.Clear();
            EnemyCodex.lxSortedCardEntries.Clear();
            EnemyCodex.lxSortedDescriptions.Clear();
            EnemyCodex.Init();

            // Reloads perk info
            RogueLikeMode.PerkInfo.lxAllPerks.Clear();
            RogueLikeMode.PerkInfo.Init();

            // Unloads sorted pins
            PinCodex.SortedPinEntries.Clear();

            // Clears all regions
            Globals.Game.xLevelMaster.denxRegionContent.Clear();

            // Reload spell variables
            SpellVariable.Init();
        }

        private static void PrepareSoGStatePostLoad()
        {
            Program.Logger.LogInformation("Reloading game state (post mod load)...");

            // Reloads menu characters for new textures and item descriptions
            Globals.Game._Menu_CharacterSelect_Init();

            // Reloads original recipes
            Crafting.CraftSystem.InitCraftSystem();
        }

        private static void LoadMods(IEnumerable<Mod> mods)
        {
            Program.Logger.LogInformation("Patching mods...");

            foreach (var assembly in mods.Where(x => !x.IsBuiltin).Select(x => x.GetType().Assembly).Distinct())
            {
                Program.Logger.LogInformation("Patching assembly {}...", assembly.GetName());

                _modPatcher.PatchAll(assembly);

                Program.Logger.LogInformation("Patched {} methods in total!", _modPatcher.GetPatchedMethods().Count());
            }

            Program.Logger.LogInformation("Loading mods...");

            foreach (Mod mod in mods)
            {
                CurrentlyLoadingMod = mod;

                Program.Logger.LogInformation("Loading {mod}..", mod.Name);

                try
                {
                    mod.Load();
                }
                catch (Exception e)
                {
                    Program.Logger.LogInformation("{mod} threw an error during loading! {}", mod.Name, e.ToString());
                }

                CurrentlyLoadingMod = null;

                AudioEntry.Entries.InitializeEntries(null);
                CommandEntry.Entries.InitializeEntries(null);
                CurseEntry.Entries.InitializeEntries(null);
                EnemyEntry.Entries.InitializeEntries(null);
                EquipmentEffectEntry.Entries.InitializeEntries(null);
                ItemEntry.Entries.InitializeEntries(null);
                LevelEntry.Entries.InitializeEntries(null);
                NetworkEntry.Entries.InitializeEntries(null);
                PerkEntry.Entries.InitializeEntries(null);
                PinEntry.Entries.InitializeEntries(null);
                QuestEntry.Entries.InitializeEntries(null);
                SpellEntry.Entries.InitializeEntries(null);
                StatusEffectEntry.Entries.InitializeEntries(null);
                WorldRegionEntry.Entries.InitializeEntries(null);

                Mods.Add(mod);
            }

            // Post Load Phase

            foreach (Mod mod in Mods)
            {
                mod.PostLoad();
            }
        }

        internal static EntryManager<IDType, EntryType> GetEntryManager<IDType, EntryType>()
            where IDType : struct, Enum
            where EntryType : Entry<IDType>
        {
            // Mind the extra parentesis for tuple types
            Dictionary<Type, object> objects = new()
            {
                [typeof((GrindlessID.AudioID, AudioEntry))] = AudioEntry.Entries,
                [typeof((GrindlessID.NetworkID, CommandEntry))] = CommandEntry.Entries,
                [typeof((RogueLikeMode.TreatsCurses, CurseEntry))] = CurseEntry.Entries,
                [typeof((EnemyCodex.EnemyTypes, EnemyEntry))] = EnemyEntry.Entries,
                [typeof((EquipmentInfo.SpecialEffect, EquipmentEffectEntry))] = EquipmentEffectEntry.Entries,
                [typeof((ItemCodex.ItemTypes, ItemEntry))] = ItemEntry.Entries,
                [typeof((Level.ZoneEnum, LevelEntry))] = LevelEntry.Entries,
                [typeof((GrindlessID.NetworkID, NetworkEntry))] = NetworkEntry.Entries,
                [typeof((RogueLikeMode.Perks, PerkEntry))] = PerkEntry.Entries,
                [typeof((PinCodex.PinType, PinEntry))] = PinEntry.Entries,
                [typeof((QuestCodex.QuestID, QuestEntry))] = QuestEntry.Entries,
                [typeof((SpellCodex.SpellTypes, SpellEntry))] = SpellEntry.Entries,
                [typeof((BaseStats.StatusEffectSource, StatusEffectEntry))] = StatusEffectEntry.Entries,
                [typeof((Level.WorldRegion, WorldRegionEntry))] = WorldRegionEntry.Entries
            };

            return objects[typeof((IDType, EntryType))] as EntryManager<IDType, EntryType>;
        }
    }
}
