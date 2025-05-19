using UnityEngine;

public class BossAttackStraight : BossAttackBase
{
    public int ProjectileToUse = 0;

    public override void ShootProjectile(int projectileIndex)
    {
        var projectile = InstantiateInPool(ProjectilePrefabs[ProjectileToUse]);

        if (Player.Instance.transform.position.x > transform.position.x)
        {
            projectile.transform.right = Vector3.right;
        }
        else
        {
            projectile.transform.right = Vector3.left;
        }
    }
}
