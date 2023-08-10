using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class PathFindManager : MonoBehaviour
{
    public static PathFindManager Instance;
    private List<Vector3> spawnersPositions = new List<Vector3>();
    private List<Vector3> pathTilePositions = new List<Vector3>();
    private Dictionary<Vector3, Queue<Vector3>> pathQueue = new Dictionary<Vector3, Queue<Vector3>>();
    private Dictionary<Vector3, bool> visitedTiles = new Dictionary<Vector3, bool>();
    private Dictionary<Vector3, Transform> pathTilesReferences = new Dictionary<Vector3, Transform>();
    private Dictionary<Vector3, Transform> buildingsReferences = new Dictionary<Vector3, Transform>();
    private List<RefractorTower> refractorTowers = new List<RefractorTower>();

    private Vector3 endPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RemoveSpawnersFromTiles();
        foreach (Vector3 spawnerPosition in spawnersPositions)
        {
            visitedTiles.Clear();
            foreach (Vector3 pathTile in pathTilePositions)
            {
                visitedTiles.Add(pathTile, false);
            }
            pathQueue[spawnerPosition] = BuildPath(spawnerPosition, pathTilePositions, new Queue<Vector3>());
        }
    }

    public void SendPosition(ITile tile)
    {
        Vector3 value = RoundVector(tile.GetTransform().position);
        switch (tile.Type)
        {
            case TileType.Spawner:
                spawnersPositions.Add(value);
                pathQueue.Add(value, new Queue<Vector3> { });
                break;
            case TileType.EndPoint:
                endPosition = value;
                break;
            case TileType.Building:
                if (buildingsReferences.ContainsKey(value))
                {
                    buildingsReferences[value] = tile.GetTransform();
                }
                else
                {
                    buildingsReferences.Add(value, tile.GetTransform());
                }
                break;
            case TileType.PathTile:
                pathTilePositions.Add(value);
                pathTilesReferences.Add(value, tile.GetTransform());
                break;
            default: break;
        }
    }

    public void RemovePosition(ITile tile)
    {
        Vector3 value = RoundVector(tile.GetTransform().position);
        switch (tile.Type)
        {
            case TileType.Building:
                buildingsReferences.Remove(value);
                break;
            case TileType.PathTile:
                pathTilePositions.Remove(value);
                break;
            default: break;
        }
    }

    private Queue<Vector3> BuildPath(Vector3 startPosition, List<Vector3> pathTilePositions, Queue<Vector3> path)
    {
        visitedTiles[startPosition] = true;
        path.Enqueue(startPosition);
        if (startPosition == endPosition)
        {
            return path;
        }
        foreach (Vector3 pathTilePosition in pathTilePositions)
        {
            if (Vector3.Distance(pathTilePosition, startPosition) < 1.1f && !visitedTiles[pathTilePosition])
            {
                visitedTiles[pathTilePosition] = true;
                Queue<Vector3> queue = BuildPath(pathTilePosition, pathTilePositions, path);
                if (queue != null) return queue;
            }
        }
        return null;
    }
    private void RemoveSpawnersFromTiles()
    {
        foreach (Vector3 spawner in spawnersPositions)
        {
            pathTilePositions.Remove(spawner);
        }
    }

    public Queue<Vector3> GetRoute(Vector3 position)
    {
        position = new Vector3(Mathf.Round(position.x), 0f, Mathf.Round(position.z));
        Queue<Vector3> returnThis = new Queue<Vector3>(pathQueue[position]);
        return returnThis;
    }

    public Vector3 RoundVector(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), 0f, Mathf.Round(position.z));
    }

    public bool IsTileOccupied(Vector3 position)
    {
        return pathTilePositions.Contains(RoundVector(position)) || buildingsReferences.ContainsKey(RoundVector(position));
    }

    public void RefractorTowerAdd(RefractorTower tower)
    {
        refractorTowers.Add(tower);
    }

    public List<RefractorTower> GetRefractorTowersList()
    {
        return refractorTowers;
    }

    public Dictionary<Vector3, Transform> GetbuildingsReferences()
    {
        return buildingsReferences;
    }
}
