using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ProjectileSO : ScriptableObject
{
    public GameObject Prefab;
    public string ProjectileName;
    public float Damage;
    public float Speed;
    public TrajectoryType Trajectory;
    public List<StatusEffectSO> StatusEffectSOs;



    [SerializeField] private List<StatusEffect> _statusEffects;

    public List<StatusEffect> statusEffects
    {
        get
        {
            if (StatusEffectSOs != null && _statusEffects == null)
            {
                _statusEffects = new List<StatusEffect>();
                foreach (StatusEffectSO status in StatusEffectSOs)
                {
                    _statusEffects.Add(new StatusEffect(status.NameOfEffect,
                        status.StatusEffectPower, status.StatusEffectDuration, status.StatusEffectTickRate, status.StatusEffectChain));
                }
            }
            return _statusEffects;
        }
    }

    private void OnValidate()
    {
        if (StatusEffectSOs != null && _statusEffects !=null)
        {
            foreach (StatusEffect status in _statusEffects)
            {
                if (status != null)
                {
                    foreach (StatusEffectSO statusSO in StatusEffectSOs)
                    {
                        status.name = statusSO.NameOfEffect;
                        status.effectPower = statusSO.StatusEffectPower;
                        status.duration = statusSO.StatusEffectDuration;
                        status.ticRateInMs = statusSO.StatusEffectTickRate;
                        status.chainPoison = statusSO.StatusEffectChain;
                    }
                }
            }

        }
    }
}


