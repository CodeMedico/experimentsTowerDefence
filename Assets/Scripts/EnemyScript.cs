using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class EnemyScript : MonoBehaviour, ITarget
{
    [SerializeField] private EnemySO enemySO;
    [SerializeField] private LayerMask layerMaskEnemy;
    [SerializeField] private int numberOfStacks;

    public event EventHandler OnDamageTaken;
    public event EventHandler<OnStatusEffect> OnStatusEffectAdd;
    public event EventHandler<OnStatusEffect> OnStaatusEffectRemove;
    public class OnStatusEffect : EventArgs
    {
        public StatusEffect statusEffect;
    }

    private Collider[] enemiesBuffer = new Collider[20];
    private Dictionary<StatusEffect.Name, StatusEffect> statusEffects = new Dictionary<StatusEffect.Name, StatusEffect>();
    private StatusEffect[] toxinStacks;

    private Queue<Vector3> route;
    private Vector3 nextDestination;
    private float speed;
    private float health;



    private void Start()
    {
        toxinStacks = new StatusEffect[numberOfStacks];
        speed = enemySO.Speed;
        health = enemySO.HealthPoint;
            route = PathFindManager.Instance.GetRoute(transform.position);
            nextDestination = route.Dequeue();
    }


    private void Update()
    {
        PerformMovement();
        DestinationAchived();
    }

    private void PerformMovement()
    {
        Vector3 moveDirection = nextDestination - transform.position;
        moveDirection = Vector3.Normalize(moveDirection);
        float moveDistance = Time.deltaTime * speed;
        transform.position += moveDirection * moveDistance;
    }

    private void DestinationAchived()
    {
        if (Mathf.Round(transform.position.x * 5) / 5 == Mathf.Round(nextDestination.x * 5) / 5 && Mathf.Round(transform.position.z * 5) / 5 == Mathf.Round(nextDestination.z * 5) / 5)
        {
            if (route.Count > 0)
            {
                nextDestination = route.Dequeue();
            }
            else
            {
                EndPointAchived();
            }
        }
    }

    private void EndPointAchived()
    {
        //reduce life coint
        Destroy(gameObject);
    }
    public float GetEnemySpeed()
    {
        return speed;
    }

    public void SetEnemySpeed(float modifyedSpeed)
    {
        speed = modifyedSpeed;
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        OnStatusEffectAdd?.Invoke(this, new OnStatusEffect
        {
            statusEffect = statusEffect
        });
        if (statusEffect.name == StatusEffect.Name.Toxin)
        {
            for (int i = 0; i < toxinStacks.Length; i++)
            {
                if (toxinStacks[i] is null) { toxinStacks[i] = statusEffect; return; }
            }
        }
        statusEffects.Add(statusEffect.name, statusEffect);
    }
    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        OnStaatusEffectRemove?.Invoke(this, new OnStatusEffect
        {
            statusEffect = statusEffect
        });
        if (statusEffect.name == StatusEffect.Name.Toxin)
        {
            for (int i = 0; i < toxinStacks.Length; i++)
            {
                if (statusEffect == toxinStacks[i]) { toxinStacks[i] = null; return; }
            }
        }
        statusEffects.Remove(statusEffect.name);
    }

    public Dictionary<StatusEffect.Name, StatusEffect> GetStatusEffects()
    {
        return statusEffects;
    }

    public StatusEffect[] GetToxinStacks()
    {
        return toxinStacks;
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (damage > 0) OnDamageTaken?.Invoke(this, EventArgs.Empty);
        if (health <= 0) Destroy(gameObject);
    }

    public void Cleanse(EnemyScript enemy, StatusEffect.Name name)
    {
        if (statusEffects.ContainsKey(name))
        {
            statusEffects[name].CancelEffect(enemy);
        }
    }

    public EnemyScript NearestEnemy()
    {
        if (IsDestroyed()) return null;
        EnemyScript nearestEnemy = null;
        List<EnemyScript> enemiesInRange = EnemyInRange(5f);
        float nearestDistance = float.MaxValue;
        foreach (EnemyScript e in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = e;
            }
        }
        return nearestEnemy;
    }
    public List<EnemyScript> EnemyInRange(float distanceToSeek)
    {
        if (IsDestroyed()) return null;
        List<EnemyScript> nearestEnemies = new List<EnemyScript>();
        int enemies = Physics.OverlapSphereNonAlloc(transform.position, distanceToSeek, enemiesBuffer, layerMaskEnemy);
        for (int i = 0; i < enemies; i++)
        {
            if (enemiesBuffer[i].TryGetComponent<EnemyScript>(out EnemyScript enemyFinded))
            {
                if (enemyFinded != this && !nearestEnemies.Contains(enemyFinded))
                {
                    nearestEnemies.Add(enemyFinded);
                }
            }
        }

        return nearestEnemies;
    }

    public float MaxEnemyHP() { return enemySO.HealthPoint; }

    public float CurrentEnemyHP() { return health; }

    public Vector3 GetPosition() { return transform.position + new Vector3(0, 0.5f, 0); }
    public bool IsDestroyed() { return this == null; }
}
