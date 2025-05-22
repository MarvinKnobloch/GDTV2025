using UnityEngine;

public class Boss_Caterpillar_Tail : BossBase
{
    void Start()
    {
    }

    public void ExecuteAttackInPhase(int phaseIndex, int attackIndex)
    {
        var phase = PhaseManager.Phases[phaseIndex];
        _currentAttack = phase.BossAttacks[attackIndex];
        _currentAttack.StartAttack();
        StartCoroutine(_currentAttack.ShootProjectiles());
    }

    public void FinishAttack()
    {
        _currentAttack.FinishAttack();
    }
}
