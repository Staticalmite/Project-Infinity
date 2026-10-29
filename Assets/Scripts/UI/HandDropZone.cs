using UnityEngine;
using UnityEngine.EventSystems;

public class HandDropZone : MonoBehaviour, IDropHandler
{
    private PlayerHand playerHand;
    private PlayerInventory playerInventory;

   
    void Awake()
    {
        playerHand = FindFirstObjectByType<PlayerHand>();
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        // Check if item is being dragged back to the hand.
        if(eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out UIDraggableItem draggedItemUI))
        {
            // Check if the player's hand is already occupied
            if (playerHand == null || playerHand.IsHandOccupied)
            {
                Debug.LogWarning("Cannot equip item: Player hands are already occupied!");
                return;
            }
            // Retrieve the item data from the inventory
            ItemData itemToEquipData = draggedItemUI.CurrentItemData;
            ActiveItem itemToEquip = draggedItemUI.LinkedPhysicalItem;

            if (itemToEquip != null)
            {
                // Try to pass it into the player's hands.
                if (playerHand.TryEquipItem(itemToEquip))
                {
                    draggedItemUI.SetLinkedPhysicalItem(itemToEquip);

                    // Remove the item from the inventory.
                    playerInventory.RemoveItem(itemToEquipData);
                    draggedItemUI.OnReturnedToHand(transform);
                }
                
            }

        }
    }

}
