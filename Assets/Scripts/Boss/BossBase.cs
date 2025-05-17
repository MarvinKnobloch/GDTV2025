using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour
{
    [Header("Combat")]
    public List<BossAttackBase> bossAttacks = new List<BossAttackBase>();
    public float TimeBetweenAttacks = 1f;

    private bool _attacking = true;
    private BossAttackBase _currentAttack;

    void Start()
    {
        StartCoroutine(AttackCoroutine());
    }

    void FixedUpdate()
    {
        _currentAttack?.AttackMovement();
    }

    void OnDestroy()
    {
        _attacking = false;
    }

    void OnDisable()
    {
        _attacking = false;
    }

    IEnumerator AttackCoroutine()
    {
        while (_attacking)
        {
            for (int i = 0; i < bossAttacks.Count; i++)
            {
                _currentAttack = bossAttacks[i];
                yield return StartCoroutine(bossAttacks[i].ShootProjectiles());
                yield return new WaitForSeconds(TimeBetweenAttacks);
            }
        }
    }
}
