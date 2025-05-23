using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Image[] difficultyButtonImages;
    [SerializeField] private Color deselectedColor, selectedColor;

    private void OnEnable()
    {
        ToggleDifficultyButtons();
    }
    public void ToggleDifficultyButtons()
    {
        for (int i = 0; i < difficultyButtonImages.Length; i++)
        {
            difficultyButtonImages[i].color = deselectedColor;
        }

        difficultyButtonImages[PlayerPrefs.GetInt("CurrentDifficulty")].color = selectedColor;
    }
}
