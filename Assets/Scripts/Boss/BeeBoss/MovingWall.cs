using UnityEngine;

public class MovingWall : MonoBehaviour
{
    [SerializeField] private Transform leftSpawn;
    [SerializeField] private Transform rightSpawn;
    [SerializeField] private float speed;

    private Vector3 targetPosition;
    public void SetWall()
    {
        if (gameObject.activeSelf) return;

        int spawn = Random.Range(0, 2);
        int ySpawn = Random.Range(0, 5);
        if (spawn == 0)
        {
            targetPosition = rightSpawn.position + Vector3.up * ySpawn;
            transform.position = leftSpawn.position + Vector3.up * ySpawn;

            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            targetPosition = leftSpawn.position + Vector3.up * ySpawn;
            transform.position = rightSpawn.position + Vector3.up * ySpawn;

            transform.localScale = new Vector3(1, 1, 1);
        }
        gameObject.SetActive(true);

        AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.enemySounds[(int)AudioManager.EnemySounds.Bee]);
    }
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if(Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            gameObject.SetActive(false);
        }
    }
}
