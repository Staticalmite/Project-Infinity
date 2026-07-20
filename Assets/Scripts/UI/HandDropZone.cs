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
            ItemData itemToEquip = draggedItemUI.CurrentItemData;

            if (itemToEquip != null && itemToEquip.physicalPrefab != null)
            {
                // Re-instantiate the Object
                GameObject newPhysicalObject = Instantiate(itemToEquip.physicalPrefab);
                ActiveItem activeItemComponent = newPhysicalObject.GetComponent<ActiveItem>();

                // Try to pass it into the player's hands.
                if (playerHand.TryEquipItem(activeItemComponent))
                {
                    draggedItemUI.SetLinkedPhysicalItem(activeItemComponent);

                    // Remove the item from the inventory.
                    playerInventory.RemoveItem(itemToEquip);
                    draggedItemUI.OnReturnedToHand(transform);
                }
                else
                {
                    // Clean it up if it failed for any reason.
                    Destroy(newPhysicalObject);
                }
            }

        }
    }

}
