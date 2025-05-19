using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpPadStrength;

    [Header("Honey")]
    [SerializeField] private LayerMask HoneyLayer;
    [SerializeField] private float jumpPadReductionOnHoneyHit;
    [SerializeField] private float restoreEachJump = 6;
    private float jumpPadReduction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.JumpPad(jumpPadStrength - jumpPadReduction);

            RestoreStrength(restoreEachJump);
        }
        else if (Utility.LayerCheck(collision, HoneyLayer))
        {
            jumpPadReduction = jumpPadReductionOnHoneyHit;
        }
    }
    public void RestoreStrength(float amount)
    {
        jumpPadReduction -= restoreEachJump;

        if (jumpPadReduction <= 0) jumpPadReduction = 0;
    }
}
