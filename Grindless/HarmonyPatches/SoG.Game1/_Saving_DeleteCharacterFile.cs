namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Saving_DeleteCharacterFile))]
static class _Saving_DeleteCharacterFile
{
    static void Prefix(int iFileSlot)
    {
        File.Delete($"{Globals.AppDataPath}Characters/{iFileSlot}.cha.gs");
    }
}
