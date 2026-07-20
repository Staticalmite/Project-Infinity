using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class UIDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    [SerializeField] private Transform originalParent;
    private PlayerHand playerHand;
    private Image uiImage;
    private bool isStowedInInventory = false;

    // Read-only ItemData property to expose the currently held item data for external access
    public ItemData CurrentItemData {  get; private set; }

    // Public method to set the linked physical item, allowing external scripts to establish the connection
    public void SetLinkedPhysicalItem(ActiveItem item)
    {
        LinkedPhysicalItem = item;
    }

    // Read-only property to expose the linked physical item for external access
    public ActiveItem LinkedPhysicalItem { get; private set; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        playerHand = FindFirstObjectByType<PlayerHand>();
        uiImage = GetComponent<Image>();
    }

    void OnEnable()
    {
        if (isStowedInInventory) return;

        // Set up the icon from the hand if it exists upon layout activation
        if (playerHand != null && playerHand.IsHandOccupied)
        {
            CurrentItemData = playerHand.CurrentlyHeldItem.Data;
            uiImage.sprite = CurrentItemData.itemIcon;
            uiImage.enabled = true;
        }
        else if (CurrentItemData == null)
        {
            uiImage.enabled = false;
        }
    }

    public void InitializeFromPickup(ItemData data, ActiveItem physicalItem)
    {
        CurrentItemData = data;
        uiImage.sprite = data.itemIcon;
        uiImage.enabled = true;
        LinkedPhysicalItem = physicalItem;

        // Reset position inside the Hand UI Slot layout parent
        rectTransform.localPosition = Vector3.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CurrentItemData == null) return;

        // Make the icon semi-transparent and allow it to be dragged without blocking raycasts
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Move the icon to the top of the canvas hierarchy so it appears above other UI elements
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the icon with the mouse, adjusting for canvas scale factor
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Not needed as inventory is entire drop zone. Keep if needed for future inventory changes
        //// If not dropped onto a valid dropzone container, snap it back home
        //if (transform.parent == canvas.transform)
        //{
        //    transform.SetParent(originalParent);
        //    rectTransform.position = originalPosition;
        //}
    }

    public void OnSuccessfulStow(Transform inventoryPanel)
    {
        isStowedInInventory = true;
        transform.SetParent(inventoryPanel);
        canvasGroup.blocksRaycasts = true;
    }

    public void OnReturnedToHand(Transform handUIslot)
    {
        // Reset the UI icon back to the hand anchor.
        isStowedInInventory = false;
        transform.SetParent(handUIslot);
        rectTransform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = true;
    }

}