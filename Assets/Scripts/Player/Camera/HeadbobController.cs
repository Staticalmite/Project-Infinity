using UnityEngine;

public class HeadbobController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform holderTransform;

    [Header("Headbob Settings")]
    [SerializeField] private bool enableHeadbob = true;

    [Header("Walk Bob Settings")]
    [SerializeField, Range(0, 0.1f)] private float walkAmplitude = 0.015f;
    [SerializeField, Range(0, 30f)] private float walkFrequency = 10.0f;

    [Header("Sprint Bob Settings")]
    [SerializeField, Range(0, 0.1f)] private float sprintAmplitude = 0.020f;
    [SerializeField, Range(0, 30f)] private float sprintFrequency = 15.0f;

    private float currentFrequency;
    private float currentAmplitude;

    private Vector3 startPosition;
    private CharacterController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Find the FirstPersonController component on the same GameObject
        // And also set the start position of the camera.
        playerController = FindFirstObjectByType<CharacterController>();
        startPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        if (!enableHeadbob) return;

        CheckMotion();
        ResetPosition();
    }

    private void PlayMotion(Vector3 motion)
    {
        // Apply the motion to the camera's local position.
        cameraTransform.localPosition = startPosition + motion;
    }

    private void CheckMotion()
    {
        // Get the horizontal speed of the player
        // We set Y to 0 to flatten the velocity vector to the horizontal plane.
        float speed = new Vector3(playerController.velocity.x, 0, playerController.velocity.z).magnitude;
        
        // If the player is not moving or not grounded, don't apply head bobbing.
        if (speed < 0.1f) return;
        if (!playerController.isGrounded) return;

        bool isSprinting = speed > 3.1f;
        float targetAmplitude = isSprinting ? sprintAmplitude : walkAmplitude;
        float targetFrequency = isSprinting ? sprintFrequency : walkFrequency;
        Debug.Log(speed);

        // Set the current amplitude and frequency to the target values.
        currentAmplitude = targetAmplitude;
        currentFrequency = targetFrequency;

        // If we want a sickening effect we can lerp the amplitude and frequency to the target values instead of snapping to them.
        // I don't know why but doing this makes the camera intensely shake.
        //currentAmplitude = Mathf.Lerp(currentAmplitude, targetAmplitude, Time.deltaTime * transitionSpeed);
        //currentFrequency = Mathf.Lerp(currentFrequency, targetFrequency, Time.deltaTime * transitionSpeed);

        PlayMotion(FootStepMotion());
    }

    private Vector3 FootStepMotion()
    {
        // Reset the position to zero.
        Vector3 pos = Vector3.zero;

        // Output a smooth wave that oscialtes between -1 and 1
        // Multiply it by amplitude to determine the height of the bobbing motion.
        pos.y += Mathf.Sin(Time.time * currentFrequency) * currentAmplitude;

        // Output a smooth wave that oscillates between -1 and 1, offset from the y motion.
        // Multiply it by amplitude to determine the side to side motion of the bobbing.
        // Divided by 2 to make the side to side motion less pronounced than the up and down motion.
        // Dividing it by 2 means we make a full cycle in double the time.
        pos.x += Mathf.Cos(Time.time * currentFrequency / 2) * currentAmplitude * 2;

        return pos;
    }

    private void ResetPosition()
    {
        if(cameraTransform.localPosition == startPosition) return;

        // Smoothly move the camera back to the start position.
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, startPosition, Time.deltaTime * 1);
    }

    // Creates a virtual gaze anchor for the camera to look at.
    // This is to allow our head bobbing to feel more natural by having the camera look at the point in front of us and rotate up and down as the head bobs.
    // Mimics human vision where the eyes are always looking at a point in front of us and the head moves up and down as we walk.
    private Vector3 FocusTarget()
    {
        // Grab the player's horizontal in the game world
        // And combine the player's ground/feet height with the camera holder's local offset.
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + holderTransform.localPosition.y, transform.position.z);
       
        // Set a position to look at 15m in front of the player and return it.
        pos += holderTransform.forward * 15.0f;
        return pos;
    }
}
