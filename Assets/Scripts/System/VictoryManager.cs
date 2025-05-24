using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{
    public Canvas VictoryCanvas;
    public Image OverlayImage;
    public float FadeInDuration = 1f;

    private float _fadeTimer = 0f;

    void PostTransition()
    {
        var bossDefeated = PlayerPrefs.GetInt(GameManager.SaveFilePlayerPrefs.BossDefeated.ToString(), 0);
        PlayerPrefs.SetInt(GameManager.SaveFilePlayerPrefs.BossDefeated.ToString(), bossDefeated + 1);

        SceneManager.LoadScene((int)GameScenes.AreaHub);
    }

    void FixedUpdate()
    {
        _fadeTimer += Time.fixedDeltaTime;
        float alpha = Mathf.Clamp01(_fadeTimer / FadeInDuration);
        var color = OverlayImage.color;
        color.a = alpha;
        OverlayImage.color = color;

        if (alpha >= 1f)
        {
            PostTransition();
        }
    }
}
