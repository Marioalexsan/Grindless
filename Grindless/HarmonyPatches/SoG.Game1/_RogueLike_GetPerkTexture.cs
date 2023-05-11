using Microsoft.Xna.Framework.Graphics;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_GetPerkTexture))]
static class _RogueLike_GetPerkTexture
{
    static bool Prefix(RogueLikeMode.Perks enPerk, ref Texture2D __result)
    {
        var entry = PerkEntry.Entries.Get(enPerk);

        if (entry == null)
        {
            __result = GrindlessResources.NullTexture;
            return false;
        }

        if (entry.TexturePath == null && entry.IsVanilla)
            return true;

        __result = Globals.Game.Content.TryLoad<Texture2D>(entry.TexturePath);
        return false;
    }
}
