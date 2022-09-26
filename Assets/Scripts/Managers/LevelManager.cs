using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour, IManager
{
    public ManagerStatus status { get; private set; }
    public float tileSize { get; private set; }
    public int EnemyCount { get; private set; }
    public float gravity { get; private set; }
    public float LavaBound { get; private set; }
    public Bounds SurfaceBounds { get; private set; }
    private int currLevel;
    private int maxLevel;
    public int LevelScore { get; private set; }
    private bool checkIfLevelCompleted;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void Init()
    {
        Debug.Log("Level manager starting...");

        // starting settings
        tileSize = 1.0f;
        gravity = -9.8f;

        // !! change it every time you adding the new levels
        maxLevel = 2;

        checkIfLevelCompleted = false;

        status = ManagerStatus.Started;
    }

    private void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.ENEMY_IS_DEAD, OnEnemyDeath);
        Messenger.AddListener(StartupEvent.MANAGERS_STARTED, OnManagersStarted);
    }

    private void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.ENEMY_IS_DEAD, OnEnemyDeath);
        Messenger.RemoveListener(StartupEvent.MANAGERS_STARTED, OnManagersStarted);
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
        // why this is in the update and not in the OnEnemyDeath function?
        // because it should be called on the NEXT frame after enemy death
        if (checkIfLevelCompleted)
        {
            EnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (EnemyCount == 0)
                Messenger<int>.Broadcast(GameEvent.LEVEL_BEATEN, LevelScore);
            checkIfLevelCompleted = false;
        }
    }

    public void LoadNextLevel()
    {
        currLevel += 1;
            if (currLevel <= maxLevel)
                SceneManager.LoadScene($"Level{currLevel}");
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetLevelData();
        Messenger.Broadcast(GameEvent.LEVEL_START);
    }

    public void GetLevelData()
    {
        EnemyCount = currLevel * 2;
        LevelScore = currLevel * 10;
        LavaBound = GetLavaBoundary();
        SurfaceBounds = GetSurfaceBounds();
    }

    private void OnEnemyDeath(int score)
    {
        checkIfLevelCompleted = true;
    }

    private void OnManagersStarted()
    {
        currLevel = 1;
        // maybe it's good this line is here?
        // can be handy when/if restart from level X would be possible
    }
}
