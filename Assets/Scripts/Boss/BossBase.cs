using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour
{
    [Header("Combat")]
    public List<BossAttackBase> bossAttacks = new List<BossAttackBase>();
    public float TimeBetweenAttacks = 1f;

    [Header("Movement")]
    public Transform TopWaypoint;
    public Transform BottomWaypoint;
    public float MovementSpeed = 1f;

    private bool _attacking = true;

    void Start()
    {
        StartCoroutine(AttackCoroutine());
    }

    void FixedUpdate()
    {
        var pos = transform.position;
        pos.y = BottomWaypoint.position.y + Mathf.PingPong(Time.time * MovementSpeed, TopWaypoint.position.y);
        transform.position = pos;
    }

    void OnDestroy()
    {
        _attacking = false;
    }

    void OnDisable()
    {
        _attacking = false;
    }

    IEnumerator AttackCoroutine()
    {
        while (_attacking)
        {
            for (int i = 0; i < bossAttacks.Count; i++)
            {
                yield return StartCoroutine(bossAttacks[i].ShootProjectiles());
                yield return new WaitForSeconds(TimeBetweenAttacks);
            }
        }
    }
}
