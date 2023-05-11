using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(EnemyCodex))]
internal static class SoG_EnemyCodex
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(EnemyCodex.GetEnemyDescription))]
    internal static bool GetEnemyDescription_Prefix(ref EnemyDescription __result, EnemyCodex.EnemyTypes enType)
    {
        __result = EnemyEntry.Entries.GetRequired(enType).Vanilla;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(EnemyCodex.GetEnemyInstance))]
    internal static bool GetEnemyInstance_Prefix(EnemyCodex.EnemyTypes enType, Level.WorldRegion enOverrideContent, ref Enemy __result)
    {
        var entry = EnemyEntry.Entries.GetRequired(enType);

        if (entry.Constructor == null && entry.IsVanilla)
            return true;

        __result = EditedMethods.GetModdedEnemyInstance(enType, enOverrideContent);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(EnemyCodex.GetEnemyDefaultAnimation))]
    public static bool GetEnemyDefaultAnimation_Prefix(ref Animation __result, EnemyCodex.EnemyTypes enType, ContentManager Content)
    {
        var entry = EnemyEntry.Entries.GetRequired(enType);

        if (entry.DefaultAnimation == null && entry.IsVanilla)
            return true;

        __result = entry.DefaultAnimation?.Invoke(Content) ??
            new Animation(0, 0, GrindlessResources.NullTexture, Vector2.Zero);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(EnemyCodex.GetEnemyDisplayIcon))]
    public static bool GetEnemyDisplayIcon_Prefix(ref Texture2D __result, EnemyCodex.EnemyTypes enType, ContentManager Content)
    {
        var entry = EnemyEntry.Entries.GetRequired(enType);

        if (entry.DisplayIconPath == null && entry.IsVanilla)
            return true;

        __result = Content.TryLoad<Texture2D>(entry.DisplayIconPath);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(EnemyCodex.GetEnemyLocationPicture))]
    public static bool GetEnemyLocationPicture_Prefix(ref Texture2D __result, EnemyCodex.EnemyTypes enType, ContentManager Content)
    {
        var entry = EnemyEntry.Entries.GetRequired(enType);

        if (entry.DisplayBackgroundPath == null && entry.IsVanilla)
            return true;

        __result = Content.TryLoad<Texture2D>(entry.DisplayBackgroundPath);
        return false;
    }
}
