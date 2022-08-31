using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LevelManager Level { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Level = GetComponentInChildren<LevelManager>();
    }

}
