using System.Collections;
using UnityEngine;

public class BossStraightGoon : MonoBehaviour, IPoolingList
{
    public float MovementSpeed = 1f;
    public float Lifetime = 10f;

    private Health _health;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    void OnEnable()
    {
        TryGetComponent(out _health);
        _health.Value = _health.MaxValue;
        _health.dieEvent.AddListener(() => Die());
        StartCoroutine(ProjectileDisable());
    }

    void OnDisable()
    {
        _health.dieEvent.RemoveAllListeners();
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

    void OnCollisionEnter2D(Collision2D collision)
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
