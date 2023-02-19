using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_DeactivatePin))]
    internal class _RogueLike_DeactivatePin
    {
        static bool Prefix(PlayerView xView, PinCodex.PinType enEffect, bool bSend)
        {
            var entry = PinEntry.Entries.GetRequired(enEffect);

            EditedMethods.SendPinDeactivation(Globals.Game, xView, enEffect, bSend);

            if (entry.UnequipAction == null && entry.IsVanilla)
                EditedMethods.RemovePinEffect(Globals.Game, xView, enEffect, bSend);

            else entry.UnequipAction?.Invoke(xView);
            return false;
        }
    }
}
