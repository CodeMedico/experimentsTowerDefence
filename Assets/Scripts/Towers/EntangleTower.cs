using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntangleTower : Tower
{
    protected override Vector3 SpawnPoint(ITarget enemy)
    {
        return enemy.GetPosition();
    }
    protected override bool EnemyTargetInValid(ITarget enemy)
    {
        return base.EnemyTargetInValid(enemy) || enemy.GetStatusEffects().ContainsKey(StatusEffect.Name.Entangle);
    }
}
