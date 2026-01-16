
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRScrewUnscrew : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform screwVisual;
    [SerializeField] private float threadLength = 0.1f;
    [SerializeField] private float degreesPerTurn = 360f;
    [SerializeField] private int turnsToRemove = 2;

    [Header("Runtime")]
    private float accumulatedRotation;
    private float initialHeight;
    private Transform screwdriver;

    private Quaternion lastDriverRotation;
    private bool engaged;

    private void Start()
    {
        initialHeight = screwVisual.localPosition.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Screwdriver"))
            return;

        screwdriver = other.transform;
        lastDriverRotation = screwdriver.rotation;
        engaged = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Screwdriver"))
            return;

        engaged = false;
        screwdriver = null;
    }

    private void Update()
    {
        if (!engaged || screwdriver == null)
            return;

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        Quaternion currentRot = screwdriver.rotation;
        Quaternion delta = currentRot * Quaternion.Inverse(lastDriverRotation);

        delta.ToAngleAxis(out float angle, out Vector3 axis);

        float signedAngle = angle * Mathf.Sign(Vector3.Dot(axis, transform.up));
        accumulatedRotation += signedAngle;

        float t = Mathf.Clamp01(accumulatedRotation / (turnsToRemove * degreesPerTurn));

        float lift = t * threadLength * turnsToRemove;
        screwVisual.localPosition =
            new Vector3(
                screwVisual.localPosition.x,
                screwVisual.localPosition.y,
                initialHeight + lift
            );

        lastDriverRotation = currentRot;

        if (t >= 1f)
        {
            PopOut();
        }
    }

    private void PopOut()
    {
        engaged = false;
        Debug.Log("✅ Screw removed!");
        screwVisual.gameObject.SetActive(false);
        screwVisual.SetParent(null);
        enabled = false;
    }
}
