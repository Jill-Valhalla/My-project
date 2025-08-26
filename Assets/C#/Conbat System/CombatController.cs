
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    MeeleFighter meeleFighter;

    private void Awake()
    {
        meeleFighter = GetComponent<MeeleFighter>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            var enemy = EnemyManager.i.GetAttackingEnemy();
            if (enemy != null && enemy.Fighter.IsCounterable && !meeleFighter.InAction)
            {
                StartCoroutine(meeleFighter.PerformCounterAttack(enemy));

            }
            else
            {
                meeleFighter.TryToAttack();
            }

                
        }
    }
}
