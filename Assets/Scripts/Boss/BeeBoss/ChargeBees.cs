using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using static BeeBoss;

public class ChargeBees : MonoBehaviour, IPoolingList
{
    [SerializeField] private float flyInSpeed;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeTime;

    private Vector2 currentEndPosition;
    private float currentFlySpeed;
    private Vector2 chargeDirection;
    private float timer;
    private float timeUntilCharge;

    //Animations
    [NonSerialized] public Animator currentAnimator;
    [NonSerialized] public string currentstate;
    const string idleState = "Idle";
    const string chargeState = "Charge";
    public PoolingSystem.PoolObjectInfo poolingList { get; set; }

    public State state;

    public enum State
    {
        EnterFight,
        Idle,
        Charge,
    }
    private void Awake()
    {
        currentAnimator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        ChangeAnimationState(idleState);
    }
    private void Update()
    {
        switch (state)
        {
            case State.EnterFight:
                MoveTowards();
                break;
            case State.Idle:
                break;
            case State.Charge:
                ChargeMovement();
                break;
        }
    }
    public void SetBeeValues(Vector2 endPosition, float chargeDelay)
    {
        currentEndPosition = endPosition;
        currentFlySpeed = flyInSpeed;
        timeUntilCharge = chargeDelay;

        StartCoroutine(TriggerCharge());
        state = State.EnterFight;
        
    }
    private void MoveTowards()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentEndPosition, currentFlySpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentEndPosition) < 0.5f)
        {
            switch (state)
            {
                case State.EnterFight:
                    state = State.Idle;
                    break;
            }
        }
    }
    IEnumerator TriggerCharge()
    {
        yield return new WaitForSeconds(timeUntilCharge);
        SetCharge();
        state = State.Charge;
    }
    private void SetCharge()
    {
        chargeDirection = (Player.Instance.transform.position - transform.position).normalized;

        if (transform.position.x > Player.Instance.transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.right = -chargeDirection;
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.right = chargeDirection;
        }

        ChangeAnimationState(chargeState);
        timer = 0;
    }
    private void ChargeMovement()
    {
        transform.Translate(chargeDirection * chargeSpeed * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        if (timer > chargeTime)
        {
            PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.health.PlayerTakeDamage(1, false);

            StopAllCoroutines();
            PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
        }
    }

    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (currentAnimator == null) return;

        currentAnimator.CrossFadeInFixedTime(newstate, 0.1f);
    }
}
