
using System.Collections.Generic;
using UnityEngine;

public class Boss3Controller : MonoBehaviour
{

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
}
