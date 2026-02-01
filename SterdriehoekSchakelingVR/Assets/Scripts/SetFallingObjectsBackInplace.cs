using System;
using UnityEngine;

public class SetFallingObjectsBackInPlace : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    [SerializeField]
    private  Transform ComebackPosition;
    void Start()
    {
        // Get initial position of the object
        _initialPosition = GetComponent<Transform>().position;
        // Get initial rotation of the object
        _initialRotation = GetComponent<Transform>().rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has collided with the trigger
        if (other.name == "FallingObjectsTrigger")
        {
            if (ComebackPosition != null)
            {
                // Reset the object's position to the comeback position
                GetComponent<Transform>().position = ComebackPosition.position;
                // Reset the object's rotation to the comeback rotation
                GetComponent<Transform>().rotation = ComebackPosition.rotation;
            }
            else
            {
                // Reset the object's position to the initial position
                GetComponent<Transform>().position = _initialPosition;
                // Reset the object's rotation to the initial rotation
                GetComponent<Transform>().rotation = _initialRotation;
            }

            // If the object has a rigidbody, reset its velocity
            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }
}