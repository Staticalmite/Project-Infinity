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
    private Transform originalParent;

    private PlayerHand playerHand;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        playerHand = FindFirstObjectByType<PlayerHand>();
    }

    void OnEnable()
    {
        // When opening the inventory screen UI, update this graphic automatically
        if (playerHand != null && playerHand.IsHandOccupied)
        {
            GetComponent<Image>().sprite = playerHand.CurrentlyHeldItem.Data.itemIcon;
            GetComponent<Image>().enabled = true;
        }
        else
        {
            GetComponent<Image>().enabled = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (playerHand == null || !playerHand.IsHandOccupied) return;

        originalPosition = rectTransform.position;
        originalParent = transform.parent;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // Lets the raycast pass through this icon to hit the drop zone panel behind it

        transform.SetParent(canvas.transform); // Move to the top layer layout so it draws above other graphics
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // If not dropped onto a valid dropzone container, snap it back home
        if (transform.parent == canvas.transform)
        {
            transform.SetParent(originalParent);
            rectTransform.position = originalPosition;
        }
    }

    public void OnSuccessfulStow(Transform inventoryPanel)
    {
        transform.SetParent(inventoryPanel);
        canvasGroup.blocksRaycasts = true;

        // This icon now lives permanently inside the loose stowed inventory panel area!
    }
}