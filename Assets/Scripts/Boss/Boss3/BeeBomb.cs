using UnityEngine;

public class BeeBomb : MonoBehaviour, IPoolingList

{
    [SerializeField] private LayerMask hitLayer;
    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Utility.LayerCheck(collision, hitLayer))
        {
            collision.gameObject.SetActive(false);
            PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
        }

    }

}
