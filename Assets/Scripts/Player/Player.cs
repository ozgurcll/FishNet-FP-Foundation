using FishNet.Object;
using UnityEngine;
using FishNet.Component.Animating;
using System;
using FishNet.Connection;


public class Player : NetworkBehaviour
{
    #region Components
    [Header("Components")]
    public Rigidbody rb { get; private set; }
    public NetworkAnimator anim { get; private set; }
    #endregion

    #region Input
    public PlayerInput controls { get; private set; }
    public Vector2 moveInput { get; private set; }
    public PlayerMotor motor { get; private set; }
    #endregion

    #region Reference
    public PlayerInteractor interactor { get; private set; }
    public PlayerInventory inventory { get; private set; }
    public PlayerEquipment equipment { get; private set; }
    public PlayerItemUse itemUse { get; private set; }
    public PlayerHealth health { get; private set; }
    public PlayerStamina stamina { get; private set; }
    #endregion

    #region State Machine
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerGroundedState groundedState { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerSprintState sprintState { get; private set; }
    public PlayerCrouchState crouchState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    #region Camera Events
    public event Action<float> OnStartWalking;
    public event Action<float> OnStartSprinting;
    public event Action OnStopMoving;
    #endregion

    void Awake()
    {
        anim = GetComponent<NetworkAnimator>();
        rb = GetComponent<Rigidbody>();

        motor = GetComponent<PlayerMotor>();

        interactor = GetComponent<PlayerInteractor>();
        inventory = GetComponent<PlayerInventory>();
        equipment = GetComponent<PlayerEquipment>();
        itemUse = GetComponent<PlayerItemUse>();
        health = GetComponent<PlayerHealth>();
        stamina = GetComponent<PlayerStamina>();

        controls = new PlayerInput();
        AssignInputEvents();

        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        sprintState = new PlayerSprintState(this, stateMachine, "Sprint");
        crouchState = new PlayerCrouchState(this, stateMachine, "Crouch");
        deadState = new PlayerDeadState(this, stateMachine, "Dead");
    }

    // FishNet'te Start yerine bunu kullanıyoruz
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (base.Owner.IsLocalClient)
        {
            Cursor.lockState = CursorLockMode.Locked;
            controls.Enable();

            UI_Inventory ui = FindFirstObjectByType<UI_Inventory>();
            if (ui != null)
                ui.Initialize(inventory);

            UI_InGame staminaUI = FindFirstObjectByType<UI_InGame>();
            if (staminaUI != null)
                staminaUI.Initialize(stamina);

            stamina.OnStaminaExhausted += ForceStopSprint;
        }
        else
        {
            controls.Disable();
        }

        stateMachine.Initialize(idleState);
    }



    void Update()
    {
        if (!base.IsOwner) return; // Sadece lokal oyuncu state günceller
        stateMachine.currentState.Update();
    }

    void FixedUpdate()
    {
        if (!base.IsOwner) return; // Sadece lokal oyuncu fizik uygular
        stateMachine.currentState.FixedUpdate();
    }

    private void AssignInputEvents()
    {
        #region Walk Input Events
        controls.Player.Move.performed += context =>
        {
            moveInput = context.ReadValue<Vector2>();

            stateMachine.ChangeState(moveState);

            OnStartWalking?.Invoke(moveInput.x);
        };

        controls.Player.Move.canceled += context =>
        {
            moveInput = Vector2.zero;

            stateMachine.ChangeState(idleState);

            OnStopMoving?.Invoke();
        };
        #endregion

        #region Sprint Input Events
        controls.Player.Sprint.performed += context =>
        {
            motor.SetSprint(true);

            stateMachine.ChangeState(sprintState);

            stamina.StartDepleting();

            OnStartSprinting?.Invoke(moveInput.x);
        };
        controls.Player.Sprint.canceled += context =>
        {
            motor.SetSprint(false);
            stamina.StartRegenerating();
        };
        #endregion

        #region Jump Input Events
        controls.Player.Jump.performed += context =>
        {
            if (motor.IsGrounded())
            {
                stateMachine.ChangeState(jumpState);
            }
        };
        #endregion

        #region Crouch Input Events
        controls.Player.Crouch.performed += context =>
        {
            motor.SetCrouch(true);
            stateMachine.ChangeState(crouchState);
        };

        controls.Player.Crouch.canceled += context =>
        {
            motor.SetCrouch(false);
        };
        #endregion

        #region Inventory Input Events
        controls.Inventory.Slot1.performed += context => inventory.SelectSlot(0);
        controls.Inventory.Slot2.performed += context => inventory.SelectSlot(1);
        controls.Inventory.Slot3.performed += context => inventory.SelectSlot(2);
        controls.Inventory.Slot4.performed += context => inventory.SelectSlot(3);
        #endregion

        #region Interaction Input Events
        controls.Player.Interact.started += context =>
        {
            interactor.StartInteract();
        };

        controls.Player.Interact.performed += context =>
        {
            interactor.CompleteInteract();
        };

        controls.Player.Interact.canceled += context =>
        {
            interactor.CancelInteract();
        };
        #endregion

        #region Drop Input Events
        controls.Player.Drop.started += context => equipment.StartDrop();
        controls.Player.Drop.performed += context => equipment.CompleteDrop();
        controls.Player.Drop.canceled += context => equipment.CancelDrop();
        #endregion

        #region Use Item Input Events
        controls.Player.Attack.performed += context => itemUse.TryUseCurrentItem();
        #endregion
    }

    private void ForceStopSprint()
    {
        motor.SetSprint(false);
        if (moveInput != Vector2.zero) stateMachine.ChangeState(moveState);
        else stateMachine.ChangeState(idleState);

        OnStartWalking?.Invoke(moveInput.x); // Kamerayı normale döndür
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();
}