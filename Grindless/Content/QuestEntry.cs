using Quests;
using SoG;
using System;

namespace Grindless
{
    /// <summary>
    /// Represents a modded quest.
    /// </summary>
    /// <remarks> 
    /// Most of the methods in this class can only be used while a mod is loading, that is, inside <see cref="Mod.Load"/>.
    /// </remarks>
    public class QuestEntry : Entry<QuestCodex.QuestID>
    {
        internal static EntryManager<QuestCodex.QuestID, QuestEntry> Entries { get; }
            = new EntryManager<QuestCodex.QuestID, QuestEntry>(45000);

        internal QuestDescription Vanilla { get; set; } = new QuestDescription();

        /// <summary>
        /// Gets or sets the display name of this quest.
        /// </summary>
        public string Name { get; set; } = "Unknown Mod Quest";

        /// <summary>
        /// Gets or sets the description of this quest's entry in the journal.
        /// </summary>
        public string Summary { get; set; } = "Some random quest from a mod! It's probably important.";

        /// <summary>
        /// Gets or sets the full description of this quest. <para/>
        /// The full description is displayed whenever you start a quest.
        /// </summary>
        public string Description { get; set; } = "Dunno man, ask the modder about the quest details! He forgot to put them in, shesh.";

        /// <summary>
        /// Gets or sets the recommended player level.
        /// The difficulty star count in the journal adapts based on the difference between
        /// the player level and the recommended level of the quest.
        /// </summary>
        public int RecommendedPlayerLevel { get; set; }

        /// <summary>
        /// Gets or sets the quest's type.
        /// </summary>
        public QuestDescription.QuestType Type { get; set; }

        /// <summary>
        /// Gets or sets the quest's reward.
        /// </summary>
        public QuestReward Reward { get; set; }

        /// <summary>
        /// Gets or sets the constructor for the quest instance. <para/>
        /// The constructor is called wheneever a new quest of this type is started.
        /// You can use it to setup objectives and other things.
        /// </summary>
        public Action<Quest> Constructor { get; set; }

        internal QuestEntry() {
            SymbolicItemFlagReward noReward = new SymbolicItemFlagReward();
            noReward.AddItem(ItemCodex.ItemTypes._Misc_BagLol, 1);

            Vanilla.xReward = noReward;
        }

        internal QuestEntry(Mod mod, string modID, QuestCodex.QuestID gameID)
            : base(mod, modID, gameID)
        {
            SymbolicItemFlagReward noReward = new SymbolicItemFlagReward();
            noReward.AddItem(ItemCodex.ItemTypes._Misc_BagLol, 1);

            Vanilla.xReward = noReward;
        }

        protected override void Initialize()
        {
            if (!IsVanilla)
            {
                Vanilla.sQuestNameReference = $"Quest_{(int)GameID}_Name";
                Vanilla.sSummaryReference = $"Quest_{(int)GameID}_Summary";
                Vanilla.sDescriptionReference = $"Quest_{(int)GameID}_Description";
            }

            Globals.Game.EXT_AddMiscText("Quests", Vanilla.sQuestNameReference, Name);
            Globals.Game.EXT_AddMiscText("Quests", Vanilla.sSummaryReference, Summary);
            Globals.Game.EXT_AddMiscText("Quests", Vanilla.sDescriptionReference, Description);
        }

        protected override void Cleanup()
        {
            Globals.Game.EXT_RemoveMiscText("Quests", Vanilla.sQuestNameReference);
            Globals.Game.EXT_RemoveMiscText("Quests", Vanilla.sSummaryReference);
            Globals.Game.EXT_RemoveMiscText("Quests", Vanilla.sDescriptionReference);
        }
    }
}
