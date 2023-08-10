using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Projectile;

public abstract class Tower : MonoBehaviour, ITile
{
    [SerializeField] protected Transform projectileSpawnPoint;
    [SerializeField] protected TowerSO towerSO;
    [SerializeField] protected LayerMask layerMaskEnemy;
    [SerializeField] protected int numberOfEnemies = 1;


    protected Projectile projectile;
    protected float towerRange;
    protected Collider[] collidersBuffer = new Collider[40];
    public float attckRate;
    public ITarget[] enemiesInTarget;

    public TileType Type { get; set; } = TileType.Building;

    protected virtual void Start()
    {
        enemiesInTarget = new ITarget[numberOfEnemies];
        towerRange = towerSO.TowerRange;
        attckRate = towerSO.AttacRate;
        projectile = towerSO.ProjectileSO.Prefab.GetComponent<Projectile>();
        PathFindManager.Instance.SendPosition(this);
    }

    protected virtual void Update()
    {
        TryPerformAttack();
    }

    protected virtual ITarget FindTarget()
    {
        ITarget enemy = null;
        float distance = float.MaxValue;
        int enemies = Physics.OverlapSphereNonAlloc(transform.position, towerRange, collidersBuffer, layerMaskEnemy);

        for (int i = 0; i < enemies; i++)
        {
            if (collidersBuffer[i].TryGetComponent<ITarget>(out ITarget enemyFinded))
            {
                float distanceToEnemyFinded = Vector3.Distance(enemyFinded.GetPosition(), transform.position);
                ValideteFindedTarget(ref distance, distanceToEnemyFinded,ref enemy,enemyFinded);
            }
        }

        return enemy;
    }

    protected virtual void TryPerformAttack()
    {

        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (EnemyTargetInValid(enemiesInTarget[i]))
            {
                enemiesInTarget[i] = FindTarget();
            }
        }

        if (CanAttack())
        {
            PerformAttack();
        }
        attckRate += Time.deltaTime;
    }

    protected virtual void PerformAttack()
    {
        CreateAndSetProjectile();
        attckRate = 0f;
    }
    protected virtual void SetBehaviour(Projectile projectile)
    {
        projectile.onCollisionBehaviour += () => Destroy(projectile.gameObject);
    }

    protected virtual void CreateAndSetProjectile()
    {
        foreach (ITarget enemyInTarget in enemiesInTarget)
        {
            if (enemyInTarget != null)
            {
                Projectile shotedProjectile = Instantiate(projectile, SpawnPoint(enemyInTarget), Quaternion.identity, transform);
                shotedProjectile.SetTarget(enemyInTarget);
                SetBehaviour(shotedProjectile);
            }
        }
    }

    protected virtual bool CanAttack()
    {
        return attckRate > towerSO.AttacRate && enemiesInTarget.Any(enemy => enemy != null);
    }

    protected virtual bool EnemyTargetInValid(ITarget enemy)
    {
        return enemy == null || enemy.IsDestroyed() || Vector3.Distance(transform.position, enemy.GetPosition()) > towerRange;
    }

    protected virtual Vector3 SpawnPoint(ITarget enemy)
    {
        return projectileSpawnPoint.position;
    }

    protected virtual void ValideteFindedTarget(ref float distance,float distanceToEnemyFinded, ref ITarget enemy, ITarget enemyFinded)
    {
        if (!enemiesInTarget.Contains(enemyFinded))
        {
            distance = distance < distanceToEnemyFinded ? distance : distanceToEnemyFinded;
            enemy = distance < distanceToEnemyFinded ? enemy : enemyFinded;
        }
    }

    public TowerSO GetTowerSO()
    {
        return towerSO;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
