using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class SimpleYRotationOnly : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private Rigidbody rb;
    private Vector3 lockedPosition;
    private Quaternion baseRotation;
    private float accumulatedYRotation; // Tracks total rotation (can go beyond 360)
    private float lastYRotation;
    
    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        
        lockedPosition = transform.position;
        baseRotation = transform.rotation;
        accumulatedYRotation = 0f;
        lastYRotation = transform.rotation.eulerAngles.y;
        
        // Configure rigidbody to prevent physics issues
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }
    
    void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
        }
    }
    
    void OnGrab(SelectEnterEventArgs args)
    {
        // Reset tracking when grabbed
        lastYRotation = transform.rotation.eulerAngles.y;
    }
    
    void OnRelease(SelectExitEventArgs args)
    {
        // Reset position
        transform.position = lockedPosition;
        
        // Apply the accumulated rotation
        float displayRotation = accumulatedYRotation % 360f;
        transform.rotation = Quaternion.Euler(baseRotation.eulerAngles.x, baseRotation.eulerAngles.y + displayRotation, baseRotation.eulerAngles.z);
    }
    
    void LateUpdate()
    {
        if (grab.isSelected)
        {
            // Lock position
            transform.position = lockedPosition;
            
            // Get current Y rotation
            float currentYRotation = transform.rotation.eulerAngles.y;
            
            // Calculate delta rotation (handling 360-degree wraparound)
            float delta = Mathf.DeltaAngle(lastYRotation, currentYRotation);
            
            // Accumulate the delta (this will always increase in the positive direction when turning clockwise)
            accumulatedYRotation += delta;
            
            // Update last rotation
            lastYRotation = currentYRotation;
            
            // Apply constraint: keep only Y rotation, lock X and Z to initial values
            transform.rotation = Quaternion.Euler(baseRotation.eulerAngles.x, currentYRotation, baseRotation.eulerAngles.z);
            
            // Debug to see the accumulated rotation
            Debug.Log($"Accumulated Y Rotation: {accumulatedYRotation}°");
        }
    }
    
    // Public method to get the accumulated rotation for your screw script
    public float GetAccumulatedYRotation()
    {
        return accumulatedYRotation;
    }
    // Public method to reset accumulated rotation (called by screw script)
    public void ResetAccumulatedRotation()
    {
        accumulatedYRotation = 0f;
        lastYRotation = transform.rotation.eulerAngles.y;
    }
}