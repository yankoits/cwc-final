using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform target;

    private Vector3 offset;
    void Start()
    {
        transform.LookAt(target);
        offset = target.position - transform.position;
    }

    void Update()
    {
        transform.position = target.position - offset;
    }
}
