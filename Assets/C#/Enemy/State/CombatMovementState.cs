using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AICombatStates { Idle, Chase, Circling}
 
public class CombatMovementState : State<EnemyController>
{
    [SerializeField] float circlingSpeed = 20f;
    [SerializeField] float distanceToStance = 3.0f;
    [SerializeField] float adjustDistanceThreshold = 1.0f;
    [SerializeField] Vector2 idleTimeRange = new Vector2(2, 5);
    [SerializeField] Vector2 circlingTimeRange = new Vector2(3, 6);

    float timer = 0f;

    int circlingDir = 1;

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
            if(timer <= 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    StartIdle();
                }
                else
                {
                    StartCircling();
                }
            }
        }
        else if(state == AICombatStates.Chase)
        {
            if(Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= distanceToStance + 0.03f)
            {
                StartIdle();
                return;
            }

            enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        }
        else if(state == AICombatStates.Circling)
        {
            if(timer <= 0)
            {
                StartIdle();
                return;
            }

            var vecToTarget = enemy.transform.position - enemy.Target.transform.position;
            var rotatedPos = Quaternion.Euler(0, circlingSpeed * circlingDir * Time.deltaTime, 0) * vecToTarget;

            enemy.NavAgent.Move(rotatedPos - vecToTarget);
            enemy.transform.rotation = Quaternion.LookRotation(-rotatedPos);

        }

        if(timer >= 0)
        {
            timer -= Time.deltaTime;
        }

        
        

    }

    void StartChase()
    {
        state = AICombatStates.Chase;

        enemy.Animator.SetBool("combatMode", false);
        
    }

    void StartIdle()
    {
        state = AICombatStates.Idle;
        timer = Random.Range(idleTimeRange.x, idleTimeRange.y);

        enemy.Animator.SetBool("combatMode", true);
        

    }

    void StartCircling()
    {
        state = AICombatStates.Circling;

        enemy.NavAgent.ResetPath();
        timer = Random.Range(circlingTimeRange.x, circlingTimeRange.y);

        circlingDir = Random.Range(0, 2) == 0 ? 1 : -1;


    }


    public override void Exit()
    {
        
    }
}
