using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerSO : ScriptableObject
{
    public enum NameIs
    {
        PoisonTower,
        PlagueTower,
        SlowTower,
        BurstTower,
        BombTower,
        LaserTower,
        MultiTargerTower,
        MagicMissleTower,
        EntangleTower,
        RefractorTower,
        Arc_MageTower,
        FocusTower,
        MagicNatureTower,
        NovaTower,
        ArrowTower,
        ElderTreeTower,
        BalistaTower,
    };
    public enum Tier
    {
        One,
        Two,
        Three,
    }
    public NameIs Name;
    public Tier TowerTier;
    public GameObject Prefab;
    public float TowerRange;
    public float AttacRate;
    public ProjectileSO ProjectileSO;
}
