using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    [SerializeField] GameObject enemyPrefab;

    private int enemyCount;
    private List<GameObject> enemies;
    private GameObject player;

    public void Init()
    {
        Debug.Log("Spawn manager starting...");

        SpawnEnemies(GameManager.Instance.Level.EnemyCount);

        status = ManagerStatus.Started;
    }

    private void Update()
    {
    }

    public void SpawnEnemies(int count)
    {
 
        if (count < 1)
            return;

        enemyCount = count;
        player = GameObject.Find("Player");
        enemies = new List<GameObject>();

        float enemyRadius;
        for (int i = 0; i < count; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            var enemyObj = enemy.GetComponent<IEnemy>();
            if (enemyObj == null)
                return;

            enemyObj.Init();
            enemyRadius = enemyObj.radius;
            do
            {
                Vector3 spawnPoint = RandomSpawnPoint(enemyRadius);
                spawnPoint.y = 1;

                // safe distance from player
                if ((player.transform.position - spawnPoint).magnitude < 2 * enemyRadius)
                    continue;

                // also safe distance from other enemies (if they exist already)
                if (enemies.Count == 0 || enemies.TrueForAll(e => (e.transform.position - spawnPoint).magnitude > 2 * enemyRadius))
                {
                    enemy.transform.position = spawnPoint;
                    enemies.Add(enemy);
                    break;
                }

            } while (true);
        }
    }

    private Vector3 RandomSpawnPoint(float enemyRadius)
    {
        Bounds surfaceBounds = GameManager.Instance.Level.SurfaceBounds;
        return new Vector3(
            UnityEngine.Random.Range(surfaceBounds.min.x + 2 * enemyRadius, surfaceBounds.max.x - 2 * enemyRadius),
            0f,
            UnityEngine.Random.Range(surfaceBounds.min.z + 2 * enemyRadius, surfaceBounds.max.z - 2 * enemyRadius)
        );
    }

}
