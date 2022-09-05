using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public float tileSize { get; private set; }
    public float gravity { get; private set; }

    public void Init()
    {
        Debug.Log("Level manager starting...");

        // starting settings
        tileSize = 1.0f;
        gravity = -9.8f;

        status = ManagerStatus.Started;
    }
}
