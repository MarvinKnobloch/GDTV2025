using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class Boss3 : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float phase1IdleTime;
    [SerializeField] private float phase2IdleTime;
    private float timeBetweenActions;
    [SerializeField] private int phaseTreshold;
    [SerializeField] private Boss3Controller boss3Controller;

    [Header("BasicAttack")]
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
    [SerializeField] private int switchSideAttackBulletAmount;
    [SerializeField] private float switchSideAttackInterval;
    [SerializeField] private int switchSideAttackAngle;
    private bool flipped;

    [Header("FlyToPlatform")]
    [SerializeField] private float flyDownSpeed;
    [SerializeField] private Transform leftSideTop, leftSideBottom;
    [SerializeField] private Transform rightSideTop, rightSideBottom;
    [SerializeField] private float sideAttackInterval;
    [SerializeField] private int sideAttackCount;
    private int currentSideAttackCount;

    [Header("Phase2Transition")]
    [SerializeField] private float phase2FlyOutSpeed;
    [SerializeField] private float phase2TriggerTime;
    [SerializeField] private float phase2FlyInSpeed;
    [SerializeField] private Transform phase2StartPosition;
    [SerializeField] private float phase2FlySpeed;
    private bool phase2;
    private bool phase2Started;
    private int currentPhase2Action;
    private bool skipPhase2Stuff;
    private int phase2AttackCycle;

    [Header("Phase2BaseAttack")]
    [SerializeField] private float phase2BaseAttackInverval;
    [SerializeField] private int phase2BaseAttackCount;

    [Header("Phase2ChargeBeens")]
    [SerializeField] private float chargeBeesDelay;
    [SerializeField] private float timeBeetweenChargeBees;
    [SerializeField] private Boss3ChargeBee[] chargeBeesStack1;
    [SerializeField] private Boss3ChargeBee[] chargeBeesStack2;
    [SerializeField] private float chargeBeesSpeed;

    [Header("Phase2Shoot")]
    [SerializeField] private float phase2ShootDelay;
    [SerializeField] private int phase2ShootBulletAmount;
    [SerializeField] private int phase2ShootAngle;

    [Header("BossDefeat")]
    [SerializeField] private DialogObj boss3EndDialog;
    [SerializeField] private VoidEventChannel boss3EndEvent;

    private Health health;
    private Collider2D bossCollider;
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
        Idle,
        Attack,
        SwitchSide,
        FlyDown,
        SideAttack,
        FlyUp,
        FlyOutOfScreen,
        Empty,
        Phase2FlyIn,
        Phase2FlyDown,
        Phase2Idle,
        Phase2FlyUp,
    }
    private void Awake()
    {
        health = GetComponent<Health>();
        bossCollider = GetComponent<Collider2D>();
        transform.position = rightArenaPosition.position;
        timeBetweenActions = phase1IdleTime;
    }
    private void OnEnable()
    {
        health.dieEvent.AddListener(OnDeath);
        boss3EndEvent.OnEventRaised += Boss3EndEvent;
    }
    private void OnDisable()
    {
        health.dieEvent.RemoveListener(OnDeath);
        boss3EndEvent.OnEventRaised -= Boss3EndEvent;
    }
    private void Update()
    {
        switch (state)
        {
            case State.Empty:
                break;
            case State.Idle:
                WaitForNextMove();
                break;
            case State.SwitchSide:
                CurceMovement();
                break;
            case State.Attack:
                BossBaseAttack();
                break;
            case State.FlyDown:
                MoveTowards();
                break;
            case State.SideAttack:
                BossSideAttack();
                break;
            case State.FlyUp:
                MoveTowards();
                break;
            case State.FlyOutOfScreen:
                MoveTowards();
                break;
            case State.Phase2FlyIn:
                MoveTowards();
                break;
            case State.Phase2FlyDown:
                MoveTowards();
                Phase2BaseAttack();
                break;
            case State.Phase2FlyUp:
                MoveTowards();
                Phase2BaseAttack();
                break;
            case State.Phase2Idle:
                WaitForNextMove();
                break;
        }
    }
    private void WaitForNextMove()
    {
        timer += Time.deltaTime;
        if (timer > timeBetweenActions)
        {
            timer = 0;

            if(phase2 == false)
            {
                Phase1Actions();
            }
            else
            {
                Phase2Actions();
            }
        }
    }
    private void Phase1Actions()
    {
        int nextAttack = Random.Range(0, 3);
        //int nextAttack = UnityEngine.Random.Range(1, 2);

        if (nextAttack == 0)
        {
            state = State.Attack;
        }
        else if (nextAttack == 1)
        {
            if (isLeft) currentEndPosition = rightArenaPosition.position;
            else currentEndPosition = leftArenaPosition.position;
            currentStartPosition = transform.position;

            state = State.SwitchSide;
        }
        else if (nextAttack == 2)
        {
            int position = Random.Range(0, 2);
            if (isLeft)
            {
                if (position == 0) currentEndPosition = leftSideTop.position;
                else currentEndPosition = leftSideBottom.position;
            }
            else
            {
                if (position == 0) currentEndPosition = rightSideTop.position;
                else currentEndPosition = rightSideBottom.position;
            }
            currentStartPosition = transform.position;
            currentFlySpeed = flyDownSpeed;

            state = State.FlyDown;
        }
    }
    private void Phase2Actions()
    {
        if(currentPhase2Action == 0)
        {
            attackTimer = 10;
            currentPhase2Action++;
            currentEndPosition = rightSideBottom.position + Vector3.up * -1f;
            state = State.Phase2FlyDown;
        }
        else if(currentPhase2Action == 1)
        {
            attackTimer = 10;
            currentPhase2Action--;
            currentEndPosition = phase2StartPosition.position;
            state = State.Phase2FlyUp;
        }
        currentStartPosition = transform.position;
        currentFlySpeed = phase2FlySpeed;
    }
    private void MoveTowards()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentEndPosition, currentFlySpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentEndPosition) < 0.5f)
        {
            switch (state)
            {
                case State.FlyDown:
                    state = State.SideAttack;
                    break;
                case State.FlyUp:
                    SwitchToIdle(phase1IdleTime);
                    break;
                case State.FlyOutOfScreen:
                    state = State.Empty;
                    break;
                case State.Phase2FlyIn:
                    Phase2StartFight();
                    SwitchToIdle(phase1IdleTime);
                    break;
                case State.Phase2FlyDown:
                    if (phase2AttackCycle == 0)
                    {
                        boss3Controller.ToggleBeeCircle();
                    }
                    phase2AttackCycle++;
                    if (phase2AttackCycle >= 2) phase2AttackCycle = 0;
                    SwitchToIdle(phase2IdleTime);
                    break;
                case State.Phase2FlyUp:
                    SwitchToIdle(phase2IdleTime);
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

        if(interpoleAmount >= 0.5f && flipped == false)
        {
            flipped = true;
            Vector3 localScale;
            localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

        attackTimer += Time.deltaTime;
        if(attackTimer > switchSideAttackInterval)
        {
            attackTimer = 0;
            BossSwitchSideAttack();
        }

        if (Vector2.Distance(transform.position, currentEndPosition) < 0.5f)
        {
            isLeft = !isLeft;

            SwitchToIdle(phase1IdleTime);
        }
    }
    private void BossBaseAttack()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0;
            GameObject prefab = PoolingSystem.SpawnObject(stingerPrefab, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);

            float randomAngle = UnityEngine.Random.Range(-attackAngle, attackAngle);
            if (isLeft) prefab.transform.Rotate(0, 0, -30 + randomAngle, Space.World);
            else prefab.transform.Rotate(0, 0, 210 + randomAngle, Space.World);

            //prefab.transform.right = transform.right;
        }
        timer += Time.deltaTime;
        if (timer >= attackDuration)
        {
            SwitchToIdle(phase1IdleTime);
        }
    }
    private void BossSwitchSideAttack()
    {

        Vector3 targetDir = Player.Instance.gameObject.transform.position - transform.position;

        float angleEachBullet = switchSideAttackAngle / switchSideAttackBulletAmount;
        float startangle = switchSideAttackAngle * 0.5f;
        for (int i = 0; i < switchSideAttackBulletAmount; i++)
        {
            GameObject prefab = PoolingSystem.SpawnObject(stingerPrefab, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);

            prefab.transform.right = targetDir;
            prefab.transform.Rotate(0, 0, transform.rotation.z - startangle + angleEachBullet * i);
        }
    }
    private void BossSideAttack()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer >  attackInterval)
        {
            attackTimer = 0;
            currentSideAttackCount++;

            GameObject prefab = PoolingSystem.SpawnObject(stingerPrefab, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);

            if (isLeft) prefab.transform.Rotate(0, 0, 0, Space.World);  //right angle
            else prefab.transform.Rotate(0, 0, 180, Space.World);       //left angle
        }

        if (currentSideAttackCount >= sideAttackCount)
        {
            if (isLeft) currentEndPosition = leftArenaPosition.position;
            else currentEndPosition = rightArenaPosition.position;
            currentStartPosition = transform.position;

            state = State.FlyUp;
        }
    }

    private void SwitchToIdle(float idleTime)
    {
        timeBetweenActions = idleTime;

        flipped = false;
        currentSideAttackCount = 0;
        interpoleAmount = 0;
        attackTimer = 0;
        timer = 0;

        if (health.Value <= phaseTreshold && phase2 == false)
        {
            phase2 = true;
            bossCollider.enabled = false;
            boss3Controller.CancelPhase1Events();

            currentEndPosition = transform.position + Vector3.up * 20;
            currentFlySpeed = phase2FlyOutSpeed;

            GameManager.Instance.playerUI.ToggleBossHealth(false);
            StartCoroutine(TriggerPhase2Barrels());
            state = State.FlyOutOfScreen;
        }
        else
        {
            if (phase2 == true)
            {
                if(skipPhase2Stuff == false)
                {
                    skipPhase2Stuff = true;
                }
                else
                {
                    Invoke("ChargeBeesDelay", chargeBeesDelay);
                    StartCoroutine(Phase2ChargeBees());
                    StartCoroutine(Phase2Shoot());
                }
            }
            state = State.Idle;
        }
    }
    IEnumerator TriggerPhase2Barrels()
    {
        yield return new WaitForSeconds(phase2TriggerTime);
        boss3Controller.TriggerPhase2Barrels();
    }
    public void Phase2Start()
    {
        transform.position = phase2StartPosition.position + Vector3.right * 10;
        currentEndPosition = phase2StartPosition.position;
        currentFlySpeed = phase2FlyInSpeed;
        skipPhase2Stuff = false;

        if (isLeft)
        {
            isLeft = !isLeft;
            Vector3 localScale;
            localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        state = State.Phase2FlyIn;
    }
    public void Phase2StartFight()
    {
        phase2Started = true;
        GameManager.Instance.menuController.gameIsPaused = false;
        health.Value = health.MaxValue;
        GameManager.Instance.playerUI.BossHealthUIUpdate(health.Value, health.MaxValue);
        GameManager.Instance.playerUI.ToggleBossHealth(true);
        bossCollider.enabled = true;
        Player.Instance.playerCollider.enabled = true;
    }
    private void Phase2BaseAttack()
    {
        if (currentSideAttackCount >= phase2BaseAttackCount) return;
        
        attackTimer += Time.deltaTime;
        if (attackTimer > phase2BaseAttackInverval)
        {
            attackTimer = 0;
            currentSideAttackCount++;

            GameObject prefab = PoolingSystem.SpawnObject(stingerPrefab, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);

            //if (isLeft) prefab.transform.Rotate(0, 0, 0, Space.World);  //right angle
            prefab.transform.Rotate(0, 0, 180, Space.World);       //left angle
        }
    }
    private void ChargeBeesDelay()
    {
        StartCoroutine(Phase2ChargeBees());
    }
    IEnumerator Phase2ChargeBees()
    {
        SpawnChargeBees(chargeBeesStack1);
        yield return new WaitForSeconds(timeBeetweenChargeBees);
        SpawnChargeBees(chargeBeesStack2);
        yield return new WaitForSeconds(timeBeetweenChargeBees);
        SpawnChargeBees(chargeBeesStack1);
        yield return new WaitForSeconds(timeBeetweenChargeBees);
        SpawnChargeBees(chargeBeesStack2);
    }
    private void SpawnChargeBees(Boss3ChargeBee[] bees)
    {
        for (int i = 0; i < bees.Length; i++)
        {
            bees[i].gameObject.SetActive(true);
            bees[i].SetValues(chargeBeesSpeed);
        }
    }
    IEnumerator Phase2Shoot()
    {
        Phase2ShootLogic();
        yield return new WaitForSeconds(phase2ShootDelay);
        Phase2ShootLogic();
    }
    private void Phase2ShootLogic()
    {
        Vector3 targetDir = Player.Instance.gameObject.transform.position - transform.position;

        float angleEachBullet = phase2ShootAngle / phase2ShootBulletAmount;
        float startangle = phase2ShootAngle * 0.5f;
        for (int i = 0; i < phase2ShootBulletAmount; i++)
        {
            GameObject prefab = PoolingSystem.SpawnObject(stingerPrefab, transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);

            prefab.transform.right = targetDir;
            prefab.transform.Rotate(0, 0, transform.rotation.z - startangle + angleEachBullet * i);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.health.PlayerTakeDamage(1, false);
        }
    }
    private void OnDeath()
    {
        if (phase2Started == false) return;

        //Dialog
        GameManager.Instance.menuController.gameIsPaused = true;
        Time.timeScale = 0;
        GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(boss3EndDialog);
        GameManager.Instance.playerUI.dialogBox.SetActive(true);
        GameManager.Instance.playerUI.ToggleBossHealth(false);
        //VictorySound
    }
    private void Boss3EndEvent()
    {
        int playerPref = 3;
        PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.BossDefeated.ToString(), playerPref);
        PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.ArenaEntranceDialog.ToString(), playerPref);

        GameManager.Instance.menuController.gameIsPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene((int)GameScenes.AreaHub);
    }
}
