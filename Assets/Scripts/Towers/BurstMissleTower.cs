using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BurstMissleTower : Tower
{
    private int missleDelay = 50;
    private int missleNumbers = 5;

    protected override async void PerformAttack()
    {
        while (missleNumbers != 0)
        {
            CreateAndSetProjectile();
            missleNumbers--;
            await Task.Delay(missleDelay);
        }
        attckRate = 0f;
        missleNumbers = 5;
    }
    protected override bool CanAttack()
    {
        return base.CanAttack() && missleNumbers == 5;
    }
}
