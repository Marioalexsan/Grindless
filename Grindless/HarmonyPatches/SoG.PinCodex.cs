using HarmonyLib;
using SoG;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(PinCodex))]
    static class SoG_PinCodex
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PinCodex.GetInfo))]
        static bool GetInfo_Prefix(ref PinInfo __result, PinCodex.PinType enType)
        {
            var entry = PinEntry.Entries.Get(enType);

            if (entry == null)
                return true;

            string[] palettes = new string[]
            {
                "Test1",
                "Test2",
                "Test3",
                "Test4",
                "Test5"
            };

            __result = new PinInfo(
                enType,
                "None",
                entry.Description,
                entry.PinShape.ToString(),
                entry.PinSymbol.ToString(),
                entry.PinColor == PinEntry.Color.White ? "TestLight" : palettes[(int)entry.PinColor],
                entry.IsSticky,
                entry.IsBroken,
                FontManager.GetFontByCategory("SmallTitle", FontManager.FontType.Bold8Spacing1),
                FontManager.GetFontByCategory("InMenuDescription", FontManager.FontType.Reg7)
                );

            return false;
        }
    }
}
