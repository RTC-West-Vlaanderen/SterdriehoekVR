using UnityEngine;

public class LidScrewTriggerScript : MonoBehaviour
{
    [SerializeField] private GameObject _lidScrewObject;
    // This script checks if the player has entered the fuse box area
    private bool _IsTriggerd = false;
    public bool IsTriggerd {
        get { return _IsTriggerd; }
        set
        {
            if (value != _IsTriggerd )
            {
                _IsTriggerd = value;
                if (_IsTriggerd)
                {
                    Debug.Log("ScrewTrigger: Triggered");
                    _lidScrewObject.SetActive(false);
                }
            }
        }
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Screwdriver"))
        {
            IsTriggerd = true;
        }
    }
}
