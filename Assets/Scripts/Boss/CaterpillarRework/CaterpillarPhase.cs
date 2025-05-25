using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarPhase : MonoBehaviour
{
    public List<CaterpillarAttack> HeadAttacks = new();
    public List<CaterpillarAttack> TailAttacks = new();
    public Transform HeadPosition;
    public Transform TailPosition;
    public float TimeBetweenAttacks = 1f;
    public int TransitionAtHealth = 1;

    public Animator HeadAnimator { private get; set; }
    public Animator TailAnimator { private get; set; }

    void Start()
    {
        Debug.Assert(HeadAttacks.Count == TailAttacks.Count, "Need same amount of head and tail attacks");
    }

    public IEnumerator Attack(bool headAttacksFromLeft, bool tailAttacksFromLeft)
    {
        for (int i = 0; i < HeadAttacks.Count; i++)
        {
            HeadAttacks[i].AttackFromLeft = headAttacksFromLeft;
            TailAttacks[i].AttackFromLeft = tailAttacksFromLeft;

            HeadAttacks[i].ProjectileOrigin = HeadPosition;
            TailAttacks[i].ProjectileOrigin = TailPosition;

            HeadAnimator.SetTrigger("Shoot");
            TailAnimator.SetBool("IsShooting", true);

            StartCoroutine(HeadAttacks[i].ShootProjectiles());
            yield return StartCoroutine(TailAttacks[i].ShootProjectiles());
            StopAllCoroutines();

            HeadAnimator.SetBool("IsShooting", false);
            TailAnimator.SetBool("IsShooting", false);


            yield return new WaitForSeconds(TimeBetweenAttacks);
        }
    }
}
