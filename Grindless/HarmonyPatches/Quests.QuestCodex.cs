using Quests;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(QuestCodex))]
    static class Quests_QuestCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(QuestCodex.GetQuestDescription))]
        static bool GetQuestDescription_Prefix(ref QuestDescription __result, QuestCodex.QuestID p_enID)
        {
            __result = QuestEntry.Entries.GetRequired(p_enID).Vanilla;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(QuestCodex.GetQuestInstance))]
        static bool GetQuestInstance_Prefix(ref Quest __result, QuestCodex.QuestID p_enID)
        {
            var entry = QuestEntry.Entries.GetRequired(p_enID);

            if (entry.Constructor == null && entry.IsVanilla)
                return true;

            __result = new Quest
            {
                enQuestID = p_enID,
                xDescription = entry.Vanilla,
                xReward = entry.Vanilla.xReward
            };

            entry.Constructor.Invoke(__result);
            return false;
        }
    }
}
