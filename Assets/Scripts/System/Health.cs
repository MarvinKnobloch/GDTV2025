using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    [NonSerialized] public PlayerUI playerUI;

    //Values
    [Header("Values")]
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;
    private int baseHealth;

    [Header("Boss")]
    [SerializeField] private bool isBoss;
    [SerializeField] private bool showBossHealthOnStart;

    [HideInInspector]
    public UnityEvent dieEvent;
    [HideInInspector]
    public UnityEvent hitEvent;

    public int Value
    {
        get { return currentHealth; }
        set { currentHealth = Math.Min(Math.Max(0, value), maxHealth); }
    }

    public int MaxValue
    {
        get { return maxHealth; }
        set { maxHealth = Math.Max(0, value); currentHealth = Math.Min(value, currentHealth); }
    }

    void Start()
    {
        if (gameObject == Player.Instance.gameObject)
        {
            playerUI = GameManager.Instance.playerUI;

            //baseHealth = MaxValue;
            //CalculatePlayerHealth();

            Value = MaxValue;
            playerUI.HealthUIUpdate(Value, MaxValue);
        }
        else
        {
            Value = MaxValue;

            if (isBoss)
            {
                playerUI = GameManager.Instance.playerUI;

                if (showBossHealthOnStart)
                {
                    playerUI.ToggleBossHealth(true);
                    playerUI.BossHealthUIUpdate(Value, MaxValue);
                }
            }
        }

    }
    public void PlayerTakeDamage(int amount, bool ignoreIFrames)
    {
        if (amount == 0)
            return;
        if (Value <= 0)
            return;
        if (Player.Instance.iframesWhileDash)
        {
            if (Player.Instance.state == Player.States.Dash) return;
        }

        if (ignoreIFrames == false)
            if (Player.Instance.iframesActive)
                return;

        Value -= amount;
        playerUI.HealthUIUpdate(Value, MaxValue);


        if (Value > 0)
            Player.Instance.IFramesStart();

        //AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.playerGetHitSounds[Player.Instance.currentElementNumber]);

        CheckForDeath();
    }

    public void EnemyTakeDamage(int amount)
    {
        if (amount == 0)
            return;
        if (Value <= 0)
            return;

        Value -= amount;

        if (isBoss)
        {
            playerUI.BossHealthUIUpdate(Value, MaxValue);
        }

        CheckForDeath();

        if (Value > 0)
        {
            hitEvent?.Invoke();
        }

    }
    private void CheckForDeath()
    {
        if (Value <= 0)
        {
            StopAllCoroutines();

            dieEvent?.Invoke();
        }
    }
    //public void Heal(int amount)
    //{
    //    if (amount == 0)
    //        return;

    //    Value += amount;

    //    if (gameObject == Player.Instance.gameObject)
    //        playerUI.HealthUIUpdate(Value, MaxValue);
    //    else
    //        EnemyHealthbarUpdate();
    //}
    public void CalculatePlayerHealth()
    {
        MaxValue = baseHealth; // + PlayerPrefs.GetInt(Upgrades.StatsUpgrades.BonusHealth.ToString());
        playerUI.HealthUIUpdate(Value, MaxValue);
    }
}
