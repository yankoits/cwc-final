using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// micro-class for projectile
public class MoveForward : MonoBehaviour
{
    private float speed = 40f;

    void FixedUpdate()
    {
        transform.position = transform.position + transform.forward * speed * Time.deltaTime;
    }
}
