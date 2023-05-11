using System.Reflection;

namespace Grindless.HarmonyPatches;

[HarmonyPatch]
internal static class CrashFinalizers
{
    static IEnumerable<MethodInfo> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Game1), "Update");
        yield return AccessTools.Method(typeof(Game1), "Draw");
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__State_LoadCommonTexturesInThread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__StartupThreadExecute));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__SynchWithServer_StartThread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__Level_BeginLoadThread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__Menu_PWRogueLikeSingleStartThread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__LevelLoading_StartPrePreAssetLoading_Thread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1._Animations_FillWeaponAnimations_Thread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__LevelLoading_PreLoadAssetsInThread_Thread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__LevelLoading_StartPrePreAssetLoading_Thread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__Network_FetchTranslationPreviewImageThread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.__Menu_PWSinglePlayerStartThread));
        yield return AccessTools.Method(typeof(Game1), nameof(Game1._Animations_FillWeaponAnimations_Thread));

    }

    static Exception Finalizer(Exception __exception)
    {
        if (__exception != null)
            ErrorHelper.ForceExit(__exception);

        return null;
    }
}
