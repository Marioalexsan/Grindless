using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Enemy_TakeDamage))]
    static class _Enemy_TakeDamage
    {
        static void Prefix(Enemy xEnemy, ref int iDamage, ref byte byType)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.OnEnemyDamaged(xEnemy, ref iDamage, ref byType);
        }
    }
}
