using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class SpringArm : MonoBehaviour
{
    [Space]
    [Header("Follow Settings \n--------------------")]
    [Space]
    public Transform target;
    public float movementSmoothTime = 0.05f;
    public float rotationSmoothTime = 0.05f;
    public float targetArmLength = 3f;
    public Vector3 socketOffset;
    public Vector3 targetOffset;

    [Space]
    [Header("Collision Settings \n-----------------------")]
    [Space]
    public bool doCollisionTest = true;
    [Range(2, 20)]
    public int collisionTestResolution = 4;
    public float collisionProbeSize = 0.3f;
    public float collisionSmoothTime = 0.05f;
    public LayerMask collisionLayerMask = ~0;

    [Space]
    [Header("Debugging \n--------------")]
    [Space]
    public bool visualDebugging = true;
    public Color springArmColor = new Color(0.75f, 0.2f, 0.2f, 0.75f);
    [Range(1f, 10f)]
    public float springArmLineWidth = 6f;
    public bool showRaycasts;
    public bool showCollisionProbe;

    #region Private Variables

    private Vector3 endPoint;
    private Vector3 socketPosition;
    private RaycastHit[] hits;
    private Vector3[] raycastPositions;

    private readonly Color collisionProbeColor = new Color(0.2f, 0.75f, 0.2f, 0.15f);

    private Vector3 moveVelocity;
    private Vector3 socketMoveVelocity; // Used for smoothing the socket position itself
    private Quaternion rotationVelocity; // For smooth rotation damping

    #endregion

    private void Awake()
    {
        raycastPositions = new Vector3[collisionTestResolution];
        hits = new RaycastHit[collisionTestResolution];
    }

    private void OnValidate()
    {
        raycastPositions = new Vector3[collisionTestResolution];
        hits = new RaycastHit[collisionTestResolution];
    }

    private void LateUpdate()
    {
        if (!target)
            return;

        // Calculate target world position and rotation base
        Vector3 targetWorldPosition = target.position + targetOffset;

        // If this object is the socket, we want to calculate where the arm extends TO from the target.
        // Let's smooth-damp the base position tracking the target first:
        // (Alternatively, if this object is directly at the target, we offset from it).

        if (rotationSmoothTime > 0f && Application.isPlaying)
        {
            transform.rotation = SmoothDampQuaternion(transform.rotation, target.rotation, ref rotationVelocity, rotationSmoothTime);
        }
        else
        {
            transform.rotation = target.rotation;
        }

        // Collision check & socket calculation
        if (doCollisionTest)
            CheckCollisions(targetWorldPosition);

        // Move THIS object (the socket) to the final calculated socket position smoothly
        transform.position = Vector3.SmoothDamp(transform.position, socketPosition, ref socketMoveVelocity, collisionSmoothTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!visualDebugging || !target)
            return;

        Vector3 basePos = target.position + targetOffset;

        Handles.color = springArmColor;
        if (showRaycasts)
            foreach (Vector3 raycastPosition in raycastPositions)
                Handles.DrawAAPolyLine(springArmLineWidth, 2, basePos, raycastPosition);
        else
            Handles.DrawAAPolyLine(springArmLineWidth, 2, basePos, endPoint);

        Handles.color = collisionProbeColor;
        if (showCollisionProbe)
            Handles.SphereHandleCap(0, socketPosition, Quaternion.identity, 2 * collisionProbeSize, EventType.Repaint);
    }
#endif

    private void CheckCollisions(Vector3 basePosition)
    {
        Transform trans = transform;
        Quaternion currentRot = trans.rotation;

        Vector3 targetArmOffset = socketOffset - new Vector3(0, 0, targetArmLength);
        endPoint = basePosition + (currentRot * targetArmOffset);

        for (int i = 0, angle = 0; i < collisionTestResolution; i++, angle += 360 / collisionTestResolution)
        {
            Vector3 raycastLocalEndPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * collisionProbeSize;
            raycastPositions[i] = endPoint + (currentRot * raycastLocalEndPoint);
            Physics.Linecast(basePosition, raycastPositions[i], out hits[i], collisionLayerMask);
        }

        if (doCollisionTest)
        {
            float minDistance = targetArmLength;
            foreach (RaycastHit hit in hits)
            {
                if (!hit.collider)
                    continue;

                float distance = Vector3.Distance(hit.point, basePosition);
                if (minDistance > distance)
                {
                    minDistance = distance;
                }
            }

            Vector3 dir = (endPoint - basePosition).normalized;
            Vector3 armOffset = dir * (targetArmLength - minDistance);
            socketPosition = endPoint - armOffset;
        }
        else
        {
            socketPosition = endPoint;
        }
    }

    // Helper method to smoothly damp quaternions
    private static Quaternion SmoothDampQuaternion(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return rot;

        if (Quaternion.Dot(rot, target) < 0f)
        {
            target.x = -target.x;
            target.y = -target.y;
            target.z = -target.z;
            target.w = -target.w;
        }

        Vector4 result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;

        return new Quaternion(result.x, result.y, result.z, result.w);
    }
}