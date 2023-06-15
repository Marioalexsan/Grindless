namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_ActivatePin))]
static class _RogueLike_ActivatePin
{
    static bool Prefix(PlayerView xView, PinCodex.PinType enEffect, bool bSend)
    {
        var entry = Entries.Pins.GetRequired(enEffect);

        EditedMethods.SendPinActivation(Globals.Game, xView, enEffect, bSend);

        if (entry.EquipAction == null && entry.IsVanilla)
            EditedMethods.ApplyPinEffect(Globals.Game, xView, enEffect, bSend);

        else entry.EquipAction?.Invoke(xView);
        return false;
    }

}
