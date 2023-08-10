using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : Projectile
{
    public override void SetTarget(ITarget target)
    {
        base.target = new PointTarget(target.GetPosition());
    }

    protected override void Update()
    {
        MoveTowardsTarget(target);
        if (Vector3.SqrMagnitude(transform.position - target.GetPosition()) < .03f || transform.position.y <0f)
        {
            onCollisionBehaviour?.Invoke();
        }
    }
}
