using EasyCharacterMovement;
using Mono.CSharp.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TestEnemyController : AgentCharacter
{
    private StateMachine stateMachine;

    private Transform currentTarget;

    protected override void OnAwake()
    {
        base.OnAwake();

        stateMachine = new StateMachine();
        var player = FindObjectOfType<TPSCombatCharacterController>();

        // Define state machine states
        var searchState = new SearchForPlayer(this, player);
        var meleeAttackState = new MeleeAttackPlayer(this, GetNavMeshAgent(), player);

        // Define state machine transitions
        stateMachine.AddTransition(searchState, 
                                   meleeAttackState, 
                                   () => Vector3.Distance(transform.position, player.GetPosition()) < 1f);

        stateMachine.SetState(searchState);
    }

    protected override void OnStart()
    {
        base.OnStart();


    }

    protected override void OnUpdate()
    {
        base.OnUpdate();


    }

    private void Attack()
    {
        print($"Attacking!");
    }


    #region States

    public class SearchForPlayer : IState
    {
        private TestEnemyController enemyController;
        private TPSCombatCharacterController player;

        public SearchForPlayer(TestEnemyController _enemyController, TPSCombatCharacterController _player)
        {
            enemyController = _enemyController;
            player = _player;
        }

        void IState.Tick()
        {
            enemyController.currentTarget = player.transform;
        }

        void IState.OnEnter()
        {
        }

        void IState.OnExit()
        {
        }
    }

    public class MoveTowardsTarget : IState
    {
        private TestEnemyController enemyController;

        public MoveTowardsTarget(TestEnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        void IState.OnEnter()
        {

        }

        void IState.OnExit()
        {

        }

        void IState.Tick()
        {
            enemyController.GetNavMeshAgent().SetDestination(enemyController.currentTarget.transform.position);
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
                enemyController.Attack();
                meleeTimer = meleeAttackCooldown;
            }

            meleeAttackCooldown -= Time.deltaTime;
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