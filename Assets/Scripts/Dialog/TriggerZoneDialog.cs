using System;
using UnityEngine;

public class TriggerZoneDialog : MonoBehaviour
{
    [SerializeField] private string firstPlayerPrefCondition;
    [SerializeField] private string secondPlayerPrefCondition;

    [SerializeField] private DialogObj[] dialog;

    private void Awake()
    {
        if(PlayerPrefs.GetInt(firstPlayerPrefCondition) != PlayerPrefs.GetInt(secondPlayerPrefCondition))
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerPrefs.SetInt(secondPlayerPrefCondition, PlayerPrefs.GetInt(secondPlayerPrefCondition) + 1);
            GameManager.Instance.playerUI.dialogBox.GetComponent<DialogBox>().DialogStart(dialog[PlayerPrefs.GetInt(firstPlayerPrefCondition)]);
            GameManager.Instance.playerUI.dialogBox.SetActive(true);

            Player.Instance.rb.linearVelocity = Vector2.zero;
            Player.Instance.SwitchToGround(true);
            Player.Instance.ChangeAnimationState("Idle");
            Player.Instance.state = Player.States.Ground;
        }
    }
}
