using System.Collections;
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
				player.gunArmAnimator.SetTrigger("Shoot");
                player.CreatePrefab(player.playerProjectile, player.projectileSpawnPosition, Quaternion.identity);
				player.StartCoroutine(MuzzleFlash());
            }
        }
    }
    public void AttackTimeReset() => attackTimer = 0;


	IEnumerator MuzzleFlash()
	{
		player.muzzleFlashAnimator.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.15f);
		player.muzzleFlashAnimator.gameObject.SetActive(false);
	}
}
