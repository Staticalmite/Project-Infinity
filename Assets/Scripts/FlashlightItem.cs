using UnityEngine;

public class FlashlightItem : ActiveItem
{
    [SerializeField] private Light flashlightLight;

    public override void OnEquip() => gameObject.SetActive(true);
    public override void OnUnequip() => gameObject.SetActive(false);

    public override void OnUseAction()
    {
        if (flashlightLight != null)
        {
            flashlightLight.enabled = !flashlightLight.enabled;
            Debug.Log($"{Data.itemName} toggled!");
        }
    }
}
