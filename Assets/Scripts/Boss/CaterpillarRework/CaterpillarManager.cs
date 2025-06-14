using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarManager : MonoBehaviour
{
    [Header("Combat")]
    public Health Health;
    public CaterpillarPhase phaseReferences;

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
    private int _ensureInitialEvenGoonSpawn = 0;
    private bool _goonSpawnsRight = false;

    [Header("BossDefeat")]
    [SerializeField] private DialogObj boss1EndDialog;
    [SerializeField] private VoidEventChannel boss1EndEvent;

    [Header("HitEffect")]
    [SerializeField] private SpriteRenderer[] bossSprites;
    [SerializeField] private float hitEffectDuration;

    [Header("Animation")]
    public Animator HeadAnimator;
    public Animator TailAnimator;
    public SpriteRenderer HeadSpriteRenderer;
    public SpriteRenderer TailSpriteRenderer;

    void Start()
    {
        StartCoroutine(ExecuteAI());
        Health.hitEvent.AddListener(HitEffect);
        Health.dieEvent.AddListener(OnDeath);
        boss1EndEvent.OnEventRaised += Boss1EndEvent;

        phaseReferences.HeadAnimator = HeadAnimator;
        phaseReferences.TailAnimator = TailAnimator;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        Health.hitEvent.RemoveAllListeners();
        Health.dieEvent.RemoveListener(OnDeath);
        boss1EndEvent.OnEventRaised -= Boss1EndEvent;
    }

    public IEnumerator ExecuteAI()
    {
        var firstIteration = true;
        while (true)
        {
            DoMovement();

            if (_headAttackFromLeft)
            {
                HeadSpriteRenderer.flipX = true;
            }
            else
            {
                HeadSpriteRenderer.flipX = false;
            }

            if (_tailAttackFromLeft)
            {
                TailSpriteRenderer.flipX = true;
            }
            else
            {
                TailSpriteRenderer.flipX = false;
            }

            if (firstIteration)
            {
                firstIteration = false;
                yield return new WaitForSeconds(TimeBetweenAI);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }

            yield return phaseReferences.Attack(this, _headAttackFromLeft, _tailAttackFromLeft);
            SpawnGoon();

            yield return new WaitForSeconds(TimeBetweenAI);
        }
    }

    void SpawnGoon()
    {
        var goonObj = PoolingSystem.SpawnObject(GoonPrefab, _goonSpawnPoint.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
        goonObj.TryGetComponent(out BossStraightGoon goon);
        if (goon != null)
        {
            goon.SpawnedRight = _goonSpawnsRight;
        }

        goonObj.TryGetComponent(out SpriteRenderer goonSpriteRenderer);
        if (goonSpriteRenderer != null)
        {
            goonSpriteRenderer.flipX = !_goonSpawnsRight;
        }
    }

    void DoMovement()
    {
        switch (_ensureInitialEvenGoonSpawn)
        {
            case 0:
                _headAttackFromLeft = true;
                _tailAttackFromLeft = false;
                _goonSpawnPoint = LeftSlots[0];
                Head.transform.position = LeftSlots[1].position;
                Tail.transform.position = RightSlots[2].position;
                _ensureInitialEvenGoonSpawn++;
                return;
            case 1:
                _headAttackFromLeft = false;
                _tailAttackFromLeft = true;
                _goonSpawnPoint = LeftSlots[1];
                Head.transform.position = RightSlots[2].position;
                Tail.transform.position = LeftSlots[0].position;
                _ensureInitialEvenGoonSpawn++;
                return;
            case 2:
                _headAttackFromLeft = true;
                _tailAttackFromLeft = true;
                _goonSpawnPoint = LeftSlots[2];
                Head.transform.position = LeftSlots[1].position;
                Tail.transform.position = LeftSlots[0].position;
                _ensureInitialEvenGoonSpawn++;
                return;
            default:
                break;
        }

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
        _goonSpawnsRight = UnityEngine.Random.Range(0, 2) == 1;
        if (_goonSpawnsRight)
        {
            _goonSpawnPoint = RightSlots[spawnSlotIndex];
        }

        Head.transform.position = headSlot.position;
        Tail.transform.position = tailSlot.position;
    }
    private void OnDeath()
    {
        if (Player.Instance.playerIsDead) return;

        Player.Instance.bossDefeated = true;
        GameManager.Instance.menuController.gameIsPaused = true;
        Time.timeScale = 0;
        GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(boss1EndDialog);
        GameManager.Instance.playerUI.dialogBox.SetActive(true);
        GameManager.Instance.playerUI.ToggleBossHealth(false);
    }
    private void Boss1EndEvent()
    {
        int playerPref = 1;
        PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.BossDefeated.ToString(), playerPref);
        PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.ArenaEntranceDialog.ToString(), playerPref);

        GameManager.Instance.menuController.gameIsPaused = false;
        Time.timeScale = 1;

        GameManager.Instance.ShowVictoryScreen();

        StopAllCoroutines();
        CancelInvoke();
        gameObject.SetActive(false);
    }
    private void HitEffect()
    {
        StartCoroutine(ChangeColor());
    }
    IEnumerator ChangeColor()
    {
        foreach (SpriteRenderer sprite in bossSprites)
        {
            sprite.color = Color.red;
        }
        yield return new WaitForSeconds(hitEffectDuration);

        foreach (SpriteRenderer sprite in bossSprites)
        {
            sprite.color = Color.white;
        }
    }
}
