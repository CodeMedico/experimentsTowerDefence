using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StatusEffectSO : ScriptableObject
{
    public StatusEffect.Name NameOfEffect;
    public float StatusEffectPower;
    public float StatusEffectDuration;
    public int StatusEffectTickRate;
    public bool StatusEffectChain;
}
