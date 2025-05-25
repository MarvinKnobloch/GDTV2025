using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolingList
{
    public float projectileSpeed;

    [Header("ProjectileValues")]
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private LayerMask collideLayer;

    [Header("EnemyValues")]
    [SerializeField] private LayerMask enemyHitLayer;
    [SerializeField] private int damage;

    private Rigidbody2D rb;
    private Vector2 direction;

	public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	public void FireProjectileLinear(Vector2 direction, float speed, float gravity = 0.0f)
	{
		this.direction = direction.normalized;
		transform.right = direction;
		this.rb.linearVelocity = direction * speed;
		this.rb.gravityScale = gravity;
	}

	public void FireProjectileAngle(float direction, float speed, float gravity = 0.0f)
	{
		var radians = Mathf.Deg2Rad * direction;
		this.direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
		transform.right = this.direction;
		this.rb.linearVelocity = this.direction * speed;
		this.rb.gravityScale = gravity;
	}

    private void OnEnable()
    {
        StartCoroutine(ProjectileDisable());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void FixedUpdate()
    {
        //rb.linearVelocityY = 0;
        //rb.transform.Translate(transform.right * (projectileSpeed + randomSpeed) * Time.deltaTime, Space.World);

		direction = rb.linearVelocity.normalized;
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
