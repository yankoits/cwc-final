using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour, IManager
{
    public ManagerStatus status { get; private set; }
    public int maxHealth { get; private set; }
    public int health { get; private set; }

    [SerializeField] GameObject playerPrefab;
    public void Init()
    {
        Debug.Log("Player manager starting...");

        // starting settings
        UpdateData(5, 5);

        status = ManagerStatus.Started;
    }


    public Transform Spawn()
    {
        GameObject player = Instantiate(playerPrefab);
        return player.transform;
    }

    public void UpdateData(int health, int maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;
    }

    public int UpdateHealth(int delta = -1)
    {
        health += delta;

        if (health > maxHealth)
            health = maxHealth;
        else if (health <= 0)
        {
            health = 0;
            Messenger.Broadcast(GameEvent.GAME_OVER);
        }
        Messenger.Broadcast(GameEvent.HEALTH_UPDATED);
        return health;
    }
}
