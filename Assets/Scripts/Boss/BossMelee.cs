using UnityEngine;

public class BossMelee : MonoBehaviour
{
    public int Damage = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().PlayerTakeDamage(Damage, false);
        }
    }
}
