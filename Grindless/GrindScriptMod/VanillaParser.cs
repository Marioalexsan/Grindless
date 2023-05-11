using Quests;
using Microsoft.Extensions.Logging;
using Grindless.HarmonyPatches;

namespace Grindless
{
    internal static class VanillaParser
    {
        public static CurseEntry ParseCurse(this VanillaMod vanillaMod, RogueLikeMode.TreatsCurses gameID)
        {
            OriginalMethods._RogueLike_GetTreatCurseInfo(Globals.Game, gameID, out var nameHandle, out var descHandle, out var scoreModifier);

            return new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString(),
                nameHandle = nameHandle,
                descriptionHandle = descHandle,
                ScoreModifier = scoreModifier,
                TexturePath = null,
                Name = Globals.Game.EXT_GetMiscText("Menus", nameHandle).sUnparsedFullLine,
                Description = Globals.Game.EXT_GetMiscText("Menus", descHandle).sUnparsedFullLine,
                // Hacky way to detemine if it's a curse or treat in vanilla
                IsTreat = Globals.Game.xShopMenu.xTreatCurseMenu.lenTreatCursesAvailable.Contains(gameID) || gameID == RogueLikeMode.TreatsCurses.Treat_MoreLoods
            };
        }

        public static EnemyEntry ParseEnemy(this VanillaMod vanillaMod, EnemyCodex.EnemyTypes gameID)
        {
            EnemyEntry entry = new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString()
            };

            if (gameID == EnemyCodex.EnemyTypes.TimeTemple_GiantWorm_Recolor)
            {
                var desc = OriginalMethods.GetEnemyDescription(EnemyCodex.EnemyTypes.TimeTemple_GiantWorm);

                // Recolored worm borrows its enemy description!
                entry.Vanilla = new EnemyDescription(gameID, desc.sNameLibraryHandle, desc.iLevel, desc.iMaxHealth)
                {
                    sOnHitSound = desc.sOnHitSound,
                    sOnDeathSound = desc.sOnDeathSound,
                    lxLootTable = new List<DropChance>(desc.lxLootTable),
                    sFullName = desc.sFullName,
                    sFlavorText = desc.sFlavorText,
                    sFlavorLibraryHandle = desc.sFlavorLibraryHandle,
                    sDetailedDescription = desc.sDetailedDescription,
                    sDetailedDescriptionLibraryHandle = desc.sDetailedDescriptionLibraryHandle,
                    iCardDropChance = desc.iCardDropChance,
                    v2ApproximateOffsetToMid = desc.v2ApproximateOffsetToMid,
                    v2ApproximateSize = desc.v2ApproximateSize,
                };

                entry.CreateJournalEntry = false;
            }
            else
            {
                entry.Vanilla = OriginalMethods.GetEnemyDescription(gameID);
                entry.CreateJournalEntry = EnemyCodex.lxSortedDescriptions.Contains(entry.Vanilla);
            }

            entry.DefaultAnimation = null;
            entry.DisplayBackgroundPath = null;
            entry.DisplayIconPath = null;

            entry.CardIllustrationPath = OriginalMethods.GetIllustrationPath(gameID);

            entry.Constructor = null;
            entry.DifficultyScaler = null;
            entry.EliteScaler = null;

            List<EnemyCodex.EnemyTypes> resetCardChance = new()
            {
                EnemyCodex.EnemyTypes.Special_ElderBoar,
                EnemyCodex.EnemyTypes.Pumpking,
                EnemyCodex.EnemyTypes.Marino,
                EnemyCodex.EnemyTypes.Boss_MotherPlant,
                EnemyCodex.EnemyTypes.TwilightBoar,
                EnemyCodex.EnemyTypes.Desert_VegetableCardEntry

            };

            if (resetCardChance.Contains(entry.GameID))
            {
                // These don't have cards, but a drop chance higher than 0 would generate a card entry
                entry.Vanilla.iCardDropChance = 0;
            }

