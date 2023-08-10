using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueTower : Tower
{
    protected override bool EnemyTargetInValid(ITarget enemy)
    {
        return base.EnemyTargetInValid(enemy) || enemy.GetStatusEffects().ContainsKey(StatusEffect.Name.Posion);
    }
}
