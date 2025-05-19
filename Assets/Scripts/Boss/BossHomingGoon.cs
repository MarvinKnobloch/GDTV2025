using UnityEngine;

public class BossHomingGoon : MonoBehaviour, IPoolingList
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
        var dx = Player.Instance.transform.position.x - transform.position.x;
        var x = Player.Instance.transform.position.x + (dx * 4);
        var y = Player.Instance.transform.position.y;

        transform.position = Vector2.MoveTowards(transform.position, new(x, y), MovementSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
