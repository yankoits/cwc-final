using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Collider currSurface;

    // TODO: change to private when all is ready
    public float moveSpeed = 15f;
    public float moveRotationSpeed = 500f;

    private float frozenRotationSpeed = 2f;
    private float rotationSpeed;


    private const float gravity = -9.8f;

    private Vector3 currMovement;

    private bool frozen;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currMovement = Vector3.forward * moveSpeed;
        rotationSpeed = moveRotationSpeed;
        frozen = false;

        // save first surface under player's feet
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.collider.CompareTag("Surface"))
                currSurface = hit.collider;
            else
                currSurface = null; // but that's probably an error you need to catch
        }
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

        movement.y = GameManager.Instance.Level.gravity;
        if (!frozen)
            controller.Move(movement * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Lava") && !frozen)
        {
            // TODO: freezing should also bring invulnerability
            frozen = true;
            rotationSpeed = frozenRotationSpeed;
            StartCoroutine(OnLavaHitted());
        }
        else if (hit.collider.CompareTag("Surface"))
        {
            currSurface = hit.collider;
        }
    }

    public IEnumerator OnLavaHitted()
    {
        // change direction of moving to straight vertical or horizontal
        // also, reverse it on chosen axis
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

        // get the closest point of "current" surface and push player to it
        Vector3 pushBack = currSurface.ClosestPoint(transform.position);
        pushBack.y = transform.position.y;
        controller.Move(pushBack - transform.position);

        // give game the time to rotate player/camera
        yield return new WaitForSeconds(2);

        // and unfreeze
        frozen = false;
        rotationSpeed = moveRotationSpeed;
    }

}
