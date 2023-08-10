using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, ITile
{
    [SerializeField] private EnemySO enemySO;
    [SerializeField] private float spawnRate;
    [SerializeField] private float spawnTimer = 0f;
    [SerializeField] private int maximumEnemyCount = 25;

    public TileType Type { get; set; } = TileType.Spawner;

    private void Start()
    {
        PathFindManager.Instance.SendPosition(this);
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        if (maximumEnemyCount > 0)
        {
            Transform enemy = Instantiate(enemySO.Prefab,spawnPosition, Quaternion.identity);
            maximumEnemyCount--;
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
