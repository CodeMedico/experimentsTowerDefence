using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class StatusEffect
{
    public enum Name
    {
        Slow,
        Posion,
        Entangle,
        Toxin,
        Haste,
        Might
    };
    public enum TypeOfEffect
    {
        Buff,
        Debuff
    }

    public Name name;
    public TypeOfEffect typeOfEffect;
    public float duration;
    public float effectPower;
    public int ticRateInMs;
    public bool chainPoison;

    private CancellationTokenSource cts;

    public StatusEffect(Name name, float effectPower = 0, float duration = 0, int ticRateInMs = 0, bool chainPosion = false)
    {
        this.name = name;
        if (this.name == Name.Slow || this.name == Name.Posion || this.name == Name.Entangle)
            typeOfEffect = TypeOfEffect.Debuff;
        else typeOfEffect = TypeOfEffect.Buff;
        this.effectPower = effectPower;
        this.duration = duration;
        this.chainPoison = chainPosion;
        this.ticRateInMs = ticRateInMs;

    }

    public StatusEffect(StatusEffect statusEffect)
    {
        this.name = statusEffect.name;
        if (this.name == Name.Slow || this.name == Name.Posion || this.name == Name.Entangle)
            typeOfEffect = TypeOfEffect.Debuff;
        else typeOfEffect = TypeOfEffect.Buff;
        this.effectPower = statusEffect.effectPower;
        this.duration = statusEffect.duration;

        this.chainPoison = statusEffect.chainPoison;
        this.ticRateInMs = statusEffect.ticRateInMs;

    }

    public void ApplyEffect(EnemyScript enemy)
    {
        if (name == Name.Slow) SlowEffect(enemy);
        else if (name == Name.Posion) PoisonEffect(enemy, duration);
        else if (name == Name.Entangle) EntangleEffect(enemy);
        else if (name == Name.Toxin) ToxinEffect(enemy);
    }

    public void ApplyEffect(Tower tower)
    {

    }


    public async void SlowEffect(EnemyScript enemy)
    {
        ResetEffectDuration(enemy);
        await Task.Delay(1);
        if (!enemy.GetStatusEffects().ContainsKey(name))
        {
            cts = new CancellationTokenSource();
            enemy.AddStatusEffect(this);
            float enemySpeed = enemy.GetEnemySpeed();
            enemy.SetEnemySpeed(enemySpeed * (1f - effectPower));
            try
            {
                float duration = this.duration;
                while (duration > 0)
                {
                    duration -= Time.deltaTime+0.001f;
                    await Task.Delay(1, cts.Token);
                }
                //await Task.Delay((int)duration * 1000, cts.Token);
            }
            catch (TaskCanceledException)
            {
                enemy.SetEnemySpeed(enemySpeed);
                enemy.RemoveStatusEffect(this);
            }
            if (enemy == null) return;
            enemy.SetEnemySpeed(enemySpeed);
            enemy.RemoveStatusEffect(this);
        }
    }

    public async void PoisonEffect(EnemyScript enemy, float duration = 0)
    {
        ResetEffectDuration(enemy);
        await Task.Delay(1);
        if (!enemy.GetStatusEffects().ContainsKey(name))
        {

            cts = new CancellationTokenSource();
            enemy.AddStatusEffect(this);
            EnemyScript nearestEnemy = null;
            try
            {
                while (duration > 0)
                {

                    if (Time.timeScale == 0)
                    {
                        await Task.Delay((int)Time.unscaledDeltaTime * 1000 + 1);
                    }
                    else
                    {
                        if (enemy != null)
                        {
                            if (chainPoison) nearestEnemy = enemy.NearestEnemy();
                            enemy.Damage(effectPower);
                        }
                        duration -= (float)ticRateInMs / 1000;
                        await Task.Delay(ticRateInMs, cts.Token);
                        if (enemy == null)
                        {
                            if (chainPoison == true && nearestEnemy != null)
                            {
                                PoisonEffect(nearestEnemy, duration);
                            }
                            return;
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                enemy.RemoveStatusEffect(this);
            }
        }
        enemy.RemoveStatusEffect(this);
    }

    public async void EntangleEffect(EnemyScript enemy)
    {
        ResetEffectDuration(enemy);
        await Task.Delay(1);
        if (!enemy.GetStatusEffects().ContainsKey(name))
        {
            cts = new CancellationTokenSource();
            enemy.AddStatusEffect(this);
            float enemySpeed = enemy.GetEnemySpeed();
            enemy.SetEnemySpeed(0f);
            try
            {
                while (duration > 0)
                {
                    if (Time.timeScale == 0)
                    {
                        await Task.Delay((int)Time.unscaledDeltaTime * 1000+1);
                    }
                    else
                    {
                        if (enemy == null)
                        {
                            return;
                        }
                        if (enemy != null)
                        {
                            enemy.Damage(effectPower);
                        }
                        duration -= (float)ticRateInMs / 1000;
                        await Task.Delay(ticRateInMs, cts.Token);
                    }

                }
            }
            catch (TaskCanceledException)
            {
                enemy.SetEnemySpeed(enemySpeed);
                enemy.RemoveStatusEffect(this);
            }
            enemy.SetEnemySpeed(enemySpeed);
            enemy.RemoveStatusEffect(this);
        }
    }

    private async void ToxinEffect(EnemyScript enemy)
    {
        float chainDistance = 3f;
        if (enemy.GetToxinStacks().Any(statusEffect => statusEffect is null))
        {
            cts = new CancellationTokenSource();
            enemy.AddStatusEffect(this);
            List<EnemyScript> nearestEnemies = new List<EnemyScript>();
            try
            {
                if (Time.timeScale == 0)
                {
                    await Task.Delay((int)Time.unscaledDeltaTime * 1000+1);
                }
                while (duration > 0)
                {

                    if (enemy != null)
                    {
                        enemy.Damage(effectPower);
                        nearestEnemies = enemy.EnemyInRange(chainDistance);
                        Debug.Log(nearestEnemies.Count);
                    }
                    duration -= (float)ticRateInMs / 1000;
                    await Task.Delay(ticRateInMs, cts.Token);
                    if (enemy == null)
                    {
                        foreach (EnemyScript nearestEnemy in nearestEnemies)
                        {
                            ToxinEffect(nearestEnemy);
                        }
                        return;
                    }

                }
            }
            catch (TaskCanceledException)
            {
                enemy.RemoveStatusEffect(this);
            }
        }
        enemy.RemoveStatusEffect(this);
    }



    public void CancelEffect(EnemyScript enemy)
    {
        cts?.Cancel();
    }

    private void ResetEffectDuration(EnemyScript enemy)
    {
        if (enemy.GetStatusEffects().ContainsKey(name))
        {
            enemy.Cleanse(enemy, name);
        }
    }

}

