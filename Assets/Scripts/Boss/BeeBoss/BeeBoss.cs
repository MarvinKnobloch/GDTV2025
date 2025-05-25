using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BeeBoss : MonoBehaviour, IGunAnimation
{
    [Header("FlyInValues")]
    [SerializeField] private Transform flyInStart;
    [SerializeField] private float flyInSpeed;
    [SerializeField] private float phase2StartDelay;

    [Header("Attack")]
    [SerializeField] private Transform attackSpawnPosition;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackInterval;
    [SerializeField] private GameObject stingerPrefab;
	[SerializeField] private float stingerSpeed = 15f;
    [SerializeField] private float attackAngle;
    [SerializeField] private float idleTimeAfterAttack;
    private float attackTimer;

    [Header("SwitchSideValues")]
    [SerializeField] private Transform rightArenaPosition;
    [SerializeField] private Transform leftArenaPosition;
    [SerializeField] private Transform curvePosition;
    [SerializeField] private float switchSideSpeed;
    [SerializeField] private float idleTimeAfterSwitchSide;
    private bool flipped;

    [Header("BeesPhaseTwo")]
    [SerializeField] private Transform[] beesSpawnPoints;
    [SerializeField] private GameObject beesPrefab;
    [SerializeField] private MovingWall movingWall;

    [Header("BossDefeat")]
    [SerializeField] private DialogObj boss2EndDialog;
    [SerializeField] private VoidEventChannel boss2EndEvent;

    private Collider2D bossCollider;
    private Health health;
    private Vector2 currentStartPosition;
    private Vector2 currentEndPosition;
    private float currentFlySpeed;
    private bool isLeft;
    private float timeBetweenActions;

    //CurceMovement;
    private Vector3 positionAB;
    private Vector3 positionBC;
    private float interpoleAmount;


    //Animations
    [Header("Animation")]
    [SerializeField] private Animator gunAnimator;
    private string currentstate;

    const string idleState = "Idle";
    const string chargeState = "Charge";
    const string attackState = "Attack";
    const string stopState = "Stop";

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
    private void OnEnable()
    {
        health.dieEvent.AddListener(OnDeath);
        boss2EndEvent.OnEventRaised += Boss2EndEvent;
    }
    private void OnDisable()
    {
        health.dieEvent.RemoveAllListeners();
        boss2EndEvent.OnEventRaised -= Boss2EndEvent;
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
                    SwitchToIdle(phase2StartDelay);
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

        if (interpoleAmount >= 0.5f && flipped == false)
        {
            flipped = true;
            Vector3 localScale;
            localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

        if (Vector2.Distance(transform.position, currentEndPosition) < 0.5f)
        {
            SwitchToIdle(idleTimeAfterSwitchSide);
        }
    }
    private void BossAttack()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer >= attackInterval)
        {
            attackTimer = 0;
            Projectile prefab = PoolingSystem.SpawnObject(stingerPrefab, attackSpawnPosition.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy).GetComponent<Projectile>();
            float randomAngle = Random.Range(-attackAngle, attackAngle);
            if(isLeft) randomAngle += 230;
            else randomAngle -= 50;

            prefab.FireProjectileAngle(randomAngle, stingerSpeed);
        }
        //bullet spawn

        timer += Time.deltaTime;
        if(timer >= attackDuration)
        {
            SpawnBees();
            SpawnWall();
            ChangeAnimationState(stopState);
            SwitchToIdle(idleTimeAfterAttack);
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

                    ChangeAnimationState(chargeState);
                    state = State.Attack;
                    break;
            }
        }
    }
    private void SwitchToIdle(float idleTime)
    {
        flipped = false;
        interpoleAmount = 0;
        attackTimer = 0;
        timer = 0;
        timeBetweenActions = idleTime;

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
    private void SpawnWall()
    {
        movingWall.SetWall();
    }
    private void OnDeath()
    {
        if (Player.Instance.playerIsDead) return;

        Player.Instance.bossDefeated = true;
        GameManager.Instance.menuController.gameIsPaused = true;
        Time.timeScale = 0;
        GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(boss2EndDialog);
        GameManager.Instance.playerUI.dialogBox.SetActive(true);
        GameManager.Instance.playerUI.ToggleBossHealth(false);
    }

    private void Boss2EndEvent()
    {
        int playerPref = 2;
        PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.BossDefeated.ToString(), playerPref);
        PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.ArenaEntranceDialog.ToString(), playerPref);

        GameManager.Instance.menuController.gameIsPaused = false;
        Time.timeScale = 1;

        GameManager.Instance.ShowVictoryScreen();

        StopAllCoroutines();
        CancelInvoke();
        gameObject.SetActive(false);
    }
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (gunAnimator == null) return;

        gunAnimator.CrossFadeInFixedTime(newstate, 0.1f);
    }

    public void AttackAnimation() => ChangeAnimationState(attackState);
    public void IdleAnimation() => ChangeAnimationState(idleState);

}
