using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpPadStrength;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.JumpPad(jumpPadStrength);
        }
    }
}
