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
            if (PlayerPrefs.GetInt("BossDefeated") == 0)
            {
                gameScenes = GameScenes.Boss1;
                PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.IntroBoss1.ToString(), 0);
            }
            else if (PlayerPrefs.GetInt("BossDefeated") == 1)
            {
                gameScenes = GameScenes.Boss2;
                PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.IntroBoss2.ToString(), 0);
            }
            else 
            { 
                gameScenes = GameScenes.Boss3;
                PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.IntroBoss3.ToString(), 0);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene((int)gameScenes);
        }
    }
}
