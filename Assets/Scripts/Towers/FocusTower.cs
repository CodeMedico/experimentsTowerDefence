using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FocusTower : LaserTowerSimple
{
    protected override void Start()
    {
        base.Start();
        FocusLaser focusLaser = projectile as FocusLaser;
        focusLaser.beamLenght = towerRange;
    }

    protected override void ValideteFindedTarget(ref float distance, float distanceToEnemyFinded, ref ITarget enemy, ITarget enemyFinded)
    {
        if (!enemiesInTarget.Contains(enemyFinded))
        {
            distance = distance < distanceToEnemyFinded ? distance : distanceToEnemyFinded;
            enemy = distance < distanceToEnemyFinded ? enemy : enemyFinded;
            if (distance != float.MaxValue)
            {
                distance = distance > distanceToEnemyFinded ? distance : distanceToEnemyFinded;
                enemy = distance > distanceToEnemyFinded ? enemy : enemyFinded;
            }
        }
    }
}
