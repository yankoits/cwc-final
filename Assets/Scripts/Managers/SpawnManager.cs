using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour, IManager
{
    public ManagerStatus status { get; private set; }

    [SerializeField] GameObject enemyPrefab;

    private List<GameObject> enemies;

    public void Init()
    {
        Debug.Log("Spawn manager starting...");

        status = ManagerStatus.Started;
    }

    public void SpawnEnemies(Vector3 playerPosition, int count)
    {
        if (count < 1)
            return;
                
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
                Vector3 spawnPoint = Geometry.RandomSpawnPoint(GameManager.Instance.Level.SurfaceBounds, enemyRadius);
                spawnPoint.y = 1;

                // safe distance from player
                if ((playerPosition - spawnPoint).magnitude < 2 * enemyRadius)
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

}
