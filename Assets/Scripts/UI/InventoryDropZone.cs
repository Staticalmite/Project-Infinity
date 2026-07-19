using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDropZone : MonoBehaviour, IDropHandler
{
    private PlayerInventory playerInventory;
    private PlayerHand playerHand;

    void Awake()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
        playerHand = FindFirstObjectByType<PlayerHand>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out UIDraggableItem draggedItemUI))
        {
            if (playerHand == null || !playerHand.IsHandOccupied)
            {
                Debug.LogWarning("Cannot stow item: Player hands are empty!");
                return;
            }

            // Verify if the linked physical item matches the physical item in hand
            if (draggedItemUI.LinkedPhysicalItem != playerHand.CurrentlyHeldItem)
            {
                Debug.Log("This item is already stowed or doesn't match the item in your hand.");
                return;
            }

            ActiveItem physicalItem = playerHand.ClearHandSlot();

            if (physicalItem != null)
            {
                // Move item data into the backend pool
                playerInventory.StowItem(physicalItem.Data);

                // Destroy the physical 3D object inside the player's camera view
                Destroy(physicalItem.gameObject);

                // Relocate the UI graphic into this drop zone container panel 
                draggedItemUI.OnSuccessfulStow(transform);
            }
        }
    }
}