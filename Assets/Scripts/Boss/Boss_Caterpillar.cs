using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss_Caterpillar : BossBase
{
    public GameObject Tail;

    void Start()
    {
        StartCoroutine(HeadTailAttackCoroutine());
    }

    IEnumerator HeadTailAttackCoroutine()
    {
        while (_attacking)
        {
            for (int i = 0; i < bossAttacks.Count; i++)
            {
                //
                // -- PREPARATION --
                //
                _currentAttack = bossAttacks[i];
                var attackType = _currentAttack.GetType();
                var tailAttack = Tail.AddComponent(attackType) as BossAttackBase;
                var shouldTailAttack = tailAttack is not BossAttackSpawnGoon;
                tailAttack.ProjectilePrefabs = _currentAttack.ProjectilePrefabs;
                tailAttack.ProjectilesToShoot = _currentAttack.ProjectilesToShoot;
                tailAttack.SecondsBetweenProjectiles = _currentAttack.SecondsBetweenProjectiles;

                if (_currentAttack is BossAttackStraight && _currentAttack.ProjectilePrefabs.Count > 1)
                {
                    (_currentAttack as BossAttackStraight).ProjectileToUse = 1;
                }

                if (_currentAttack is BossAttackCaterpillarMove)
                {
                    (_currentAttack as BossAttackCaterpillarMove).IsHead = true;
                    (tailAttack as BossAttackCaterpillarMove).IsHead = false;
                    (tailAttack as BossAttackCaterpillarMove).TailSlots = (_currentAttack as BossAttackCaterpillarMove).TailSlots;
                    (tailAttack as BossAttackCaterpillarMove).CurrentSlot = (_currentAttack as BossAttackCaterpillarMove).CurrentSlot;
                }

                if (_currentAttack is BossAttackSpawnGoon)
                {
                    var spawnSlot = FindFirstObjectByType<Boss_Caterpillar_SpawnSlot>();

                    if (spawnSlot != null)
                    {
                        var goon = (_currentAttack as BossAttackSpawnGoon).Goons.First();
                        goon.SpawnPosition = spawnSlot.GetSpawnSlot();
                        (_currentAttack as BossAttackSpawnGoon).Goons = new List<BossAttackGoonData> { goon };
                    }
                }

                //
                // -- ATTACK --
                //
                _currentAttack.StartAttack();

                if (shouldTailAttack)
                {
                    tailAttack.StartAttack();
                    StartCoroutine(tailAttack.ShootProjectiles());
                }

                yield return StartCoroutine(bossAttacks[i].ShootProjectiles());

                _currentAttack.FinishAttack();

                if (shouldTailAttack)
                {
                    StopCoroutine(tailAttack.ShootProjectiles());
                    tailAttack.FinishAttack();
                }

                yield return new WaitForSeconds(TimeBetweenAttacks);
                Destroy(tailAttack);
            }
        }
    }
}
