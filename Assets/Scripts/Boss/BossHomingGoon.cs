using System.Collections;
using UnityEngine;

public class BossHomingGoon : MonoBehaviour, IPoolingList
{
    public float MovementSpeed = 1f;
    public float Lifetime = 10f;

    private Health _health;
    private float timer;

    private bool faceRight;

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
        var dx = Player.Instance.transform.position.x - transform.position.x;
        var x = Player.Instance.transform.position.x + (dx * 4);
        var y = Player.Instance.transform.position.y;

        transform.position = Vector2.MoveTowards(transform.position, new(x, y), MovementSpeed * Time.fixedDeltaTime);
        
        timer += Time.fixedDeltaTime;
        if(timer > 0.2f)
        {
            timer = 0;
            if (transform.position.x > Player.Instance.transform.position.x && faceRight == true) flip();
            if (transform.position.x < Player.Instance.transform.position.x && faceRight == false) flip();
        }
    }
    private void flip()
    {
        faceRight = !faceRight;
        Vector3 localScale;
        localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    IEnumerator ProjectileDisable()
    {
        yield return new WaitForSeconds(Lifetime);
        Die();
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
