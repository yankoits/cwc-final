using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    private float speed = .1f;

    void Update()
    {
        transform.position = transform.position + transform.forward * speed;
    }
}
