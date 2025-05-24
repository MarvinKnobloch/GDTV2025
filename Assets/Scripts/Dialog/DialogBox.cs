using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    private Controls controls;

    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] public TextMeshProUGUI boxText;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private TextMeshProUGUI continueText;

    private DialogObj currentDialog;
    private int currentDialogNumber;

    private bool readInput;

    private bool cantSkipDialog;
    private bool disableInputs;
    private bool pauseGame;
    private float autoPlayInterval;

    private float timer;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Update()
    {
        if (autoPlayInterval != 0)
        {
            timer += Time.deltaTime;
            if (timer > autoPlayInterval)
            {
                timer = 0;
                DialogContinue();
            }
        }
        else
        {
            if (readInput == false)
            {
                readInput = true;
            }
            else
            {
                if (controls.Player.Interact.WasPerformedThisFrame() || controls.Player.Attack.WasPerformedThisFrame())
                {
                    DialogContinue();
                }
                else if (controls.Menu.MenuEsc.WasPerformedThisFrame())
                {
                    if (cantSkipDialog) return;
                    DialogBoxDisable();
                }
            }
        }
    }
    public void DialogStart(DialogObj dialog)
    {
        GameManager.Instance.ActivateCursor();

        readInput = false;

        cantSkipDialog = dialog.cantSkipDialog;
        disableInputs = dialog.disableInputs;
        pauseGame = dialog.pauseGame;

        if (cantSkipDialog) skipButton.SetActive(false);
        else skipButton.SetActive(true);

        if (disableInputs) GameManager.Instance.menuController.gameIsPaused = true;
        if (pauseGame) GameManager.Instance.menuController.TimeScaleToZero();

        currentDialog = dialog;
        currentDialogNumber = 0;
        DialogUpdate();
    }
    public void DialogContinue()
    {
        //AudioManager.Instance.PlaySoundOneshot((int)AudioManager.UtilitySounds.DialogNext);

        if (currentDialogNumber < currentDialog.dialogs.Length - 1)
        {
            currentDialogNumber++;
            DialogUpdate();
        }
        else DialogBoxDisable();
    }
    private void DialogUpdate()
    {
        timer = 0;
        autoPlayInterval = currentDialog.dialogs[currentDialogNumber].autoPlayInterval;

        if (autoPlayInterval == 0)
        {
            if (controls == null) controls = Keybindinputmanager.Controls;

            continueText.text = "Continue (" + controls.Player.Attack.GetBindingDisplayString() + " or " + controls.Player.Interact.GetBindingDisplayString() + ")";
        }
        else continueText.text = string.Empty;

        if (currentDialog.dialogs[currentDialogNumber].characterSprite != null) characterImage.sprite = currentDialog.dialogs[currentDialogNumber].characterSprite;
        else characterImage.sprite = null;

        characterName.text = currentDialog.dialogs[currentDialogNumber].characterName;
        boxText.text = currentDialog.dialogs[currentDialogNumber].dialogText;

        if (currentDialog.dialogs[currentDialogNumber].dialogEvent.Length != 0)
        {
            for (int i = 0; i < currentDialog.dialogs[currentDialogNumber].dialogEvent.Length; i++)
            {
                if(currentDialog.dialogs[currentDialogNumber].dialogEvent[i] != null)
                {
                    currentDialog.dialogs[currentDialogNumber].dialogEvent[i].OnEventRaised.Invoke();
                }
            }
        }
    }
    public void DialogBoxDisable()
    {
        if (currentDialog.eventAfterDialogEnd.Length != 0)
        {
            for (int i = 0; i < currentDialog.eventAfterDialogEnd.Length; i++)
            {
                if (currentDialog.eventAfterDialogEnd[i] != null)
                {
                    currentDialog.eventAfterDialogEnd[i].OnEventRaised.Invoke();
                }
            }
        }

        readInput = false;
        if (pauseGame) GameManager.Instance.menuController.ResetTimeScale();

        GameManager.Instance.menuController.gameIsPaused = false;
        gameObject.SetActive(false);

        Player.Instance.AttackTimeReset();
        //GameManager.Instance.DeactivateCursor();
    }
}
