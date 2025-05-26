using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement
{
    public Player player;
    private float dashTimer;

    const string idleState = "Idle";
    const string runState = "Run";
    const string runBackWardsstate = "RunBackwards";
    const string jumpState = "Jump";
    const string fallState = "Fall";
    const string dashState = "Dash";

    //private float platformGroundDrag = -10;

    public void PlayerMove(float grounddrag)
    {
        player.playerVelocity.Set(player.moveDirection.x * player.movementSpeed, grounddrag);

        player.rb.linearVelocity = player.playerVelocity;

        //Animation
        if (player.state == Player.States.Ground)
        {
            if (player.moveDirection == Vector2.zero)
            {
                player.ChangeAnimationState(idleState);
            }
            else
            {
                if(player.mousePosition.x > player.transform.position.x && player.moveDirection.x < 0) player.ChangeAnimationState(runBackWardsstate);
                else if(player.mousePosition.x < player.transform.position.x && player.moveDirection.x > 0) player.ChangeAnimationState(runBackWardsstate);
                else player.ChangeAnimationState(runState);
            }
        }
    }
    public void GroundMovement()
    {
        PlayerMove(player.playerGroundDrag);
    }
    public void GroundIntoAirTransition()
    {
        player.groundIntoAirTimer += Time.deltaTime;

        if (player.groundIntoAirTimer > player.groundIntoAirOffset)
        {
            player.SwitchToAir();
        }
    }
    public void AirMovement()
    {
        if (player.rb.linearVelocity.y < player.maxFallSpeed) PlayerMove(player.maxFallSpeed);
        else PlayerMove(player.rb.linearVelocity.y);

        if (player.rb.linearVelocity.y < -0.5f)
        {
            player.ChangeAnimationState(fallState);
        }
        else
        {
            player.ChangeAnimationState(jumpState);
        }
    }
    public void RotatePlayer()
    {
        if (player.moveDirection.x > 0 && player.faceRight == true) flip();
        if (player.moveDirection.x < 0 && player.faceRight == false) flip();
    }
    public void RotatePlayerToMouse()
    {
        if (player.mousePosition.x > player.transform.position.x && player.faceRight == true) flip();
        if (player.mousePosition.x < player.transform.position.x && player.faceRight == false) flip();
    }
    private void flip()
    {
        player.faceRight = !player.faceRight;
        Vector3 localScale;
        localScale = player.transform.localScale;
        localScale.x *= -1;
        player.transform.localScale = localScale;
    }
    public void JumpInput(InputAction.CallbackContext ctx)
    {

        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            JumpInputPerformed();
        }
    }
    private void JumpInputPerformed()
    {
        if (player.menuController.gameIsPaused) return;

        int count = player.maxJumpCount;
        if (player.currentJumpCount >= count) return;

        switch (player.state)
        {
            case Player.States.Ground:
                Jump();
                break;
            case Player.States.GroundIntoAir:
                Jump();
                break;
            case Player.States.Air:
                Jump();
                break;
        }
    }
    private void Jump()
    {
        player.currentJumpCount++;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(new Vector2(0, player.jumpStrength), ForceMode2D.Impulse);

        player.jumpPerformed = true;
        player.jumpTimer = 0;
        //player.ChangeAnimationState(jumpState);

        if (player.state != Player.States.Air) player.SwitchGroundIntoAir();

        AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.playerSounds[(int)AudioManager.PlayerSounds.PlayerJump]);
    }
    public void JumpIsPressed()
    {
        if (player.jumpPerformed == false) return;

        player.jumpTimer += Time.deltaTime;
        if (player.jumpTimer > player.maxJumpTime)
        {
            player.jumpPerformed = false;
        }
        if (player.controls.Player.Jump.WasReleasedThisFrame())
        {
            float velocityReduce = player.maxJumpTime - player.jumpTimer;
            player.rb.AddForce(new Vector2(0, velocityReduce * -20), ForceMode2D.Impulse);
            player.jumpPerformed = false;
        }
    }
    public void DashInput(InputAction.CallbackContext ctx)
    {
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            DashInputPerformed();
        }
    }
    private void DashInputPerformed()
    {
        if (player.menuController.gameIsPaused) return;
        if (player.EnergyValue < player.dashCosts) return;

        switch (player.state)
        {
            case Player.States.Ground:
                StartDash();
                break;
            case Player.States.GroundIntoAir:
                StartDash();
                break;
            case Player.States.Air:
                StartDash();
                break;
            case Player.States.Attack:
                StartDash();
                break;
            case Player.States.Fly:
                StartDash();
                break;
        }
    }
    private void StartDash()
    {
        player.EnergyUpdate(-player.dashCosts);

        player.rb.linearVelocity = Vector2.zero;
        player.rb.gravityScale = 0;

        RotatePlayer();

        player.ChangeAnimationState(dashState);
        dashTimer = 0;
        player.state = Player.States.Dash;

        AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.playerSounds[(int)AudioManager.PlayerSounds.PlayerDash]);
    }
    public void DashMovement()
    {
        Vector2 movement = new Vector2(player.dashStrength, 0);

        if (player.faceRight) player.rb.linearVelocity = movement * -player.transform.right;
        else player.rb.linearVelocity = movement * player.transform.right;
    }
    public void DashTime()
    {
        dashTimer += Time.deltaTime;
        if (dashTimer >= player.dashTime)
        {
            if (player.isFlying) player.SwitchToFly();
            else player.SwitchToAir();
        }
    }
    public void FlyInput(InputAction.CallbackContext ctx)
    {
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            player.isFlying = !player.isFlying;
            if(player.isFlying == true)
            {
                player.SwitchToFly();
            }
            else
            {
                player.SwitchToAir();
            }
        }
    }
    public void FlyMovement()
    {
        player.playerVelocity.Set(player.moveDirection.x * player.movementSpeed, player.moveDirection.y * player.movementSpeed);

        player.rb.linearVelocity = player.playerVelocity;
    }
    public void JumpPad(float jumpPadStrength)
    {
        player.isFlying = false;
        player.jumpPerformed = false;
        player.currentJumpCount = player.maxJumpCount -1;

        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(new Vector2(0, jumpPadStrength), ForceMode2D.Impulse);

        player.SwitchGroundIntoAir();
    }
    public void PlatformDropInput()
    {
        if(player.moveDirection.y < 0 && player.currentOneWayPlatform != null)
        {
            player.rb.AddForce(new Vector2(0, player.platformDropStrength), ForceMode2D.Impulse);
            player.currentOneWayPlatform.layer = 11;
            player.currentOneWayPlatform.GetComponent<Collider2D>().isTrigger = true;
        }
    }
}
