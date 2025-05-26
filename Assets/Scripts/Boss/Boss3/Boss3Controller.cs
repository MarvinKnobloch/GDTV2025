
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss3Controller : MonoBehaviour
{
    [SerializeField] private Boss3 boss3;

    [Header("BombBees")]
    [SerializeField] private float bombBeesInterval;
    [SerializeField] private float bombBeesDelay;
    [SerializeField] private BeeCarry bombBeeCarry;
    [SerializeField] private Transform[] platformPositions;
    private List<Transform> platformList = new List<Transform>();

    [Header("BarrelBees")]
    [SerializeField] private float barrelBeesInterval;
    [SerializeField] private float barrelBeesDelay;
    [SerializeField] private BeeCarry barrelBeeCarry;
    [SerializeField] private Transform[] barrelDropPositions;
    private List<Transform> barrelDropList = new List<Transform>();

    [Header("Phase2Transition")]
    [SerializeField] private GameObject barrelsPrefab;
    [SerializeField] private GameObject[] allPlatforms;
    [SerializeField] private GameObject playerPhase2Spawn;

    [Header("BeeCircle")]
    [SerializeField] private GameObject beeCircle;

    private bool secondSpawnWave;
    void Start()
    {
        AddPlatforms();

        foreach (Transform obj in barrelDropPositions)
        {
            barrelDropList.Add(obj);
        }

        InvokeRepeating("SpawnBombBee", bombBeesDelay, bombBeesInterval);
        InvokeRepeating("SpawnBarrelBee", barrelBeesDelay, barrelBeesInterval);
    }

    private void SpawnBombBee()
    {
        if(bombBeeCarry.gameObject.activeSelf == true)
        {
            Debug.Log("BombBee still active");
            return;
        }

        if (platformList.Count == 0 && secondSpawnWave == false)
        {
            secondSpawnWave = true;
            AddPlatforms();
        }
        else if (platformList.Count == 0 && secondSpawnWave == true)
        {
            CancelInvoke();
            return;
        }

        int randomPlatform = Random.Range(0, platformList.Count);
        bombBeeCarry.SetSpawnValues(platformList[randomPlatform].position.x);

        platformList.Remove(platformList[randomPlatform]);
    }
    private void AddPlatforms()
    {
        foreach (Transform obj in platformPositions)
        {
            platformList.Add(obj);
        }
    }
    private void SpawnBarrelBee()
    {
        if (barrelBeeCarry.gameObject.activeSelf == true)
        {
            Debug.Log("BarrelBee still active");
            return;
        }
        int randomPosition = Random.Range(0, barrelDropList.Count);
        barrelBeeCarry.SetSpawnValues(barrelDropList[randomPosition].position.x);
    }

    public void CancelPhase1Events()
    {
        CancelInvoke();
    }
    public void TriggerPhase2Barrels()
    {
        for (int i = 0; i < barrelDropPositions.Length; i++)
        {
            GameObject prefab = PoolingSystem.SpawnObject(barrelsPrefab, barrelDropPositions[i].position + Vector3.up * 20, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
        }
        StartCoroutine(SetPhase2());
    }
    IEnumerator SetPhase2()
    {
        yield return new WaitForSeconds(1.2f);
        GameManager.Instance.menuController.gameIsPaused = true;

        for (int i = 0; i < allPlatforms.Length; i++)
        {
            allPlatforms[i].SetActive(false);
        }

        Player.Instance.playerCollider.enabled = false;
        Player.Instance.transform.position = playerPhase2Spawn.transform.position;
        Player.Instance.SwitchToFly();

        if(Player.Instance.faceRight == true)
        {
            Player.Instance.faceRight = false;
            Vector3 localScale;
            localScale = Player.Instance.transform.localScale;
            localScale.x *= -1;
            Player.Instance.transform.localScale = localScale;
        }

        boss3.Phase2Start();

        AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.enemySounds[(int)AudioManager.EnemySounds.Explosion]);
        //InvokeRepeating("ToggleBeeCircle", beeCircleDelay, beeCircleInterval);
    }
    public void ToggleBeeCircle()
    {
        beeCircle.SetActive(false);
        StartCoroutine(ActivateBeeCircle());
    }
    IEnumerator ActivateBeeCircle()
    {
        yield return null;
        beeCircle.SetActive(true);
    }
}
