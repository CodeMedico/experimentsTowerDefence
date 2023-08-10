using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MagicNatureTower : Tower
{
    protected override ITarget FindTarget()
    {
        ITarget enemy = null;
        float distance = float.MaxValue;
        int enemies = Physics.OverlapSphereNonAlloc(transform.position, towerRange, collidersBuffer, layerMaskEnemy);
        int i = Random.Range(0, enemies);
        if (!collidersBuffer[i].IsDestroyed() && collidersBuffer[i] != null)
        {
            if (collidersBuffer[i].TryGetComponent<ITarget>(out ITarget enemyFinded))
            {
                float distanceToEnemyFinded = Vector3.Distance(enemyFinded.GetPosition(), transform.position);
                ValideteFindedTarget(ref distance, distanceToEnemyFinded, ref enemy, enemyFinded);
            }
        }
        return enemy;
    }

    protected override void PerformAttack()
    {
        base.PerformAttack();
        for (int i = 0; i < numberOfEnemies; i++)
        {
            enemiesInTarget[i] = FindTarget();
        }
    }
}
