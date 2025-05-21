
using System.Collections.Generic;
using UnityEngine;

public class Boss3Controller : MonoBehaviour
{
    [SerializeField] private Transform[] platformPositions;
    private List<Transform> platformList = new List<Transform>();

    [Header("BombBees")]
    [SerializeField] private float bombbeesInterval;
    [SerializeField] private float bombbeesDelay;
    [SerializeField] private BeeCarry beeCarry;

    private bool secondSpawnWave;
    void Start()
    {
        AddPlatforms();

        InvokeRepeating("SpawnBombBee", bombbeesDelay, bombbeesInterval);
    }

    private void SpawnBombBee()
    {
        if(beeCarry.gameObject.activeSelf == true)
        {
            Debug.Log("Bee still active");
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
        beeCarry.SetSpawnValues(platformList[randomPlatform].position.x);

        platformList.Remove(platformList[randomPlatform]);
    }
    private void AddPlatforms()
    {
        foreach (Transform obj in platformPositions)
        {
            platformList.Add(obj);
        }
    }
}
