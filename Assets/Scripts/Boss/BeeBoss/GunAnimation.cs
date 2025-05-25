using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    [SerializeField] private BeeBoss beeBoss;

    public void AttackAnimation() => beeBoss.GunAttackAnimation();
    public void IdleAnimation() => beeBoss.GunIdleAnimation();
}
