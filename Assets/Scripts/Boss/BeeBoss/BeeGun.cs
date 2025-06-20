using System.Collections;
using UnityEngine;

public class BeeGun : MonoBehaviour
{
    [SerializeField] private GameObject gunProjectile;
	[SerializeField] private float projectileSpeed;
    [SerializeField] private Transform projectileSpawnPosition;
    [SerializeField] private float gunUptime;
    [SerializeField] private float gunRotationSpeed;
    [SerializeField] private float topSpawnInterval;
    [SerializeField] private float sideSpawnInterval;
    [SerializeField] private float topAngleOffset;
    [SerializeField] private float sideAngleOffset;
    private float angleHalfed;
    private float sideAngleHalfed;
    private float timer;
    private float spawnInterval;

    private void Awake()
    {
        angleHalfed = topAngleOffset * 0.5f;
        sideAngleHalfed = sideAngleOffset * 0.5f;
    }
    private void OnEnable()
    {
        timer = 0;
        StartCoroutine(GunDisable());
    }
    public Position gunPosition;
    public enum Position
    {
        left,
        top,
        right,
    }
    void Update()
    {
        switch (gunPosition)
        {
            case Position.left:
                transform.parent.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * gunRotationSpeed, sideAngleOffset) - 310 - sideAngleHalfed);   //90
                break;
            case Position.top:
                transform.parent.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * gunRotationSpeed, topAngleOffset) - 335 - angleHalfed);  //180
                break;
            case Position.right:
                transform.parent.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * gunRotationSpeed, sideAngleOffset) - 45 - sideAngleHalfed);
                break;
        }

        timer += Time.deltaTime;
        if(timer > spawnInterval)
        {
            timer = 0;
            Projectile prefab = PoolingSystem.SpawnObject(gunProjectile, projectileSpawnPosition.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy).GetComponent<Projectile>();
			Vector2 direction;

            if(gunPosition == Position.left)
            {
                direction = -projectileSpawnPosition.right;
            }
            else
            {
                direction = projectileSpawnPosition.right;
            }
			prefab.FireProjectileLinear(direction, projectileSpeed);
        }
    }
    IEnumerator GunDisable()
    {
        yield return new WaitForSeconds(gunUptime);
        GunOff();
    }
    public void BulletSpawnInterval()
    {
        if (gunPosition == Position.top) spawnInterval = topSpawnInterval;
        else spawnInterval = sideSpawnInterval;
    }
    public void GunOff()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
