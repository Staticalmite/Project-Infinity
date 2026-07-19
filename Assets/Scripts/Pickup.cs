using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ActiveItem itemComponent;

    public string InteractionPrompt => (itemComponent != null && itemComponent.Data != null)
        ? $"Press E to pick up {itemComponent.Data.itemName}"
        : "Pick up item";

    public void Interact(PlayerControls player)
    {
        if (itemComponent == null || itemComponent.Data == null) return;

        if (player.TryGetComponent(out PlayerHand hand))
        {
            // Try to place this specific physical object directly into the player's hands
            if (hand.TryEquipItem(itemComponent))
            {
                Debug.Log($"Picked up {itemComponent.Data.itemName} into hands.");
            }
            else
            {
                Debug.Log("Hands full! Open inventory and stow your item first.");
            }
        }
    }
}