using UnityEngine;

public class BossAttackV : BossAttackBase
{
    public override void ShootProjectile(int projectileIndex)
    {
        var projectileDown = InstantiateInPool(ProjectilePrefabs[0]);
        var projectileUp = InstantiateInPool(ProjectilePrefabs[0]);
        projectileDown.transform.right = new Vector3(-1, -0.5f, 0);
        projectileUp.transform.right = new Vector3(-1, 0.5f, 0);
    }
}
