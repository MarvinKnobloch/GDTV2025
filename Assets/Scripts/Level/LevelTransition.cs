using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private GameScenes gameScenes;

    [SerializeField] private bool switchTransitionOnBossDefeat;

    private void Awake()
    {
        if (switchTransitionOnBossDefeat)
        {
            if (PlayerPrefs.GetInt("BossDefeated") == 0) gameScenes = GameScenes.Boss1;
            else if (PlayerPrefs.GetInt("BossDefeated") == 1) gameScenes = GameScenes.Boss2;
            else gameScenes = GameScenes.Boss3;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene((int)gameScenes);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
