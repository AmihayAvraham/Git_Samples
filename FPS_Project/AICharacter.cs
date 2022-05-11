using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacter : MonoBehaviour, ITeamMember
{
    public float distanceToTarget;
    public float maxRandomMovement = 1;
    public float updateDestinationFrequency = 0.5f;
    public float updateDestinationDistance = 0.5f;
    public float wanderTimer;
    public float speedRotation;
    public bool rotateToTarget = true;
    public float distanceLockRotateToTarget;

    public OrbitalTargeting orbit;

    public Transform Target;
    public OrbitalTargeting currentTargetOrbit;
    public float timeToChangeTarget = 3;

    public bool instantiateRagdollOnDeath = true;
    public GameObject prefabRagdollOnDeath;

    [SerializeField]
    private EnemiesCollider enemiesCollider;
    [SerializeField]
    private GameObject objTeam1indication;
    [SerializeField]
    private GameObject objTeam2indication;

    [SerializeField]
    private Material materialNormal;
    [SerializeField]
    private Material materialTargeted;
    [SerializeField]
    private Renderer[] renderers;

    [Header("Run")]
    [SerializeField]
    private bool canRun = false;
    [SerializeField]
    private string runAnimBoolParam;
    [SerializeField]
    private float runToDistance;
    [SerializeField]
    private float runSpeedMultiple = 1.2f;

    [Header("Roll")]
    [SerializeField]
    [Range(0, 1f)]
    private float rollChance = 0.2f;
    [SerializeField]
    private float rollDistance = 1f;
    [SerializeField]
    private float rollDuration = 3f;
    [SerializeField]
    private float rollCooldownTime = 3f;
    [SerializeField]
    private MMCooldown rollCooldown;
    [SerializeField]
    private float rollMinAngle;
    [SerializeField]
    private float rollMaxAngle;
    [SerializeField]
    public AnimationCurve rollCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
    [SerializeField]
    private string rollAnimTrigger;

    private Health health;
    private Animator anim;
    private NavMeshAgent agent;
    private bool shouldMove;
    private Transform current;
    private bool isMovingAround;
    private Transform currentTarget;
    private float lastRoll;
    private AIBossCharacter bossCharacter;
    private float agentInitialSpeed;

    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    Vector3 nextPos;

    private void Awake()
    {
        current = transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        bossCharacter = GetComponent<AIBossCharacter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        agent.updatePosition = false;
        agent.isStopped = false;

        health.OnDeath += OnDeath;

        agentInitialSpeed = agent.speed;

        currentTarget = Target;
        agent.SetDestination(currentTarget.position);
        nextPos = agent.nextPosition;
    }

    private void UpdateNextPosition()
    {
        if (IsRolling)
        {
            if (rollTimer < rollDuration)
            {
                //_dashAnimParameterDirection = (rollDestination - _dashOrigin).normalized;
                nextPos = Vector3.Lerp(rollOrigin, rollDestination, rollCurve.Evaluate(rollTimer / rollDuration));
                agent.nextPosition = nextPos;
                //agent.Move(nextPos - current.position);
                rollTimer += Time.deltaTime;
                //controller.Move(rollNewPosition - this.transform.position);
            }
            else
            { // Stop roll
                isMovingAround = true;
                //agent.isStopped = false;
                agent.SetDestination(currentTarget.position);
                rollCooldown.Start();
                //behaviourManager.UnlockTempBehaviour(this.behaviourCode);
                lastRoll = Time.time;
                IsRolling = false;
                health.Invulnerable = false;
            }
        }
        else
        {
            
            if (Time.time - timeTargetChanged >= timeToChangeTarget)
            {
                currentTarget = currentTargetOrbit.GetRandomTarget();
                timeTargetChanged = Time.time;
            }

            isMovingAround = 
                Vector3.Distance(currentTarget.position, current.position) <= distanceToTarget || 
                Time.time - lastRoll <= 1f;

            if (isMovingAround)
            {
                timer += Time.deltaTime;
                if (timer >= wanderTimer)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, maxRandomMovement, -1);
                    //Debug.Log("nextPos isMovingAround");
                    agent.SetDestination(newPos);
                    timer = 0;
                }

                if (Time.time - lastRoll >= rollCooldownTime && Random.Range(0, 1f) <= rollChance)
                {
                    rollRequest = true;
                    RollHandler();
                }
            }
            else
            {
                //Debug.Log("nextPos agent.nextPosition");
                agent.SetDestination(currentTarget.position);
            }
            
            nextPos = agent.nextPosition;
        }

        /*if (isMovingAround)
        {
            timer += Time.deltaTime;
            if (timer >= wanderTimer)
            {
                nextPos = RandomNavSphere(transform.position, maxRandomMovement, -1);
                timer = 0;
            }

            if (Time.time - lastRoll >= rollCooldownTime && Random.Range(0, 1f) <= rollChance)
            {
                rollRequest = true;
                //RollHandler();
            }
        }
        else
        {
            nextPos = agent.nextPosition;
        }*/

        /*if (Vector3.Distance(nextPos, current.position) <= updateDestinationDistance)
        {
            if (!agent.isStopped)
            {
                nextPos = agent.nextPosition;
                isMovingAround = false;
            }
            else
            {
                agent.SetDestination(current.position + Vector3.Cross(Target.position - current.position, Vector3.up));

                //nextPos = current.position + current.right * Random.Range(-maxRandomMovement, maxRandomMovement);
                isMovingAround = true;
            }

            lastDestUpdate = Time.time;
        } */     
    }

    private float timer;
    private float timeTargetChanged;
    private Vector3 worldDeltaPosition;
    private void Update()
    {
        if (health.CurrentHealth == 0)
        {
            agent.isStopped = true;
        }
        else
        {
            /*if (!IsRolling)
            {
                if (Time.time - timeTargetChanged >= timeToChangeTarget)
                {
                    currentTarget = TargetOrbit.GetRandomTarget();
                    timeTargetChanged = Time.time;
                }

                isMovingAround = Vector3.Distance(currentTarget.position, current.position) <= distanceToTarget;

                if (isMovingAround)
                {
                    timer += Time.deltaTime;
                    if (timer >= wanderTimer)
                    {
                        Vector3 newPos = RandomNavSphere(transform.position, maxRandomMovement, -1);
                        agent.SetDestination(newPos);
                        timer = 0;
                    }
                }
                else
                {
                    agent.SetDestination(currentTarget.position);
                }
            }*/

            worldDeltaPosition = agent.nextPosition - current.position;
            //worldDeltaPosition = nextPos - current.position;

            // Map 'worldDeltaPosition' to local space
            float dx = Vector3.Dot(current.right, worldDeltaPosition);
            float dy = Vector3.Dot(current.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2(dx, dy);

            // Low-pass filter the deltaMove
            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

            // Update velocity if time advances
            if (Time.deltaTime > 1e-5f)
                velocity = smoothDeltaPosition / Time.deltaTime;

            shouldMove = true;// isMovingAround || (velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius);

            if (bossCharacter != null && bossCharacter.IsAttacking)
            {
                shouldMove = false;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }

            if (canRun) 
            {
                if (Vector3.Distance(currentTarget.position, current.position) > runToDistance)
                {
                    agent.speed = agentInitialSpeed * runSpeedMultiple;
                    anim.SetBool(runAnimBoolParam, true);
                }
                else
                {
                    agent.speed = agentInitialSpeed;
                    anim.SetBool(runAnimBoolParam, false);
                }
            }
            

            //if (!shouldMove)
            //    UpdateNextPosition();

                // Update animation parameters
            anim.SetBool("move", shouldMove);

            if (shouldMove)
            {
                anim.SetFloat("velx", velocity.x);
                anim.SetFloat("vely", velocity.y);
            }

            ApplyImpact();
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    public bool IsRolling { get; private set; }
    private bool rollRequest;
    private bool runRequest;
    private float rollTimer;
    private Vector3 rollNewPosition;
    private Vector3 rollOrigin;
    private Vector3 rollDestination;
    private Vector3 rollAngle = Vector3.zero;
    private void RollHandler()
    {
        float angle = 0f;

        if (rollRequest)
        {
            IsRolling = true;
            rollRequest = false;
            health.Invulnerable = true;
            rollAngle = Vector3.zero;
            rollOrigin = this.transform.position;
            agent.ResetPath();
            //agent.isStopped = true;

            //angle = Vector3.SignedAngle(this.transform.forward, CurrentMovement.normalized, Vector3.up);
            rollDestination = rollOrigin + this.transform.forward * rollDistance;
            rollAngle.y = Random.Range(rollMinAngle, rollMaxAngle);
            rollDestination = MMMaths.RotatePointAroundPivot(rollDestination, rollOrigin, rollAngle);
            rollTimer = 0f;
            anim.SetTrigger(rollAnimTrigger);
            //behaviourManager.GetAnim.SetTrigger(rollAnimTrigger);
           // behaviourManager.LockTempBehaviour(this.behaviourCode);

           // if (aimManager.IsReloading)
            //    aimManager.StopReload();
        }

        /*if (IsRolling)
        {
            if (rollTimer < rollDuration)
            {
                //_dashAnimParameterDirection = (rollDestination - _dashOrigin).normalized;
                rollNewPosition = Vector3.Lerp(rollOrigin, rollDestination, rollCurve.Evaluate(rollTimer / rollDuration));
                rollTimer += Time.deltaTime;
                //controller.Move(rollNewPosition - this.transform.position);
            }
            else
            { // Stop roll
                rollCooldown.Start();
                //behaviourManager.UnlockTempBehaviour(this.behaviourCode);
                IsRolling = false;
                health.Invulnerable = false;
            }
        }*/
    }

    private Quaternion _lookRotation;
    private Vector3 _direction;
    private float lastDestUpdate;
    private void FixedUpdate()
    {
        /*if (agent.remainingDistance <= distanceToTarget)
        {
            isMovingAround = true;
            //agent.SetDestination(current.position + Vector3.Cross(Target.position - current.position, Vector3.up));
            agent.SetDestination(Target.position + Target.right * Random.Range(-maxRandomMovement, maxRandomMovement));
        }
        else
        {
            isMovingAround = false;
            agent.SetDestination(Target.position);
        }*/

        //if (!agent.isStopped || Time.time - lastDestUpdate > updateDestinationFrequency)
        //    UpdateNextPosition();

        if (rotateToTarget && !IsRolling && Vector3.Distance(currentTarget.position, current.position) <= distanceLockRotateToTarget)
        {
            _direction = (Target.position - current.position).normalized;

            //create the rotation we need to be in to look at the target
            _lookRotation = Quaternion.LookRotation(_direction);

            //rotate us over time according to speed until we are in the required rotation
            current.rotation = Quaternion.Slerp(current.rotation, _lookRotation, speedRotation);
        }
        /*else if (!rotateToTarget && shouldMove)
        {
            _direction = (currentTarget.position - current.position).normalized;

            //create the rotation we need to be in to look at the target
            _lookRotation = Quaternion.LookRotation(_direction);

            //rotate us over time according to speed until we are in the required rotation
            current.rotation = Quaternion.Slerp(current.rotation, _lookRotation, speedRotation);
        }*/

        if (!currentTarget.gameObject.activeInHierarchy && Random.Range(0, 1f) > 0.9f)
            GameManager.Instance.SetTarget(this);
    }

    private Vector3 impact = Vector3.zero;
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force;
    }

    private void ApplyImpact()
    {
        // apply the impact force:
        //if (impact.magnitude > 0.2) rb.AddForce(impact * Time.deltaTime, ForceMode.Force);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    public void SetTargeted(bool value)
    {
        if (renderers == null || renderers.Length == 0 || materialTargeted == null || materialNormal == null)
            return;

        foreach (Renderer rend in renderers)
        {
            if (value)
                rend.material = materialTargeted;
            else
                rend.material = materialNormal;
        }
    }

    private void OnDeath(DamageType damageType)
    {
        if (instantiateRagdollOnDeath)
        {
            GameObject obj = Instantiate(prefabRagdollOnDeath, transform.position, transform.rotation, transform.parent);
            RagdollOnDeath ragdoll = obj.GetComponent<RagdollOnDeath>();
            ragdoll.Init(impact, damageType);
        }

        GameManager.Instance.OnCharacterDeath(this);
    }

    void OnAnimatorMove()
    {
        UpdateNextPosition();
        //if (!agent.isStopped)
        {
            // Update position to agent position
            //current.position = agent.nextPosition;
            current.position = nextPos;
        }
    }

    public Team Team;
    private ITeamMember target;
    public void Init(Team team)
    {
        Team = team;

        if (team == Team.Team1)
        {
            enemiesCollider.Init(Team.Team2);

            if (objTeam1indication != null)
                objTeam1indication.SetActive(true);
        }
        else
        {
            enemiesCollider.Init(Team.Team1);

            if (objTeam2indication != null)
                objTeam2indication.SetActive(true);
        }

        this.tag = team.ToString();
    }

    public void SetTarget(ITeamMember target)
    {
        if (target != null)
        {
            this.target = target;
            Target = target.GetTransform();
            currentTargetOrbit = target.GetOrbitalTargeting();

            currentTarget = Target;
        }
    }

    public Team GetTeam()
    {
        return Team;
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public OrbitalTargeting GetOrbitalTargeting()
    {
        return orbit;
    }

    public ITeamMember GetTarget()
    {
        return target;
    }
}
