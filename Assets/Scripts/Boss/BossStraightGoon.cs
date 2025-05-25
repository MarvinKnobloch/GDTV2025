using System.Collections;
using UnityEditor;
using UnityEngine;

public class BossStraightGoon : MonoBehaviour, IPoolingList
{
    public float MovementSpeed = 1f;
    public float Lifetime = 10f;
    public float PuddleSpawnDeviation = 0.1f;
    public GameObject PuddlePrefab;
    public Vector2 PuddleLaunchVelocity = new(0, 1f);

    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    void OnEnable()
    {
        RaycastHit2D hit;
        do
        {
            hit = Physics2D.Raycast(
                transform.position,
                Vector2.right,
                100f,
                LayerMask.GetMask("Puddles")
            );

            if (hit.collider != null)
            {
                var hitGameObject = hit.collider.gameObject;

                if (hitGameObject.transform.parent != null)
                {
                    Destroy(hitGameObject.transform.parent.gameObject);
                }
                else
                {
                    Destroy(hitGameObject);
                }
            }
        } while (hit.collider != null);

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
        }

        if (collision.gameObject.CompareTag("Puddle"))
        {
            StartCoroutine(SpawnPuddle());
        }
    }

    IEnumerator SpawnPuddle()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, PuddleSpawnDeviation));

        var puddle = Instantiate(PuddlePrefab, transform.position, Quaternion.identity);
        puddle.GetComponent<Rigidbody2D>().linearVelocity = PuddleLaunchVelocity;
    }

    void Die()
    {
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
