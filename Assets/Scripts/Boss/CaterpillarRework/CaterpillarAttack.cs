using System.Collections;
using UnityEngine;

public class CaterpillarAttack : AbstractAttack
{
    public void ShootProjectile(int index, Transform origin, bool attackFromLeft, AttackPattern attackPattern)
    {
        var projectile = InstantiateInPool(attackPattern.ProjectilePrefab, origin).GetComponent<Projectile>();
        var direction = 0f;

		switch (attackPattern.attackFunction)
		{
			case AttackFunction.Burst:
			{
				// Zufälliger Winkel zwischen start und end
				direction = UnityEngine.Random.Range(attackPattern.startAngle, attackPattern.endAngle);
				break;
			}
			case AttackFunction.Serie:
			{
				// Gleichmäßiger Winkel von start bis end
				direction = Mathf.Lerp(attackPattern.startAngle, attackPattern.endAngle, index / (attackPattern.ProjectilesToShoot - 1f));
				break;
			}
			case AttackFunction.Hunt:
			{
				// Richtung direkt auf den Spieler berechnen
				Vector2 toPlayer = (Player.Instance.transform.position - origin.position).normalized;
				projectile.FireProjectileLinear(toPlayer, attackPattern.ProjectileSpeed, attackPattern.ProjectileGravity);
				return;
			}
		}

        if (!attackFromLeft)
        {
            // flip angle on 90°
            direction = 90.0f + (90.0f - direction);
        }

		direction += UnityEngine.Random.Range(-attackPattern.ProjectileDeviation, attackPattern.ProjectileDeviation);
		projectile.FireProjectileAngle(direction, attackPattern.ProjectileSpeed, attackPattern.ProjectileGravity);
    }

    public override IEnumerator ShootProjectiles(Transform origin, bool attackFromLeft, AttackPattern attackPattern)
    {
        for (int i = 0; i < attackPattern.ProjectilesToShoot; i++)
        {
            ShootProjectile(i, origin, attackFromLeft, attackPattern);
            yield return new WaitForSeconds(attackPattern.SecondsBetweenProjectiles);
        }
    }
}
