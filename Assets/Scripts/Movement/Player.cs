using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerLook))]
[RequireComponent(typeof(PlayerShooting))]
public class Player : MonoBehaviour
{
    private Rigidbody rb;

    private float moveSpeed;
    private float rotationSpeed;

    private float frozenRotation = 180f;

    private float pushbackForce;
    private float pushbackY = 0; // zero for now to exclude billiard death

    private PlayerLook Look;
    private PlayerShooting Gun;

    private Vector3 currMovement;

    private PlayerState state;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Look = GetComponent<PlayerLook>();
        Look.Init();

        Gun = GetComponent<PlayerShooting>();
        Gun.FastReload();

        pushbackForce = 350f;
        moveSpeed = 12f;
        rotationSpeed = 2f;

        currMovement = Vector3.forward;

        state = PlayerState.JUST_SPAWNED;
    }

    void FixedUpdate()
    {
        if (state == PlayerState.MOVING)
        {
            Move();
        }
        else if (state == PlayerState.FROZEN)
        {
            Freeze();
        }
    }
    private void Update()
    {
        if (state == PlayerState.JUST_SPAWNED)
        {
            if (Input.anyKeyDown)
                state = PlayerState.MOVING;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Gun.Shoot();
            Gun.Reload(Look);
        }
    }

    private void Move()
    {
        float horInput = Input.GetAxis("Horizontal");
        Vector3 right = Vector3.Cross(Vector3.up, currMovement);

        Vector3 movement = right * horInput * rotationSpeed * Time.deltaTime + currMovement;
        movement *= moveSpeed;
        // angular speed clamp
        movement = Vector3.ClampMagnitude(movement, moveSpeed);

        currMovement = movement;

        rb.MovePosition(transform.position + movement * Time.deltaTime);
        rb.MoveRotation(Quaternion.LookRotation(movement));
    }

    private void Freeze()
    {
        if (!Look.IsBlinking)
            StartCoroutine(Look.Blink());
        Quaternion direction = Quaternion.LookRotation(currMovement * Time.deltaTime);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, direction, frozenRotation * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lava"))
        {
            LoseHealth(1);
            if (GameManager.Instance.Player.health > 0)
                StartCoroutine(OnLavaHitted());
        }
        else if (other.CompareTag("Enemy"))
        {
            IEnemy enemy = other.gameObject.GetComponent<IEnemy>();
            if (enemy == null) // shouldn't be true by design, but just in case
                return;

            // ignoring shot / wasted enemies, because:
            // 1) they don't have any influence on player
            // 2) they should understand everything themselves
            if (enemy.state != EnemyState.SHOT && enemy.state != EnemyState.WASTED)
            {
                LoseHealth(enemy.power);
                if (GameManager.Instance.Player.health > 0)
                    StartCoroutine(OnEnemyCollision(other));
            }
        }
    }

    private IEnumerator OnLavaHitted()
    {
        Vector3 pbDirection = -transform.position.normalized;
        pbDirection.y = pushbackY;

        rb.AddForce(pushbackForce * pbDirection);

        currMovement = new Vector3(0, transform.position.y, 0) - transform.position;

        // give the game time to rotate player
        yield return new WaitForSeconds(1);

        // and unfreeze
        state = PlayerState.MOVING;

    }

    private IEnumerator OnEnemyCollision(Collider other)
    {
        Vector3 pbDirection = other.transform.position - transform.position;
        pbDirection = -pbDirection.normalized;
        pbDirection.y = pushbackY;
        rb.AddForce(pushbackForce * pbDirection);

        yield return new WaitForSeconds(1);
        state = PlayerState.MOVING;
    }

    public void LoseHealth(int harmDone)
    {
        // invulnerability if player just took hit
        if (state == PlayerState.FROZEN)
            return;

        int health = GameManager.Instance.Player.UpdateHealth(-harmDone);

        state = (health <= 0) ? PlayerState.PASSIVE : PlayerState.FROZEN;
    }

}
