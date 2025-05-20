using UnityEngine;

public class BeeHive : MonoBehaviour
{
    [SerializeField] private GameObject beePrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int spawnAmount;

    private Health health;

    [SerializeField] private Transform[] spawnPositions;

    [SerializeField] private BeeBossController beeBossController;
    [SerializeField] private int hiveNumber;

    private void Start()
    {
        InvokeRepeating("SpawnBees", 2, spawnInterval);

        health = GetComponent<Health>();
        if (health != null) health.dieEvent.AddListener(OnDeath);
    }
    public void SpawnBees()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject prefab = PoolingSystem.SpawnObject(beePrefab, spawnPositions[i].position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
        }
    }
    private void OnDeath()
    {
        health.dieEvent.RemoveAllListeners();
        CancelInvoke();
        beeBossController.TriggerBossAndHoney(hiveNumber);
        Destroy(gameObject);
    }
}
