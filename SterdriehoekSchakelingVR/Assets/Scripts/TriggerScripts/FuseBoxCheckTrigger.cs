using UnityEngine;

public class FuseBoxCheckTrigger : MonoBehaviour
{
    
    private bool _IsTriggerd = false;
    public bool IsTriggerd {
        get { return _IsTriggerd; }
        set
        {
            if (value != _IsTriggerd )
            {
                _IsTriggerd = value;
                if (_IsTriggerd) Debug.Log("TutorialTeleportTrigger: Triggered");
            }
        }
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IsTriggerd = true;
        }
    }
}
