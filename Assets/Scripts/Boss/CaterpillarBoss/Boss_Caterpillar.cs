using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss_Caterpillar : BossBase
{
    public Boss_Caterpillar_Tail Tail;

    void Start()
    {
        Health.hitEvent.AddListener(() => PhaseManager.CheckForTransition(Health));
        StartCoroutine(HeadTailAttackCoroutine());
    }

    public void PhaseTransition()
    {
        StopAllCoroutines();
        StartCoroutine(HeadTailAttackCoroutine());
    }

    IEnumerator HeadTailAttackCoroutine()
    {
        while (_attacking)
        {
            var phase = PhaseManager.CurrentPhase;
            for (int i = 0; i < phase.BossAttacks.Count; i++)
            {
                //
                // -- PREPARATION --
                //
                _currentAttack = phase.BossAttacks[i];
                // var attackType = _currentAttack.GetType();
                // var tailAttack = Tail.AddComponent(attackType) as BossAttackBase;
                // var shouldTailAttack = tailAttack is not BossAttackSpawnGoon;
                // tailAttack.ProjectilePrefabs = _currentAttack.ProjectilePrefabs;
                // tailAttack.ProjectilesToShoot = _currentAttack.ProjectilesToShoot;
                // tailAttack.SecondsBetweenProjectiles = _currentAttack.SecondsBetweenProjectiles;

                // if (_currentAttack is BossAttackStraight && _currentAttack.ProjectilePrefabs.Count > 1)
                // {
                //     (_currentAttack as BossAttackStraight).ProjectileToUse = 1;
                // }

                // if (_currentAttack is BossAttackCaterpillarMove)
                // {
                //     (_currentAttack as BossAttackCaterpillarMove).IsHead = true;
                //     (tailAttack as BossAttackCaterpillarMove).IsHead = false;
                //     (tailAttack as BossAttackCaterpillarMove).TailSlots = (_currentAttack as BossAttackCaterpillarMove).TailSlots;
                // }

                // if (_currentAttack is BossAttackSpawnGoon)
                // {
                //     var spawnSlot = FindFirstObjectByType<Boss_Caterpillar_SpawnSlot>();

                //     if (spawnSlot != null)
                //     {
                //         var goon = (_currentAttack as BossAttackSpawnGoon).Goons.First();
                //         goon.SpawnPosition = spawnSlot.GetSpawnSlot();
                //         (_currentAttack as BossAttackSpawnGoon).Goons = new List<BossAttackGoonData> { goon };
                //     }
                // }

                //
                // -- ATTACK --
                //
                _currentAttack.StartAttack();
                Tail.ExecuteAttackInPhase(PhaseManager.CurrentPhaseIndex, i);

                // if (shouldTailAttack)
                // {
                //     if (tailAttack is BossAttackCaterpillarMove)
                //     {
                //         (tailAttack as BossAttackCaterpillarMove).CurrentSlot = (_currentAttack as BossAttackCaterpillarMove).CurrentSlot;
                //     }

                //     tailAttack.StartAttack();
                //     StartCoroutine(tailAttack.ShootProjectiles());
                // }

                yield return StartCoroutine(phase.BossAttacks[i].ShootProjectiles());

                _currentAttack.FinishAttack();
                Tail.FinishAttack();

                // if (shouldTailAttack)
                // {
                //     StopCoroutine(tailAttack.ShootProjectiles());
                //     tailAttack.FinishAttack();
                // }

                yield return new WaitForSeconds(TimeBetweenAttacks);
                // Destroy(tailAttack);
            }
        }
    }
}
