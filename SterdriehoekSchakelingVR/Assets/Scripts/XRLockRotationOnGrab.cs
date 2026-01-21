using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRLockYRotationOnly : MonoBehaviour
{
    XRGrabInteractable grab;

    bool isGrabbed;
    float lockedX;
    float lockedZ;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        Vector3 euler = transform.localEulerAngles;
        lockedX = euler.x;
        lockedZ = euler.z;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    void LateUpdate()
    {
        if (!isGrabbed) return;

        Vector3 euler = transform.localEulerAngles;

        transform.localRotation = Quaternion.Euler(
            lockedX,
            euler.y,
            lockedZ
        );
    }
}