using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Nova : Projectile
{
    private float novaRadius;
    private LayerMask layerMaskEnemy;
    private bool damaged = false;
    private Material novaMaterial;
    private float timer = 0f;
    private int novaExistTime = 200;
    private bool novaSwitch = false;

    protected override void Start()
    {
        base.Start();
        novaRadius = transform.localScale.x / 2f;
        novaMaterial = GetComponent<Renderer>().material;
    }

    protected override void Update()
    {
        base.Update();
        novaMaterial.SetFloat("_timer", timer * 75f);
        if (novaSwitch)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }
        if (timer < -0.07f)
        {
            novaSwitch = true;
        }
    }
    protected override async void MoveTowardsTarget(ITarget enemy)
    {
        Collider[] enemiesBuffer = new Collider[10];
        int enemies = Physics.OverlapSphereNonAlloc(transform.position, novaRadius, enemiesBuffer, layerMaskEnemy);
        if (!damaged)
        {
            for (int i = 0; i < enemies; i++)
            {
                EnemyScript enemyInArea = enemiesBuffer[i].GetComponent<EnemyScript>();

                enemyInArea.Damage(GetProjectileSO().Damage);
                StatusEffect?.Invoke(enemyInArea);
                StatusEffect = null;
                if (projectileSO.statusEffects != null)
                {
                    foreach (StatusEffect effect in projectileSO.statusEffects)
                    {
                        StatusEffect += new StatusEffect(effect).ApplyEffect;
                    }

                }
            }
            damaged = true;
        }
        await Task.Delay(novaExistTime);
        timer = 0;
        gameObject.SetActive(false);
    }

    public void SetLayerMask(LayerMask layerMaskEnemy)
    {
        this.layerMaskEnemy = layerMaskEnemy;
    }
    public void IsDamaged(bool isDamaged)
    {
        damaged = isDamaged;
    }
    public void NovaSwitch(bool novaSwitch)
    {
        this.novaSwitch = novaSwitch;
    }
}
