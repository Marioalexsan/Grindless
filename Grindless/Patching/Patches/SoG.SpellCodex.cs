using HarmonyLib;
using Microsoft.Xna.Framework;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(SpellCodex))]
    static class SoG_SpellCodex
    {
        /// <summary>
        /// Gets the spell instance of an entry.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SpellCodex.GetSpellInstance), typeof(SpellCodex.SpellTypes), typeof(int), typeof(Level.WorldRegion))]
        static bool GetSpellInstance_Prefix(ref ISpellInstance __result, SpellCodex.SpellTypes enType, int iPowerLevel, Level.WorldRegion enOverrideRegion)
        {
            var entry = SpellEntry.Entries.Get(enType);

            ErrorHelper.Assert(entry != null, ErrorHelper.UnknownEntry);

            if (entry.Builder == null && entry.IsVanilla)
            {
                return true;  // Get from vanilla
            }

            __result = entry.Builder.Invoke(iPowerLevel, enOverrideRegion);

            if (__result.xRenderComponent == null)
            {
                __result.xRenderComponent = new AnimatedRenderComponent(__result)
                {
                    xTransform = __result.xTransform
                };
            }

            __result.xRenderComponent.xOwnerObject = __result;

            if (__result.xRenderComponent is AnimatedRenderComponent arc && arc.dixAnimations.Count == 0)
            {
                arc.dixAnimations.Add(0, new Animation(0, 0, RenderMaster.txNullTex, new Vector2(8f, 6f), 4, 1, 17, 32, 0, 0, 6, Animation.LoopSettings.Looping, Animation.CancelOptions.IgnoreIfPlaying, true, true));
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(SpellCodex.IsMagicSkill))]
        static bool IsMagicSkill_Prefix(SpellCodex.SpellTypes enType, ref bool __result)
        {
            var entry = SpellEntry.Entries.Get(enType);

            ErrorHelper.Assert(entry != null, ErrorHelper.UnknownEntry);

            __result = entry.IsMagicSkill;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(SpellCodex.IsMeleeSkill))]
        static bool IsMeleeSkill_Prefix(SpellCodex.SpellTypes enType, ref bool __result)
        {
            var entry = SpellEntry.Entries.Get(enType);

            ErrorHelper.Assert(entry != null, ErrorHelper.UnknownEntry);

            __result = entry.IsMeleeSkill;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(SpellCodex.IsUtilitySkill))]
        static bool IsUtilitySkill_Prefix(SpellCodex.SpellTypes enType, ref bool __result)
        {
            var entry = SpellEntry.Entries.Get(enType);

            if (entry == null)
            {
                __result = false;
            }
            else
            {
                __result = entry.IsUtilitySkill;
            }

            return false;
        }
    }
}
