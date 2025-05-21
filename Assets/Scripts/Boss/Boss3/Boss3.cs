using UnityEngine;

public class Boss3 : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float timeBetweenActions;

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
    private bool fliped;

    [Header("FlyToPlatform")]
    [SerializeField] private float flyDownSpeed;
    [SerializeField] private Transform leftSideTop, leftSideBottom;
    [SerializeField] private Transform rightSideTop, rightSideBottom;
    [SerializeField] private float sideAttackInterval;
    [SerializeField] private int sideAttackCount;
    private int currentSideAttackCount;

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
        Idle,
        Attack,
        SwitchSide,
        FlyDown,
        SideAttack,
        FlyUp,
    }
    private void Awake()
    {
        health = GetComponent<Health>();
        transform.position = rightArenaPosition.position;
    }
    private void Update()
    {
        switch (state)
        {
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
        }
    }
    private void WaitForNextMove()
    {
        timer += Time.deltaTime;
        if (timer > timeBetweenActions)
        {
            timer = 0;
            int nextAttack = UnityEngine.Random.Range(0, 3);
            //int nextAttack = UnityEngine.Random.Range(1, 2);

            if(nextAttack == 0)
            {
                state = State.Attack;
            }
            else if(nextAttack == 1)
            {
                if (isLeft) currentEndPosition = rightArenaPosition.position;
                else currentEndPosition = leftArenaPosition.position;
                currentStartPosition = transform.position;

                state = State.SwitchSide;
            }
            else if(nextAttack == 2)
            {
                int position = UnityEngine.Random.Range(0, 2);
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

        if(interpoleAmount >= 0.5f && fliped == false)
        {
            fliped = true;
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

            SwitchToIdle();
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
            SwitchToIdle();
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

    private void SwitchToIdle()
    {
        fliped = false;
        currentSideAttackCount = 0;
        interpoleAmount = 0;
        attackTimer = 0;
        timer = 0;

        state = State.Idle;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.health.PlayerTakeDamage(1, false);
        }
    }
}
