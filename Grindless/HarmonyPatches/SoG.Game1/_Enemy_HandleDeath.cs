using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Enemy_HandleDeath))]
    static class _Enemy_HandleDeath
    {
        static void Postfix(Enemy xEnemy, AttackPhase xAttackPhaseThatHit)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.PostEnemyKilled(xEnemy, xAttackPhaseThatHit);
        }
    }
}
