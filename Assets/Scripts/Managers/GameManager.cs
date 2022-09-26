using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(SpawnManager))]
[RequireComponent(typeof(ScoreManager))]
[RequireComponent(typeof(ObjectPooler))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;
    private List<IManager> startSequence;
    public LevelManager Level { get; private set; }
    public PlayerManager Player { get; private set; }
    public SpawnManager Spawn { get; private set; }
    public ScoreManager Score { get; private set; }
    public ObjectPooler Ammo { get; private set; }

    private bool paused;

    private void OnEnable()
    {
        Messenger.AddListener(GameEvent.GAME_OVER, OnGameOver);
        Messenger.AddListener(GameEvent.LEVEL_START, OnLevelStart);
        Messenger<int>.AddListener(GameEvent.LEVEL_BEATEN, OnLevelBeaten);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.GAME_OVER, OnGameOver);
        Messenger.RemoveListener(GameEvent.LEVEL_START, OnLevelStart);
        Messenger<int>.RemoveListener(GameEvent.LEVEL_BEATEN, OnLevelBeaten);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            CleanData();
            return;
        }
        
        Instance = this;
        paused = false;

        DontDestroyOnLoad(gameObject);

        Ammo = GetComponentInChildren<ObjectPooler>();
        Ammo.CreatePool();

        Level = GetComponentInChildren<LevelManager>();
        Player = GetComponentInChildren<PlayerManager>();
        Spawn = GetComponentInChildren<SpawnManager>();
        Score = GetComponentInChildren<ScoreManager>();

        startSequence = new List<IManager>();
        startSequence.Add(Level);
        startSequence.Add(Player);
        startSequence.Add(Spawn);
        startSequence.Add(Score);

        StartCoroutine(InitializeManagers());
    }

    private IEnumerator InitializeManagers()
    {
        foreach (IManager manager in startSequence)
            manager.Init();

        yield return null;

        int numModules = startSequence.Count;
        int numReady = 0;

        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IManager manager in startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                    numReady++;
            }

            if (numReady > lastReady)
            {
                Debug.Log($"Progress: {numReady}/{numModules}");
                // probably no need
                // Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules, MessengerMode.DONT_REQUIRE_LISTENER);
            }
            yield return null;
        }

        Debug.Log("All managers started up");

        Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }
    private void ResumeGame()
    {
        Time.timeScale = 1;
    }

    private void OnGameOver()
    {
        PauseGame();
        StartCoroutine(WaitAndLoadMainMenu());
    }
    private void OnLevelBeaten(int score)
    {
        PauseGame();
        StartCoroutine(WaitAndLoadNextLevel());
    }

    private void OnLevelStart()
    {
        Transform playerTransform = Instance.Player.Spawn();
        GetComponentInChildren<CameraMovement>().Init(playerTransform);
        Instance.Spawn.SpawnEnemies(playerTransform.position, Instance.Level.EnemyCount);
    }

    private void CleanData()
    {
        Instance.Score.Reset();
        Instance.Level.Reset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                ResumeGame();
                paused = false;
                Messenger.Broadcast(GameEvent.UNPAUSE);
            }
            else
            {
                PauseGame();
                paused = true;
                Messenger.Broadcast(GameEvent.PAUSE);
            }
        }
    }

    private IEnumerator WaitAndLoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(3);

        Instance.Level.LoadNextLevel();
        ResumeGame();
        yield return new WaitForNextFrameUnit();
    }

    private IEnumerator WaitAndLoadMainMenu()
    {
        yield return new WaitForSecondsRealtime(3);
        SceneManager.LoadScene("Menu");
        ResumeGame();
    }

}
