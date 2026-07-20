using UnityEngine;

public class InteractableLever : MonoBehaviour, IFocusable
{
    [Header("Position Settings")]
    [SerializeField] private Transform leverHandle;
    [SerializeField] private float dragSpeed = 0.2f;
    [SerializeField] private float minX = 0.3f;
    [SerializeField] private float maxX = -0.3f;


    public string InteractionPrompt => "Pull Lever";

    private bool isFocused = false;
    private PlayerControls playerRef;
    private bool leverFinished = false;

    [SerializeField] private GameObject gateToOpen;

    public void Interact(PlayerControls player)
    {
        if(!leverFinished) OnFocus(player);
    }

    public void OnFocus(PlayerControls player)
    {
        isFocused = true;
        playerRef = player;

        // Unlock Mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnUpdateInteraction(float inputDelta)
    {
        if (!isFocused) return;

        // Calculate position change
        float xChange = inputDelta * dragSpeed * Time.deltaTime;

        // Get current local position
        Vector3 currentPos = leverHandle.localPosition;

        // Apply and Clamp
        currentPos.x = Mathf.Clamp(currentPos.x - xChange, maxX, minX);

        leverHandle.localPosition = currentPos;

        // Check if pulled to the bottom
        // We use a small buffer (e.g., 0.05f) to ensure it triggers if it's "close enough" to the bottom
        if (Mathf.Abs(currentPos.x + maxX) < 0.05f)
        {
            leverFinished = true;
            OpenGate();
            OnUnfocus();
        }
    }

    public void OnUnfocus()
    {
        isFocused = false;

        // Lock mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerRef != null)
        {
            var controller = playerRef.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.controlsLocked = false;
            }
        }

        Debug.Log("Lever Unfocused: Mouse Locked");
    }

    private void OpenGate()
    {
        // Simply Destroys the Gate Object. Later on implement opening.
        if (gateToOpen != null) Destroy(gateToOpen);
    }
}