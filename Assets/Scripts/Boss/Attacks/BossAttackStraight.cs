using UnityEngine;

public class BossAttackStraight : BossAttackBase
{
    public override void ShootProjectile(int projectileIndex)
    {
        var projectile = InstantiateInPool(ProjectilePrefabs[0]);
        projectile.transform.right = Vector3.left;
    }
}
