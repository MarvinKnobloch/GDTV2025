using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackBase : MonoBehaviour
{
    public int ProjectilesToShoot = 1;
    public float SecondsBetweenProjectiles = 0.1f;
    public List<GameObject> ProjectilePrefabs = new();

    public virtual void ShootProjectile(int projectileIndex)
    { }

    public virtual void AttackMovement()
    { }

    public virtual void StartAttack()
    { }

    public virtual void FinishAttack()
    { }

    public IEnumerator ShootProjectiles()
    {
        for (int i = 0; i < ProjectilesToShoot; i++)
        {
            ShootProjectile(i);
            yield return new WaitForSeconds(SecondsBetweenProjectiles);
        }
    }

    protected GameObject InstantiateInPool(GameObject prefab)
    {
        return PoolingSystem.SpawnObject(prefab, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
    }
}
