﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Grindless;

public class ColliderRC : RenderComponent
{
		public bool RenderCombat { get; set; }
		public bool RenderLevel { get; set; }
		public bool RenderMovement { get; set; }

		public ColliderRC()
    {
			xTransform = new TransformComponent(Vector2.Zero);
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        PlayerView localPlayer = Globals.Game.xLocalPlayer;
        CollisionMaster colliders = Globals.Game.xCollisionMaster;

        if (!localPlayer.bInitializedToServer || Globals.Game.xLevelMaster.xZoningHelper.IsZoning)
            return;

        if (RenderLevel)
        {
            foreach (Collider col in colliders.lxStaticColliders)
            {
                int iNine = 512;
                if ((iNine & col.ibitLayers) == 0 && (col.ibitLayers & localPlayer.xEntity.xCollisionComponent.ibitCurrentColliderLayer) != 0)
                {
                    col.Render(spriteBatch);
                }
            }
        }

        if (RenderCombat)
        {
            foreach (Collider col in colliders.lxAttackboxColliders)
            {
                col.Render(spriteBatch);
            }

            foreach (var pair in colliders.dexHitboxColliders)
            {
                if (pair.Key == Collider.ColliderLayers.HeighDifferenceIntercept)
                    continue;

                foreach (Collider col in pair.Value)
                {
                    col.Render(spriteBatch);
                }
            }
        }

        if (RenderMovement)
        {
            foreach (Collider col in colliders.lxMovementColliders)
            {
                col.Render(spriteBatch);
            }
        }
    }
}
