using Microsoft.Xna.Framework.Graphics;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._Menu_Render))]
    static class _Menu_Render
    {
        static void Postfix()
        {
            SpriteBatch spriteBatch = Globals.SpriteBatch;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, null);

            if (Globals.Game.xGlobalData.xMainMenuData.enMenuLevel == MainMenuWorker.ReservedModMenuID)
                MainMenuWorker.ModMenuRender();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, null);
        }

    }
}
