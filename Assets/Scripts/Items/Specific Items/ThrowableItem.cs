using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ThrowableItem : ActiveItem
{

    private PlayerHand playerHand;
    private FirstPersonController firstPersonController;
    [Header("Shatter Effect")]
    [SerializeField] private GameObject shatterPrefab;
    private bool hasThrown = false;
    [Header("Hold and Throw settings.")]
    [SerializeField] private float maxHoldTime = 1.0f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float minimumThrowForce = 0.1f;
    [SerializeField] private float shakeSpeed = 20f;
    private float holdTimer = 0f;
    private bool isHolding = false;

    // UI Elements
    private Image holdTimerImage;

    private Vector3 originalLocalPosition;

    private void Start()
    {
        firstPersonController = FindFirstObjectByType<FirstPersonController>();
        playerHand = FindFirstObjectByType<PlayerHand>();
        holdTimerImage = GameObject.FindGameObjectWithTag("Hold Timer Image").GetComponent<Image>();

        // Cache the base position of the item in the hand
        originalLocalPosition = transform.localPosition;
    }


    private void Update()
    {
       // Handle the hold timer incrementing while the use action is being held down.
        if (isHolding)
        {
            // Increment the hold timer while the item is being held.
            holdTimer = Mathf.Clamp(holdTimer + Time.deltaTime, 0f, maxHoldTime);
            Debug.Log($"Hold Timer: {holdTimer}");

            // Update the UI to reflect the current hold timer value.
            holdTimerImage.fillAmount = holdTimer / maxHoldTime;

            Shake(); // Call the Shake method to create a shaking effect while holding the item.
        }
        ;
    }

    private void Shake()
    {
        // Calculate a small offset based on the hold timer and shake speed to create a shaking effect.
        float offsetX = Mathf.Sin(Time.time * (holdTimer / shakeSpeed)) * Random.Range(0.001f, 0.02f);
        float offsetY = Mathf.Sin(Time.time * (holdTimer / shakeSpeed)) * Random.Range(0.001f, 0.02f);

        // Apply the calculated offset to the item's local position to create the shaking effect.
        transform.localPosition = originalLocalPosition + new Vector3(offsetX, offsetY, 0f);

    }
    public override void OnEquip()
    {
        gameObject.SetActive(true);
        holdTimer = 0f; // Reset the hold timer when the item is equipped.
        holdTimerImage.fillAmount = 0f; // Reset the UI fill amount.

        // Recache the local position when equipped
        originalLocalPosition = transform.localPosition;
    }
    public override void OnUnequip() => gameObject.SetActive(false);
    public override void OnUseAction()
    {
        // Start the hold timer when the player begins holding the use action and reduce the speed.
        isHolding = true;
        firstPersonController.otherSpeedMultiplier = 0.1f;
    }

    public override void OnReleaseAction()
    {
        // Set isHolding to false.

        isHolding = false;
        // Call the drop method to detach the item from the player's hand and enable physics.
        OnDrop();

        // Update the player's hand state to reflect that the item is no longer held.
        playerHand.DropItem();

        // Once we've detached, we can apply a force to throw the item in the direction the player is facing.
        if(holdTimer <= minimumThrowForce)
        {
            holdTimer = minimumThrowForce; // Makes it so that we have at least a little bit of force applied even if the player just taps the throw button.
        }
        // Apply the throw force based on the hold timer and the predefined throw force.
        // If the hold timer is greater than 1, we divide the throw force by the hold timer to prevent the throw from being too strong.
        if(holdTimer >= 1f)
        {
            rb.AddForce(Camera.main.transform.forward * (throwForce / holdTimer), ForceMode.Impulse);
            Debug.Log($"Item thrown with force: {throwForce / holdTimer}");
        }
        else
        {
            rb.AddForce(Camera.main.transform.forward * (throwForce * holdTimer), ForceMode.Impulse);
            Debug.Log($"Item thrown with force: {throwForce * holdTimer}");
        }
        hasThrown = true;

        // Restore the player's speed multiplier to normal after throwing.
        firstPersonController.otherSpeedMultiplier = 1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(hasThrown)
        {
            // Instantiate the shatter effect at the point of collision.
            if (shatterPrefab != null)
            {
                Instantiate(shatterPrefab, transform.position, Quaternion.identity);
            };

            if(collision.gameObject.CompareTag("Enemy"))
            {
                print("Collided with Enemy");
                // If the item collides with an enemy, apply damage to the enemy.
                EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.Stun(10f); // Adjust damage value as needed.
                }
            }
            // Destroy this item after it has collided.
            Destroy(this.gameObject);
        }
    }
}
