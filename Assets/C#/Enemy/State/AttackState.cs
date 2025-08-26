using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<EnemyController>
{
    [SerializeField] float attackDistance = 1f;

    bool isAttacking;

    EnemyController enemy;
    
    public override void Enter(EnemyController owner)
    {
        enemy = owner;

        enemy.NavAgent.stoppingDistance = attackDistance;
    }
    
    public override void Execute()
    {
        if(isAttacking)
        {
            return;
        }

        if (enemy.Target == null)
        {
            Debug.LogWarning("AttackState: Target is null, switching to CombatMovement.");
            enemy.ChangeState(EnemyStates.CombatMovement);
            return;
        }


        enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        
        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= attackDistance + 0.03f)
        {
            StartCoroutine(Attack(Random.Range(0, enemy.Fighter.Attacks.Count + 1)));
        }
    }
    
    IEnumerator Attack(int comboCount = 1)
    {
        if (enemy == null || enemy.Target == null)
        {
            isAttacking = false; 
            yield break; 
        }

        isAttacking = true;
        enemy.Animator.applyRootMotion = true;

        if (enemy.Target != null)
        {
            enemy.Fighter.TryToAttack();
        }
        
        for (int i = 1; i < comboCount; i++)
        {
            yield return new WaitUntil(() => enemy.Fighter.AttackState == AttackStates.Cooldown);
            enemy.Fighter.TryToAttack();
        }

        yield return new WaitUntil(() => enemy.Fighter.AttackState == AttackStates.Idle ||  enemy.Target == null);


        if (enemy != null)
        {
            enemy.Animator.applyRootMotion = false;
            isAttacking = false;

            if (enemy.Target != null)
            {
                enemy.ChangeState(EnemyStates.RetreatAfterAttack);
            }
            else
            {
                
                enemy.ChangeState(EnemyStates.CombatMovement);
            }
        }
    }
    
    public override void Exit()
    {
        enemy.NavAgent.ResetPath();
    }   
}
    

