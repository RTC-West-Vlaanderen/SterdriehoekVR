using System.Runtime.CompilerServices;
using UnityEngine;

public class CableTriggerScript : MonoBehaviour
{
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
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(_NeededCableName))
        {
            _IsTriggerd = true;
            Debug.Log("CableTriggerScript: Triggered" + _NeededCableName);
        }
    }
}
