using HarmonyLib;
using Quests;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(QuestCodex))]
    static class Quests_QuestCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(QuestCodex.GetQuestDescription))]
        static bool GetQuestDescription_Prefix(ref QuestDescription __result, QuestCodex.QuestID p_enID)
        {
            var entry = QuestEntry.Entries.Get(p_enID);

            if (entry == null)
            {
                return true;  // Unknown mod entry?!
            }

            __result = entry.Vanilla;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(QuestCodex.GetQuestInstance))]
        static bool GetQuestInstance_Prefix(ref Quest __result, QuestCodex.QuestID p_enID)
        {
            var entry = QuestEntry.Entries.Get(p_enID);

            if (entry == null)
            {
                return true;  // Unknown mod entry?!
            }

            if (entry.Constructor == null && entry.IsVanilla)
            {
                __result = OriginalMethods.GetQuestInstance(p_enID);
                return false;
            }

            __result = new Quest
            {
                enQuestID = p_enID,
                xDescription = entry.Vanilla
            };

            entry.Constructor.Invoke(__result);

            __result.xReward = entry.Vanilla.xReward;

            return false;
        }

    }
}
