using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PathTile : MonoBehaviour, ITile
{
    [SerializeField] private TileSO tileSO;
    [SerializeField] private Transform greenRedQuads;

    private delegate void ApllyStatusEffect(EnemyScript enemy);
    private ApllyStatusEffect OnEnemyStep;
    private EnemyScript lastEnemy;
    private List<StatusEffect> statusEffects = new List<StatusEffect>();

    public TileType Type { get; set; } = TileType.PathTile;

    private void Start()
    {
        PathFindManager.Instance.SendPosition(this);
        /*if (PathFindManager.Instance.RoundVector(transform.position) == Vector3.zero)
        {
            AddStatusProperty(StatusEffect.Name.Slow, 0.7f, 100);
            AddStatusProperty(StatusEffect.Name.Posion, 1f, 20, 500);
        }*/
    }


    private void Update()
    {
        if (OnEnemyStep != null)
        {
            TryApplyEffect();
        }
    }


    private void TryApplyEffect()
    {
        Physics.Raycast(transform.position, Vector3.up, out RaycastHit rayCastHit, 1f);
        if (rayCastHit.transform != null)
        {
            if (rayCastHit.transform.TryGetComponent<EnemyScript>(out EnemyScript enemyHited))
            {
                if (enemyHited != lastEnemy)
                {
                    OnEnemyStep(enemyHited);
                    lastEnemy = enemyHited;
                    OnEnemyStep = null;
                    foreach (StatusEffect effect in statusEffects)
                    {
                        OnEnemyStep += new StatusEffect(effect).ApplyEffect;
                    }

                }
            }
        }
    }

    public void AddStatusProperty(StatusEffect.Name name, float effectPower, float duration, int ticRateInMs = 0, bool chainPosion = false)
    {
        StatusEffect statusEffect = new StatusEffect(name, effectPower, duration, ticRateInMs, chainPosion);
        statusEffects.Add(statusEffect);
        OnEnemyStep += statusEffect.ApplyEffect;
    }

    public Transform GetTransform() { return transform; }
}
