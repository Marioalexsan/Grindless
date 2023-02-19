using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.__StartupThreadExecute))]
    static class __StartupThreadExecute
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
        {
            var codeList = code.ToList();

            MethodInfo target = typeof(DialogueCharacterLoading).GetMethod("Init");

            MethodInfo targetTwo = typeof(Game1).GetMethod(nameof(Game1._Loading_LoadGlobalFile));

            var insert = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => PrepareModLoader()))
            };

            var moreInsert = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => Globals.UpdateVersionNumber()))
            };

            return codeList
                .InsertAfterMethod(target, insert)
                .InsertBeforeMethod(targetTwo, moreInsert);
        }

        static void PrepareModLoader()
        {
            GrindlessResources.ReloadResources();
            ModManager.Reload();

            MainMenuWorker.UpdateStorySaveCompatibility();
            MainMenuWorker.UpdateArcadeSaveCompatibility();
        }
    }
}
