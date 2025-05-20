using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossPhase : MonoBehaviour
{
    public List<BossAttackBase> BossAttacks = new();
    public float TimeBetweenAttacks = 1f;
    public int TransitionAtHealth = 1;
    public UnityEvent TransitionEvent;


}
