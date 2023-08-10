using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITile
{
    TileType Type { get; set; }
    Transform GetTransform();
}

public enum TileType
{
    Spawner,
    EndPoint,
    Building,
    PathTile
}
