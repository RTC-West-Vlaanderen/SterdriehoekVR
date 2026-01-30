using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ConnectorScript : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _materialFault;
    [SerializeField] private string _NeededName = "connector";

    [SerializeField] private GameObject _Bolt;
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
        if (other.gameObject.CompareTag(_NeededName))
        {
            _IsTriggerd = true;
            Debug.Log("CableTriggerScript: Triggered" + _NeededName);
            // Get the XR Grab Interactable component
            //other.GetComponent<XRGrabInteractable>().enabled = false;
            // Disable the mesh 
           
            if (_Bolt != null)
            {
                _Bolt.SetActive(true);    
            }
            
        }
    }
}
