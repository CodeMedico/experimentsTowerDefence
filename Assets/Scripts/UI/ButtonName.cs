using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonName : MonoBehaviour
{
    [SerializeField] private TowerSO.NameIs Name;

    public TowerSO.NameIs GetName() { return Name; }
}
