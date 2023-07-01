using EasyCharacterMovement;
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class HermitRobotAIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AgentCharacter agentCharacter;
    [SerializeField] private EntityHealthController healthController;
    private StateMachine stateMachine;
    [SerializeField] private TMP_Text currentStateText;

    [Header("Idle State Presets")]
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 5f;

    [Header("Wander State Presets")]
    [SerializeField] private float maxWanderRadius = 15f;
    [SerializeField] private float maxWanderTime = 10f;

    [Header("Chase State Presets")]
    [SerializeField] private LayerMask targetEntityLayerMask;
    [SerializeField] private float awarenessRadius = 20f;
    [SerializeField] private float highAlertAwarenessRadius = 50f;
    [SerializeField] private EntityTeam_t enemyTeams = EntityTeam_t.NULL;
    public bool IsOnHighAlert { get; private set; }
    private List<Entity> enemiesInAwarenessRadius = new List<Entity>();
    public Entity EntityTarget { get; private set; }

    [Header("Ranged Aggression State Presets")]
    [SerializeField] private float minRangedAgressionRadius = 25f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnpoint;
    [SerializeField] private float rangedAttackCooldown = 3f;
    [SerializeField] private float fleeRadius = 8f;

    [Header("Defending State Presets")]
    [SerializeField] private float defendTime = 4f;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private float damageChargeThresholdForDefense = 25f;
    private float damageCharge = 0f;
    [SerializeField] private float damageChargeDecreaseRate = 1f;
    [SerializeField] private int numberOfDefenseShields = 1;
    private int numberOfShieldsRemaining;
     

    private void Awake()
    {
        stateMachine = new StateMachine();

        AssignStateMachineStates();
    }
    

    private void Update()
    {
        stateMachine.Tick();
        EntityTarget = CheckForEntityInRadius();
        if (damageCharge > 0f)
        {
            damageCharge = Mathf.Max(0f, damageCharge - damageChargeDecreaseRate * Time.deltaTime);
        }
    }

    private void Start()
    {
        numberOfShieldsRemaining = numberOfDefenseShields;
        IsOnHighAlert = false;
    }

    private void OnEnable()
    {
        healthController.OnDamageAtPoint += TookDamageAtPoint;
        stateMachine.OnStateChanged += UpdateStateText;
    }

    private void OnDisable()
    {
        healthController.OnDamageAtPoint -= TookDamageAtPoint;
        stateMachine.OnStateChanged -= UpdateStateText;
    }

    private void AssignStateMachineStates()
    {
        // Create states
        var idleState = new Idle_s(minIdleTime, maxIdleTime);
        var wanderState = new Wander_s(agentCharacter.GetNavMeshAgent(), maxWanderRadius, agentCharacter.GetPosition(), maxWanderTime);
        var chaseState = new Chase_s(agentCharacter.GetNavMeshAgent(), this);
        var rangedState = new RangedAggression_s(agentCharacter.GetNavMeshAgent(), this);

        var defendState = new Defending_s(agentCharacter.GetNavMeshAgent(), this);

        // Assign states
        stateMachine.AddTransition(idleState,
                                   wanderState,
                                   () => idleState.IsDoneIdle);
        stateMachine.AddTransition(wanderState,
                                   idleState,
                                   () => wanderState.IsDoneWandering);
        stateMachine.AddTransition(idleState,
                                   chaseState,
                                   () => EntityTarget != null);
        stateMachine.AddTransition(wanderState,
                                   chaseState,
                                   () => EntityTarget != null);
        stateMachine.AddTransition(chaseState,
                                   idleState,
                                   () => EntityTarget == null);
        stateMachine.AddTransition(chaseState,
                                   rangedState,
                                   () => Vector3.Distance(EntityTarget.transform.position, transform.position) <= minRangedAgressionRadius);
        stateMachine.AddTransition(rangedState,
                                   chaseState,
                                   () => Vector3.Distance(EntityTarget.transform.position, transform.position) > minRangedAgressionRadius);

        stateMachine.AddAnyTransition(defendState,
                                      () => numberOfShieldsRemaining > 0 && damageCharge >= damageChargeThresholdForDefense);
        stateMachine.AddTransition(defendState,
                                   idleState,
                                   () => !defendState.IsDefending);

        // Set Default state
        stateMachine.SetState(idleState);
        UpdateStateText(idleState.ToString());
    }

    private Entity CheckForEntityInRadius()
    {
        Entity target = null;

        // If enemyTeams is 0, this entity has no enemy teams
        if (enemyTeams == 0)
        {
            return target;
        }
        enemiesInAwarenessRadius.Clear();
        Collider[] cols = Physics.OverlapSphere(transform.position, IsOnHighAlert ? highAlertAwarenessRadius : awarenessRadius, targetEntityLayerMask);
        foreach (Collider col in cols)
        {
            Entity colEntity = col.GetComponentInParent<Entity>();
            if ((colEntity.EntityData.Team & enemyTeams) != 0)
            {
                enemiesInAwarenessRadius.Add(colEntity);
            }
        }

        return GetClosestEntity(enemiesInAwarenessRadius);
    }

    private Entity GetClosestEntity(List<Entity> entities)
    {
        Entity closest = null;
        float closestDistance = float.MaxValue;
        float d = 0;

        foreach (Entity entity in entities)
        {
            d = Vector3.Distance(transform.position, entity.transform.position);
            if (d < closestDistance)
            {
                closest = entity;
                closestDistance = d;
            }
        }
        
        return closest;
    }

    private void TookDamageAtPoint(int amount, Vector3? position)
    {
        IsOnHighAlert = true;

        damageCharge += amount;
    }

    private void UpdateStateText(string stateString)
    {
        if (currentStateText == null) return;

        currentStateText.text = stateString;
    }

    #region States

    /// <summary>
    /// Idle_s
    ///  * No target
    ///  * Not moving
    /// </summary>
    public class Idle_s : IState
    {
        public bool IsDoneIdle { get; private set; }
        private float idleTimer = 0f;
        private float maxIdleTime;
        private float minIdleTime;
        private float targetIdleTime;

        public Idle_s(float _minIdle, float _maxIdle)
        {
            maxIdleTime = _maxIdle;
            minIdleTime = _minIdle;
        }

        public void OnEnter()
        {
            targetIdleTime = Random.Range(minIdleTime, maxIdleTime);
            IsDoneIdle = false;
            idleTimer = 0f;
        }

        public void OnExit()
        {

        }

        public void Tick()
        {
            if (idleTimer >= targetIdleTime)
            {
                IsDoneIdle = true;
            }
            idleTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Wander_s
    ///  * No target
    ///  * Moving to various places
    /// </summary>
    public class Wander_s : IState
    {
        public bool IsDoneWandering { get; private set; }
        private float wanderTimer = 0f;
        float maxWanderTime = float.MaxValue;
        private NavMeshAgent agent;
        private Vector3 targetPosition;
        private Vector3 originPosition;
        private float wanderRadius;

        public Wander_s(NavMeshAgent _agent, float _wanderRadius, Vector3 _origin, float _maxWanderTime)
        {
            agent = _agent;
            wanderRadius = _wanderRadius;
            originPosition = _origin;
            maxWanderTime = _maxWanderTime;
        }

        public void OnEnter()
        {
            IsDoneWandering = false;
            wanderTimer = 0f;
            GetRandomPosition();
        }

        public void OnExit()
        {

        }

        public void Tick()
        {
            if (wanderTimer >= maxWanderTime || (Vector3.Distance(agent.destination, agent.transform.position) <= agent.stoppingDistance * 1.2f))
            {
                IsDoneWandering = true;
            }
            wanderTimer += Time.deltaTime;
        }

        private void GetRandomPosition()
        {
            Vector2 target2d = Random.insideUnitCircle * Random.Range(wanderRadius / 2f, wanderRadius);
            Vector3 target3d = Vector3.forward * target2d.x + Vector3.right * target2d.y;
            targetPosition = target3d + originPosition;
            agent.SetDestination(targetPosition);
        }
    }

    /// <summary>
    /// Chase_s
    ///  * Has target
    ///  * Not close enough to target for ranged or melee attack
    ///  * Moving towards target
    /// </summary>
    public class Chase_s : IState
    {
        private NavMeshAgent agent;
        private HermitRobotAIController aiController;

        public Chase_s(NavMeshAgent _agent, HermitRobotAIController _aiController)
        {
            agent = _agent;
            aiController = _aiController;
        }

        public void OnEnter()
        {

        }

        public void OnExit()
        {
            agent.SetDestination(agent.transform.position);
            aiController.IsOnHighAlert = false;
        }

        public void Tick()
        {
            if (aiController.EntityTarget != null)
            {
                agent.SetDestination(aiController.EntityTarget.transform.position);
            }
        }
    }

    /// <summary>
    /// RangedAggression_s
    ///  * Has Target
    ///  * Close enough to target for ranged attacks, but too far for melee
    ///  * Moving within radius of player to maintain ranged distance
    /// </summary>
    public class RangedAggression_s : IState
    {
        private NavMeshAgent agent;
        private HermitRobotAIController aiController;
        private GameObject projectilePrefab;
        private Transform projectileSpawnpoint;
        private float attackCooldown = float.MaxValue;
        private float attackCooldownTimer = 0f;
        private float fleeRadius;
        public bool IsFleeing = false;


        public RangedAggression_s(NavMeshAgent _agent, HermitRobotAIController _aiController)
        {
            agent = _agent;
            aiController = _aiController;
            projectilePrefab = aiController.projectilePrefab;
            projectileSpawnpoint = aiController.projectileSpawnpoint;
            attackCooldown = aiController.rangedAttackCooldown;
            attackCooldownTimer = attackCooldown;
            fleeRadius = aiController.fleeRadius;
        }

        public void OnEnter()
        {
            aiController.IsOnHighAlert = true;
            IsFleeing = false;
        }

        public void OnExit()
        {

        }

        public void Tick()
        {
            float d = Vector3.Distance(aiController.transform.position, aiController.EntityTarget.transform.position);

            // if we arent fleeing, and our distance is within the flee radius, start fleeing
            if (d <= fleeRadius)
            {
                Transform targetTransform = aiController.EntityTarget.transform;
                Vector3 dir = agent.transform.position - targetTransform.position;
                // Pick a spot 45 degrees away from the player's forward (towards self)
                // set that as the destination
                agent.SetDestination(dir * fleeRadius * 1.5f);
                IsFleeing = true;
            }
            else
            {
                IsFleeing = false;
            }

            // If we arent fleeing, and our cooldown is good, we can shoot
            if (!IsFleeing && attackCooldownTimer >= attackCooldown)
            {
                LookAtTarget();
                ShootAtTarget();
                attackCooldownTimer = 0f;
            }

            attackCooldownTimer += Time.deltaTime;
        }

        private void LookAtTarget()
        {
            if (aiController.EntityTarget == null) return;

            agent.transform.LookAt(aiController.EntityTarget.transform);
        }

        private void ShootAtTarget()
        {
            if (aiController.EntityTarget == null) return;

            Instantiate(projectilePrefab, projectileSpawnpoint.position, projectileSpawnpoint.rotation);
        }
    }

    /// <summary>
    /// MeleeAggression_s
    ///  * Has target
    ///  * Close enough to target to hit with melee attack
    ///  * Attack then quickly move towards ranged distance
    /// </summary>
    public class MeleeAggression_s { }

    /// <summary>
    /// Defending_s
    ///  * Health is low and hasn't received damage in X seconds (punish player for not finishing off enemy)
    ///  * Stop moving and take considerably less damage
    /// </summary>
    public class Defending_s : IState
    {
        private NavMeshAgent agent;
        private HermitRobotAIController aiController;
        private GameObject shieldPrefab;
        private float defendTime;
        private float defendTimer;
        EntityHealthController shieldHealthController;

        public bool IsDefending = true;


        public Defending_s(NavMeshAgent _agent, HermitRobotAIController _aiController)
        {
            agent = _agent;
            aiController = _aiController;
            defendTime = aiController.defendTime;
            shieldPrefab = aiController.shieldPrefab;
        }

        public void OnEnter()
        {
            agent.SetDestination(agent.transform.position);
            defendTimer = 0f;
            IsDefending = true;

            shieldHealthController = Instantiate(shieldPrefab, agent.transform.position, Quaternion.identity).GetComponent<EntityHealthController>();
            shieldHealthController.OnEntityDied += OnShieldDied;
            aiController.numberOfShieldsRemaining--;
        }

        public void OnExit()
        {

        }

        public void Tick()
        {
            // If we run out of time, kill the shield
            if (defendTimer >= defendTime)
            {
                if (shieldHealthController != null)
                {
                    shieldHealthController?.Kill();
                }

                // we are no longer defending
                IsDefending = false;
            }

            defendTimer += Time.deltaTime;
        }

        private void OnShieldDied()
        {
            // If the shield breaks, we are no longer defending
            IsDefending = false;
        }
    }

    #endregion
}
