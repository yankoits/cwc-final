using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public bool canShoot { get; private set; }
    public float reloadTime { get; private set; }

    private void Start()
    {
        canShoot = true;
        reloadTime = 0.8f;
    }

    private void Update()
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
                StartCoroutine(Reload());
            }
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        canShoot = true;
    }

}
