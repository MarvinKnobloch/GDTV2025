using Unity.VisualScripting;
using UnityEngine;

public class Boss2 : MonoBehaviour
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

    void Start()
    {
        InvokeRepeating("SpawnWall", movingWallStartDelay, movingWallInterval);
        InvokeRepeating("SpawnBeeGun", beeGunStartDelay, beeGunIntervall);
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

}
