using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
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

}
