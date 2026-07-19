using UnityEngine;


public abstract class ActiveItem : MonoBehaviour
{
    [Header("Data Reference")]
    [SerializeField] private ItemData data;
    [Header("Hand Placement Configuration")]
    [Tooltip("The Euler angle rotation this item needs to face forward properly")]
    [SerializeField] private Vector3 targetHandRotation;
    // Read-only property to safely access the needed info from other scripts
    public ItemData Data => data;
    public Vector3 TargetHandRotation => targetHandRotation;

    // Abstract methods that must be implemented by subclasses, anything using this class must implement these methods.

    // <summary> What happens when the item is used</summary>
    public abstract void OnUseAction();

    // <summary> What happens when the item is equipped</summary>
    public abstract void OnEquip();

    // <summary> What happens when the item is unequipped</summary>
    public abstract void OnUnequip();


}