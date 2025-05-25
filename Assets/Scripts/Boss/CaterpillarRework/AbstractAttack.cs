using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractAttack : MonoBehaviour
{
    public abstract IEnumerator ShootProjectiles(Transform origin, bool attackFromLeft, AttackPattern attackPattern);

    protected GameObject InstantiateInPool(GameObject prefab, Transform origin)
    {
        return PoolingSystem.SpawnObject(prefab, origin.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
    }
}