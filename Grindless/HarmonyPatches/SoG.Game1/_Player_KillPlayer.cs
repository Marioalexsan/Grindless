using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Player_KillPlayer), new Type[] { typeof(PlayerView), typeof(bool), typeof(bool) })]
    static class _Player_KillPlayer
    {
        static void Prefix(PlayerView xView)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.OnPlayerKilled(xView);
        }
    }
}
