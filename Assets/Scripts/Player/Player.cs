using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [NonSerialized] public MenuController menuController;
    [NonSerialized] public PlayerUI playerUI;

    [NonSerialized] public Controls controls;
    private InputAction moveInput;

    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public BoxCollider2D playerCollider;
    [NonSerialized] public Health health;
    [NonSerialized] private SpriteRenderer spriteRenderer;
    [NonSerialized] private Camera cam;
    private Transform playerArm;

    [Header("Movement")]
    public float movementSpeed;
    public int maxFallSpeed;
    public float groundIntoAirOffset;
    [NonSerialized] public float groundIntoAirTimer;
    [NonSerialized] public Vector2 moveDirection;
    [NonSerialized] public Vector2 playerVelocity;
    [NonSerialized] public bool faceRight;
    [NonSerialized] public float baseGravityScale;
    public LayerMask groundCheckLayer;
    public int playerGroundDrag;
    //[NonSerialized] public MovingPlatform movingPlatform;
    //[NonSerialized] public bool abilityGroundCheck;

    [Header("Jump")]
    public float jumpStrength;
    public int maxJumpCount;
    [NonSerialized] public int currentJumpCount;
    public float maxJumpTime;
    [NonSerialized] public float jumpTimer;
    [NonSerialized] public bool jumpPerformed;

    [Header("Dash")]
    public float dashTime;
    public float dashStrength;
    public int dashCosts;
    [SerializeField] private int maxEnergy;
    private int currentEnergy;
    [SerializeField] private float energyRestoreInterval;
    public bool iframesWhileDash;

    public int EnergyValue
    {
        get { return currentEnergy; }
        set { currentEnergy = Math.Min(Math.Max(0, value), maxEnergy); }
    }

    public int EnergyMaxValue
    {
        get { return maxEnergy; }
        set { maxEnergy = Math.Max(0, value); currentEnergy = Math.Min(value, currentEnergy); }
    }

    [Header("IFrames")]
    public float iFramesDuration;
    private float iFramesTimer;
    [SerializeField] private float iFramesBlinkSpeed;
    private float iFramesBlinkTimer;
    [NonSerialized] public bool iFramesBlink;
    [NonSerialized] public bool iframesActive;

    [Header("Attack")]
    public float attackCooldown;
    public GameObject playerProjectile;
	public float projectileSpeed;
    [NonSerialized] public Vector2 attackDirection;
    [Range(0.0f, 1)]
    public float maxAttackAngle;

    //Fly
    [NonSerialized] public bool isFlying;

    [Header("Platform")]
    [SerializeField] private LayerMask platformLayer;
    public float platformDropStrength;
    [NonSerialized] public GameObject currentOneWayPlatform;
    //Platform

    [Header("Other")]
    public Transform projectileSpawnPosition;
    [NonSerialized] public Vector3 mousePosition;
    public bool rotateWithMouse;
    [NonSerialized] public bool bossDefeated;
    [NonSerialized] public bool playerIsDead;

    //Animations
    [NonSerialized] public Animator currentAnimator;
    [NonSerialized] public string currentstate;
    const string deathState = "Death";
    const string flyState = "Fly";

    //Interaction
    [NonSerialized] public List<IInteractables> interactables = new List<IInteractables>();
    [NonSerialized] public IInteractables currentInteractable;
    public IInteractables closestInteraction;

    [NonSerialized] public PlayerMovement playerMovement = new PlayerMovement();
    [NonSerialized] public PlayerCollision playerCollision = new PlayerCollision();
    [NonSerialized] public PlayerAbilities playerAbilities = new PlayerAbilities();
    [NonSerialized] public PlayerInteraction playerInteraction = new PlayerInteraction();

    [Space]
    public States state;
    public enum States
    {
        Ground,
        GroundIntoAir,
        Air,
        Dash,
        Death,
        Attack,
        Emtpy,
        Fly,
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        controls = Keybindinputmanager.Controls;
        moveInput = controls.Player.Move;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        playerArm = transform.GetChild(0).transform;
        currentAnimator = GetComponent<Animator>();

        baseGravityScale = rb.gravityScale;

        playerMovement.player = this;
        playerCollision.player = this;
        playerAbilities.player = this;
        playerInteraction.player = this;

        if (SceneManager.GetActiveScene().buildIndex == (int)GameScenes.TitelScreen)
        {
            gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        menuController = GameManager.Instance.menuController;
        playerUI = GameManager.Instance.playerUI;

        state = States.Air;
        if (health != null) health.dieEvent.AddListener(OnDeath);

        EnergyUpdate(EnergyMaxValue);
        InvokeRepeating("EnergyInterval", energyRestoreInterval, energyRestoreInterval);
    }
    private void OnEnable()
    {
        controls.Enable();
        EnableInputs(true);
    }
    private void OnDisable()
    {
        controls.Disable();
        EnableInputs(false);
    }
    public void EnableInputs(bool enabled)
    {
        if (enabled && this.enabled)
        {
            controls.Player.Jump.performed += playerMovement.JumpInput;
            controls.Player.Dash.performed += playerMovement.DashInput;
            //controls.Player.Fly.performed += playerMovement.FlyInput;
            controls.Player.Interact.performed += playerInteraction.InteractInput;
        }
        else
        {
            controls.Player.Jump.performed -= playerMovement.JumpInput;
            controls.Player.Dash.performed -= playerMovement.DashInput;
            //controls.Player.Fly.performed -= playerMovement.FlyInput;
            controls.Player.Interact.performed -= playerInteraction.InteractInput;
        }
    }
    private void FixedUpdate()
    {
        if (menuController.gameIsPaused) return;

        switch (state)
        {
            case States.Emtpy:
                break;
            case States.Ground:
                playerMovement.GroundMovement();
                break;
            case States.GroundIntoAir:
                playerMovement.AirMovement();
                break;
            case States.Air:
                playerMovement.AirMovement();
                break;
            case States.Dash:
                playerMovement.DashMovement();
                break;
            case States.Fly:
                playerMovement.FlyMovement();
                break;
        }
    }
    private void Update()
    {
        if (menuController.gameIsPaused) return;

        ReadMovementInput();
        GunRotation();
        playerAbilities.AttackInput();
        //playerInteraction.InteractionUpdate();

        switch (state)
        {
            case States.Emtpy:
                break;
            case States.Ground:
                playerCollision.GroundCheck();
                playerMovement.PlatformDropInput();
                if (rotateWithMouse) playerMovement.RotatePlayerToMouse();
                else playerMovement.RotatePlayer();
                break;
            case States.GroundIntoAir:
                playerMovement.JumpIsPressed();
                playerMovement.GroundIntoAirTransition();
                playerCollision.AirCheck();
                if (rotateWithMouse) playerMovement.RotatePlayerToMouse();
                else playerMovement.RotatePlayer();
                break;
            case States.Air:
                playerMovement.JumpIsPressed();
                playerCollision.AirCheck();
                if (rotateWithMouse) playerMovement.RotatePlayerToMouse();
                else playerMovement.RotatePlayer();
                break;
            case States.Dash:
                playerMovement.DashTime();
                break;
            case States.Fly:
                playerMovement.RotatePlayerToMouse();
                break;
        }
    }
    private void ReadMovementInput()
    {
        moveDirection.x = moveInput.ReadValue<Vector2>().x;
        moveDirection.y = moveInput.ReadValue<Vector2>().y;
    }
    private void GunRotation()
    {
        mousePosition = cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, -cam.transform.position.z));
        mousePosition.z = 0;

        attackDirection = ((Vector2)mousePosition - (Vector2)playerArm.transform.position).normalized;

        if (faceRight) attackDirection *= -1;
        if (attackDirection.x < maxAttackAngle) attackDirection.x = maxAttackAngle;

        playerArm.transform.right = attackDirection;
    }
    public void SwitchToGround(bool onlyResetValues)
    {
        rb.gravityScale = baseGravityScale;
        currentJumpCount = 0;

        jumpPerformed = false;

        if (onlyResetValues == false)
        {
            state = States.Ground;
        }
    }
    public void SwitchGroundIntoAir()
    {
        groundIntoAirTimer = 0;
        state = States.GroundIntoAir;
    }
    public void SwitchToAir()
    {
        if (currentJumpCount == 0) currentJumpCount++;
        rb.gravityScale = baseGravityScale;

        state = States.Air;
    }
    public void SwitchToFly()
    {

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        ChangeAnimationState(flyState);
        isFlying = true;
        state = States.Fly;
    }
    public void JumpPad(float jumpPadStrength) => playerMovement.JumpPad(jumpPadStrength);
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (currentAnimator == null) return;

        currentAnimator.CrossFadeInFixedTime(newstate, 0.1f);
    }
    public void CreatePrefab(GameObject obj, Transform spawnPosition, Quaternion rotation)
    {
        Projectile prefab = PoolingSystem.SpawnObject(obj, spawnPosition.transform.position, rotation, PoolingSystem.ProjectileType.Player).GetComponent<Projectile>();
        Vector2 direction = attackDirection;
        if (faceRight) direction *= -1;

        prefab.FireProjectileLinear(direction, projectileSpeed);


    }
    public void IFramesStart()
    {
        iframesActive = true;
        iFramesTimer = 0;
        iFramesBlink = false;
        iFramesBlinkTimer = 0;
        StartCoroutine(IFrames());
    }
    IEnumerator IFrames()
    {
        while (iFramesTimer < iFramesDuration)
        {
            iFramesTimer += Time.deltaTime;
            iFramesBlinkTimer += Time.deltaTime;

            if (iFramesBlinkTimer >= iFramesBlinkSpeed)
            {
                iFramesBlinkTimer = 0;
                iFramesBlink = !iFramesBlink;

                if (iFramesBlink) spriteRenderer.color = Color.red;
                else spriteRenderer.color = Color.white;
            }
            yield return null;
        }

        iFramesBlink = false;
        spriteRenderer.color = Color.white;
        iframesActive = false;

    }
    private void EnergyInterval()
    {
        EnergyUpdate(1);
    }
    public void EnergyUpdate(int amount)
    {
        EnergyValue += amount;
        playerUI.EnergyUIUpdate(EnergyValue, EnergyMaxValue);
    }
    private void OnDeath()
    {
        //animation
        playerIsDead = true;
        rb.linearVelocity = Vector2.zero;
        ChangeAnimationState(deathState);
        state = States.Death;

        playerUI.GameOver();
        //AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilityFiles[(int)AudioManager.UtilitySounds.PlayerDeath]);
    }
    public void RestartGame()
    {
        menuController.ResetPlayer(false);
    }
    public void StopPlayerControls()
    {
        Player.Instance.rb.linearVelocity = Vector2.zero;
        Player.Instance.SwitchToGround(true);
        Player.Instance.ChangeAnimationState("Idle");
        Player.Instance.state = Player.States.Ground;

        GameManager.Instance.menuController.gameIsPaused = true;
    }
    public void StartPlayerControls()
    {
        GameManager.Instance.menuController.gameIsPaused = false;
    }
    public void AttackTimeReset() => playerAbilities.AttackTimeReset();
    public void PlayFootStep1() => AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.playerSounds[(int)AudioManager.PlayerSounds.Footstep1]);
    public void PlayFootStep2() => AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.playerSounds[(int)AudioManager.PlayerSounds.Footstep2]);
}
