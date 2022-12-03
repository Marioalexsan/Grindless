using SoG;
using static SoG.InGameMenu;

namespace Grindless
{
    public static class SpellExtension
    {
        public static bool IsGeneralTalent(this SpellCodex.SpellTypes skill)
        {
            return new SkillView().lenGeneralTalentDisplayOrder.Contains(skill);
        }

        public static bool IsMeleeTalent(this SpellCodex.SpellTypes skill)
        {
            return new SkillView().lenMeleeTalentDisplayOrder.Contains(skill);
        }

        public static bool IsMagicTalent(this SpellCodex.SpellTypes skill)
        {
            return new SkillView().lenMagicTalentDisplayOrder.Contains(skill);
        }

        public static bool IsTalent(this SpellCodex.SpellTypes skill)
        {
            return SpellCodex.IsTalent(skill);
        }
    }
}
