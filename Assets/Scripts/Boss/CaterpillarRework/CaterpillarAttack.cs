using System;
using System.Collections;
using UnityEngine;

public class CaterpillarAttack : MonoBehaviour
{
    [Header("Attack")]
    [NonSerialized] public bool AttackFromLeft = true;
    [NonSerialized] public Transform ProjectileOrigin;
    public int ProjectilesToShoot = 1;
    public float SecondsBetweenProjectiles = 0.1f;
    public GameObject ProjectilePrefab;
	public float ProjectileSpeed = 20f;
	public float ProjectileGravity = 1.0f;

    [Header("Projectile")]
    public float ProjectileDeviation = 0.1f;

    public void ShootProjectile()
    {
        var projectile = InstantiateInPool(ProjectilePrefab).GetComponent<Projectile>();
        var direction = Vector3.right;

        if (!AttackFromLeft)
        {
            direction = Vector3.left;
        }

		direction += new Vector3(0, UnityEngine.Random.Range(-ProjectileDeviation, ProjectileDeviation), 0);
		projectile.FireProjectileLinear(direction, ProjectileSpeed, ProjectileGravity);
    }

    public IEnumerator ShootProjectiles()
    {
        for (int i = 0; i < ProjectilesToShoot; i++)
        {
            ShootProjectile();
            yield return new WaitForSeconds(SecondsBetweenProjectiles);
        }
    }

    protected GameObject InstantiateInPool(GameObject prefab)
    {
        return PoolingSystem.SpawnObject(prefab, ProjectileOrigin.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
    }
}
