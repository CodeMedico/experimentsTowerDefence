using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static RefractorTower;

public class RefractorTower : LaserTowerSimple, ITarget
{

    private List<RefractorTower> refractorTowers = new List<RefractorTower>();
    private List<RefractorTower> route = new List<RefractorTower>();
    private float cumulatedDamage;

    public enum AttackState
    {
        SeekTarget,
        AttackTower,
        AttackEnemy,
    }
    public AttackState attackState = AttackState.SeekTarget;

    protected override void Start()
    {
        base.Start();
        PathFindManager.Instance.RefractorTowerAdd(this);
        cumulatedDamage = towerSO.ProjectileSO.Damage;
        UpdateTowersList();
        foreach (var tower in refractorTowers)
        {
            if (tower != this)
            {
                tower.UpdateTowersList();
            }
        }
    }
    protected override ITarget FindTarget()
    {
        route.Clear();
        ITarget target = null;
        target = base.FindTarget();
        if (target is not null)
        {
            route.Add(this);
            attackState = AttackState.AttackEnemy;
            return target;
        }
        int routeCount = int.MaxValue;
        foreach (RefractorTower refractorTower in refractorTowers)
        {
            if (Vector3.Distance(refractorTower.GetPosition(), transform.position) < towerRange && refractorTower != this)
            {
                List<RefractorTower> tryRoute = new List<RefractorTower>(refractorTower.GetRoute());
                if (tryRoute.Count > 0 && routeCount > tryRoute.Count &&
                    tryRoute.Any(tower => tower.attackState == AttackState.AttackEnemy))
                {
                    target = tryRoute[tryRoute.Count - 1];
                    attackState = AttackState.AttackTower;
                    route = new List<RefractorTower>(tryRoute) { this };
                    routeCount = tryRoute.Count;
                }
            }
        }
        if (target == null) attackState = AttackState.SeekTarget;
        return target;
    }

    protected override bool CanAttack()
    {
        if (attackState == AttackState.AttackEnemy) return true;
        if (route.Any(tower => tower.attackState == AttackState.AttackEnemy)) return true;
        return false;
    }

    protected override bool EnemyTargetInValid(ITarget target)
    {
        if (target is EnemyScript)
        {
            return base.EnemyTargetInValid(target);
        }
        else
        {
            return !route.Any(tower => tower.attackState == AttackState.AttackEnemy);
        }
    }
    protected override void CreateAndSetProjectile()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (lasers[i] == null)
            {
                float delay = 0;
                if (enemiesInTarget[i] is RefractorTower)
                {
                    RefractorTower refractorTower = (RefractorTower)enemiesInTarget[i];
                    LaserBurst burst = refractorTower.lasers[i] as LaserBurst;
                    delay = burst.TimeToNextAttack();
                }
                lasers[i] = Instantiate(projectile as Laser, projectileSpawnPoint.transform.position, Quaternion.identity, transform);
                lasers[i].SetStartTransformAndRate(projectileSpawnPoint, towerSO.AttacRate-delay,towerSO.AttacRate);
            }
            if (lasers[i].target != enemiesInTarget[i])
            {
                lasers[i].SetTarget(enemiesInTarget[i]);
            }
        }
    }

    private void UpdateTowersList()
    {
        refractorTowers = PathFindManager.Instance.GetRefractorTowersList();
    }

    public void Damage(float damage)
    {
        cumulatedDamage += damage;
        Debug.Log(cumulatedDamage);
    }

    public float GetCumulatedDamage()
    {
        float damage = cumulatedDamage;
        cumulatedDamage = towerSO.ProjectileSO.Damage;
        return damage;
    }

    public List<RefractorTower> GetRoute()
    {
        return route;
    }

    public Vector3 GetPosition() { return projectileSpawnPoint.position; }

    public bool IsDestroyed() { return this == null; }

    public EnemyScript NearestEnemy() { return null; }

    public Dictionary<StatusEffect.Name, StatusEffect> GetStatusEffects() { return null; }
}