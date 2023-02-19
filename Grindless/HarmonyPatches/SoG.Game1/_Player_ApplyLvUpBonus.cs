using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Player_ApplyLvUpBonus))]
    static class _Player_ApplyLvUpBonus
    {
        static void Postfix(PlayerView xView)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.PostPlayerLevelUp(xView);
        }
    }
}
