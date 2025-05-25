using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities
{
    public Player player;
    private float attackTimer;

    public void AttackInput()
    {
        attackTimer += Time.deltaTime;
        if (player.controls.Player.Attack.IsPressed())
        {
            if (attackTimer > player.attackCooldown)
            {
                attackTimer = 0;
                AudioManager.Instance.PlayAudioFileOneShot(
                    AudioManager.Instance.playerSounds[(int)AudioManager.PlayerSounds.PlayerShoot]
                );
                player.CreatePrefab(player.playerProjectile, player.projectileSpawnPosition, Quaternion.identity);
            }
        }
    }
    public void AttackTimeReset() => attackTimer = 0;
}
