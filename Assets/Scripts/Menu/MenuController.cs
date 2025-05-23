using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class MenuController : MonoBehaviour
{
    private Controls controls;

    private GameObject baseMenu;
    private GameObject currentOpenMenu;
    [NonSerialized] public bool gameIsPaused;

    [SerializeField] private GameObject titleMenu;
    [SerializeField] private GameObject ingameMenu;

    [SerializeField] private GameObject confirmController;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI confirmText;

    [SerializeField] private GameObject loadGameButton;
    private float normalFixedDeltaTime;

    [SerializeField] private GameObject resetButton;
    [SerializeField] private GameObject arenaHubButton;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
        normalFixedDeltaTime = Time.fixedDeltaTime;
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == (int)GameScenes.TitelScreen)
        {
            baseMenu = titleMenu;
            baseMenu.SetActive(true);
            GameManager.Instance.playerUI.gameObject.SetActive(false);
            if(PlayerPrefs.GetInt("NewGame") == 0)
            {
                loadGameButton.GetComponent<Button>().enabled = false;
                loadGameButton.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                loadGameButton.GetComponent<Button>().enabled = true;
                loadGameButton.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            if(SceneManager.GetActiveScene().buildIndex == (int)GameScenes.AreaHub)
            {
                resetButton.SetActive(false);
                arenaHubButton.SetActive(false);
            }

            GameManager.Instance.DeactivateCursor();

            baseMenu = ingameMenu;
        }
    }

    void Update()
    {
        if (controls.Menu.MenuEsc.WasPerformedThisFrame())
        {
            HandleMenu();
        }
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    public void HandleMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex == (int)GameScenes.TitelScreen)
        {
            if (confirmController.activeSelf == true) confirmController.SetActive(false);
            else if (titleMenu.activeSelf == true) return;
            else CloseSelectedMenu();
        }
        else
        {
            if (Player.Instance == null) return;

            if (GameManager.Instance.playerUI.dialogBox.activeSelf == true) return;
            if (confirmController.activeSelf == true) confirmController.SetActive(false);
            else if (ingameMenu.activeSelf == false)
            {
                if (gameIsPaused == false)
                {
                    PauseGame();
                    ingameMenu.SetActive(true);

                }
                else CloseSelectedMenu();
            }
            else
            {
                ingameMenu.SetActive(false);
                EndPause();
            }
        }
    }

    public void OpenSelection(GameObject currentMenu)
    {
        {
            currentOpenMenu = currentMenu;
            currentMenu.SetActive(true);

            titleMenu.SetActive(false);
            ingameMenu.SetActive(false);

            AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
        }
    }

    public void ResumeGame()
    {
        ingameMenu.SetActive(false);
        EndPause();
    }
    //public void SetNewGame()
    //{
    //    OpenConfirmController(NewGame, "Start new game?");
    //}
    public void SetBackToArenaHubConfirm()
    {
        OpenConfirmController(BackToArenaHub, "Enter Arena Hub?");
    }
    public void SetBackToMainMenuConfirm()
    {
        OpenConfirmController(BackToMainMenu, "Back to main menu?");
    }
    public void NewGame()
    {
        PlayerPrefs.SetInt("NewGame", 0);
        SceneManager.LoadScene((int)GameScenes.AreaHub);
    }
    public void LoadGame()
    {
        AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
        gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;
        SceneManager.LoadScene((int)GameScenes.AreaHub);
    }
    public void ResetPlayer(bool playSound)
    {
        if(playSound) AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
        gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //if (GameManager.Instance.LoadFormCheckpoint)
        //{
        //    SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
        //}
        //else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void BackToArenaHub()
    {
        AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
        gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;
        SceneManager.LoadScene((int)GameScenes.AreaHub);
    }

    private void BackToMainMenu()
    {
        AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
        gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;
        SceneManager.LoadScene((int)GameScenes.TitelScreen);
    }
    public void CloseSelectedMenu()
    {
        if (currentOpenMenu != null)
        {
            currentOpenMenu.SetActive(false);
            currentOpenMenu = null; // Clear previous menu after returning
            baseMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No previous menu to return to. Going back to inGameMenu.");
            baseMenu.SetActive(true);
        }
        AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
    }

    private void PauseGame()
    {
        GameManager.Instance.ActivateCursor();

        gameIsPaused = true;
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;

        AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
    }
    public void EndPause()
    {
        GameManager.Instance.DeactivateCursor();

        gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;

        AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
    }
    private void OpenConfirmController(UnityAction buttonEvent, string text)
    {

        confirmText.text = text;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => buttonEvent());
        confirmController.SetActive(true);

        AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
    }
    public void CloseConfirmSelection()
    {
        confirmController.SetActive(false);

        AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
    }
    public void TimeScaleToZero()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;
    }
    public void ResetTimeScale()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltaTime;
    }
}
