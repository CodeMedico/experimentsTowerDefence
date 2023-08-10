using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
    Vector3 GetPosition();
    bool IsDestroyed();
    EnemyScript NearestEnemy();
    public Dictionary<StatusEffect.Name, StatusEffect> GetStatusEffects();
    public void Damage(float damage);
}
