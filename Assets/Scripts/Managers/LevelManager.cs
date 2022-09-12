using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public float tileSize { get; private set; }
    public int EnemyCount { get; private set; }
    public float gravity { get; private set; }
    public float LavaBound { get; private set; }
    public Bounds SurfaceBounds { get; private set; }
    private int currLevel;
    private bool checkIfLevelCompleted;
    public void Init()
    {
        Debug.Log("Level manager starting...");

        // starting settings
        tileSize = 1.0f;
        gravity = -9.8f;

        currLevel = 1;
        GetLevelData();

        checkIfLevelCompleted = false;

        status = ManagerStatus.Started;
    }

    private void OnEnable()
    {
        Messenger.AddListener(GameEvent.ENEMY_IS_DEAD, OnEnemyDeath);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.ENEMY_IS_DEAD, OnEnemyDeath);
    }

    public float GetLavaBoundary()
    {
        GameObject[] lava = GameObject.FindGameObjectsWithTag("Lava");
        float max = 0;
        for (int i = 0; i < lava.Length; i++)
        {
            Vector3 lavaCenter = lava[i].transform.position;
            Vector3 lavaScale = lava[i].transform.localScale;
            float lavaMax = Mathf.Max(
                Mathf.Abs(lavaCenter.x + lavaScale.x / 2),
                Mathf.Abs(lavaCenter.z + lavaScale.z / 2),
                Mathf.Abs(lavaCenter.x - lavaScale.x / 2),
                Mathf.Abs(lavaCenter.z - lavaScale.z / 2)
            );
            if (lavaMax > max)
                max = lavaMax;
        }
        return max;
    }

    public Bounds GetSurfaceBounds()
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        GameObject[] surface = GameObject.FindGameObjectsWithTag("Surface");
        
        for (int i = 0; i < surface.Length; i++)
        {
            Vector3 surfaceCenter = surface[i].transform.position;
            Vector3 surfaceScale = surface[i].transform.localScale;

            float minX = surfaceCenter.x - surfaceScale.x / 2;
            float maxX = surfaceCenter.x + surfaceScale.x / 2;
            float minZ = surfaceCenter.z - surfaceScale.z / 2;
            float maxZ = surfaceCenter.z + surfaceScale.z / 2;

            Vector3 min = new Vector3(Mathf.Min(bounds.min.x, minX), 0, Mathf.Min(bounds.min.z, minZ));
            Vector3 max = new Vector3(Mathf.Max(bounds.max.x, maxX), 0, Mathf.Max(bounds.max.z, maxZ));

            bounds.SetMinMax(min, max);
        }
        Debug.Log(bounds);
        return bounds;
    }

    private void Update()
    {
        if (checkIfLevelCompleted) { 
            EnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (EnemyCount == 0)
            {
                Messenger.Broadcast(GameEvent.LEVEL_BEATEN);
            }
            else
            {
                Debug.Log(EnemyCount);
            }
            checkIfLevelCompleted = false;
        }
    }

    public void LoadNextLevel()
    {
        currLevel += 1;
        if (currLevel <= 2)
            SceneManager.LoadScene($"Level{currLevel}");
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        GetLevelData();
    }

    public void GetLevelData() { 
        EnemyCount = currLevel /** 3*/;
        LavaBound = GetLavaBoundary();
        SurfaceBounds = GetSurfaceBounds();
    }

    public void OnEnemyDeath()
    {
        checkIfLevelCompleted = true;
    }
}
