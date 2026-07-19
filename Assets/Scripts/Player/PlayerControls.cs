using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    // Public read-only accessors for other scripts to read input states
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool IsSprinting { get; private set; }

    private PlayerInputActions playerActions;
    private FirstPersonController firstPersonController;
    private PlayerHand playerHand;

    void Awake()
    {
        playerActions = new PlayerInputActions();
        firstPersonController = GetComponent<FirstPersonController>();
        playerHand = GetComponent<PlayerHand>();
    }

    void OnEnable()
    {
        playerActions.Enable();

        // Bind callbacks to update our state variables
        playerActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        playerActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        playerActions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        playerActions.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        playerActions.Player.Sprint.performed += ctx => IsSprinting = true;
        playerActions.Player.Sprint.canceled += ctx => IsSprinting = false;

        playerActions.Player.Interact.performed += Handle_Interact;
        playerActions.Player.HandAction.performed += Handle_HandAction;
        playerActions.Player.ToggleInventory.performed += Handle_InventoryToggle;

    }

    void OnDisable()
    {
        playerActions.Disable();

        playerActions.Player.Interact.performed -= Handle_Interact;
        playerActions.Player.HandAction.performed += Handle_HandAction;
    }

    private void Handle_HandAction(InputAction.CallbackContext context)
    {
        playerHand.ExecuteHandAction();
    }

    void OnDestroy()
    {
        playerActions.Dispose();
    }

    private void Handle_Interact(InputAction.CallbackContext context)
    {
        firstPersonController.PerformInteractionCheck();
    }

    private void Handle_InventoryToggle(InputAction.CallbackContext context)
    {
        firstPersonController.toggleInventory();
    }
}