using System.Collections;
using UnityEngine;
using static BeeBoss;

public class BeeCarry : MonoBehaviour
{
    [SerializeField] private float flySpeed;
    [SerializeField] private float idleTime;

    [SerializeField] private Transform leftSpawn, rightSpawn;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform prefabSpawnPosition;

    private Vector3 currentEndPosition;
    private float timer;
    private bool spawnLeft;
    private Vector3 baseScale;

    public State state;
    public enum State
    {
        FlyIn,
        Idle,
        FlyOut,
    }
    private void Awake()
    {
        baseScale = transform.localScale;
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        prefabSpawnPosition.gameObject.SetActive(true);
    }

    void Update()
    {
        switch (state)
        {
            case State.FlyIn:
                MoveTowards();
                break;
            case State.Idle:
                Idle();
                break;
            case State.FlyOut:
                MoveTowards();
                break;

        }
    }
    public void SetSpawnValues(float xIdleValue)
    {

        int spawnPosition = Random.Range(0, 2);
        if (spawnPosition == 0)
        {
            spawnLeft = true;
            transform.position = leftSpawn.position;

            Vector3 localScale;
            localScale = baseScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        else 
        {
            spawnLeft = false;
            transform.position = rightSpawn.position;
            transform.localScale = baseScale;
        }
        currentEndPosition = transform.position;
        currentEndPosition.x = xIdleValue;

        timer = 0;

        state = State.FlyIn;
        gameObject.SetActive(true);
    }

    private void MoveTowards()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentEndPosition, flySpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentEndPosition) < 0.2f)
        {
            switch (state)
            {
                case State.FlyIn:
                    state = State.Idle;
                    break;
                case State.FlyOut:
                    gameObject.SetActive(false);
                    break;
            }

        }
    }
    private void Idle()
    {
        timer += Time.deltaTime;
        if (timer > idleTime)
        {
            if (spawnLeft) currentEndPosition = rightSpawn.position;
            else currentEndPosition = leftSpawn.position;

            GameObject prefab = PoolingSystem.SpawnObject(bombPrefab, prefabSpawnPosition.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
            state = State.FlyOut;

            prefabSpawnPosition.gameObject.SetActive(false);
        }
    }
}
