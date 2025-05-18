using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using static GameManager;

public class PlayerUI : MonoBehaviour
{
    private Controls controls;
    //private bool playCurrenySound;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionField;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Health")]
    //[SerializeField] private Image healthbar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Energy")]
    [SerializeField] private Image energybar;

    //[Header("Currency")]
    //[SerializeField] private TextMeshProUGUI currencyText;

    //[Header("MessageBox")]
    //public GameObject messageBox;
    //[SerializeField] private TextMeshProUGUI messageBoxText;
    //[SerializeField] private TextMeshProUGUI messageBoxCloseText;

    //[Header("DialogBox")]
    //public GameObject dialogBox;

    [Header("BossHealth")]
    [SerializeField] private GameObject bossHealthbarObject;
    [SerializeField] private Image bossHealthbar;

    [Header("GameOver")]
    [SerializeField] private GameObject gameOverScreen;

    private float timer;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Start()
    {
        StartCoroutine(InteractionFieldDisable());
    }
    IEnumerator InteractionFieldDisable()
    {
        yield return null;
        interactionField.SetActive(false);
        interactionField.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        //playCurrenySound = true;
    }
    public void HandleInteractionBox(bool state)
    {
        if(interactionField != null) interactionField.SetActive(state);
    }
    public void InteractionTextUpdate(string text)
    {
        interactionText.text = text + " (<color=green>" + controls.Player.Interact.GetBindingDisplayString() + "</color>)";
    }
    public void HealthUIUpdate(int current, int max)
    {
        //healthbar.fillAmount = (float)current / max;
        healthText.text = current.ToString(); // + "/" + max;
    }
    public void EnergyUIUpdate(int current, int max)
    {
        energybar.fillAmount = (float)current / max;
    }

    //public void PlayerCurrencyUpdate(int amount)
    //{
    //    GameManager.Instance.playerCurrency += amount;
    //    currencyText.text = GameManager.Instance.playerCurrency.ToString();

    //    PlayerPrefs.SetInt("PlayerCurrency", GameManager.Instance.playerCurrency);

    //    if(amount > 0 && playCurrenySound) AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilityFiles[(int)AudioManager.UtilitySounds.CurrencyGain]);
    //}

    public void ToggleBossHealth(bool activate)
    {
        bossHealthbarObject.SetActive(activate);
    }
    public void BossHealthUIUpdate(int current, int max)
    {
        bossHealthbar.fillAmount = (float)current / max;
    }
    public void GameOver()
    {
        GameManager.Instance.menuController.gameIsPaused = true;
        gameOverScreen.SetActive(true);
    }
    //public void MessageBoxEnable(string text)
    //{
    //    GameManager.Instance.ActivateCursor();

    //    GameManager.Instance.menuController.TimeScaleToZero();
    //    GameManager.Instance.menuController.gameIsPaused = true;

    //    messageBox.SetActive(true);
    //    messageBoxText.text = text;
    //    messageBoxCloseText.text = "Close (<color=green>" + controls.Player.Interact.GetBindingDisplayString() + "</color>)"; 
    //}
    //public void MessageBoxDisable()
    //{
    //    GameManager.Instance.DeactivateCursor();

    //    GameManager.Instance.menuController.ResetTimeScale();
    //    GameManager.Instance.menuController.gameIsPaused = false;
    //    messageBox.SetActive(false);
    //}

    //public void ActivateShop()
    //{
    //    GameManager.Instance.ActivateCursor();

    //    GameManager.Instance.menuController.TimeScaleToZero();
    //    GameManager.Instance.menuController.gameIsPaused = true;
    //    shop.SetActive(true);
    //}
    //public void DeactivateShop()
    //{
    //    GameManager.Instance.DeactivateCursor();

    //    GameManager.Instance.menuController.EndPause();
    //    shop.SetActive(false);
    //}

}
