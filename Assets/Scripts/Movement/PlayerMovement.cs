using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    //[SerializeField] 

    public float rotationSpeed = 40f;
    public float moveSpeed = 15f;

    private float gravity = -9.8f;

    private Vector3 currMovement;

    private bool frozen;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currMovement = Vector3.forward * moveSpeed;
        frozen = false;
    }

    void Update()
    {
        Vector3 movement = currMovement;
        if (!frozen)
        {
            float horInput = Input.GetAxis("Horizontal");

            Vector3 right = Vector3.Cross(Vector3.up, currMovement);
            movement = right * horInput * Time.deltaTime + currMovement;

            movement *= moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed);

            currMovement = movement;
        }

        Quaternion direction = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotationSpeed * Time.deltaTime);

        movement.y = gravity;
        if (!frozen)
            controller.Move(movement * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Lava") && !frozen)
        {
            StartCoroutine(ReverseMovement());
        }
    }

    public IEnumerator ReverseMovement()
    {
        frozen = true;

        if (Mathf.Abs(transform.position.x) > (Mathf.Abs(transform.position.z)))
        {
            currMovement.x *= -1;
            currMovement.z *= 0;
        }
        else
        {
            currMovement.x *= 0;
            currMovement.z *= -1;

        }

        rotationSpeed /= moveSpeed;


        Vector3 pushBack = GameObject.FindGameObjectWithTag("Surface").GetComponent<Collider>().ClosestPoint(transform.position);
        pushBack.y = transform.position.y;

        controller.Move(pushBack - transform.position);

        yield return new WaitForSeconds(2);

        frozen = false;
        rotationSpeed *= moveSpeed;
    }

}
