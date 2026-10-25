using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ActiveItem itemComponent;

    [Header("UI Instantiation Setup")]
    [Tooltip("The generic draggable UI Prefab that holds the UIDraggableItem script")]
    [SerializeField] private GameObject uiItemPrefab;

    private FirstPersonController playerController;

    private void Awake()
    {
        playerController = FindFirstObjectByType<FirstPersonController>();
    }

    public string InteractionPrompt => (itemComponent != null && itemComponent.Data != null)
        ? $"Pick up {itemComponent.Data.itemName}"
        : "Pick up item";

    public void Interact(PlayerControls player)
    {
        if (itemComponent == null || itemComponent.Data == null || uiItemPrefab == null) return;

        if (player.TryGetComponent(out PlayerHand hand))
        {
            if (hand.TryEquipItem(itemComponent))
            {
                playerController.toggleInventory();
                // Find the Hand UI Slot in the scene
                HandDropZone handDropZone = FindFirstObjectByType<HandDropZone>();

                if (handDropZone != null)
                {
                    // Spawn the UI Prefab as a child of the Hand UI Slot
                    GameObject spawnedUI = Instantiate(uiItemPrefab, handDropZone.transform);

                    // Pass the data directly into the UIDraggableItem script
                    if (spawnedUI.TryGetComponent(out UIDraggableItem draggable))
                    {
                        draggable.InitializeFromPickup(itemComponent.Data, itemComponent);
                    }
                }
                // Close the inventory panel. This should be done quickly so the player doesn't see the inventory open and close again.
                // Maybe consider changing this to involve a solution that doesn't involve toggling the inventory state.
                playerController.toggleInventory();

                Debug.Log($"Picked up {itemComponent.Data.itemName} into hands and instantiated UI.");
            }
            else
            {
                Debug.Log("Hands full! Open inventory and stow your item first.");
            }
        }
    }
}