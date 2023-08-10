using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class EnemySO : ScriptableObject
{
    public Transform Prefab;
    public float HealthPoint;
    public float Speed;
}
