using UnityEngine;

public class ShatterLifetimeManager : MonoBehaviour
{
    private float lifetime = 5f;
    private float currentLifetime = 0f;

    // A simple lifetime manager attached to the parent object of the shatter prefab. It will destroy the parent object after a set amount of time.
    void FixedUpdate()
    {
        currentLifetime += Time.fixedDeltaTime;
        if (currentLifetime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
