using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._NPC_TakeDamage))]
    static class _NPC_TakeDamage
    {
        static void Prefix(NPC xEnemy, ref int iDamage, ref byte byType)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.OnNPCDamaged(xEnemy, ref iDamage, ref byType);
        }

    }
}
