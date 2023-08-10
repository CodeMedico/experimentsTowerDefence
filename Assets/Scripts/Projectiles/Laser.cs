using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Projectile
{
    protected Transform initialPoint;

    protected LineRenderer laserBody;
    protected float damageFrequency;
    protected float maxDamageFrequency;
    protected float timeWithNoTarget = 0f;

    protected override void Start()
    {
        if (projectileSO.statusEffects != null)
        {
            foreach (StatusEffect statusEffect in projectileSO.statusEffects)
            {
                StatusEffect += new StatusEffect(statusEffect).ApplyEffect;
            }
        }
        laserBody = GetComponent<LineRenderer>();
        laserBody.positionCount = 2;
        laserBody.SetPosition(0, initialPoint.position);
        laserBody.SetPosition(1, initialPoint.position);
    }
    protected override void Update()
    {
        base.Update();
        laserBody.SetPosition(0, initialPoint.position);
    }
    protected override void MoveTowardsTarget(ITarget enemy)
    {
        if (IsTargetValid())
        {
            LaserHit();
        }
        if (!IsTargetValid())
        {
            if (timeWithNoTarget > 0.05f)
            {
                laserBody.SetPosition(1, initialPoint.position);
            }
            timeWithNoTarget += Time.deltaTime;
        }
        else if (damageFrequency >= maxDamageFrequency)
        {
            LaserDamageTarget();
        }
        damageFrequency += Time.deltaTime;
    }

    public void SetStartTransformAndRate(Transform transform, float damageFrequency, float maxDamageFrequency)
    {
        initialPoint = transform;
        this.maxDamageFrequency = maxDamageFrequency;
        this.damageFrequency = damageFrequency;
    }

    protected virtual void LaserHit()
    {
        laserBody.SetPosition(1, Vector3.Lerp(laserBody.GetPosition(1), target.GetPosition(), .3f));
    }

    protected virtual void LaserDamageTarget()
    {
        if (Physics.Raycast(target.GetPosition() + new Vector3(0, 4f, 0), Vector3.down, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.TryGetComponent<EnemyScript>(out EnemyScript enemyHitted))
            {
                StatusEffect?.Invoke(enemyHitted);
                StatusEffect = null;
                if (projectileSO.statusEffects != null)
                {
                    foreach (StatusEffect effect in projectileSO.statusEffects)
                    {
                        StatusEffect += new StatusEffect(effect).ApplyEffect;
                    }

                }
                onCollisionBehaviour?.Invoke();
                enemyHitted.Damage(LaserDamage(projectileSO.Damage));
            }
            damageFrequency = 0;
            timeWithNoTarget = 0f;
        }
    }

    public virtual float LaserDamage(float damage)
    {
        return damage;
    }
}
