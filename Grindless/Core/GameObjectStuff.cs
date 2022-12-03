using System;
using System.Collections.Generic;
using ItemID = SoG.ItemCodex.ItemTypes;
using ItemEffectID = SoG.EquipmentInfo.SpecialEffect;
using LevelID = SoG.Level.ZoneEnum;
using WorldID = SoG.Level.WorldRegion;
using PerkID = SoG.RogueLikeMode.Perks;
using CurseID = SoG.RogueLikeMode.TreatsCurses;
using EnemyID = SoG.EnemyCodex.EnemyTypes;
using QuestID = Quests.QuestCodex.QuestID;
using SpecialObjectiveID = Quests.Objective_SpecialObjective.UniqueID;
using SpellID = SoG.SpellCodex.SpellTypes;
using StatusEffectID = SoG.BaseStats.StatusEffectSource;
using PinID = SoG.PinCodex.PinType;
using SoG;

namespace Grindless
{
    /// <summary>
    /// Defines IDs for game object entries that come from Grindless.
    /// </summary>
    public static class GrindlessID
	{
		public enum AudioID { }
		public enum CommandID { }
		public enum NetworkID { }
	}

	/// <summary>
	/// Provides extension methods for vanilla IDs.
	/// </summary>
	public static class IDExtension
	{
		private static Dictionary<Type, object> s_vanillaIDs = new Dictionary<Type, object>();

		static IDExtension()
        {
			AddVanillaIDs<ItemID>();
			AddVanillaIDs<ItemEffectID>();
			AddVanillaIDs<LevelID>();
			AddVanillaIDs<WorldID>();
			AddVanillaIDs<PerkID>();
			AddVanillaIDs<CurseID>();
			AddVanillaIDs<EnemyID>();
			AddVanillaIDs<QuestID>();
			AddVanillaIDs<SpecialObjectiveID>();
			AddVanillaIDs<SpellID>();
			AddVanillaIDs<StatusEffectID>();
			AddVanillaIDs<PinID>();

			// If it is needed, manually remove problematic IDs

			RemoveVanillaIDs(
				CurseID.None, CurseID.Curse_Hard, CurseID.Treat_Easy
				);

			// Do not remove card entries, or else kaboom!
			RemoveVanillaIDs(
				EnemyID.Null, EnemyID.YellowSlime, EnemyID.SeasonWarden, 
				EnemyID.MtBloom_Troll, EnemyID.GhostShip_Placeholder4, 
				EnemyID.Lood_Placeholder, EnemyID.Lood_Placeholder2, EnemyID.Lood_Placeholder3
				);

			RemoveVanillaIDs(
				ItemID.Null, ItemID._Furniture_Decoration_CompletedPlant_OneHandWeaponPlant_Empty,
				ItemID._ChaosModeUpgrade_HPUp,
				ItemID._ChaosModeUpgrade_ATKUp,
				ItemID._ChaosModeUpgrade_CSPDUp,
				ItemID._ChaosModeUpgrade_EPRegUp,
				ItemID._ChaosModeUpgrade_MaxEPUp,
				ItemID._ChaosModeUpgrade_TalentPoints,
				ItemID._ChaosModeUpgrade_LastDroppableGeneric,
				ItemID._ChaosModeUpgrade_SpellStart,
				ItemID._ChaosModeUpgrade_SpellEnd
				);

			RemoveVanillaIDs(
				LevelID.SeasonChange_F4_LAST,
				LevelID.TimeTown_ENDOFREGION,
				LevelID.Desert_ENDOFREGION,
				LevelID.GhostShip_ENDOFREGION,
				LevelID.Endgame_ENDOFREGION,
				LevelID.MarinoMansion_Cellar,  // Not used
				LevelID.WinterLand_ToyFactory_PreBossRoom,  // Not used
				LevelID.MountBloom_PoisonObstacleRoom,  // Not used
				LevelID.TimeTown_Map03_BossRoom,  // Not used
				LevelID.GhostShip_F1OutsideEntrance,  // Not used
				LevelID.Lobby,
				LevelID.None
				);

			RemoveVanillaIDs(PerkID.None,
				PerkID.MoreNormalItems,  // Not implemented
				PerkID.StartAtLvlTwo,  // Not implemented fully
				PerkID.MoreRegenAfterFloors,  // Not implemented
				PerkID.RegenAfterRooms  // Not implemented
				);

			RemoveVanillaIDs(
				QuestID._SideQuest_Knark,
				QuestID.None,
				QuestID._SideQuest_TheSpectralBall_OBSOLETE,
				QuestID._SideQuest_TheSpectralBall_MK2_OBSOLETE,
				QuestID._RogueLikeQuest_GrindeaChallenge03,
				QuestID._RogueLikeQuest_GrindeaChallenge04
				);
		}

		private static void AddVanillaIDs<T>()
			where T : struct, Enum
		{
			s_vanillaIDs[typeof(T)] = new HashSet<T>((T[])Enum.GetValues(typeof(T)));
        }

		private static void RemoveVanillaIDs<T>(params T[] ids)
			where T : struct, Enum
		{
			if (!s_vanillaIDs.TryGetValue(typeof(T), out _))
            {
				s_vanillaIDs[typeof(T)] = new HashSet<T>();
            }

			((HashSet<T>)s_vanillaIDs[typeof(T)]).ExceptWith(ids);
		}

		public static bool IsFromSoG<T>(this T id)
			where T : struct, Enum
		{
			if (s_vanillaIDs.ContainsKey(typeof(T)))
            {
				return ((HashSet<T>)s_vanillaIDs[typeof(T)]).Contains(id);
            }

			return false;
		}

		public static IEnumerable<T> GetAllSoGIDs<T>()
        {
			if (s_vanillaIDs.ContainsKey(typeof(T)))
			{
				return (IEnumerable<T>)s_vanillaIDs[typeof(T)];
			}

			return Array.Empty<T>();
		}

		public static bool IsFromMod<IDType>(this IDType id)
			where IDType : struct, Enum
		{
			return !IsFromSoG(id);
		}
	}

	/// <summary>
	/// Provides utility methods for working with game objects.
	/// </summary>
	internal static class GameObjectStuff
    {
		static GameObjectStuff()
        {
			_originalPinCollection = new List<PinID>(PinCodex.SortedPinEntries);
        }

		private static List<PinID> _originalPinCollection;

		public static List<PinID> GetOriginalPinCollection()
        {
			return _originalPinCollection;
		}
    }
}
