using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 offset;
    public void Init(Transform player)
    {
        transform.position = new Vector3(0, 30, -10);
        playerTransform = player;
        transform.LookAt(playerTransform);
        offset = playerTransform.position - transform.position;
    }

    void Update()
    {
        if (playerTransform != null)
            transform.position = playerTransform.position - offset;
    }
}
