using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTarget : ITarget
{
    private Vector3 target;

    public PointTarget(Vector3 target)
    {
        this.target = target;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(target.x,0f,target.z);
    }
    public bool IsDestroyed()
    {
        return false;
    }

    public EnemyScript NearestEnemy()
    {
        return null;
    }

    public Dictionary<StatusEffect.Name, StatusEffect> GetStatusEffects()
    {
        return null;
    }

    public void Damage(float damage)
    {
        return;
    }
}

