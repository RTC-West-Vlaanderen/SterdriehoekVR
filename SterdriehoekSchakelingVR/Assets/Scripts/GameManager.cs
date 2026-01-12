using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
      public string name;
      public AudioClip instructionAudio;
      public Func<bool> completionCondition;
      public Action onStartAction;
      public Action onCompleteAction;
      public Action interactablesToEnable;
    }
    // VLaams
    [SerializeField] private List<AudioClip> _instructionClipsVL = new List<AudioClip>();
    // English
    [SerializeField] private List<AudioClip> _instructionClipsEN = new List<AudioClip>();
    [SerializeField] private List<Step> _steps = new List<Step>();
    [SerializeField] private List<AudioClip> _instructionClips = new List<AudioClip>();
    private int _currentStepIndex = -1;
    [SerializeField]private AudioSource _audioSource;
    
    
    [Header("XR Grab Interactable References")]
    [SerializeField] private XRGrabInteractable _LidInteractable;


    [Header("Params Step1")] 
    [SerializeField] private bool _HasUserLookedAtFusebox = false;
    [SerializeField] private FuseBoxCheckTrigger _teleportTrigger;
    
    private void Awake()
    {
        _instructionClips = _instructionClipsVL; // Future = Make it possible for en
        _steps = new List<Step>();
        InitializeSteps();
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartNextStep();
    }

    private void StartNextStep()
    {
        _currentStepIndex++;
        if (_currentStepIndex >= _steps.Count) return;
        var step = _steps[_currentStepIndex];
        step.onStartAction?.Invoke();
        step.interactablesToEnable?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_HasUserLookedAtFusebox) HasLookedAtFusebox();
    }


    private void InitializeSteps()
    {
        _steps = new List<Step>
        {
            // Step 0: Initial instruction and teleport
            new Step
            {
                name = "StartInstruction",
                //instructionAudio = _instructionClips[0],
                completionCondition = () => _HasUserLookedAtFusebox,
                //onCompleteAction = () => _goodSFX.Play(),
            },
        };
    }
    
    // Check if user has looked to the zekeringskast/Fusebox
    private bool HasLookedAtFusebox()
    {
        if (_teleportTrigger.IsTriggerd)
        {
            _HasUserLookedAtFusebox = true;
            _teleportTrigger.IsTriggerd = true;
            Debug.Log("GameManager: User has looked at the fusebox.");
        }
        // Implement logic to check if the user has looked at the fusebox
        return _HasUserLookedAtFusebox;
    }
}
