using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State<EnemyController>
{
    [SerializeField] float distanceToStance = 3.0f;

    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;

        enemy.NavAgent.stoppingDistance = distanceToStance;

    }

    public override void Execute()
    {
        enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        enemy.Animator.SetFloat("moveAmount", enemy.NavAgent.velocity.magnitude / enemy.NavAgent.speed);

    }

    public override void Exit()
    {
        
    }
}
