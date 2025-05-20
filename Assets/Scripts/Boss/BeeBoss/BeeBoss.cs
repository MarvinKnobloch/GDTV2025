using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BeeBoss : MonoBehaviour
{
    [Header("FlyInValues")]
    [SerializeField] private Transform flyInStart;
    [SerializeField] private float flyInSpeed;

    [Header("Attack")]
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackInterval;
    [SerializeField] private GameObject stingerPrefab;
    [SerializeField] private float attackAngle;
    private float attackTimer;

    [Header("SwitchSideValues")]
    [SerializeField] private Transform rightArenaPosition;
    [SerializeField] private Transform leftArenaPosition;
    [SerializeField] private Transform curvePosition;
    [SerializeField] private float switchSideSpeed;

    [Header("Values")]
    [SerializeField] private float timeBetweenActions;

    [Header("BeesPhaseTwo")]
    [SerializeField] private Transform[] beesSpawnPoints;
    [SerializeField] private GameObject beesPrefab;

    private Collider2D bossCollider;
    private Health health;
    private Vector2 currentStartPosition;
    private Vector2 currentEndPosition;
    private float currentFlySpeed;
    private bool isLeft;

    //CurceMovement;
    private Vector3 positionAB;
    private Vector3 positionBC;
    private float interpoleAmount;

    private float timer;

    public State state;

    public enum State
    {
        EnterFight,
        Idle,
        Attack,
        MoveToLeft,
        MoveToRight,
    }
    public NextAction nextAction;
    public enum NextAction
    {
        MoveToLeft,
        MoveToRight,
        Attack,
    }
    private void Awake()
    {
        health = GetComponent<Health>();
        bossCollider = GetComponent<Collider2D>();
        bossCollider.enabled = false;

        state = State.EnterFight;

        transform.position = flyInStart.position;
        currentEndPosition = rightArenaPosition.position;
        currentFlySpeed = flyInSpeed;
    }
    private void Update()
    {
        switch (state)
        {
            case State.EnterFight:
                MoveTowards();
                break;
            case State.Idle:
                WaitForNextMove();
                break;
            case State.MoveToLeft:
                CurceMovement();
                break;
            case State.MoveToRight:
                CurceMovement();
                break;
            case State.Attack:
                BossAttack();
                break;
        }
    }

    private void MoveTowards()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentEndPosition, currentFlySpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentEndPosition) < 0.5f)
        {
            switch (state)
            {
                case State.EnterFight:
                    GameManager.Instance.playerUI.ToggleBossHealth(true);
                    GameManager.Instance.playerUI.BossHealthUIUpdate(health.Value, health.MaxValue);
                    bossCollider.enabled = true;
                    nextAction = NextAction.Attack;
                    SwitchToIdle();
                    break;
            }
        }
    }
    private void CurceMovement()
    {
        interpoleAmount = interpoleAmount += switchSideSpeed * Time.deltaTime;
        positionAB = Vector3.Lerp(currentStartPosition, curvePosition.position, interpoleAmount);
        positionBC = Vector3.Lerp(curvePosition.position, currentEndPosition, interpoleAmount);

        transform.position = Vector3.Lerp(positionAB, positionBC, interpoleAmount);

        if (Vector2.Distance(transform.position, currentEndPosition) < 0.5f)
        {
            SwitchToIdle();
        }
    }
    private void BossAttack()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer >= attackInterval)
        {
            attackTimer = 0;
            GameObject prefab = PoolingSystem.SpawnObject(stingerPrefab, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);

            float randomAngle = Random.Range(-attackAngle, attackAngle);
            if(isLeft) prefab.transform.Rotate(0, 0, 210 + randomAngle, Space.World);  //right angle
            else prefab.transform.Rotate(0, 0, -30 + randomAngle, Space.World);       //left angle

            //prefab.transform.right = transform.right;
        }
        //bullet spawn

        timer += Time.deltaTime;
        if(timer >= attackDuration)
        {
            SwitchToIdle();
        }

    }
    private void WaitForNextMove()
    {
        timer += Time.deltaTime;
        if(timer > timeBetweenActions)
        {
            timer = 0;

            switch (nextAction)
            {
                case NextAction.MoveToLeft:
                    currentStartPosition = transform.position;
                    currentEndPosition = leftArenaPosition.position;
                    currentFlySpeed = switchSideSpeed;

                    nextAction = NextAction.Attack;
                    state = State.MoveToLeft;
                    break;
                case NextAction.MoveToRight:
                    currentStartPosition = transform.position;
                    currentEndPosition = rightArenaPosition.position;
                    currentFlySpeed = switchSideSpeed;

                    state = State.MoveToRight;
                    nextAction = NextAction.Attack;
                    break;
                case NextAction.Attack:

                    if(isLeft == false)
                    {
                        nextAction = NextAction.MoveToLeft;
                    }
                    else
                    {
                        nextAction = NextAction.MoveToRight;
                    }
                    isLeft = !isLeft;

                    SpawnBees();
                    state = State.Attack;
                    break;
            }
        }
    }
    private void SwitchToIdle()
    {
        interpoleAmount = 0;
        attackTimer = 0;
        timer = 0;

        state = State.Idle;
    }
    private void SpawnBees()
    {
        for (int i = 0; i < beesSpawnPoints.Length; i++)
        {
            GameObject prefab = PoolingSystem.SpawnObject(beesPrefab, beesSpawnPoints[i].position + Vector3.up * 3, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);

            prefab.GetComponent<ChargeBees>().SetBeeValues(beesSpawnPoints[i].position, attackDuration + timeBetweenActions + i);
        }
    }
}
