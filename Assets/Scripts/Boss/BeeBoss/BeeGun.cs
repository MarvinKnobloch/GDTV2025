using System.Collections;
using UnityEngine;

public class BeeGun : MonoBehaviour
{
    [SerializeField] private GameObject gunProjectile;
    [SerializeField] private Transform projectileSpawnPosition;
    [SerializeField] private float gunUptime;
    [SerializeField] private float gunRotationSpeed;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float angleOffset;
    private float angleHalfed;
    private float timer;

    private void Awake()
    {
        angleHalfed = angleOffset * 0.5f;
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
                transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * gunRotationSpeed, angleOffset) - 90 - angleHalfed);
                break;
            case Position.top:
                transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * gunRotationSpeed, angleOffset) - 180 - angleHalfed);
                break;
            case Position.right:
                transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * gunRotationSpeed, angleOffset) - 270 - angleHalfed);
                break;
        }

        timer += Time.deltaTime;
        if(timer > spawnInterval)
        {
            timer = 0;
            GameObject prefab = PoolingSystem.SpawnObject(gunProjectile, projectileSpawnPosition.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
            prefab.transform.right = transform.up;
        }
    }
    IEnumerator GunDisable()
    {
        yield return new WaitForSeconds(gunUptime);
        GunOff();
    }
    public void GunOff()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

}
