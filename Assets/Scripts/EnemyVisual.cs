using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    private const string BRIGHTNESS = "_Brightness";
    private const string COLOR = "_Color";

    [SerializeField] private Transform slowAnimation;
    [SerializeField] private EnemyScript enemy;

    private Material material;
    private Color initialColor;
    private float brightness;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        initialColor = material.GetColor(COLOR);
        brightness = material.GetFloat(BRIGHTNESS);
        enemy.OnDamageTaken += Enemy_OnDamageTaken;
        enemy.OnStatusEffectAdd += Enemy_OnStatusEffectAdd;
        enemy.OnStaatusEffectRemove += Enemy_OnStaatusEffectRemove;

    }

    private void Enemy_OnStaatusEffectRemove(object sender, EnemyScript.OnStatusEffect e)
    {
        if (e.statusEffect.name == StatusEffect.Name.Posion)
        {
            PoisonVisualToggle(false);
        }
        else if (e.statusEffect.name == StatusEffect.Name.Slow)
        {
            SlowVisualToggle(false);
        }
    }

    private void Enemy_OnStatusEffectAdd(object sender, EnemyScript.OnStatusEffect e)
    {
        if (e.statusEffect.name == StatusEffect.Name.Posion)
        {
            PoisonVisualToggle(true);
        }
        else if (e.statusEffect.name == StatusEffect.Name.Slow)
        {
            SlowVisualToggle(true);
        }
    }

    private async void Enemy_OnDamageTaken(object sender, System.EventArgs e)
    {
        material.SetFloat(BRIGHTNESS, .5f);
        await Task.Delay(80);
        if (sender == null) return;
        material.SetFloat(BRIGHTNESS, brightness);
    }

    private void PoisonVisualToggle(bool On)
    {
        if (On)
        {
            material?.SetColor(COLOR, Color.green);
        }
        else
        {
            material?.SetColor(COLOR, initialColor);
        }
    }

    private void SlowVisualToggle(bool On)
    {
        if (slowAnimation != null)
        {
            slowAnimation?.gameObject.SetActive(On);
        }
    }
}
