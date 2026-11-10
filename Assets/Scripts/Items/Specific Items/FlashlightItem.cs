using UnityEngine;

public class FlashlightItem : ActiveItem
{
    [SerializeField] private Light flashlightLight;

    public override void OnEquip() => gameObject.SetActive(true);
    public override void OnUnequip() => gameObject.SetActive(false);

    public override void OnUseAction()
    {
        // Toggle the flashlight's light on and off when the item is used
        if (flashlightLight != null)
        {
            flashlightLight.enabled = !flashlightLight.enabled;
            Debug.Log($"{Data.itemName} toggled!");
        }
    }
    public override void OnReleaseAction()
    {
        // No specific action on release for the flashlight
    }
}
