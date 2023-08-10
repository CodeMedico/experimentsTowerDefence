using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static RefractorTower;

public class LaserBurst : Laser
{
    protected override async void LaserHit()
    {
        if (damageFrequency >= maxDamageFrequency)
        {
            laserBody.SetPosition(1, Vector3.Lerp(laserBody.GetPosition(1), target.GetPosition(), 1f));
            await Task.Delay(100);
            laserBody.SetPosition(1, Vector3.Lerp(laserBody.GetPosition(1), transform.position, 1f));
        }
    }


    protected override void LaserDamageTarget()
    {
        if (target is EnemyScript)
        {
            base.LaserDamageTarget();
        }
        else
        {
            target.Damage(LaserDamage(0));
            damageFrequency = 0;
            timeWithNoTarget = 0f;
        }
    }

    protected override bool IsTargetValid()
    {
        if (target is not EnemyScript)
        {
            RefractorTower target = this.target as RefractorTower;
            return target.attackState != RefractorTower.AttackState.SeekTarget;
        }
        return base.IsTargetValid();
    }

    public override float LaserDamage(float damage)
    {
        return GetComponentInParent<RefractorTower>().GetCumulatedDamage();
    }

    public float TimeToNextAttack()
    {
        return maxDamageFrequency - damageFrequency;
    }
}
