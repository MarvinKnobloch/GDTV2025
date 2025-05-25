using UnityEngine;

[System.Serializable]
public struct AttackPattern
{
    [Header("Attack")]
    public int ProjectilesToShoot;
    public float SecondsBetweenProjectiles;
    public GameObject ProjectilePrefab;
	public float ProjectileSpeed;
	public float ProjectileGravity;

	public AttackFunction attackFunction;

    [Header("Streuung")]
    public float ProjectileDeviation;


	[Header("Winkel")]
    public float startAngle;
    public float endAngle;

	public float Duration => SecondsBetweenProjectiles * ProjectilesToShoot;
}

public enum AttackFunction
{
    Burst,
    Serie,
	Hunt,
}
