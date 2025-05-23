using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Tutorials : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private TextMeshProUGUI moveText, jumpText, dashText, dropDownText;

    private void Awake()
    {
        controls = Keybindinputmanager.Controls;
    }
    private void Start()
    {
        TutorialUpdate();
    }
    public void TutorialUpdate()
    {
        MoveTutorial();
        JumpTutorial();
        DashTutorial();
        DropDownTutorial();
    }
    public void MoveTutorial()
    {
        moveText.text = "Use <color=green>" + controls.Player.Move.GetBindingDisplayString(0) + "</color> to move.\n" +
                        "Hotkey can be changed in the Menu(<color=green>" + controls.Menu.MenuEsc.GetBindingDisplayString(0) + "</color>) -> Settings";
    }
    public void JumpTutorial()
    {
        jumpText.text = "Press <color=green>" + controls.Player.Jump.GetBindingDisplayString(0) + "</color> jump.\n" +
                "Holding the jump button will increase your jumpheight.\n" +
                "In air press <color=green>" + controls.Player.Jump.GetBindingDisplayString(0) + "</color> again to perform a second jump.";
    }
    public void DashTutorial()
    {
        dashText.text = "Press <color=green>" + controls.Player.Dash.GetBindingDisplayString(0) + "</color> to dash.\n" +
                "Dashing cosumes energy which will refill over time";
    }
    public void DropDownTutorial()
    {
        dropDownText.text = "Press <color=green>" + controls.Player.Move.GetBindingDisplayString(2) + "</color> to drop down from small plaforms.";
    }
}
