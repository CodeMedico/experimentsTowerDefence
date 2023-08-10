using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTower : Tower
{
    protected override void SetBehaviour(Projectile projectile)
    {
        base.SetBehaviour(projectile);
        projectile.onCollisionBehaviour += () => CollisionBehaviour.Splash(projectile, 3, layerMaskEnemy);
    }
}
