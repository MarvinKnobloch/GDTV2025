using UnityEngine;

public class Boss2 : MonoBehaviour
{
    [SerializeField] private MovingWall movingWall;

    void Start()
    {
        InvokeRepeating("SpawnWall", 6, 6);
    }

    private void SpawnWall()
    {
        movingWall.SetWall();
    }
}
