using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackDirection : BossAttackBase
{
    public GameObject Melee;

    [Header("Movement")]
    public List<Transform> TargetPoints;
    public float TimeBetweenDash = 2f;
    public float DashSpeed = 1f;
    public float MeleeActiveDuration = 0.5f;

    private float _timeSinceLastDash = 0f;
    private int _currentTargetIndex = 0;

    public override void ShootProjectile(int projectileIndex)
    {
        StartCoroutine(MeleeCoroutine());
        _timeSinceLastDash = 0f;
        _currentTargetIndex = 0;
    }

    public override void AttackMovement()
    {
        _timeSinceLastDash += Time.fixedDeltaTime;

        if (_timeSinceLastDash >= TimeBetweenDash)
        {
            StartCoroutine(MeleeCoroutine());
            _timeSinceLastDash = 0f;
            _currentTargetIndex = (_currentTargetIndex + 1) % TargetPoints.Count;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            TargetPoints[_currentTargetIndex].position,
            DashSpeed * Time.fixedDeltaTime
        );
    }

    IEnumerator MeleeCoroutine()
    {
        Melee.SetActive(true);
        yield return new WaitForSeconds(MeleeActiveDuration);
        Melee.SetActive(false);
    }
}
