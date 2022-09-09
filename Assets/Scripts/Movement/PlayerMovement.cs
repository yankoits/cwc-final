using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    private float moveSpeed;

    private float frozenRotation = 90f;

    private float pushbackForce;
    private float pushbackY = 0.5f;

    private Vector3 currMovement;

    private PlayerState state;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lava"))
        {
            // TODO: freezing should also bring invulnerability?
            state = PlayerState.FROZEN;
            StartCoroutine(OnLavaHitted());
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyState enemyState = other.gameObject.GetComponent<IEnemy>().state;
            if (enemyState != EnemyState.SHOT && enemyState != EnemyState.WASTED)
            {
                Debug.Log(other.GetComponent<IEnemy>().state);
                state = PlayerState.FROZEN;
                GameManager.Instance.Player.UpdateHealth(-1);

                StartCoroutine(OnEnemyCollision(other));
            }
        }
    }

    public IEnumerator OnLavaHitted()
    {
        Vector3 pbDirection = -transform.position.normalized;
        pbDirection.y = pushbackY;

        rb.AddForce(pushbackForce * pbDirection);

        GameManager.Instance.Player.UpdateHealth(-1);

        currMovement = new Vector3(0, transform.position.y, 0) - transform.position;
        
        // give the game time to rotate player
        yield return new WaitForSeconds(2);

        // and unfreeze
        state = PlayerState.MOVING;
    }

    public IEnumerator OnEnemyCollision(Collider other)
    {
        Vector3 pbDirection = other.transform.position - transform.position;
        pbDirection = -pbDirection.normalized;
        pbDirection.y = pushbackY;
        rb.AddForce(pushbackForce * pbDirection);

        yield return new WaitForSeconds(2);
        state = PlayerState.MOVING;
    }

}
