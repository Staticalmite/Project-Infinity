using UnityEngine;

public class CandleItem : ActiveItem
{
    [SerializeField] private Light candleLight;

    public override void OnEquip() => gameObject.SetActive(true);
    public override void OnUnequip() => gameObject.SetActive(false);

    public override void OnUseAction()
    {
        if (candleLight != null)
        {
            candleLight.range = Mathf.Clamp(candleLight.range + 3f, 0f, 30f); // Increase the range of the candle light by 1 unit
        }
    }

    void Update()
    {
        if (candleLight != null)
        {
            // Gradually decrease the range of the candle light over time
            candleLight.range = Mathf.Max(0f, candleLight.range - Time.deltaTime - 0.01f); 
        }
    }
}
