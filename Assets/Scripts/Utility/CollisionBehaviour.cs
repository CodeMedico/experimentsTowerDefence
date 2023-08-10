using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Projectile;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class CollisionBehaviour
{

    public static void DefaultBehaviour(Projectile projectile)
    {
        projectile.onCollisionBehaviour += () => UnityEngine.Object.Destroy(projectile.gameObject);
    }

    public static void Chain(Projectile projectile, int chains, LayerMask layerMask, bool lastHitExplosion = false, ITarget[] targetsHited = null)
    {
        if (targetsHited == null)
        {
            targetsHited = new ITarget[chains];
        }
        chains--;
        if (projectile.target == null) return;
        targetsHited[targetsHited.Length - chains - 1] = projectile.target;
        projectile.SetTarget(SearchTarget(projectile, targetsHited, layerMask));
        if (chains == 0)
        {
            DefaultBehaviour(projectile);
            if (lastHitExplosion)
            {
                projectile.onCollisionBehaviour += () => Splash(projectile, 3, layerMask);
            }
        }
        else
        {
            Delegate[] invocationList = projectile.onCollisionBehaviour.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                Debug.Log(invocationList[i].Method.Name);
                if (invocationList[i].Method.Name.Contains("Chain") || invocationList[i].Method.Name.Contains("SetBehaviour"))
                {
                    Debug.Log("removed" + invocationList[i].Method.Name);
                    projectile.onCollisionBehaviour -= (OnCollisionBehaviour)invocationList[i];
                }
            }
            projectile.onCollisionBehaviour += () => Chain(projectile, chains, layerMask, lastHitExplosion, targetsHited);
        }
    }

    public static void Splash(Projectile projectile, float splashRadius, LayerMask layerMask)
    {
        if (projectile.target == null) return;
        Collider[] enemiesBuffer = new Collider[30];
        int enemies = Physics.OverlapSphereNonAlloc(projectile.transform.position, splashRadius, enemiesBuffer, layerMask);
        for (int i = 0; i < enemies; i++)
        {
            EnemyScript enemyInArea = enemiesBuffer[i].GetComponent<EnemyScript>();
            if (enemyInArea as ITarget != projectile.target)
            {
                enemyInArea.Damage(projectile.GetProjectileSO().Damage);
                projectile.StatusEffect?.Invoke(enemyInArea);
                projectile.StatusEffect = null;
                if (projectile.projectileSO.statusEffects != null)
                {
                    foreach (StatusEffect effect in projectile.projectileSO.statusEffects)
                    {
                        projectile.StatusEffect += new StatusEffect(effect).ApplyEffect;
                    }

                }
            }
        }
    }

    private static ITarget SearchTarget(Projectile projectile, ITarget[] targetsHited, LayerMask layerMask)
    {
        Collider[] enemiesBuffer = new Collider[20];
        ITarget nearestEnemy = null;
        int enemies = Physics.OverlapSphereNonAlloc(projectile.transform.position, 5f, enemiesBuffer, layerMask);
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < enemies; i++)
        {
            if (enemiesBuffer[i].TryGetComponent<ITarget>(out ITarget enemyFinded))
            {
                float distance = Vector3.Distance(projectile.transform.position, enemyFinded.GetPosition());
                if (distance < nearestDistance && !targetsHited.Contains(enemyFinded))
                {
                    nearestDistance = distance;
                    nearestEnemy = enemyFinded;
                }
            }
        }

        return nearestEnemy;
    }

}
