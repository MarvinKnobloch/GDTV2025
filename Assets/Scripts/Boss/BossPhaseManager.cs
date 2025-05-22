using System.Collections.Generic;
using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    public List<BossPhase> Phases = new();
    public int CurrentPhaseIndex = 0;

    public BossPhase CurrentPhase => Phases[CurrentPhaseIndex];
    public BossPhase NextPhase => CurrentPhaseIndex < Phases.Count - 1 ? Phases[CurrentPhaseIndex + 1] : null;

    public void CheckForTransition(Health health)
    {
        if (NextPhase != null
            && health.Value >= NextPhase.TransitionAtHealth
            && health.Value < CurrentPhase.TransitionAtHealth
        )
        {
            CurrentPhase.TransitionEvent?.Invoke();
            CurrentPhaseIndex++;
        }
    }
}
