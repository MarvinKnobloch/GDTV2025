using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{
    public Canvas VictoryCanvas;
    public Image OverlayImage;
    public float FadeInDuration = 1f;

    private bool _showVictoryScreen = false;
    private float _fadeTimer = 0f;

    public void ShowVictoryScreen()
    {
        VictoryCanvas.enabled = true;
        _showVictoryScreen = true;
    }

    void FixedUpdate()
    {
        if (_showVictoryScreen)
        {
            _fadeTimer += Time.fixedDeltaTime;
            float alpha = Mathf.Clamp01(_fadeTimer / FadeInDuration);
            var color = OverlayImage.color;
            color.a = alpha;
            OverlayImage.color = color;

            if (alpha >= 1f)
            {
                _showVictoryScreen = false;
                SceneManager.LoadScene((int)GameScenes.AreaHub);
            }
        }
    }
}
