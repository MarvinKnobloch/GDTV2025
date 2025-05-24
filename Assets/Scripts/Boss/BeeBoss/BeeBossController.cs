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
    private Vector3 gunBaseSacle;

    [Header("Honey")]
    [SerializeField] private CrateHoney leftHoney;
    [SerializeField] private CrateHoney rightHoney;
    private int activeHoneyCount;
    private bool leftHoneyActive;
    private bool switchSpawn;
    [SerializeField] private float honeyDropInterval;
    [SerializeField] private float honeyDropStartDelay;

    [Header("Boss")]
    [SerializeField] private GameObject beeBoss;

    void Start()
    {
        gunBaseSacle = beeGun.transform.localScale;

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
        if (spawn == 0)
        {
            Vector3 localScale;
            localScale = gunBaseSacle;
            localScale.x *= -1;
            beeGun.transform.localScale = localScale;
            beeGun.gunPosition = BeeGun.Position.left;
        }
        else if (spawn == 1)
        {
            beeGun.transform.localScale = gunBaseSacle;
            beeGun.gunPosition = BeeGun.Position.top;
        }
        else
        {
            beeGun.transform.localScale = gunBaseSacle;
            beeGun.gunPosition = BeeGun.Position.right; 
        }

        beeGun.gameObject.transform.parent.transform.position = beeGunSpawns[spawn].position;
        beeGun.gameObject.SetActive(true);
    }
    public void TriggerBossAndHoney(int number)
    {
        if (number == 1)
        {
            activeHoneyCount++;
            leftHoneyActive = true;
        }
        else if (number == 2) 
        {
            activeHoneyCount++;
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
            if (leftHoneyActive == true) leftHoney.CrateDrop();
            else rightHoney.CrateDrop();
        }
        else
        {
            if (switchSpawn) leftHoney.CrateDrop();
            else rightHoney.CrateDrop();
        }
    }
}
