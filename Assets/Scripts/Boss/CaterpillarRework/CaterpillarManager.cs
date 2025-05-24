using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarManager : MonoBehaviour
{
    [Header("Combat")]
    public Health Health;
    public List<CaterpillarPhase> Phases = new();
    [NonSerialized] public int CurrentPhaseIndex = 0;
    public CaterpillarPhase CurrentPhase => Phases[CurrentPhaseIndex];
    public float TimeBetweenAI = 1f;

    [Header("Goon")]
    public GameObject GoonPrefab;

    [Header("Movement")]
    public List<Transform> LeftSlots = new();
    public List<Transform> RightSlots = new();
    public GameObject Head;
    public GameObject Tail;

    private Transform _goonSpawnPoint;
    private bool _headAttackFromLeft = true;
    private bool _tailAttackFromLeft = true;

    void Start()
    {
        StartCoroutine(ExecuteAI());
        Health.hitEvent.AddListener(PhaseTransition);
        Health.dieEvent.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        Health.hitEvent.RemoveListener(PhaseTransition);
        Health.dieEvent.RemoveListener(OnDeath);
    }

    private void OnDeath()
    {
        StopAllCoroutines();
        GameManager.Instance.ShowVictoryScreen();
    }

    private void PhaseTransition()
    {
        if (Health.Value <= CurrentPhase.TransitionAtHealth)
        {
            StopAllCoroutines();
            CurrentPhaseIndex++;
            StartCoroutine(ExecuteAI());
        }
    }

    IEnumerator ExecuteAI()
    {
        while (true)
        {
            DoMovement();
            yield return StartCoroutine(CurrentPhase.Attack(_headAttackFromLeft, _tailAttackFromLeft));
            SpawnGoon();

            yield return new WaitForSeconds(TimeBetweenAI);
        }
    }

    void SpawnGoon()
    {
        PoolingSystem.SpawnObject(GoonPrefab, _goonSpawnPoint.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
    }

    void DoMovement()
    {
        var useLeftSlotsForHead = UnityEngine.Random.Range(0, 2);
        var useLeftSlotsForTail = UnityEngine.Random.Range(0, 2);

        _headAttackFromLeft = useLeftSlotsForHead == 1;
        _tailAttackFromLeft = useLeftSlotsForTail == 1;

        var headSlotIndex = UnityEngine.Random.Range(0, LeftSlots.Count);
        int tailSlotIndex;
        do
        {
            tailSlotIndex = UnityEngine.Random.Range(0, LeftSlots.Count);
        } while (tailSlotIndex == headSlotIndex);

        var headSlot = useLeftSlotsForHead == 1 ? LeftSlots[headSlotIndex] : RightSlots[headSlotIndex];
        var tailSlot = useLeftSlotsForTail == 1 ? LeftSlots[tailSlotIndex] : RightSlots[tailSlotIndex];

        int spawnSlotIndex;
        do
        {
            spawnSlotIndex = UnityEngine.Random.Range(0, LeftSlots.Count);
        } while (spawnSlotIndex == headSlotIndex || spawnSlotIndex == tailSlotIndex);

        _goonSpawnPoint = LeftSlots[spawnSlotIndex];

        Head.transform.position = headSlot.position;
        Tail.transform.position = tailSlot.position;
    }
}
