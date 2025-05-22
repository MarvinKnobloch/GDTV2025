using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour
{
    [Header("Combat")]
    public BossPhaseManager PhaseManager;
    public Health Health;
    public float TimeBetweenAttacks = 1f;

    protected bool _attacking = true;
    protected BossAttackBase _currentAttack;

    void Start()
    {
        Health.hitEvent.AddListener(() => PhaseManager.CheckForTransition(Health));
        StartCoroutine(AttackCoroutine());
    }

    void FixedUpdate()
    {
        _currentAttack?.AttackMovement();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        _attacking = false;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        _attacking = false;
    }

    IEnumerator AttackCoroutine()
    {
        while (_attacking)
        {
            var phase = PhaseManager.CurrentPhase;
            for (int i = 0; i < phase.BossAttacks.Count; i++)
            {
                _currentAttack = phase.BossAttacks[i];
                _currentAttack.StartAttack();
                yield return StartCoroutine(phase.BossAttacks[i].ShootProjectiles());
                _currentAttack.FinishAttack();
                yield return new WaitForSeconds(TimeBetweenAttacks);
            }
        }
    }
}
