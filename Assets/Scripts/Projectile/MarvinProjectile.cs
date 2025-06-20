using System;
using System.Collections;
using UnityEngine;

public class MarvinProjectile : MonoBehaviour, IPoolingList
{
    private Rigidbody2D rb;
    private Vector2 direction;
    [NonSerialized] public Vector2 oldPosition;
    [NonSerialized] public float randomSpeed;

    [Header("ProjectileValues")]
    [SerializeField] private float lifetime = 2f;
    public float projectileSpeed;
    [SerializeField] private LayerMask collideLayer;

    [Header("EnemyValues")]
    [SerializeField] private LayerMask enemyHitLayer;
    [SerializeField] private int damage;

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        oldPosition = transform.position;
        StartCoroutine(ProjectileDisable());
    }
    private void FixedUpdate()
    {
        rb.linearVelocityY = 0;
        rb.transform.Translate(transform.right * (projectileSpeed + randomSpeed) * Time.deltaTime, Space.World);

        direction = ((Vector2)transform.position - oldPosition).normalized;
        oldPosition = transform.position;
        transform.right = direction;
    }
    private IEnumerator ProjectileDisable()
    {
        yield return new WaitForSeconds(lifetime);
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit Check
        if (Utility.LayerCheck(other, enemyHitLayer))
        {
            if (other.TryGetComponent(out Health health))
            {
                if (other.gameObject == Player.Instance.gameObject)
                {
                    health.PlayerTakeDamage(damage, false);
                }
                else
                {
                    health.EnemyTakeDamage(damage);
                }
            }
            PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
        }
        // Terrain Collision
        else if (Utility.LayerCheck(other, collideLayer))
        {
            PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
        }
    }
}

