using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName = "Generic Item";
    [TextArea] public string itemDescription;
    public Sprite itemIcon; // The 2D sprite image used for dragging in the UI
}