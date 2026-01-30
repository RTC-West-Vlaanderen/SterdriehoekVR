using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ConnectorScript : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _material;
    [SerializeField] private string _NeededName = "connector";

    [SerializeField] private GameObject _Bolt;
    [SerializeField] private bool _WrongConnector = false;
    // This script checks if the player has entered the fuse box area
    private bool _IsTriggerd = false;

    private void Awake()
    {
        if (_NeededName =="Wrong") _WrongConnector = true;
    }

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
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(_NeededName))
        {
            if (_WrongConnector)
            {
                _IsTriggerd = true;
                Debug.Log("CableTriggerScript: Triggered" + _NeededName);
                if (_Bolt != null)
                {
                    if (_meshRenderer != null) _meshRenderer.material = _material; 
                    _Bolt.SetActive(true);    
                }
            }
            else
            {
                _IsTriggerd = true;
                Debug.Log("CableTriggerScript: Triggered" + _NeededName);
                if (_Bolt != null)
                {
                    if (_meshRenderer != null) _meshRenderer.material = _material; 
                    _Bolt.SetActive(true);    
                }   
            }
            
        }
    }
}
