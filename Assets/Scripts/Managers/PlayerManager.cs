using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public int maxHealth { get; private set; }
    public int health { get; private set; }
    public void Init()
    {
        Debug.Log("Player manager starting...");

        // starting settings
        UpdateData(3, 3);

        status = ManagerStatus.Started;
    }

    public void UpdateData(int health, int maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;
    }

    public void UpdateHealth(int delta = -1)
    {
        health += delta;

        if (health > maxHealth)
            health = maxHealth;
        else if (health < 0)
        {
            health = 0;
            // add DEATH here 
        }

        Debug.Log($"Health: {health}/{maxHealth}");
    }
}
