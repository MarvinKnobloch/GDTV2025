using System;
using UnityEngine;

public class HoneyPad : MonoBehaviour
{
    //Animations
    [NonSerialized] public Animator currentAnimator;
    [NonSerialized] public string currentstate;

    private void Awake()
    {
        currentAnimator = GetComponent<Animator>();
    }
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (currentAnimator == null) return;

        currentAnimator.CrossFadeInFixedTime(newstate, 0.1f);
    }
}
