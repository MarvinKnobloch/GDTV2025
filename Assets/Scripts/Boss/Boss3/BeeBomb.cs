using UnityEngine;

public class BeeBomb : MonoBehaviour, IPoolingList

{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius;
    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utility.LayerCheck(collision, hitLayer))
        {
            collision.gameObject.SetActive(false);

            GameObject prefab = PoolingSystem.SpawnObject(explosionPrefab, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
            prefab.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1);

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayer);

            if (cols.Length != 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if(cols[i].gameObject == Player.Instance.gameObject)
                    {
                        Player.Instance.health.PlayerTakeDamage(1, false);
                    }
                }
            }
            PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
        }
    }
}
