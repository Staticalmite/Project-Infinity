using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [Header("Hand Anchor Configuration")]
    [SerializeField] private Transform handSlotAnchor; // A child transform nested under your Main Camera

    public ActiveItem CurrentlyHeldItem { get; private set; }
    public bool IsHandOccupied => CurrentlyHeldItem != null;

    public bool TryEquipItem(ActiveItem newItem)
    {
        if (IsHandOccupied) return false;

        CurrentlyHeldItem = newItem;

        // Reparent the 3D item to your camera's hand slot anchor location
        Transform itemTransform = CurrentlyHeldItem.transform;
        itemTransform.SetParent(handSlotAnchor);
        itemTransform.localPosition = Vector3.zero;
        itemTransform.localRotation = Quaternion.Euler(newItem.TargetHandRotation);

        // Disable standard physics and colliders while held
        if (CurrentlyHeldItem.TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
        if (CurrentlyHeldItem.TryGetComponent(out Collider col)) col.enabled = false;

        CurrentlyHeldItem.OnEquip();
        return true;
    }

    public ActiveItem ClearHandSlot()
    {
        if (!IsHandOccupied) return null;

        ActiveItem itemLeavingHand = CurrentlyHeldItem;
        itemLeavingHand.OnUnequip();

        CurrentlyHeldItem = null;
        return itemLeavingHand;
    }

    public void ExecuteHandAction()
    {
        if (IsHandOccupied)
        {
            CurrentlyHeldItem.OnUseAction();
        }
    }
}