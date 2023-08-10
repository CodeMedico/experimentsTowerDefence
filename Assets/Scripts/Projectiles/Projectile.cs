using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] public ProjectileSO projectileSO;

    public delegate void OnCollisionBehaviour();
    public delegate void ApplyAllStatusEffets(EnemyScript enemy);
    public ApplyAllStatusEffets StatusEffect;
    public OnCollisionBehaviour onCollisionBehaviour;
    public ITarget target;


    protected float lerpT;
    protected Vector3 pointTwo;
    protected Vector3 pointThree;
    protected Vector3 mainMovementVector;
    protected float distanceToEnemy;
    protected float mod = 1f;

    protected virtual void Start()
    {
        //onCollisionBehaviour += () => Destroy(gameObject);

        if (projectileSO.statusEffects != null)
        {
            foreach (StatusEffect statusEffect in projectileSO.statusEffects)
            {
                StatusEffect += new StatusEffect(statusEffect).ApplyEffect;
            }
            FreshForLerp();
            if (IsTargetValid())
            {
                mainMovementVector = target.GetPosition() - transform.position;
            }
            Mathf.Clamp(transform.position.y, 0f, Mathf.Infinity);
        }
    }

    protected virtual void Update()
    {
        MoveTowardsTarget(target);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<EnemyScript>(out EnemyScript enemy))
        {
            if (enemy as ITarget == this.target)
            {
                StatusEffect?.Invoke(enemy);
                StatusEffect = null;
                if (projectileSO.statusEffects != null)
                {
                    foreach (StatusEffect effect in projectileSO.statusEffects)
                    {
                        StatusEffect += new StatusEffect(effect).ApplyEffect;
                    }

                }
                onCollisionBehaviour?.Invoke();
                enemy.Damage(projectileSO.Damage);
                FreshForLerp();
            }
        }
    }

    protected virtual void MoveTowardsTarget(ITarget enemy)
    {
        if (!IsTargetValid())
        {
            Destroy(gameObject);
        }
        else
        {
            switch (projectileSO.Trajectory)
            {
                case TrajectoryType.BezierCurve:
                    transform.position = Trajectory.BezierCurve(transform.position, pointTwo, pointThree, enemy.GetPosition(),
                projectileSO.Speed, distanceToEnemy, ref lerpT);
                    break;
                case TrajectoryType.Simple:
                    transform.position = Trajectory.SimpleTrajectory(transform.position, enemy.GetPosition(), projectileSO.Speed);
                    break;
                case TrajectoryType.ArcTrajectory:
                    transform.position = Trajectory.ArcTrajectory(transform.position, enemy.GetPosition(), mainMovementVector, projectileSO.Speed);
                    break;
                /*case TrajectoryType.AcrTrajectoryMovingTarget:
                    transform.position = Trajectory.ArcTrajectoryMovingTarrget(transform.position,enemy.GetPosition(),projectileSpawnPoint,projectileSO.Speed);
                    break;*/
                default: break;
            }
        }
    }

    public virtual void SetTarget(ITarget target)
    {
        this.target = target;
    }

    private void FreshForLerp()
    {
        lerpT = 0;
        if (IsTargetValid())
        {
            distanceToEnemy = Vector3.Distance(transform.position, target.GetPosition());
            Trajectory.BuildLerpPoints(transform.position, target.GetPosition(), out pointTwo, out pointThree, mod);
        }
    }

    public ProjectileSO GetProjectileSO()
    {
        return projectileSO;
    }

    protected virtual bool IsTargetValid()
    {
        return target is not null && !target.IsDestroyed();
    }
}

