using UnityEngine;


public abstract class ActiveItem : MonoBehaviour
{
    [Header("Data Reference")]
    [SerializeField] private ItemData data;
    [Header("Hand Placement Configuration")]
    [Tooltip("The Euler angle rotation this item needs to face forward properly")]
    [SerializeField] private Vector3 targetHandRotation;
    [SerializeField] private UIDraggableItem linkedUIItem;

    // Read-only property to safely access the needed info from other scripts
    public ItemData Data => data;
    public Vector3 TargetHandRotation => targetHandRotation;

    protected Rigidbody rb;
    protected Collider col;

    // Abstract methods that must be implemented by subclasses, anything using this class must implement these methods.

    // <summary> What happens when the item is used</summary>
    public abstract void OnUseAction();

    // <summary> What happens when the item is equipped</summary>
    public abstract void OnEquip();

    // <summary> What happens when the item is unequipped</summary>
    public abstract void OnUnequip();

    // <summary> What happens when the item is dropped</summary>
    public void OnDrop()
    {
        gameObject.SetActive(true);
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
        if (linkedUIItem != null)
        {
            Destroy(linkedUIItem.gameObject);
        }
        else
        {
            Debug.LogWarning("No Linked UI Item found for " + gameObject.name);
        }
    }

    // <summary> Sets the linked UI item for this physical item</summary>
    public void SetLinkedUIItem(UIDraggableItem uiItem)
    {
        linkedUIItem = uiItem;
    }

    // Base Awake handles component caching automatically for the rigidbody and collider, so subclasses don't have to worry about it.
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb == null)
            Debug.LogWarning($"{gameObject.name}: ActiveItem is missing a Rigidbody.", this);
        if (col == null)
            Debug.LogWarning($"{gameObject.name}: ActiveItem is missing a Collider.", this);
    }

}