using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Quests;
using System.Reflection.Emit;
using static SoG.ShopMenu;

namespace Grindless.HarmonyPatches
{
    /// <summary>
    /// Contains a collection of reverse patches created from the original game methods.
    /// Calling method from this class is almost the same as if you called the vanilla ones.
    /// That is, no prefix / postfix / whatever patches are applied.
    /// </summary>
    [HarmonyPatch]
    public static class OriginalMethods
    {
        #region Curse related

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_GetTreatCurseInfo))]
        public static void _RogueLike_GetTreatCurseInfo(Game1 __instance, RogueLikeMode.TreatsCurses enTreatCurse, out string sNameHandle, out string sDescriptionHandle, out float fScoreModifier)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(TreatCurseMenu), nameof(TreatCurseMenu.FillCurseList))]
        public static void FillCurseList(TreatCurseMenu __instance)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(TreatCurseMenu), nameof(TreatCurseMenu.FillTreatList))]
        public static void FillTreatList(TreatCurseMenu __instance)
        {
            throw new NotImplementedException("Stub method.");
        }

        #endregion

        #region Enemy related

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(CardCodex), nameof(CardCodex.GetIllustrationPath))]
        public static string GetIllustrationPath(EnemyCodex.EnemyTypes enEnemy)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(EnemyCodex), nameof(EnemyCodex.GetEnemyDescription))]
        public static EnemyDescription GetEnemyDescription(EnemyCodex.EnemyTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(EnemyCodex), nameof(EnemyCodex.GetEnemyDefaultAnimation))]
        public static Animation GetEnemyDefaultAnimation(EnemyCodex.EnemyTypes enType, ContentManager Content)
        {
            throw new NotImplementedException("Stub method.");
        }

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(EnemyCodex), nameof(EnemyCodex.GetEnemyDisplayIcon))]
        public static Texture2D GetEnemyDisplayIcon(EnemyCodex.EnemyTypes enType, ContentManager Content, bool bBigIfPossible)
        {
            throw new NotImplementedException("Stub method.");
        }

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(EnemyCodex), nameof(EnemyCodex.GetEnemyLocationPicture))]
        public static Texture2D GetEnemyLocationPicture(EnemyCodex.EnemyTypes enType, ContentManager Content)
        {
            throw new NotImplementedException("Stub method.");
        }

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(Game1), nameof(Game1._Enemy_AdjustForDifficulty))]
        public static void _Enemy_AdjustForDifficulty(Game1 __instance, Enemy xEn)
        {
            throw new NotImplementedException("Stub method.");
        }

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(Game1), nameof(Game1._Enemy_MakeElite))]
        public static bool _Enemy_MakeElite(Game1 __instance, Enemy xEn, bool bAttachEffect)
        {
            throw new NotImplementedException("Stub method.");
        }

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(EnemyCodex), nameof(EnemyCodex.GetEnemyInstance_CacuteForward))]
        public static Enemy GetEnemyInstance_CacuteForward(EnemyCodex.EnemyTypes enType, Level.WorldRegion enOverrideContent)
        {
            throw new NotImplementedException("Stub method.");
        }

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(EnemyCodex), nameof(EnemyCodex.GetEnemyInstance))]
        public static Enemy GetEnemyInstance(EnemyCodex.EnemyTypes enType, Level.WorldRegion enOverrideContent)
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var original = AccessTools.Method(typeof(EnemyCodex), nameof(EnemyCodex.GetEnemyInstance_CacuteForward));
                var replacement = AccessTools.Method(typeof(OriginalMethods), nameof(OriginalMethods.GetEnemyInstance_CacuteForward));

                return instructions.MethodReplacer(original, replacement);
            }

            _ = Transpiler(null);
            throw new NotImplementedException("Stub method.");
        }

        #endregion

        #region Item related

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ItemCodex), nameof(ItemCodex.GetItemDescription_PostSpecial))]
        public static ItemDescription GetItemDescription_PostSpecial(ItemCodex.ItemTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ItemCodex), nameof(ItemCodex.GetItemDescription))]
        public static ItemDescription GetItemDescription(ItemCodex.ItemTypes enType)
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var original = AccessTools.Method(typeof(ItemCodex), nameof(ItemCodex.GetItemDescription_PostSpecial));
                var replacement = AccessTools.Method(typeof(OriginalMethods), nameof(OriginalMethods.GetItemDescription_PostSpecial));

                return instructions.MethodReplacer(original, replacement);
            }

            _ = Transpiler(null);
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(EquipmentCodex), nameof(EquipmentCodex.GetAccessoryInfo))]
        public static EquipmentInfo GetAccessoryInfo(ItemCodex.ItemTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(EquipmentCodex), nameof(EquipmentCodex.GetArmorInfo))]
        public static EquipmentInfo GetArmorInfo(ItemCodex.ItemTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(EquipmentCodex), nameof(EquipmentCodex.GetShieldInfo))]
        public static EquipmentInfo GetShieldInfo(ItemCodex.ItemTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(EquipmentCodex), nameof(EquipmentCodex.GetShoesInfo))]
        public static EquipmentInfo GetShoesInfo(ItemCodex.ItemTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(FacegearCodex), nameof(FacegearCodex.GetHatInfo))]
        public static FacegearInfo GetFacegearInfo(ItemCodex.ItemTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(HatCodex), nameof(HatCodex.GetHatInfo))]
        public static HatInfo GetHatInfo(ItemCodex.ItemTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(WeaponCodex), nameof(WeaponCodex.GetWeaponInfo))]
        public static WeaponInfo GetWeaponInfo(ItemCodex.ItemTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        #endregion

        #region

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(LevelBlueprint), nameof(LevelBlueprint.GetBlueprint))]
        public static LevelBlueprint GetBlueprint(Level.ZoneEnum enZoneToGet)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Game1), nameof(Game1._LevelLoading_DoStuff))]
        public static void _LevelLoading_DoStuff(Game1 __instance, Level.ZoneEnum enLevel, bool bStaticOnly)
        {
            throw new NotImplementedException("Stub method.");
        }

        #endregion

        #region Perks

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_GetPerkTexture))]
        public static Texture2D _RogueLike_GetPerkTexture(Game1 __instance, RogueLikeMode.Perks enPerk)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(RogueLikeMode.PerkInfo), nameof(RogueLikeMode.PerkInfo.Init))]
        public static void PerkInfoInit()
        {
            throw new NotImplementedException("Stub method.");
        }

        #endregion

        #region Pins

        //[HarmonyReversePatch]
        //[HarmonyPatch(typeof(PinCodex), nameof(PinCodex.GetInfo))]
        public static PinInfo GetPinInfo(RogueLikeMode.Perks enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        public static List<PinCodex.PinType> LastRandomPinList { get; private set; } = new();

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_GetRandomPin))]
        public static PinCodex.PinType FillRandomPinList(Game1 __instance, Random randOverride = null)
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codeList = instructions.ToList();

                int start = codeList.FindPosition((code, pos) => code[pos].opcode == OpCodes.Ldc_I4_0, 1) - 2;
                int end = codeList.Count;

                var labels = codeList[start].ExtractLabels();

                return codeList.InsertAt(start, new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldloc_1).WithLabels(labels),
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertySetter(typeof(OriginalMethods), nameof(LastRandomPinList))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ret)
                });
            }

            _ = Transpiler(null);
            throw new InvalidOperationException("Stub method.");
        }

        #endregion

        #region Quests

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(QuestCodex), nameof(QuestCodex.GetQuestDescription))]
        public static QuestDescription GetQuestDescription(QuestCodex.QuestID p_enID)
        {
            throw new NotImplementedException("Stub method.");
        }

        #endregion

        #region Spells

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(SpellCodex), nameof(SpellCodex.IsMagicSkill))]
        public static bool SpellIsMagicSkill(SpellCodex.SpellTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(SpellCodex), nameof(SpellCodex.IsMeleeSkill))]
        public static bool SpellIsMeleeSkill(SpellCodex.SpellTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(SpellCodex), nameof(SpellCodex.IsUtilitySkill))]
        public static bool SpellIsUtilitySkill(SpellCodex.SpellTypes enType)
        {
            throw new NotImplementedException("Stub method.");
        }

        #endregion
    }
}
