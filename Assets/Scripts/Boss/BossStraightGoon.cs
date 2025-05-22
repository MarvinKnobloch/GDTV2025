using System.Collections;
using UnityEngine;

public class BossStraightGoon : MonoBehaviour, IPoolingList
{
    public float MovementSpeed = 1f;
    public float Lifetime = 10f;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    void OnEnable()
    {
        StartCoroutine(ProjectileDisable());
    }

    void OnDisable()
    {
        Die();
    }

    void FixedUpdate()
    {
        transform.position += MovementSpeed * Time.fixedDeltaTime * transform.right;
    }

    IEnumerator ProjectileDisable()
    {
        yield return new WaitForSeconds(Lifetime);
        Die();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Health>().PlayerTakeDamage(1, false);
            Die();
        }
    }

    void Die()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
