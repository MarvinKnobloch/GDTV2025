using Unity.VisualScripting;
using UnityEngine;

public class BeeBossController : MonoBehaviour
{
    [Header("MovingWall")]
    [SerializeField] private MovingWall movingWall;
    [SerializeField] private int movingWallInterval;
    [SerializeField] private int movingWallStartDelay;

    [Header("BeeGun")]
    [SerializeField] private BeeGun beeGun;
    [SerializeField] private int beeGunIntervall;
    [SerializeField] private int beeGunStartDelay;
    [SerializeField] private Transform[] beeGunSpawns;

    [Header("Honey")]
    [SerializeField] private GameObject leftHoney;
    [SerializeField] private GameObject rightHoney;
    private int activeHoneyCount;
    private bool switchSpawn;
    [SerializeField] private float honeyDropInterval;
    [SerializeField] private float honeyDropStartDelay;
    [SerializeField] private GameObject honeyPrefab;

    [Header("Boss")]
    [SerializeField] private GameObject beeBoss;

    void Start()
    {
        InvokeRepeating("SpawnWall", movingWallStartDelay, movingWallInterval);
        InvokeRepeating("SpawnBeeGun", beeGunStartDelay, beeGunIntervall);
        InvokeRepeating("SpawnHoneyDrop", honeyDropStartDelay, honeyDropInterval);
    }

    private void SpawnWall()
    {
        movingWall.SetWall();
    }

    private void SpawnBeeGun()
    {
        int spawn = Random.Range(0, 3);
        if (spawn == 0) beeGun.gunPosition = BeeGun.Position.left;   
        else if (spawn == 1) beeGun.gunPosition = BeeGun.Position.top;
        else beeGun.gunPosition = BeeGun.Position.right;

        beeGun.gameObject.transform.position = beeGunSpawns[spawn].position;
        beeGun.gameObject.SetActive(true);
    }
    public void TriggerBossAndHoney(int number)
    {
        if (number == 1)
        {
            activeHoneyCount++;
            leftHoney.SetActive(true); 
        }
        else if (number == 2) 
        {
            activeHoneyCount++;

            rightHoney.SetActive(true); 
        }

        if(activeHoneyCount >= 2)
        {
            CancelInvoke("SpawnBeeGun");
            beeGun.GunOff();
            beeBoss.SetActive(true);
        }

    }
    private void SpawnHoneyDrop()
    {
        if (activeHoneyCount == 0) return;

        switchSpawn = !switchSpawn;
        if (activeHoneyCount == 1)
        {
            if (leftHoney.activeSelf == true) HoneySpawn(leftHoney);
            else HoneySpawn(rightHoney);
        }
        else
        {
            if (switchSpawn) HoneySpawn(leftHoney);
            else HoneySpawn(rightHoney);
        }
    }
    private void HoneySpawn(GameObject side)
    {
        GameObject prefab = PoolingSystem.SpawnObject(honeyPrefab, side.transform.GetChild(0).transform.position, Quaternion.identity, PoolingSystem.ProjectileType.Enemy);
        prefab.transform.right = transform.right;
    }
}
