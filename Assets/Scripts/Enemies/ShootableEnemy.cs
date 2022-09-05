using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IEnemy))]
public class ShootableEnemy : MonoBehaviour
{
    private IEnemy body;

    private void Start()
    {
        body = GetComponentInChildren<IEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            other.gameObject.SetActive(false);
            body.ReactToShot();
        }
    }

}