            return entry;
        }

        public static EquipmentEffectEntry ParseEquipmentEffect(this VanillaMod vanillaMod, EquipmentInfo.SpecialEffect gameID)
        {
            return new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString()
            };
        }

        public static ItemEntry ParseItem(this VanillaMod vanillaMod, ItemCodex.ItemTypes gameID)
        {
            ItemEntry entry = new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString(),
                vanillaItem = OriginalMethods.GetItemDescription(gameID)
            };

            EquipmentInfo equip = null;

            if (entry.vanillaItem.lenCategory.Contains(ItemCodex.ItemCategories.Weapon))
            {
                var weapon = OriginalMethods.GetWeaponInfo(gameID);
                equip = weapon;
                entry.EquipType = EquipmentType.Weapon;

                entry.WeaponType = weapon.enWeaponCategory;
                entry.MagicWeapon = weapon.enAutoAttackSpell != WeaponInfo.AutoAttackSpell.None;
            }
            else if (entry.vanillaItem.lenCategory.Contains(ItemCodex.ItemCategories.Shield))
            {
                equip = OriginalMethods.GetShieldInfo(gameID);
                entry.EquipType = EquipmentType.Shield;
            }
            else if (entry.vanillaItem.lenCategory.Contains(ItemCodex.ItemCategories.Armor))
            {
                equip = OriginalMethods.GetArmorInfo(gameID);
                entry.EquipType = EquipmentType.Armor;
            }
            else if (entry.vanillaItem.lenCategory.Contains(ItemCodex.ItemCategories.Hat))
            {
                var hat = OriginalMethods.GetHatInfo(gameID);
                equip = hat;
                entry.EquipType = EquipmentType.Hat;

                entry.defaultSet = hat.xDefaultSet;

                foreach (var pair in hat.denxAlternateVisualSets)
                {
                    entry.altSets[pair.Key] = pair.Value;
                    entry.hatAltSetResourcePaths[pair.Key] = null;
                }

                entry.HatDoubleSlot = hat.bDoubleSlot;
            }
            else if (entry.vanillaItem.lenCategory.Contains(ItemCodex.ItemCategories.Facegear))
            {
                var facegear = OriginalMethods.GetFacegearInfo(gameID);
                equip = facegear;
                entry.EquipType = EquipmentType.Facegear;

                Array.Copy(facegear.abOverHair, entry.FacegearOverHair, 4);
                Array.Copy(facegear.abOverHat, entry.FacegearOverHat, 4);
                Array.Copy(facegear.av2RenderOffsets, entry.FacegearOffsets, 4);

                for (int i = 0; i < 4; i++)
                    entry.FacegearSides[i] = facegear.atxTextures[i] != RenderMaster.txNullTex;
            }
            else if (entry.vanillaItem.lenCategory.Contains(ItemCodex.ItemCategories.Shoes))
            {
                equip = OriginalMethods.GetShoesInfo(gameID);
                entry.EquipType = EquipmentType.Shoes;
            }
            else if (entry.vanillaItem.lenCategory.Contains(ItemCodex.ItemCategories.Accessory))
            {
                equip = OriginalMethods.GetAccessoryInfo(gameID);
                entry.EquipType = EquipmentType.Accessory;
            }

            entry.IconPath = null;
            entry.ShadowPath = null;
            entry.EquipResourcePath = null;

            if (equip != null)
            {
                entry.stats = new Dictionary<EquipmentInfo.StatEnum, int>(equip.deniStatChanges);
                entry.effects = new HashSet<EquipmentInfo.SpecialEffect>(equip.lenSpecialEffects);
                entry.EquipResourcePath = equip.sResourceName;
            }

            // Obviously we're not gonna use the modded format to load vanilla assets
            entry.UseVanillaResourceFormat = true;

            if (entry.vanillaItem.txDisplayImage == null)
                Program.Logger.LogWarning("ALERT! {GameID} display image is null!!111", gameID);

            return entry;
        }

        public static LevelEntry ParseLevel(this VanillaMod vanillaMod, Level.ZoneEnum gameID)
        {
            Level.WorldRegion worldRegion = Level.WorldRegion.PillarMountains;

            try
            {
                worldRegion = OriginalMethods.GetBlueprint(gameID).enRegion;
            }
            catch { }

            return new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString(),
                WorldRegion = worldRegion
            };
        }

        public static PerkEntry ParsePerk(this VanillaMod vanillaMod, RogueLikeMode.Perks gameID)
        {
            var perkInfo = RogueLikeMode.PerkInfo.lxAllPerks.FirstOrDefault(x => x.enPerk == gameID) ?? gameID switch
            {
                RogueLikeMode.Perks.PetWhisperer => new RogueLikeMode.PerkInfo(RogueLikeMode.Perks.PetWhisperer, 20, "PetWhisperer"),
                RogueLikeMode.Perks.MoreFishingRooms => new RogueLikeMode.PerkInfo(RogueLikeMode.Perks.MoreFishingRooms, 25, "MoreFishingRooms"),
                RogueLikeMode.Perks.OnlyPinsAfterChallenges => new RogueLikeMode.PerkInfo(RogueLikeMode.Perks.OnlyPinsAfterChallenges, 30, "OnlyPinsAfterChallenges"),
                RogueLikeMode.Perks.ChanceAtPinAfterBattleRoom => new RogueLikeMode.PerkInfo(RogueLikeMode.Perks.ChanceAtPinAfterBattleRoom, 30, "ChanceAtPinAfterBattleRoom"),
                RogueLikeMode.Perks.MoreLoods => new RogueLikeMode.PerkInfo(RogueLikeMode.Perks.MoreLoods, 25, "MoreLoods"),
                _ => throw new Exception("Perk description unavailable.")
            };

            return new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString(),
                UnlockCondition = gameID switch
                {
                    RogueLikeMode.Perks.PetWhisperer => () => CAS.WorldRogueLikeData.henActiveFlags.Contains(FlagCodex.FlagID._Roguelike_TalkedToWeivForTheFirstTime),
                    RogueLikeMode.Perks.MoreFishingRooms => () => CAS.WorldRogueLikeData.henActiveFlags.Contains(FlagCodex.FlagID._Roguelike_Improvement_Aquarium),
                    RogueLikeMode.Perks.OnlyPinsAfterChallenges => () => CAS.WorldRogueLikeData.henActiveFlags.Contains(FlagCodex.FlagID._Roguelike_PinsUnlocked),
                    RogueLikeMode.Perks.ChanceAtPinAfterBattleRoom => () => CAS.WorldRogueLikeData.henActiveFlags.Contains(FlagCodex.FlagID._Roguelike_PinsUnlocked),
                    RogueLikeMode.Perks.MoreLoods => () => CAS.WorldRogueLikeData.henActiveFlags.Contains(FlagCodex.FlagID._Roguelike_HasSeenLood),
                    _ => null
                },
                EssenceCost = perkInfo.iEssenceCost,
                TextEntry = perkInfo.sNameHandle,
                Name = Globals.Game.EXT_GetMiscText("Menus", "Perks_Name_" + perkInfo.sNameHandle)?.sUnparsedFullLine,
                Description = Globals.Game.EXT_GetMiscText("Menus", "Perks_Description_" + perkInfo.sNameHandle)?.sUnparsedFullLine,
                TexturePath = null
            };
        }

        public static PinEntry ParsePin(this VanillaMod vanillaMod, PinCodex.PinType gameID)
        {
            PinInfo info = PinCodex.GetInfo(gameID);

            Enum.TryParse(info.sSymbol, out PinEntry.Symbol pinSymbol);
            Enum.TryParse(info.sShape, out PinEntry.Shape pinShape);

            return new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString(),
                PinSymbol = pinSymbol,
                PinShape = pinShape,
                PinColor = info.sPalette switch
                {
                    "Test1" => PinEntry.Color.YellowOrange,
                    "Test2" => PinEntry.Color.Seagull,
                    "Test3" => PinEntry.Color.Coral,
                    "Test4" => PinEntry.Color.Conifer,
                    "Test5" => PinEntry.Color.BilobaFlower,
                    "TestLight" => PinEntry.Color.White,
                    _ => PinEntry.Color.White
                },
                IsSticky = info.bSticky,
                IsBroken = info.bBroken,
                Description = info.sDescription,
                // Hey, do you like ultra hacky code?
                ConditionToDrop = () =>
                {
                    _ = OriginalMethods.FillRandomPinList(Globals.Game);
                    return OriginalMethods.LastRandomPinList.Contains(gameID);
                },
                CreateCollectionEntry = GameObjectStuff.OriginalPinCollection.Contains(gameID)
            };
        }

        public static QuestEntry ParseQuest(this VanillaMod vanillaMod, QuestCodex.QuestID gameID)
        {
            var desc = OriginalMethods.GetQuestDescription(gameID);

            return new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString(),
                Vanilla = desc,
                Name = Globals.Game.EXT_GetMiscText("Quests", desc.sQuestNameReference)?.sUnparsedFullLine,
                Description = Globals.Game.EXT_GetMiscText("Quests", desc.sDescriptionReference)?.sUnparsedFullLine,
                Summary = Globals.Game.EXT_GetMiscText("Quests", desc.sSummaryReference)?.sUnparsedFullLine,
                Constructor = null
            };
        }

        public static SpellEntry ParseSpell(this VanillaMod vanillaMod, SpellCodex.SpellTypes gameID)
        {
            return new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString(),
                IsMagicSkill = OriginalMethods.SpellIsMagicSkill(gameID),
                IsMeleeSkill = OriginalMethods.SpellIsMeleeSkill(gameID),
                IsUtilitySkill = OriginalMethods.SpellIsUtilitySkill(gameID),

                Builder = null
            };
        }

        public static StatusEffectEntry ParseStatusEffect(this VanillaMod vanillaMod, BaseStats.StatusEffectSource gameID)
        {
            return new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString(),
                TexturePath = null
            };
        }

        public static WorldRegionEntry ParseWorldRegion(this VanillaMod vanillaMod, Level.WorldRegion gameID)
        {
            return new()
            {
                Mod = vanillaMod,
                GameID = gameID,
                ModID = gameID.ToString()
            };
        }
    }
}
