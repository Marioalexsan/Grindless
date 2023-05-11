using Microsoft.Extensions.Logging;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Loading_LoadCharacterFromFile))]
static class _Loading_LoadCharacterFromFile
{
    static void Prefix()
    {
        // Required so that the vanilla save loads the "SoG-only" version
        Globals.SetVersionTypeAsModded(false);
    }

    static void Postfix(int iFileSlot, bool bAppearanceOnly)
    {
        Globals.SetVersionTypeAsModded(true);
        LoadCharacterMetadataFile(iFileSlot);
    }

    static void LoadCharacterMetadataFile(int slot)
    {
        string ext = ModSaving.SaveFileExtension;

        string chrFile = Globals.Game.sAppData + "Characters/" + $"{slot}.cha{ext}";

        if (!File.Exists(chrFile)) return;

        using var file = File.OpenRead(chrFile);

        Program.Logger.LogInformation("Loading mod character {Slot}...", slot);
        ModSaving.LoadModCharacter(file);
    }
}
