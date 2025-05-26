using UnityEngine;

public class BeeBarrel : MonoBehaviour, IPoolingList
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius;
    [SerializeField] private GameObject explosionProjectiles;
    [SerializeField] private int explosionProjectilesCount;
    [SerializeField] private float projectileRandomSpeed;
    [SerializeField] private float projectileRandomAngle;
    [SerializeField] private float minAngle;

    private bool hitPlayer;
    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    private void OnEnable()
    {
        hitPlayer = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utility.LayerCheck(collision, hitLayer))
        {
            GameObject prefab = PoolingSystem.SpawnObject(explosionPrefab, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
            prefab.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1);

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, explosionRadius, hitLayer);

            if (cols.Length != 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].gameObject == Player.Instance.gameObject)
                    {
                        hitPlayer = true;
                        Player.Instance.health.PlayerTakeDamage(1, false);
                        break;
                    }
                }
            }

            if(hitPlayer == false && explosionProjectilesCount > 0)
            {
                hitPlayer = true; //because two explosions got triggered

                for (int i = 0; i < explosionProjectilesCount; i++)
                {

                    GameObject barrelprojectiles = PoolingSystem.SpawnObject(explosionProjectiles, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);

                    float randomSpeed = Random.Range(-projectileRandomSpeed, projectileRandomSpeed);
                    barrelprojectiles.GetComponent<MarvinProjectile>().randomSpeed = randomSpeed;

                    float randomAngle = Random.Range(-projectileRandomAngle, projectileRandomAngle);
                    if (randomAngle >= 0)
                    {
                        if (randomAngle < minAngle) randomAngle = minAngle;
                    }
                    else
                    {
                        if (randomAngle > -minAngle) randomAngle = -minAngle;
                    }
                    barrelprojectiles.transform.Rotate(0, 0, 90 + randomAngle);

                    //barrelprojectiles.transform.right = transform.up;
                }
            }

            PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
        }
    }
}
