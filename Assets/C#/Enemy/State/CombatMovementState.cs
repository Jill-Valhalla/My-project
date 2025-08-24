using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AICombatStates { Idle, Chase,Circling}

public class CombatMovementState : State<EnemyController> 
{
    [SerializeField] float circlingSpeed = 20f;
    [SerializeField] float distanceToStand = 3.0f;
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

        enemy.NavAgent.stoppingDistance = distanceToStand;
        //enemy.CombatMovementTimer = 0f;
    }

    public override void Execute()
    {
        if(Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) > distanceToStand + adjustDistanceThreshold)
        {
            StartChase();
        }

        if(state == AICombatStates.Idle)
        {
            if(timer <= 0f)
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
            if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= distanceToStand + 0.05f)
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
            var rotatePos = Quaternion.Euler(0, circlingDir * circlingSpeed * Time.deltaTime, 0) * vecToTarget;

            enemy.NavAgent.Move(rotatePos - vecToTarget);
            enemy.transform.rotation = Quaternion.LookRotation(-rotatePos);
        }

        if(timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        //enemy.CombatMovementTimer += Time.deltaTime;

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
        //enemy.CombatMovementTimer = 0f;
    }
}
