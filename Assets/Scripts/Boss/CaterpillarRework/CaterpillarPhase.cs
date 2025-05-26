using System.Collections;
using UnityEngine;

public class CaterpillarPhase : MonoBehaviour
{
    public CaterpillarPhaseSO[] phases;

    public Transform HeadPosition;
    public Transform TailPosition;

    public AbstractAttack attacker;

    public Animator HeadAnimator { private get; set; }
    public Animator TailAnimator { private get; set; }

    private int phaseIndex = 0;

    public IEnumerator Attack(CaterpillarManager manager, bool headAttacksFromLeft, bool tailAttacksFromLeft)
    {
        var phase = phases[phaseIndex];

        for (int i = 0; i < phase.HeadAttacks.Length; i++)
        {
            HeadAnimator.SetBool("IsShooting", true);
            TailAnimator.SetBool("IsShooting", true);

            StopAllCoroutines();
            var firstAttack = StartAttack(HeadPosition, headAttacksFromLeft, phase.HeadAttacks[i]);
            var secondAttack = StartAttack(TailPosition, tailAttacksFromLeft, phase.TailAttacks[i]);

            yield return firstAttack;
            HeadAnimator.SetBool("IsShooting", false);
            yield return secondAttack;
            TailAnimator.SetBool("IsShooting", false);

            yield return new WaitForSeconds(phase.TimeBetweenAttacks);
        }

        PhaseTransition(manager);
    }

    private void PhaseTransition(CaterpillarManager manager)
    {
        var transitionAtHealth = Mathf.RoundToInt(phases[phaseIndex].TransitionAtHealth * (PlayerPrefs.GetFloat("BossHealthMultiplier") * 0.01f));
        if (manager.Health.Value <= transitionAtHealth)
        {
            StopAllCoroutines();
            phaseIndex++;
        }
    }

    private Coroutine StartAttack(Transform origin, bool attackFromLeft, AttackPattern attackPattern)
    {
        return StartCoroutine(attacker.ShootProjectiles(origin, attackFromLeft, attackPattern));
    }
}
