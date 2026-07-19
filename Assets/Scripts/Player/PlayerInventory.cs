using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // A dynamic list tracking all items stowed in the inventory slotlessly
    private List<ItemData> stowedItems = new List<ItemData>();
    public int ItemCount => stowedItems.Count;

    public void StowItem(ItemData item)
    {
        stowedItems.Add(item);
        Debug.Log($"Stowed {item.itemName} into inventory stash. Total items: {ItemCount}");
    }

    public void RemoveItem(ItemData item)
    {
        if (stowedItems.Contains(item))
        {
            stowedItems.Remove(item);
        }
    }
}