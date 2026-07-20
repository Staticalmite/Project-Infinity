using UnityEngine;

public interface IInteractable
{
    // The message to display when the player can interact with this object.
    string InteractionPrompt { get; }

    // What happens when the player presses the key.
    void Interact(PlayerControls player);
}

public interface IFocusable : IInteractable
{
    void OnFocus(PlayerControls player);
    void OnUpdateInteraction(float inputDelta); // Used for lever Y-axis dragging
    void OnUnfocus();
}