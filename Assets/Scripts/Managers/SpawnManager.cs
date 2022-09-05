using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public float bound;

    public void Init()
    {
        Debug.Log("Spawn manager starting...");

        bound = 50;

        status = ManagerStatus.Started;
    }

    private void Update()
    {
    }
}
