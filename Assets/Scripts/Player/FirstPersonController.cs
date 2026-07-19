using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerControls))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upperLookLimit = -80f;
    [SerializeField] private float lowerLookLimit = 80f;

    [Header("FOV Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 75f;
    [SerializeField] private float fovChangeSpeed = 8f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer; // The raycast will ignore everything not on this layer

    private CharacterController characterController;
    private PlayerControls inputProvider;

    private Vector3 velocity;
    private float verticalRotation = 0f;
    public bool controlsLocked = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputProvider = GetComponent<PlayerControls>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = normalFOV;
        }
    }

    void Update()
    {
        if (controlsLocked) return;

        HandleRotation();
        HandleMovement();
        HandleFOV();
    }

    private void HandleMovement()
    {
        Vector2 input = inputProvider.MoveInput;

        // Only allow sprinting if moving forward.
        bool isMovingForward = input.y > 0;
        float currentSpeed = (inputProvider.IsSprinting && isMovingForward) ? sprintSpeed : walkSpeed;

        Vector3 moveDirection = transform.forward * input.y + transform.right * input.x;
        Vector3 movement = moveDirection * currentSpeed;

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        movement.y = velocity.y;
        characterController.Move(movement * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (playerCamera == null) return;

        Vector2 look = inputProvider.LookInput;
        transform.Rotate(Vector3.up * look.x * mouseSensitivity);

        verticalRotation -= look.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, upperLookLimit, lowerLookLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleFOV()
    {
        if (playerCamera == null) return;

        // Determine if player is actively moving forward and holding sprint
        bool isMovingForward = inputProvider.MoveInput.y > 0;
        float targetFOV = (inputProvider.IsSprinting && isMovingForward) ? sprintFOV : normalFOV;

        // Smoothly transition between the two FOV states
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }

    public void PerformInteractionCheck()
    {
        if (playerCamera == null || controlsLocked) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionDistance, Color.red, 10f);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(inputProvider);
            }
        }
    }
}