using System.Collections;
using UnityEngine;

public class BackToPool : MonoBehaviour, IPoolingList
{
    [SerializeField] private float lifeTime;
    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    private void OnEnable()
    {
        StartCoroutine(Deactivate());
    }
    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(lifeTime);
        PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
    }
}
