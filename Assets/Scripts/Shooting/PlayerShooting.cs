using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private PlayerLook Look;
    public bool canShoot { get; private set; }
    public float reloadTime { get; private set; }

    private void OnEnable()
    {
        Messenger.AddListener(GameEvent.GUN_RELOADED, OnReloaded);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.GUN_RELOADED, OnReloaded);
    }

    private void Start()
    {
        canShoot = true;
        reloadTime = 3f;
        Look = GetComponent<PlayerLook>();
    }

    public void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Get a projectile object from the pool
            GameObject projectile = ObjectPooler.SharedInstance.GetPooledObject();
            if (projectile != null && canShoot)
            {
                projectile.SetActive(true); // activate it
                projectile.transform.position = transform.position; // position it at player
                projectile.transform.rotation = transform.rotation;
                canShoot = false;
            }
        }
    }

    public void Reload(PlayerLook look)
    {
        StartCoroutine(look.Reload(reloadTime));
    }

    private void OnReloaded()
    {
        canShoot = true;
    }

}
