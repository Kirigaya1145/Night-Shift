using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum State { Wander, Chase, Flee}

    public GameObject target;
    public AStarPathFinder pathFinder;
    public Animator animator;

    public string speedParam = "Speed";

    //Decision Making - Chase
    public float chaseDetecRad = 8f;
    public float loseChaseRad = 11f;

    //Decision Making - Flee
    public float fleeSightRad = 10f;
    public LayerMask sightMask;
    public float eyeHeight = 1.5f;

    //Seek Settings
    public float satisfactionRad = 1.5f;
    public float slowRad = 5f;
    public float chaseMaxSpd = 7f;

    //Flee Settings
    public float fleeMaxSpd = 8f;

    //Wander Settings
    public float wanderMaxSpd = 3f;
    public float wanderCooldown = 1f;
    public float wanderRate = 0.4f;

    //Pathfinding
    public float pathCalculateInterval = 0.4f;

    public State currentState {  get; private set; } = State.Wander;

    Rigidbody rb;
    PlayerFlashlight playerFlashlight;
    float moveSpd;
    Vector3 moveVec = Vector3.zero;

    //Wander memory
    float wanderAngle;
    float wanderTimer;

    //Seek memory / A*
    List<Vector3> currentPath;
    int pathIndex;
    float pathTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        wanderAngle = Random.Range(0f, Mathf.PI * 2f);
        if (animator == null) animator = GetComponent<Animator>();
        if (target != null) playerFlashlight = target.GetComponent<PlayerFlashlight>();
    }
    void FixedUpdate()
    {
        EvaluateState();
        switch (currentState)
        {
            case State.Wander: UpdateWander(); break;
            case State.Chase: UpdateSeek(); break;
            case State.Flee: UpdateFlee(); break;
        }
        rb.AddForce(moveVec * moveSpd);

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        flatVelocity = Vector3.ClampMagnitude(flatVelocity, GetCurrentMaxSpeed());
        rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);

        UpdateAnimator(flatVelocity.magnitude);

        if (flatVelocity.sqrMagnitude > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.fixedDeltaTime);
        }
    }
    void UpdateAnimator(float currentSpeed)
    {
        if (animator == null) return;
        animator.SetFloat(speedParam, currentSpeed);
    }
    float GetCurrentMaxSpeed()
    {
        return currentState switch
        {
            State.Chase => chaseMaxSpd,
            State.Flee => fleeMaxSpd,
            _ => wanderMaxSpd
        };
    }
    //Decision Making - Finite State Machine
    void EvaluateState()
    {
        if (playerFlashlight != null && playerFlashlight.HasFlashLight && CanSeePlayer())
        {
            currentState = State.Flee;
            return;
        }
        if (currentState == State.Flee) currentState = State.Wander;
        Vector3 diff = target.transform.position - transform.position;
        diff.y = 0;
        float distToPlayer = diff.magnitude;

        if (currentState == State.Chase)
        {
            if (distToPlayer > loseChaseRad) currentState = State.Wander;
        }
        else
        {
            if (distToPlayer <= chaseDetecRad) currentState = State.Chase;
        }
    }
    bool CanSeePlayer()
    {
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = target.transform.position + Vector3.up * eyeHeight;
        Vector3 toTarget = targetPos - eyePos;
        float distance = toTarget.magnitude;

        if(distance > fleeSightRad) return false;
        if(Physics.Raycast(eyePos, toTarget.normalized, out RaycastHit hit, distance, sightMask))
            return false;
        return true;
    }
    //Wander Movement
    void UpdateWander()
    {
        currentPath = null;

        wanderTimer -= Time.fixedDeltaTime;
        if (wanderTimer <= 0f)
        {
            wanderAngle += Random.Range(-wanderRate, wanderRate);
            wanderTimer = wanderCooldown;
        }
        moveVec = new Vector3(Mathf.Cos(wanderAngle), 0, Mathf.Sin(wanderAngle));
        moveSpd = wanderMaxSpd;
    }
    //Pathfinding + Movement AI - A* + Seek (state Chase)
    void UpdateSeek()
    {
        pathTimer -= Time.fixedDeltaTime;
        if (pathTimer <= 0f)
        {
            currentPath = pathFinder.FindPath(transform.position, target.transform.position);
            pathIndex = 0;
            pathTimer = pathCalculateInterval;
        }
        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            moveVec = Vector3.zero;
            moveSpd = 0;
            return;
        }
        Vector3 waypoint = currentPath[pathIndex];
        Vector3 toWaypoint = waypoint - transform.position;
        toWaypoint.y = 0;
        if (toWaypoint.magnitude < 0.5f)
        {
            pathIndex++;
            if (pathIndex >= currentPath.Count)
            {
                moveVec = Vector3.zero;
                moveSpd = 0;
                return;
            }
            waypoint = currentPath[pathIndex];
            toWaypoint = waypoint - transform.position;
            toWaypoint.y = 0;
        }
        if (toWaypoint.magnitude < satisfactionRad)
        {
            moveVec = Vector3.zero;
            moveSpd = 0;
        }
        else if (toWaypoint.magnitude < slowRad)
        {
            moveVec = toWaypoint.normalized;
            moveSpd = chaseMaxSpd * toWaypoint.magnitude / slowRad;
        }
        else
        {
            moveVec = toWaypoint.normalized;
            moveSpd = chaseMaxSpd;
        }
    }
    //Movement AI - Flee
    void UpdateFlee()
    {
        currentPath = null;
        Vector3 direction = transform.position - target.transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f) direction = transform.forward;
        moveVec = direction.normalized;
        moveSpd = fleeMaxSpd;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDetecRad);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, loseChaseRad);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, fleeSightRad);

        if (currentPath != null)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < currentPath.Count - 1; i++)
                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
            foreach (var p in currentPath)
                Gizmos.DrawSphere(p, 0.15f);
        }
    }

}
