using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BeeBoss : MonoBehaviour
{
    [Header("FlyInValues")]
    [SerializeField] private Transform flyInStart;
    [SerializeField] private float flyInSpeed;

    [Header("SwitchSideValues")]
    [SerializeField] private Transform rightArenaPosition;
    [SerializeField] private Transform leftArenaPosition;
    [SerializeField] private Transform curvePosition;
    [SerializeField] private float switchSideSpeed;

    [Header("Values")]
    [SerializeField] private float timeBetweenActions;

    private Collider2D bossCollider;
    private Health health;
    private Vector2 currentStartPosition;
    private Vector2 currentEndPosition;
    private float currentFlySpeed;
    private int currentAction;

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
            switch (state)
            {
                case State.MoveToLeft:
                    currentAction = 1;
                    SwitchToIdle();
                    break;
                case State.MoveToRight:
                    currentAction = 0;
                    SwitchToIdle();
                    break;
            }
        }
    }
    private void WaitForNextMove()
    {
        timer += Time.deltaTime;
        if(timer > timeBetweenActions)
        {
            timer = 0;

            if(currentAction == 0)
            {
                currentStartPosition = transform.position;
                currentEndPosition = leftArenaPosition.position;
                currentFlySpeed = switchSideSpeed;
                state = State.MoveToLeft;
            }
            else if(currentAction == 1)
            {
                currentStartPosition = transform.position;
                currentEndPosition = rightArenaPosition.position;
                currentFlySpeed = switchSideSpeed;
                state = State.MoveToRight;
            }
        }
    }
    private void SwitchToIdle()
    {
        interpoleAmount = 0;
        timer = 0;
        state = State.Idle;
    }
}
