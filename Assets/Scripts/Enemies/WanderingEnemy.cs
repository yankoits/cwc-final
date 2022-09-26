using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WanderingEnemyLook))]
public class WanderingEnemy : MonoBehaviour, IEnemy
{
    // IEnemy inherited properties
    // comment for unity learn course: ENCAPSULATION, but pretty generic ofc
    public EnemyState state { get; private set; }
    public Vector3? moveDestination { get; private set; }
    public float radius { get; private set; }
    public int score { get; } = 1;
    public int power { get; } = 1;


    // she's got the look
    private WanderingEnemyLook Look;

    private bool berzerkerModeOn;

    // max distance for every next destination to move
    protected const float moveRadius = 12.0f;

    // main speeds
    private const float rotationSpeed = 180.0f;
    private const float movingSpeed = 9.0f;
    private float currSpeed;

    // main accelerations/decelerations
    private float deceleration = 75.0f;
    private float accidentDeceleration = 20.0f;

    private float deathBoost = 2.5f;
    private float berzerkerBoost = 2.0f;


    private Coroutine OnPause;

    private float destinationDelta;
    private const float angleDelta = 2;

    private float waitTimeWaySearch = 2.0f;
    private float waitTimeBeingShot = 5.0f;


    public void Init()
    {
        Look = GetComponent<WanderingEnemyLook>();
        Look.Init();
        radius = Look.GetRadius();

        berzerkerModeOn = false;

        currSpeed = movingSpeed;
        destinationDelta = GameManager.Instance.Level.tileSize;

        OnPause = null;
        state = EnemyState.WAITING;
    }

    // comment for unity learn course: ABSTRACTION
    // all the methods to call in various states are pretty abstracted
    void Update()
    {
        switch (state)
        {
            case EnemyState.WAITING:
                if (moveDestination == null)
                    OnPause = StartCoroutine(PauseAndRunAgain(waitTimeWaySearch));
                break;

            case EnemyState.TURNING:
                Turn();
                break;

            case EnemyState.MOVING:
                Move();
                break;

            case EnemyState.TRAFFIC_ACCIDENT:
                PushbackFromAccident();
                break;

            case EnemyState.WASTED:
                FlyOffSurface();
                break;

            default:
                break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lava"))
        {
            ReactToLava();
        }
        else if (other.CompareTag("Player") && (state == EnemyState.SHOT))
        {
            // kick from the collision point
            // not very realistic, maybe think of better ways
            ReactToKick(transform.position - other.transform.position);
        }
        else if (other.CompareTag("Enemy"))
        {
            ReactToFriendlyCollision(other);
        }
        else if (other.CompareTag("Bullet"))
        {
            other.gameObject.SetActive(false);
            ReactToShot();
        }
        else
        {
            // if something else is on the map (shouldn't be though)
            moveDestination = null;
            state = EnemyState.WAITING;
        }
    }

    // slowly turns enemy so that after the last iteration
    // he looks straight to the point he should be moving
    private void Turn()
    {
        if (moveDestination == null)
        {
            state = EnemyState.WAITING;
            return;
        }

        float berzCoeff = berzerkerModeOn ? berzerkerBoost : 1;

        Vector3 destination = (Vector3)moveDestination;

        Quaternion targetRotation = Quaternion.LookRotation(destination - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * berzCoeff * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, targetRotation) < angleDelta)
            state = EnemyState.MOVING;
    }

    // slowly moves enemy towards the destination point
    // which is forward, cause enemy was turned so beforehand
    // decelerates near the final point to a full stop
    private void Move()
    {
        if (moveDestination == null)
        {
            state = EnemyState.WAITING;
            return;
        }

        float berzCoeff = berzerkerModeOn ? berzerkerBoost : 1;

        Vector3 movement = currSpeed * berzCoeff * transform.forward * Time.deltaTime;
        movement.y = 0f;
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
    }

    // slowly moves enemy to the destination point
    // this is pushback, so enemy moving backwards
    // decelerates from the start with every iteration
    private void PushbackFromAccident()
    {
        if (moveDestination == null)
        {
            state = EnemyState.WAITING;
            return;
        }

        Vector3 pushback = currSpeed * (Vector3)moveDestination * Time.deltaTime;
        transform.Translate(pushback, Space.World);

        currSpeed -= accidentDeceleration * Time.deltaTime;
        if (currSpeed < 0)
        {
            moveDestination = null;
            state = EnemyState.WAITING;
        }
    }

    // moving the enemy off the surface at high speed
    private void FlyOffSurface()
    {
        // should be true just after collision with player
        if (moveDestination == null)
            moveDestination = -transform.forward;

        Vector3 roadToDeath = currSpeed * deathBoost * (Vector3)moveDestination * Time.deltaTime;
        transform.Translate(roadToDeath, Space.World);
    }


    private IEnumerator PauseAndRunAgain(float pauseTime)
    {
        if (OnPause != null)
            StopCoroutine(OnPause);

        moveDestination = GetNextDestination();

        yield return new WaitForSeconds(pauseTime);

        Look.Unfreeze(berzerkerModeOn);

        currSpeed = movingSpeed;
        state = EnemyState.TURNING;
        OnPause = null;
    }

    protected virtual Vector3 GetNextDestination()
    {
        Vector3 direction;

        Vector3 castOriginPoint = transform.position;

        // casting on the surface level, cause only lava needs to be found
        castOriginPoint.y = 0;

        // didn't want to leave the endless cycle here, so
        // enemy have 2048 tries to find its way, or he goes straight into lava
        int tries = 0;
        int maxTries = 2048;
        do
        {
            direction = Geometry.RandomPointOnCircleEdge(maxRadius: moveRadius);
            Ray sphereCast = new Ray(castOriginPoint, direction);

            // if no lava on the chosen way, cycle breaks
            if (!Physics.SphereCast(sphereCast, radius, direction.magnitude))
                break;

        } while (++tries < maxTries);

        return new Vector3(
            direction.x + transform.position.x,
            transform.position.y,
            direction.z + transform.position.z
        );
    }

    public void ReactToShot()
    {
        Look.Freeze();
        moveDestination = null;
        state = EnemyState.SHOT;
        
        OnPause = StartCoroutine(PauseAndRunAgain(waitTimeBeingShot));
        berzerkerModeOn = true;
    }

    public void ReactToKick(Vector3 lastDestination)
    {
        Look.Freeze();
        state = EnemyState.WASTED;
        if (OnPause != null)
            StopCoroutine(OnPause);
        currSpeed = movingSpeed;

        // no underground, no flying high
        lastDestination.y = 0;
        moveDestination = lastDestination;
    }

    public void ReactToLava()
    {
        if (OnPause != null)
            StopCoroutine(OnPause);
        Destroy(gameObject);
        Messenger<int>.Broadcast(GameEvent.ENEMY_IS_DEAD, score);
    }

    public void ReactToFriendlyCollision(Collider coll)
    {
        // should be true by design, but check just in case
        IEnemy collidedEnemy = coll.gameObject.GetComponent<IEnemy>();
        if (collidedEnemy == null)
            return;

        // first variation: wasted on the fly and any other on march
        if (state == EnemyState.WASTED)
            collidedEnemy.ReactToKick((Vector3)moveDestination);

        // second variation: both were on march
        else if (state != EnemyState.SHOT && state != EnemyState.WASTED && collidedEnemy.state != EnemyState.WASTED)
        {
            state = EnemyState.TRAFFIC_ACCIDENT;

            Vector3 pbDestination = transform.position - coll.transform.position;
            pbDestination.y = 0;
            moveDestination = pbDestination;

            currSpeed = movingSpeed;
        }
    }

}
