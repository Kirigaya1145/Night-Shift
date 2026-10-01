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
    public float chaseDetecRad = 14f;
    public float loseChaseRad = 18f;
    public float catchDistance = 1.5f;

    //Decision Making - Search
    public float searchDuration = 4f;
    public float searchSpeedMult = 1.6f;
    Vector3 lastKnownPos;
    bool isSearching;
    float searchTimer;

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
    bool hasSpeedParam = false;
    bool speedParamChecked = false;

    public float waypointReachDist = 1f; // jarak untuk dianggap "sudah sampai"

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
        CheckCatch();
        switch (currentState)
        {
            case State.Wander: UpdateWander(); break;
            case State.Chase: UpdateSeek(); break;
            case State.Flee: UpdateFlee(); break;
        }
        
        Vector3 desiredVelocity = Vector3.ClampMagnitude(moveVec * moveSpd, GetCurrentMaxSpeed());
        rb.linearVelocity = new Vector3(desiredVelocity.x, rb.linearVelocity.y, desiredVelocity.z);

        UpdateAnimator(desiredVelocity.magnitude);

        if (desiredVelocity.sqrMagnitude > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.fixedDeltaTime);
        }
    }
    void CheckCatch()
    {
        if (currentState != State.Chase || GameManager.Instance == null) return;
        Vector3 d = target.transform.position - transform.position;
        d.y = 0;
        if (d.magnitude <= catchDistance) GameManager.Instance.Lose();
    }
    void UpdateAnimator(float currentSpeed)
    {
        if (animator == null) return;
        if (!speedParamChecked)
        {
            foreach (var param in animator.parameters)
            {
                if (param.name == speedParam && param.type == AnimatorControllerParameterType.Float)
                {
                    hasSpeedParam = true;
                    break;
                }
            }
            speedParamChecked = true;
        }
        if (hasSpeedParam) animator.SetFloat(speedParam, currentSpeed);
    }
    float GetCurrentMaxSpeed()
    {
        return currentState switch
        {
            State.Chase => chaseMaxSpd,
            State.Flee => fleeMaxSpd,
            _ => isSearching ? wanderMaxSpd * searchSpeedMult : wanderMaxSpd
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
            if (distToPlayer > loseChaseRad)
            {
                lastKnownPos = target.transform.position;
                isSearching = true;
                searchTimer = searchDuration;
                currentState = State.Wander;
            }
        }
        else
        {
            if (distToPlayer <= chaseDetecRad) 
            { 
                currentState = State.Chase; 
                isSearching = false;
            }
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
        if (isSearching)
        {
            searchTimer -= Time.fixedDeltaTime;
            if (searchTimer <= 0f)
            {
                isSearching = false; // benar-benar menyerah, lanjut wander acak
            }
            else
            {
                MoveAlongPathTo(lastKnownPos, wanderMaxSpd * searchSpeedMult);
                return;
            }
        }
        currentPath = null;
        pathFinder.grid.ReportPath(this,currentPath);

        wanderTimer -= Time.fixedDeltaTime;
        if (wanderTimer <= 0f)
        {
            wanderAngle += Random.Range(-wanderRate, wanderRate);
            wanderTimer = wanderCooldown;
        }

        Vector3 wanderDir = new Vector3(Mathf.Cos(wanderAngle), 0, Mathf.Sin(wanderAngle));
        float probeDist = 2.2f;
        if (pathFinder.grid.FreeDistance(transform.position, wanderDir, probeDist) < probeDist)
        {
            // terhalang: coba sudut acak sampai ketemu arah yang lapang (maks 12 percobaan)
            for (int i = 0; i < 12; i++)
            {
                float a = Random.Range(0f, Mathf.PI * 2f);
                Vector3 d = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
                if (pathFinder.grid.FreeDistance(transform.position, d, probeDist) >= probeDist)
                {
                    wanderAngle = a;
                    break;
                }
            }
            wanderTimer = wanderCooldown;
        }
        moveVec = new Vector3(Mathf.Cos(wanderAngle), 0, Mathf.Sin(wanderAngle));
        moveSpd = wanderMaxSpd;
    }
    //Pathfinding + Movement AI - A* + Seek (state Chase)
    void UpdateSeek()
    {
        MoveAlongPathTo(target.transform.position, chaseMaxSpd);
    }
    void MoveAlongPathTo(Vector3 destination, float speed)
    {
        // 1. Hitung ulang path secara berkala
        pathTimer -= Time.fixedDeltaTime;
        if (pathTimer <= 0f)
        {
            currentPath = pathFinder.FindPath(transform.position, destination);
            if (currentPath == null) Debug.Log($"{name}: FindPath GAGAL, target mungkin di luar grid atau tidak ada rute");
            pathFinder.grid.ReportPath(this, currentPath);
            pathIndex = 0;
            pathTimer = pathCalculateInterval;
        }
        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            moveVec = Vector3.zero;
            moveSpd = 0;
            return;
        }
        // 2. Lewati waypoint tengah yang sudah dekat
        Vector3 toWaypoint = currentPath[pathIndex] - transform.position;
        toWaypoint.y = 0;
        while (pathIndex < currentPath.Count - 1 && toWaypoint.magnitude < waypointReachDist)
        {
            pathIndex++;
            toWaypoint = currentPath[pathIndex] - transform.position;
            toWaypoint.y = 0;
        }
        // 3. Arah selalu menuju waypoint saat ini
        moveVec = toWaypoint.normalized;
        bool isLast = pathIndex == currentPath.Count - 1;
        // 4. Seek + Arrive: melambat dan berhenti HANYA di waypoint terakhir
        float dist = toWaypoint.magnitude;
        bool chasing = currentState == State.Chase;
        float stopRad = chasing ? 0.2f : satisfactionRad;   // saat Chase hampir tanpa arrive
        float slow = chasing ? 1f : slowRad;
        if (isLast && dist < satisfactionRad)
        {
            moveVec = Vector3.zero;
            moveSpd = 0;
        }
        else if (isLast && dist < slowRad)
        {
            moveSpd = speed * dist / slowRad;
        }
        else
        {
            moveSpd = speed;
        }
    }
    //Movement AI - Flee
    void UpdateFlee()
    {
        currentPath = null;
        pathFinder.grid.ReportPath(this, currentPath);
        Vector3 direction = transform.position - target.transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f) direction = transform.forward;
        moveVec = FreeDirection(direction.normalized);
        moveSpd = fleeMaxSpd;
    }
    Vector3 FreeDirection(Vector3 desired)
    {
        float probe = 2.2f;
        for (int a = 0; a <= 180; a += 30)
        {
            for (int s = -1; s <= 1; s += 2)
            {
                Vector3 d = Quaternion.AngleAxis(a * s, Vector3.up) * desired;
                if (pathFinder.grid.FreeDistance(transform.position, d, probe) >= probe) return d;
            }
        }
        return desired;
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
