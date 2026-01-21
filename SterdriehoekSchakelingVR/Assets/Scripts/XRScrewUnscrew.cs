using UnityEngine;


public class XRScrewUnscrew : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform screwVisual;
    [SerializeField] private Transform alignPoint; // The point where screwdriver should attach
    [SerializeField] private float threadLength = 0.1f;
    [SerializeField] private float degreesPerTurn = 360f;
    [SerializeField] private int turnsToRemove = 2;

    [Header("Runtime")]
    private float accumulatedRotation;
    private float initialHeight;
    private Transform screwdriver;
    private Rigidbody screwdriverRb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable screwdriverGrab;

    private Quaternion lastDriverRotation;
    private bool engaged;
    private Vector3 targetPosition;

    private void Start()
    {
        initialHeight = screwVisual.localPosition.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Screwdriver"))
            return;

        // Find the XRGrabInteractable component
        screwdriverGrab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        /*if (screwdriverGrab == null || !screwdriverGrab.isSelected)
            return;
        */
        screwdriver = other.transform;
        screwdriverRb = screwdriver.GetComponent<Rigidbody>();
        
        if (screwdriverRb == null)
            screwdriverRb = screwdriver.GetComponentInParent<Rigidbody>();

        lastDriverRotation = screwdriver.rotation;
        engaged = true;

        if (alignPoint != null)
        {
            targetPosition = alignPoint.position;
        }

        Debug.Log("🔧 Screwdriver engaged!");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Screwdriver"))
            return;

        engaged = false;
        screwdriver = null;
        screwdriverGrab = null;
        screwdriverRb = null;
    }

    private void FixedUpdate()
    {
        if (!engaged || screwdriver == null)
            return;

        // Use physics to lock position instead of direct transform manipulation
        /*if (alignPoint != null && screwdriverRb != null)
        {
            targetPosition = alignPoint.position;
            Vector3 positionError = targetPosition - screwdriver.position;
            screwdriverRb.linearVelocity = positionError * positionLockStrength * Time.fixedDeltaTime;
        }
        */
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        // Get accumulated rotation from the screwdriver component
        SimpleYRotationOnly rotationController = screwdriver.GetComponent<SimpleYRotationOnly>();
        if (rotationController != null)
        {
            float totalRotation = rotationController.GetAccumulatedYRotation();
        
            // Use absolute value to ensure positive rotation
            accumulatedRotation = Mathf.Abs(totalRotation);
        }
    
        // Rest of your existing code...
        float t = Mathf.Clamp01(accumulatedRotation / (turnsToRemove * degreesPerTurn));
    
        float lift = t * threadLength * turnsToRemove;
        screwVisual.localPosition = new Vector3(
            screwVisual.localPosition.x,
            screwVisual.localPosition.y,
            initialHeight + lift
        );

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
        
        screwdriver = null;
        screwdriverGrab = null;
        screwdriverRb = null;
        
        enabled = false;
    }
}