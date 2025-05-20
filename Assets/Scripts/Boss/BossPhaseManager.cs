using System.Collections.Generic;
using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    public List<BossPhase> Phases = new();

    private int _currentPhaseIndex = 0;

    public BossPhase CurrentPhase => Phases[_currentPhaseIndex];
    public BossPhase NextPhase => _currentPhaseIndex < Phases.Count - 1 ? Phases[_currentPhaseIndex + 1] : null;

    public void CheckForTransition(Health health)
    {
        if (NextPhase != null
            && health.Value >= NextPhase.TransitionAtHealth
            && health.Value < CurrentPhase.TransitionAtHealth
        )
        {
            CurrentPhase.TransitionEvent?.Invoke();
            _currentPhaseIndex++;
        }
    }
}
