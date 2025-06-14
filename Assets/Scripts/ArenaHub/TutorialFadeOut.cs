using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class TutorialFadeOut : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private float transitionTime;
    [SerializeField] private int dontShowTutorial;

    private Color backgroundColor;
    private float timer;
    private bool _disableTriggerExit = false;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(GameManager.SaveFilePlayerPrefs.BossDefeated.ToString()) >= dontShowTutorial) gameObject.SetActive(false);

        backgroundColor = tutorialText.color;
        backgroundColor.a = 0;

        tutorialText.color = backgroundColor;
    }

    void OnApplicationQuit()
    {
        _disableTriggerExit = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GameManager.Instance.menuController.gameIsPaused == false)
        {
            StopAllCoroutines();

            if (!_disableTriggerExit && gameObject.activeSelf == true) StartCoroutine(FadeOut());
        }
    }
    IEnumerator FadeIn()
    {
        while (backgroundColor.a < 0.95f)
        {
            timer += Time.deltaTime;
            float time = timer / transitionTime;
            backgroundColor.a = time;
            tutorialText.color = backgroundColor;
            yield return null;
        }
        backgroundColor.a = 1;
        tutorialText.color = backgroundColor;
        StopAllCoroutines();
    }
    IEnumerator FadeOut()
    {
        if (gameObject.activeSelf == false) StopAllCoroutines();

        while (backgroundColor.a > 0.05f)
        {
            timer -= Time.deltaTime;
            float time = timer / transitionTime;
            backgroundColor.a = time;
            tutorialText.color = backgroundColor;
            yield return null;
        }
        backgroundColor.a = 0;
        tutorialText.color = backgroundColor;
        StopAllCoroutines();
    }
}
