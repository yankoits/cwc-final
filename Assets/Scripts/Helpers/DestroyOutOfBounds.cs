using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{

    private void Start()
    {
        
    }
    void Update()
    {
        if (Mathf.Abs(transform.position.x) > GameManager.Instance.Spawn.bound ||
            Mathf.Abs(transform.position.z) > GameManager.Instance.Spawn.bound)
        {
            // Just deactivate it
            gameObject.SetActive(false);

        }
    }
}
