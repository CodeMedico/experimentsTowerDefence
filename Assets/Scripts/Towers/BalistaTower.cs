using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BalistaTower : Tower
{
    [SerializeField] private int maximumCharges = 5;
    [SerializeField] private float maximumShotInterval = .2f;

    private int charges = 0;
    private float shotInterval = 0;

    protected override void TryPerformAttack()
    {
        base.TryPerformAttack();
        if (attckRate >= towerSO.AttacRate)
        {
            charges++;
            if (charges > maximumCharges)
            {
                charges = maximumCharges;
            }
            attckRate = 0;
        }
        shotInterval += Time.deltaTime;
    }

    protected override void PerformAttack()
    {
        base.PerformAttack();
        charges -= 1;
        shotInterval = 0;
    }

    protected override bool CanAttack()
    {
        return charges > 0 && shotInterval > maximumShotInterval && enemiesInTarget.Any(enemy => enemy != null);
    }
}
