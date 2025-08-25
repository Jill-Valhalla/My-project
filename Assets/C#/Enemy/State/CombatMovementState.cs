using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AICombatStates { Idle, Chase, Circling}
 
public class CombatMovementState : State<EnemyController>
{
    [SerializeField] float distanceToStance = 3.0f;
    [SerializeField] float adjustDistanceThreshold = 1.0f;

    AICombatStates state;

    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;

        enemy.NavAgent.stoppingDistance = distanceToStance;

    }

    public override void Execute()
    {
        if(Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) > distanceToStance + adjustDistanceThreshold)
        {
            StartChase();
        }



        if (state == AICombatStates.Idle)
        {

        }
        else if(state == AICombatStates.Chase)
        {
            if(Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= distanceToStance + 0.03f)
            {
                StartIdel();
                return;
            }

            enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        }
        else if(state == AICombatStates.Circling)
        {
        }

        
        enemy.Animator.SetFloat("moveAmount", enemy.NavAgent.velocity.magnitude / enemy.NavAgent.speed);

    }

    void StartChase()
    {
        state = AICombatStates.Chase;
        enemy.Animator.SetBool("combatMode", false);
    }

    void StartIdel()
    {
        state = AICombatStates.Idle;
        enemy.Animator.SetBool("combatMode", true);
        
    }


    public override void Exit()
    {
        
    }
}
