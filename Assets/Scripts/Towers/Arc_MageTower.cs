using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arc_MageTower : Tower
{
    [SerializeField] int numberOfChains;
    protected override void SetBehaviour(Projectile projectile)
    {
        projectile.onCollisionBehaviour += () => CollisionBehaviour.Chain(projectile, numberOfChains, layerMaskEnemy, true);
    }
}
