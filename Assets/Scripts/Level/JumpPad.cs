using System;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpPadStrength;

    [Header("Honey")]
    [SerializeField] private LayerMask HoneyLayer;
    [SerializeField] private float jumpPadReductionOnHoneyHit;
    [SerializeField] private float restoreEachJump = 6;
    private float jumpPadReduction;

    //Animations
    [NonSerialized] public Animator currentAnimator;
    [NonSerialized] public string currentstate;
    const string idleState = "Idle";
    const string activeState = "Active";

    private void Awake()
    {
        currentAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.JumpPad(jumpPadStrength - jumpPadReduction);

            ChangeAnimationState(activeState);
            RestoreStrength(restoreEachJump);
        }
        else if (Utility.LayerCheck(collision, HoneyLayer))
        {
            jumpPadReduction = jumpPadReductionOnHoneyHit;
            Destroy(collision.gameObject);
        }
    }
    public void RestoreStrength(float amount)
    {
        jumpPadReduction -= restoreEachJump;

        if (jumpPadReduction <= 0) jumpPadReduction = 0;
    }

    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (currentAnimator == null) return;

        currentAnimator.CrossFadeInFixedTime(newstate, 0.1f);
    }
    public void BackToIdle()
    {
        ChangeAnimationState(idleState);
    }
}
