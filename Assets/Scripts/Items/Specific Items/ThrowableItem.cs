using Unity.VisualScripting;
using UnityEngine;

public class ThrowableItem : ActiveItem
{

    private PlayerHand playerHand;
    [SerializeField] private GameObject shatterPrefab;
    private bool hasThrown = false;

    private void Start()
    {
        playerHand = FindFirstObjectByType<PlayerHand>();
    }

    public override void OnEquip() => gameObject.SetActive(true);
    public override void OnUnequip() => gameObject.SetActive(false);
    public override void OnDrop()
    {
        gameObject.SetActive(true);
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
    }
    public override void OnUseAction()
    {
        // Call the drop method to detach the item from the player's hand and enable physics.
        OnDrop();

        // Update the player's hand state to reflect that the item is no longer held.
        playerHand.DropItem();

        // Once we've detached, we can apply a force to throw the item in the direction the player is facing.
        rb.AddForce(Camera.main.transform.forward * 10f, ForceMode.Impulse);
        hasThrown = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(hasThrown)
        {
            // Instantiate the shatter effect at the point of collision.
            if (shatterPrefab != null)
            {
                Instantiate(shatterPrefab, transform.position, Quaternion.identity);
            }
            // Destroy this item after it has collided.
            Destroy(this.gameObject);
        }
    }
}
