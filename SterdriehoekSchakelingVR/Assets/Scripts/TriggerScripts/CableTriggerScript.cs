using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CableTriggerScript : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _materialFault;
    [SerializeField] private string _NeededCableName;
    // This script checks if the player has entered the fuse box area
    private bool _IsTriggerd = false;
    public bool IsTriggerd {
        get { return _IsTriggerd; }
        set
        {
            if (value != _IsTriggerd )
            {
                _IsTriggerd = value;
                if (_IsTriggerd) Debug.Log("CableTriggerScript: Triggered");
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_NeededCableName))
        {
            _IsTriggerd = true;
            Debug.Log("CableTriggerScript: Triggered" + _NeededCableName);
            // Get the XR Grab Interactable component
            other.GetComponent<XRGrabInteractable>().enabled = false;
            // Disable the mesh renderer
            _meshRenderer.enabled = false;
        }
        else
        {
            _meshRenderer.material = _materialFault;
        }
    }
}
