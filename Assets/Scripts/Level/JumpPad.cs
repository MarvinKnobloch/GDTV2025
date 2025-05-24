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

    [NonSerialized] private HoneyPad honeyPad;
    [NonSerialized] private SpriteRenderer honeySpriteRenderer;
    private Color honeyColor;

    private void Awake()
    {
        currentAnimator = GetComponent<Animator>();
        honeyPad = GetComponentInChildren<HoneyPad>();
        honeySpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        honeyColor = honeySpriteRenderer.color;
        honeyColor.a = 0;
        honeySpriteRenderer.color = honeyColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.JumpPad(jumpPadStrength - jumpPadReduction);

            ChangeAnimationState(activeState);
            honeyPad.ChangeAnimationState(activeState);
            RestoreStrength(restoreEachJump);
        }
        else if (Utility.LayerCheck(collision, HoneyLayer))
        {
            honeyColor.a = 1;
            honeySpriteRenderer.color = honeyColor;

            jumpPadReduction = jumpPadReductionOnHoneyHit;
            Destroy(collision.gameObject);
        }
    }
    public void RestoreStrength(float amount)
    {
        jumpPadReduction -= restoreEachJump;

        if (jumpPadReduction <= 0)
        {
            honeyColor.a = 0;
            honeySpriteRenderer.color = honeyColor;
            jumpPadReduction = 0;
        }
        else
        {
            honeyColor.a = 0.5f;
            honeySpriteRenderer.color = honeyColor;
        }
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
        honeyPad.ChangeAnimationState(idleState);
    }
}
