using UnityEngine;

public class BossStraightGoon : MonoBehaviour, IPoolingList
{
    public float MovementSpeed = 1f;

    private Health _health;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    void OnEnable()
    {
        TryGetComponent(out _health);
        _health.Value = _health.MaxValue;
        _health.dieEvent.AddListener(() => Die());
    }

    void OnDisable()
    {
        _health.dieEvent.RemoveAllListeners();
    }

    void FixedUpdate()
    {
        transform.position += MovementSpeed * Time.fixedDeltaTime * transform.right;
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
