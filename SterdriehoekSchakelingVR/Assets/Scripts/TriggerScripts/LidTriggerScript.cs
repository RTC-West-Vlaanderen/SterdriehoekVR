using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LidTriggerScript : MonoBehaviour
{
    // This script checks if the player has entered the fuse box area
    private bool _IsTriggerd = false;

    public bool IsTriggerd
    {
        get { return _IsTriggerd; }
        set
        {
            if (value != _IsTriggerd)
            {
                _IsTriggerd = value;
                if (_IsTriggerd) Debug.Log("CableTriggerScript: Triggered");
            }
        }
    }


    [SerializeField] private XRSocketInteractor triggerSocket;

    private void Awake()
    {
        triggerSocket.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs arg0)
    {
        // Check if the selected object has the tag "lid"
        if (arg0.interactableObject.transform.gameObject.CompareTag("Lid"))
        {
            IsTriggerd = true;
        }
    }
}
