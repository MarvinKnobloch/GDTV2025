using System;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    [SerializeField] private GameObject boss;

    public void AttackAnimation() => boss.GetComponent<IGunAnimation>().AttackAnimation();
    public void IdleAnimation() => boss.GetComponent<IGunAnimation>().IdleAnimation();
}
