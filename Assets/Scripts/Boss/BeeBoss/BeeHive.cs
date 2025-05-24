using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeeHive : MonoBehaviour
{
    [SerializeField] private GameObject beePrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int spawnAmount;
    [SerializeField] private float colorChangeTime;

    private Health health;
    private SpriteRenderer hiveImage;

    [Space]
    [SerializeField] private Transform[] spawnPositions;

    [SerializeField] private BeeBossController beeBossController;
    [SerializeField] private int hiveNumber;

    private void Start()
    {
        InvokeRepeating("SpawnBees", 2, spawnInterval);

        health = GetComponent<Health>();
        if (health != null)
        {
            health.hitEvent.AddListener(HitEffect);
            health.dieEvent.AddListener(OnDeath); 
        }

        hiveImage = GetComponent<SpriteRenderer>();
    }
    public void SpawnBees()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject prefab = PoolingSystem.SpawnObject(beePrefab, spawnPositions[i].position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
        }
    }
    private void HitEffect()
    {
        StartCoroutine(ChangeColor());
    }
    IEnumerator ChangeColor()
    {
        hiveImage.color = Color.red;
        yield return new WaitForSeconds(colorChangeTime);
        hiveImage.color = Color.white;
    }
    private void OnDeath()
    {
        health.hitEvent.RemoveAllListeners();
        health.dieEvent.RemoveAllListeners();
        CancelInvoke();
        beeBossController.TriggerBossAndHoney(hiveNumber);
        Destroy(gameObject);
    } 
}
