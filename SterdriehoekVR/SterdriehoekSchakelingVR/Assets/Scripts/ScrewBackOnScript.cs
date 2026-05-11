using UnityEngine;


public class XRScrewBack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform screwVisual;         // The screw object (same one XRScrewUnscrew hid)
    [SerializeField] private Transform alignPoint;         // The hole/target where the screw goes back in
    [SerializeField] private XRScrewUnscrew unscrewScript; // Reference to the original script on this object

    [Header("Screw Settings — must match XRScrewUnscrew")]
    [SerializeField] private float threadLength = 0.1f;
    [SerializeField] private float degreesPerTurn = 360f;
    [SerializeField] private int turnsToRemove = 2;        // Same value = same number of turns to screw back in
    [SerializeField] private bool unscrewClockwise = false; // Keep the same as XRScrewUnscrew

    [Header("Re-enable")]
    [SerializeField] private GameObject _ScrewdriverToReenable; // The screwdriver GameObject that was disabled

    [Header("Runtime")]
    private float accumulatedRotation;
    private float initialHeight;  // The fully-screwed-in Z position
    private Transform screwdriver;
    private SimpleYRotationOnly rotationController;
    private bool engaged;

    // The screw starts fully out; this is the lifted Z when fully unscrewed
    private float fullyOutHeight;

    /// <summary>
    /// True while the screw is still removed (not yet fully screwed back in).
    /// GameManager polls this to know when all screws are back in place.
    /// </summary>
    public bool IsScrewRemoved
    {
        get { return !screwVisual.gameObject.activeSelf || unscrewScript.IsScrewRemoved; }
    }

    private void Start()
    {
        // The screw should start hidden (already removed by XRScrewUnscrew)
        // initialHeight is where the screw sits when fully screwed in
        initialHeight = alignPoint != null ? alignPoint.localPosition.z : 0f;
        fullyOutHeight = initialHeight + threadLength * turnsToRemove;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Screwdriver"))
            return;

        // Only allow engagement if the screw is currently removed
        if (!unscrewScript.IsScrewRemoved)
            return;

        screwdriver = other.transform;
        rotationController = screwdriver.GetComponent<SimpleYRotationOnly>();

        if (rotationController == null)
            return;

        // Snap the screw back into existence at the fully-out position
        screwVisual.gameObject.SetActive(true);
        screwVisual.SetParent(transform);
        screwVisual.localPosition = new Vector3(
            screwVisual.localPosition.x,
            screwVisual.localPosition.y,
            fullyOutHeight
        );

        // Reset tracking
        rotationController.ResetAccumulatedRotation();
        accumulatedRotation = 0f;
        engaged = true;

        Debug.Log("🔧 Screwing back in — engaged!");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Screwdriver"))
            return;

        engaged = false;
        screwdriver = null;
        rotationController = null;
    }

    private void FixedUpdate()
    {
        if (!engaged || screwdriver == null || rotationController == null)
            return;

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        float totalRotation = rotationController.GetAccumulatedYRotation();

        // Screwing IN is the opposite direction of unscrewing
        if (unscrewClockwise)
        {
            // Unscrew was clockwise, so screw-in is counter-clockwise (negative)
            accumulatedRotation = Mathf.Max(0, -totalRotation);
        }
        else
        {
            // Unscrew was counter-clockwise, so screw-in is clockwise (positive)
            accumulatedRotation = Mathf.Max(0, totalRotation);
        }

        // If user rotates the wrong way, reset so it doesn't go negative
        if (accumulatedRotation <= 0)
        {
            rotationController.ResetAccumulatedRotation();
            accumulatedRotation = 0f;
        }

        // Calculate progress (0 = fully out, 1 = fully screwed in)
        float t = Mathf.Clamp01(accumulatedRotation / (turnsToRemove * degreesPerTurn));

        // Move screw downward (from fullyOutHeight back to initialHeight)
        float currentZ = fullyOutHeight - (t * threadLength * turnsToRemove);
        screwVisual.localPosition = new Vector3(
            screwVisual.localPosition.x,
            screwVisual.localPosition.y,
            currentZ
        );

        Debug.Log($"Screw-in progress: {t * 100f}% ({accumulatedRotation}° / {turnsToRemove * degreesPerTurn}°)");

        if (t >= 1f)
        {
            ScrewedIn();
        }
    }

    private void ScrewedIn()
    {
        engaged = false;
        screwdriver = null;
        rotationController = null;

        // Re-enable the unscrewing script so it can be unscrewed again
        unscrewScript.enabled = true;

        // Re-enable the screwdriver if it was disabled
        if (_ScrewdriverToReenable != null)
            _ScrewdriverToReenable.SetActive(true);

        Debug.Log("✅ Screw fully screwed back in!");
    }
}