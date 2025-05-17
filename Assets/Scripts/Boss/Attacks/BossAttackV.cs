using UnityEngine;

public class BossAttackV : BossAttackBase
{
    [Header("Movement")]
    public Transform TopWaypoint;
    public Transform BottomWaypoint;
    public float MovementSpeed = 1f;

    public override void ShootProjectile(int projectileIndex)
    {
        var projectileDown = InstantiateInPool(ProjectilePrefabs[0]);
        var projectileUp = InstantiateInPool(ProjectilePrefabs[0]);
        projectileDown.transform.right = new Vector3(-1, -0.5f, 0);
        projectileUp.transform.right = new Vector3(-1, 0.5f, 0);
    }

    public override void AttackMovement()
    {
        var pos = transform.position;
        pos.y = BottomWaypoint.position.y + Mathf.PingPong(Time.time * MovementSpeed, TopWaypoint.position.y);
        pos.y = Mathf.Lerp(transform.position.y, pos.y, Time.fixedDeltaTime * MovementSpeed / 2f);
        pos.x = Mathf.Lerp(transform.position.x, BottomWaypoint.position.x, Time.fixedDeltaTime * MovementSpeed);
        transform.position = pos;
    }
}
