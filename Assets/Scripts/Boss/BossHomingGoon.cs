using UnityEngine;

public class BossHomingGoon : MonoBehaviour
{
    public float MovementSpeed = 1f;

    private Health _health;

    void Start()
    {
        TryGetComponent(out _health);
        _health.dieEvent.AddListener(() => Destroy(gameObject));
    }

    void FixedUpdate()
    {
        var dx = Player.Instance.transform.position.x - transform.position.x;
        var x = Player.Instance.transform.position.x + (dx * 4);
        var y = Player.Instance.transform.position.y;

        transform.position = Vector2.MoveTowards(transform.position, new(x, y), MovementSpeed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Health>().PlayerTakeDamage(1, false);
            Destroy(gameObject);
        }
    }
}
