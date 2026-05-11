using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ConnectorScript : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _material;
    [SerializeField] private string _NeededName = "connector";
    [SerializeField] private GameObject _Bolt;
    [SerializeField] private bool _WrongConnector = false;

    // This script checks if the player has entered the fuse box area
    private bool _IsTriggerd = false;
    private XRGrabInteractable _grabInteractable;
    private bool _connectorInTrigger = false;

    public bool IsTriggerd
    {
        get { return _IsTriggerd; }
        set
        {
            if (value != _IsTriggerd)
            {
                _IsTriggerd = value;
                if (_IsTriggerd)
                    Debug.Log("CableTriggerScript: Triggered");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_NeededName))
        {
            _connectorInTrigger = true;
            
            // Get the XRGrabInteractable component
            if (_grabInteractable == null)
            {
                _grabInteractable = other.GetComponent<XRGrabInteractable>();
                
                if (_grabInteractable == null)
                {
                    _grabInteractable = other.GetComponentInParent<XRGrabInteractable>();
                }
            }

            // Subscribe to selection events if we found the component
            if (_grabInteractable != null)
            {
                _grabInteractable.selectExited.AddListener(OnConnectorReleased);
            }

            Debug.Log("Connector entered trigger zone: " + _NeededName);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(_NeededName) && !_IsTriggerd)
        {
            // Check if connector is not being grabbed
            bool isBeingGrabbed = _grabInteractable != null && _grabInteractable.isSelected;

            if (!isBeingGrabbed)
            {
                ActivateConnector();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(_NeededName))
        {
            _connectorInTrigger = false;
            
            // Unsubscribe from events
            if (_grabInteractable != null)
            {
                _grabInteractable.selectExited.RemoveListener(OnConnectorReleased);
                _grabInteractable = null;
            }

            Debug.Log("Connector exited trigger zone");
        }
    }

    private void OnConnectorReleased(SelectExitEventArgs args)
    {
        // Check if connector is still in trigger when released
        if (_connectorInTrigger && !_IsTriggerd)
        {
            Debug.Log("Connector released in trigger zone");
            ActivateConnector();
        }
    }

    private void ActivateConnector()
    {
        _IsTriggerd = true;
        Debug.Log("CableTriggerScript: Activated " + _NeededName + " | Wrong: " + _WrongConnector);
        
        if (_Bolt != null)
        {
            if (_meshRenderer != null)
                _meshRenderer.material = _material;
            _Bolt.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (_grabInteractable != null)
        {
            _grabInteractable.selectExited.RemoveListener(OnConnectorReleased);
        }
    }
}