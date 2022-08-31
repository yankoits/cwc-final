using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float tileSize { get; private set; }
    public float gravity { get; private set; }

     public void Awake()
    {
        tileSize = 1.0f;
        gravity = -9.8f;
    }
}
