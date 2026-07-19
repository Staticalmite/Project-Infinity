using UnityEngine;

public interface IInteractable
{
    // The message to display when the player can interact with this object.
    string InteractionPrompt { get; }

    // What happens when the player presses the key.
    void Interact(PlayerControls player);
}
