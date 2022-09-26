using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{

    void Update()
    {
        if (Mathf.Abs(transform.position.x) > GameManager.Instance.Level.LavaBound ||
            Mathf.Abs(transform.position.z) > GameManager.Instance.Level.LavaBound)
        {
            // Just deactivate it
            gameObject.SetActive(false);

        }
    }
}
