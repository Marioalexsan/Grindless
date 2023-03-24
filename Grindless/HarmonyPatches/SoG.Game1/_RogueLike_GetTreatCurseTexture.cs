using Microsoft.Xna.Framework.Graphics;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._RogueLike_GetTreatCurseTexture))]
    static class _RogueLike_GetTreatCurseTexture
    {
        static bool Prefix(RogueLikeMode.TreatsCurses enTreat, ref Texture2D __result)
        {
            var entry = CurseEntry.Entries.Get(enTreat);

            if (entry == null)
            {
                // Covers the case when the treat / curse is None.
                __result = RenderMaster.txNullTex;
                return false;
            }

            if (entry.TexturePath == null && entry.IsVanilla)
                return true;

            __result = Globals.Game.Content.TryLoad<Texture2D>(entry.TexturePath);
            return false;
        }

    }
}
