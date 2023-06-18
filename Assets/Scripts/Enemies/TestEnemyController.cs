using EasyCharacterMovement;
using Mono.CSharp.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TestEnemyController : AgentCharacter
{
    private StateMachine stateMachine;

    private Vector3 currentTarget;

    private TPSCombatCharacterController player;

    protected override void OnAwake()
    {
        base.OnAwake();

        stateMachine = new StateMachine();
        player = FindObjectOfType<TPSCombatCharacterController>();

        // Define state machine states
        var wanderState = new Wander(this);
        var moveToTargetState = new MoveTowardsTarget(this, player.transform);
        //var searchState = new SearchForPlayer(this, player);
        var meleeAttackState = new MeleeAttackPlayer(this, GetNavMeshAgent(), player);

        // Define state machine transitions
        stateMachine.AddTransition(wanderState,
                                   moveToTargetState, 
                                   () => Vector3.Distance(GetPosition(), player.GetPosition()) <= 10f);

        stateMachine.AddTransition(moveToTargetState,
                                   meleeAttackState,
                                   () => Vector3.Distance(GetPosition(), player.GetPosition()) <= 1f);

        stateMachine.AddTransition(meleeAttackState,
                                   moveToTargetState,
                                   () => Vector3.Distance(GetPosition(), player.GetPosition()) > 3f);

        stateMachine.AddTransition(moveToTargetState,
                                   wanderState,
                                   () => Vector3.Distance(GetPosition(), player.GetPosition()) > 10f);

        stateMachine.SetState(wanderState);
    }

    protected override void OnStart()
    {
        base.OnStart();


    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        stateMachine.Tick();
    }

    private void Attack()
    {
        print($"Attacking!");
    }


    #region States

    public class Wander : IState
    {
        TestEnemyController enemyController;

        public Wander(TestEnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        public void Tick()
        {
            float d = Vector3.Distance(enemyController.transform.position, enemyController.currentTarget);


            if (d <= .5f)
            {
                GetNewTarget();
            }
        }

        public void OnEnter()
        {
            GetNewTarget();
        }

        public void OnExit()
        {

        }   

        private void GetNewTarget()
        {
            enemyController.currentTarget = enemyController.transform.position + Vector3.forward * Random.Range(-10f, 10f) + Vector3.right * Random.Range(-10f, 10f);
            enemyController.GetNavMeshAgent().SetDestination(enemyController.currentTarget);
        }
    }


    public class MoveTowardsTarget : IState
    {
        private TestEnemyController enemyController;

        private Transform targetTransform;

        public MoveTowardsTarget(TestEnemyController _enemyController, Transform _targetTransform)
        {
            enemyController = _enemyController;
            targetTransform = _targetTransform;
        }

        void IState.OnEnter()
        {

        }

        void IState.OnExit()
        {

        }

        void IState.Tick()
        {
            enemyController.currentTarget = targetTransform.position;
            enemyController.GetNavMeshAgent().SetDestination(enemyController.currentTarget);
        }
    }

    public class MeleeAttackPlayer : IState
    {
        private TestEnemyController enemyController;
        private NavMeshAgent agent;
        private TPSCombatCharacterController player;

        private float meleeAttackCooldown = 1.25f;
        private float meleeTimer = 0f;

        public MeleeAttackPlayer(TestEnemyController _enemyController, NavMeshAgent _agent, TPSCombatCharacterController _player)
        {
            enemyController = _enemyController;
            agent = _agent;
            player = _player;
        }

        void IState.Tick()
        {
            if (meleeTimer <= 0f)
            {
                enemyController.RotateTowards(player.transform.position);
                enemyController.Attack();
                meleeTimer = meleeAttackCooldown;
            }

            meleeTimer -= Time.deltaTime;
        }

        void IState.OnEnter()
        {

        }

        void IState.OnExit()
        {

        }
    }

    #endregion
}