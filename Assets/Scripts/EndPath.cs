using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPath : MonoBehaviour, ITile
{
    public TileType Type { get; set; } = TileType.EndPoint;

    private void Start()
    {
        PathFindManager.Instance.SendPosition(this);
    }

    public Transform GetTransform() { return transform; }
}
