using UnityEngine;

public class NpcDialog : MonoBehaviour, IInteractables
{
    public GameObject interactObj { get => gameObject; }

    [SerializeField] private string actionText;
    public string interactiontext => actionText;

    [SerializeField] private DialogObj[] dialog;
    public void Interaction()
    {
        if (dialog[PlayerPrefs.GetInt("BossDefeated")].pauseGame == false)
        {
            if (dialog[PlayerPrefs.GetInt("BossDefeated")].disableInputs == true)
            {
                Player.Instance.rb.linearVelocity = Vector2.zero;
                Player.Instance.SwitchToGround(true);
                Player.Instance.ChangeAnimationState("Idle");
                Player.Instance.state = Player.States.Ground;
            }
        }

        GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(dialog[PlayerPrefs.GetInt("BossDefeated")]);
        GameManager.Instance.playerUI.dialogBox.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.playerInteraction.AddInteraction(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.playerInteraction.RemoveInteraction(this);
        }
    }
}
