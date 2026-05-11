using UnityEngine;

public class LimitMovementToBox : MonoBehaviour
{
    [Header("Toegelaten volume (Box Collider van motor)")]
    public Collider allowedArea;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (allowedArea == null) return;

        Bounds bounds = allowedArea.bounds;
        Vector3 pos = rb.position;

        pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
        pos.y = Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y);
        pos.z = Mathf.Clamp(pos.z, bounds.min.z, bounds.max.z);

        rb.MovePosition(pos);
    }
}