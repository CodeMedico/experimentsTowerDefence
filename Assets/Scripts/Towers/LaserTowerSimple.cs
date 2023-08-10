using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTowerSimple : Tower
{

    protected Laser[] lasers;

    protected override void Start()
    {
        base.Start();
        lasers = new Laser[numberOfEnemies];
    }
    protected override bool CanAttack()
    {
        return true;
    }

    protected override void CreateAndSetProjectile()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (lasers[i] == null)
            {
                lasers[i] = Instantiate(projectile as Laser, projectileSpawnPoint.transform.position, Quaternion.identity, transform);
                lasers[i].SetStartTransformAndRate(projectileSpawnPoint, towerSO.AttacRate, towerSO.AttacRate);
            }
            if (lasers[i].target != enemiesInTarget[i])
            {
                lasers[i].SetTarget(enemiesInTarget[i]);
            }
        }
    }

    protected override void SetBehaviour(Projectile projectile)
    {

    }
}
