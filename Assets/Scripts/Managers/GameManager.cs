using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(SpawnManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private List<IGameManager> startSequence;
    public LevelManager Level { get; private set; }
    public PlayerManager Player { get; private set; }
    public SpawnManager Spawn { get; private set; }

    private bool paused;

    private void OnEnable()
    {
        Messenger.AddListener(GameEvent.GAME_OVER, OnGameOver);
        Messenger.AddListener(GameEvent.LEVEL_BEATEN, OnLevelBeaten);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.GAME_OVER, OnGameOver);
        Messenger.RemoveListener(GameEvent.LEVEL_BEATEN, OnLevelBeaten);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Instance.paused = false;

        Level = GetComponentInChildren<LevelManager>();
        Player = GetComponentInChildren<PlayerManager>();
        Spawn = GetComponentInChildren<SpawnManager>();

        startSequence = new List<IGameManager>();
        startSequence.Add(Level);
        startSequence.Add(Player);
        startSequence.Add(Spawn);

        StartCoroutine(InitializeManagers());
    }

    private IEnumerator InitializeManagers()
    {
        foreach (IGameManager manager in startSequence)
        {
            manager.Init();
        }

        yield return null;

        int numModules = startSequence.Count;
        int numReady = 0;

        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                    numReady++;
            }

            if (numReady > lastReady)
            {
                Debug.Log($"Progress: {numReady}/{numModules}");
                // for the better future!
                // Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules);
            }
            yield return null;
        }

        Debug.Log("All managers started up");
        // for the better future!
        // Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
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
    private void OnLevelBeaten()
    {
        PauseGame();
        StartCoroutine(WaitAndLoadNextLevel());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Instance.paused)
            {
                ResumeGame();
                Instance.paused = false;
                Messenger.Broadcast(GameEvent.UNPAUSE);
            }
            else
            {
                PauseGame();
                Instance.paused = true;
                Messenger.Broadcast(GameEvent.PAUSE);
            }
        }
    }

    private IEnumerator WaitAndLoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(3);

        int maxHealth = Instance.Player.maxHealth;
        Instance.Player.UpdateData(maxHealth, maxHealth);

        Instance.Level.LoadNextLevel();
        ResumeGame();
        yield return new WaitForNextFrameUnit();
        Instance.Spawn.SpawnEnemies(Instance.Level.EnemyCount);
    }

    private IEnumerator WaitAndLoadMainMenu()
    {
        yield return new WaitForSecondsRealtime(3);
        Destroy(gameObject);
        SceneManager.LoadScene("Menu");
        ResumeGame();
    }

}
