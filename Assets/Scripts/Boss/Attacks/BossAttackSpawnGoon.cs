using System;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackSpawnGoon : BossAttackBase
{
    public List<BossAttackGoonData> Goons;
    public float TimeBetweenSpawn = 2f;
    public float DashToSpawnSpeed = 1f;
    public bool MoveToSpawn = true;

    private float _timeSinceLastSpawn = 0f;
    private int _currentGoonIndex = 0;

    public override void ShootProjectile(int projectileIndex)
    {
        _timeSinceLastSpawn = 0f;
        _currentGoonIndex = 0;
    }

    public override void AttackMovement()
    {
        _timeSinceLastSpawn += Time.fixedDeltaTime;

        if (_timeSinceLastSpawn >= TimeBetweenSpawn)
        {
            SpawnGoon();
            _timeSinceLastSpawn = 0f;
            _currentGoonIndex = (_currentGoonIndex + 1) % Goons.Count;
        }

        if (!MoveToSpawn) return;

        transform.position = Vector3.Lerp(
            transform.position,
            Goons[_currentGoonIndex].SpawnPosition.position,
            DashToSpawnSpeed * Time.fixedDeltaTime
        );
    }

    private void SpawnGoon()
    {
        var goon = InstantiateInPool(Goons[_currentGoonIndex].GoonPrefab);
        goon.transform.SetPositionAndRotation(Goons[_currentGoonIndex].SpawnPosition.position, Quaternion.identity);
    }
}

[Serializable]
public struct BossAttackGoonData
{
    public GameObject GoonPrefab;
    public Transform SpawnPosition;
}
