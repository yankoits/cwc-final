using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingEnemy : MonoBehaviour, IEnemy
{
    public float moveRadius = 10.0f;
    public float rotationSpeed = 90.0f;
    public float movingSpeed = 3.0f;

    public float deceleration = 25.0f;

    private float radius;
    private float currSpeed;

    public EnemyState state { get; private set; }

    private Coroutine OnPause;

    private Vector3? moveDestination;
    private float destinationDelta;
    private const float angleDelta = 2;

    [SerializeField] LayerMask Lava;

    private float waitWaySearching = 2;
    private float waitBeingShot = 5;

    void Start()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;
        radius = Mathf.Sqrt(Mathf.Pow(bounds.max.z - bounds.min.z, 2) * 2);

        destinationDelta = GameManager.Instance.Level.tileSize;
        currSpeed = movingSpeed;

        OnPause = null;

        state = EnemyState.WAITING;
    }

    void Update()
    {
        switch (state)
        {
            case EnemyState.WAITING:
                if (moveDestination == null)
                    OnPause = StartCoroutine(PauseAndRunAgain(waitWaySearching));
                break;

            case EnemyState.TURNING:
                Vector3 destination = (Vector3)moveDestination;

                Quaternion targetRotation = Quaternion.LookRotation(destination - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(transform.rotation, targetRotation) < angleDelta)
                    state = EnemyState.MOVING;
                break;

            case EnemyState.MOVING:
                Vector3 movement = currSpeed * transform.forward * Time.deltaTime;
                transform.Translate(movement, Space.World);

                if (Vector3.Distance((Vector3)moveDestination, transform.position) < destinationDelta)
                {
                    currSpeed -= deceleration * Time.deltaTime;
                    if (currSpeed <= 0)
                    {
                        moveDestination = null;
                        state = EnemyState.WAITING;
                    }
                }
                break;

            default:
                break;
        }
    }

    private IEnumerator PauseAndRunAgain(float pauseTime)
    {
        if (OnPause != null)
            StopCoroutine(OnPause);

        moveDestination = GetNextDestination();
        yield return new WaitForSeconds(pauseTime);

        state = EnemyState.TURNING;
        OnPause = null;
    }

    private Vector3 GetNextDestination()
    {
        Vector3 direction;

        Vector3 castOriginPoint = transform.position;
        castOriginPoint.y = 0;

        do
        {
            direction = RandomPointOnCircleEdge(moveRadius);

            Ray sphereCast = new Ray(castOriginPoint, direction);

            // if lava is on the way, find another direction
            if (Physics.SphereCast(sphereCast, radius, direction.magnitude, Lava))
                direction = Vector3.zero;

        } while (direction == Vector3.zero);

        direction = new Vector3(
            direction.x + transform.position.x,
            transform.position.y,
            direction.z + transform.position.z
        );

        currSpeed = movingSpeed;

        return direction;
    }

    private Vector3 RandomPointOnCircleEdge(float maxRadius)
    {
        Vector2 point;
        do
        {
            point = Random.insideUnitCircle.normalized;
            float randomRadius = Random.Range(maxRadius / 2, maxRadius);
            if (point != Vector2.zero)
                return new Vector3(point.x * randomRadius, 0, point.y * randomRadius);
        } while (true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            moveDestination = null;
            state = EnemyState.WAITING;
        }
    }

    public void ReactToShot()
    {
        moveDestination = null;
        state = EnemyState.SHOT;
        OnPause = StartCoroutine(PauseAndRunAgain(waitBeingShot));
    }

}
