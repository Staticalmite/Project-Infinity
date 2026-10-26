using UnityEngine;

public class ThrownObjectShatter : MonoBehaviour
{
    [SerializeField] float minShatterForce = 3f;
    [SerializeField] float maxShatterForce = 6f;

    Rigidbody rb;

    // We want to call awake instead of start as the object is instantiated during runtime.
    // Simply apply a random force to the object in the forward direction of the object when it is instantiated.
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        rb.AddForce(Random.Range(minShatterForce, maxShatterForce) * Vector3.forward, ForceMode.Impulse);
    }

}
