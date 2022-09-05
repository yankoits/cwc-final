using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Collider currSurface;

    private float moveSpeed;

    private float frozenRotation = 90f;

    private float pushbackForce;
    private float pushbackY = 0.5f;

    private Vector3 currMovement;

    private PlayerState state;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // save first surface under player's feet
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.collider.CompareTag("Surface"))
                currSurface = hit.collider;
            else
                currSurface = null; // but that's probably an error you need to catch
        }

        pushbackForce = 350f;
        moveSpeed = 30f;
        currMovement = Vector3.forward * moveSpeed;
        state = PlayerState.JUST_SPAWNED;
    }

    void Update()
    {
        Vector3 movement = currMovement;
        Quaternion direction;
        if (state == PlayerState.MOVING)
        {
            float horInput = Input.GetAxis("Horizontal");

            Vector3 right = Vector3.Cross(Vector3.up, currMovement);
            movement = right * horInput * Time.deltaTime + currMovement;

            movement *= moveSpeed;
            // angular speed clamp
            movement = Vector3.ClampMagnitude(movement, moveSpeed);

            currMovement = movement;

            rb.MovePosition(transform.position + movement * Time.deltaTime);

            direction = Quaternion.LookRotation(movement);
            rb.MoveRotation(direction);
        }
        else if (state == PlayerState.FROZEN)
        {
            direction = Quaternion.LookRotation(currMovement * Time.deltaTime);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, direction, frozenRotation * Time.deltaTime));
        }
        else if (state == PlayerState.JUST_SPAWNED)
        {
            if (Input.anyKeyDown)
                state = PlayerState.MOVING;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Lava"))
        {
            // TODO: freezing should also bring invulnerability?
            state = PlayerState.FROZEN;
            StartCoroutine(OnLavaHitted());
        }
        else if (collision.collider.CompareTag("Enemy"))
        {
            state = PlayerState.FROZEN;
            GameManager.Instance.Player.UpdateHealth(-1);

            StartCoroutine(OnEnemyCollision(collision));
        }
        else if (collision.collider.CompareTag("Surface"))
        {
            currSurface = collision.collider;
        }
    }

    public IEnumerator OnLavaHitted()
    {
        Vector3 pbDirection = -transform.position.normalized;
        pbDirection.y = pushbackY;

        rb.AddForce(pushbackForce * pbDirection);

        GameManager.Instance.Player.UpdateHealth(-1);

        currMovement = new Vector3(0, transform.position.y, 0) - transform.position;
        // give game the time to rotate player/camera
        yield return new WaitForSeconds(2);

        // and unfreeze
        state = PlayerState.MOVING;
    }

    public IEnumerator OnEnemyCollision(Collision col)
    {
        Vector3 pbDirection = col.collider.transform.position - transform.position;
        pbDirection = -pbDirection.normalized;
        pbDirection.y = pushbackY;
        rb.AddForce(pushbackForce * pbDirection);

        yield return new WaitForSeconds(2);
        state = PlayerState.MOVING;
    }

}
