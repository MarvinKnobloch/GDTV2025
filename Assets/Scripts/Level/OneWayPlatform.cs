using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.currentOneWayPlatform = gameObject;

            if (Player.Instance.playerVelocity.y > 0)
            {
                gameObject.layer = 11;
                platformCollider.isTrigger = true;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.currentOneWayPlatform = null;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.currentOneWayPlatform = null;
            gameObject.layer = 10;
            platformCollider.isTrigger = false;
        }
    }
}
