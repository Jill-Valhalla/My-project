using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Vector2 timeRangeBetweenAttacks = new Vector2(1, 4);

    [SerializeField] CombatController player;

    public static EnemyManager i {  get; private set; }
    
    private void Awake()
    {
        i = this;
    }

    private List<EnemyController> enemiesInRange = new List<EnemyController>();
    float notAttackingTimer = 2f;

    public void AddEnemyInRange(EnemyController enemy)
    {
        if (!enemiesInRange.Contains(enemy)) 
        { 
            enemiesInRange.Add(enemy);
        }
    }
    public void RemoveEnemyInRange(EnemyController enemy)
    {
        enemiesInRange.Remove(enemy);
    }

    float timer = 0f;

    private void Update()
    {
        if(enemiesInRange.Count == 0)
        {
            return;
        }

        if (!enemiesInRange.Any(e => e.IsInState(EnemyStates.Attack)))
        {
            if(notAttackingTimer > 0) 
            { 
                notAttackingTimer -= Time.deltaTime; 
            }
            if(notAttackingTimer < 0)
            {
                var attackingEnemy = SelectEnemyForAttack();

                if(attackingEnemy != null)
                {
                    attackingEnemy.ChangeState(EnemyStates.Attack);
                    notAttackingTimer = Random.Range(timeRangeBetweenAttacks.x, timeRangeBetweenAttacks.y);
                }
                else
                {
                    notAttackingTimer = 1f;                                          
                }

            }
        }
        if(timer >= 0.1f)
        {
            timer = 0f;
            player.targetEnemy = GetClosestEnemyToPlayerDir();
        }

        timer += Time.deltaTime;
        
    }
    
    EnemyController SelectEnemyForAttack()
    {
        if (enemiesInRange.Count == 0)
            return null;
        return enemiesInRange.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault(e => e.Target != null);
    }
    
    public EnemyController GetAttackingEnemy()
    {
        return enemiesInRange.FirstOrDefault(e => e.IsInState(EnemyStates.Attack));
    } 

    public EnemyController GetClosestEnemyToPlayerDir()
    {
        var targetingDir = player.GetTargetingDir();

        float minDistance = Mathf.Infinity;
        EnemyController closestEnemy = null;

        foreach(var enemy in enemiesInRange)
        {
            var vecToEnemy = enemy.transform.position - player.transform.position;
            vecToEnemy.y = 0;

            float angle = Vector3.Angle(targetingDir,vecToEnemy);
            float distance = vecToEnemy.magnitude * Mathf.Sin(angle * Mathf.Deg2Rad);

            if(distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}
