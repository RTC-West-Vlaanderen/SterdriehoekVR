using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Linq;
public class GameManager : MonoBehaviour
{
    // this class represents a single step in the tutorial
    [System.Serializable]
    public class Step
    {
      public string name;
      public AudioClip instructionAudio;
      public Func<bool> completionCondition;
      public Action onStartAction;
      public Action onCompleteAction;
      public List<XRGrabInteractable> interactablesToEnable;
    }
    // Vlaams
    [SerializeField] private List<AudioClip> _instructionClipsVL = new List<AudioClip>();
    // English
    [SerializeField] private List<AudioClip> _instructionClipsEN = new List<AudioClip>();
    // List of steps in the tutorial
    [SerializeField] private List<Step> _steps = new List<Step>();
    [SerializeField] private List<AudioClip> _instructionClips = new List<AudioClip>();
    private int _currentStepIndex = -1;
    [SerializeField]private AudioSource _audioSource;
    
    
    [Header("XR Grab Interactable References")]
    [SerializeField] private XRGrabInteractable _LidInteractable;


    [Header("Params Step1")] 
    [SerializeField] private bool _HasUserLookedAtFusebox = false;
    [SerializeField] private FuseBoxCheckTrigger _teleportTrigger;
    [Header("Params Step2")] 
    [SerializeField] private bool _IsUserAtWorkbench = false;
    [SerializeField] private WorkBenchTriggerScript _teleportTriggerWorkbench;
    private void Awake()
    {
        // Init everything
        _instructionClips = _instructionClipsVL; // Future = Make it possible for en
        _steps = new List<Step>();
        InitializeSteps();
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start the first step starting with step 0
        StartNextStep();
    }

    private void StartNextStep()
    {
        _currentStepIndex++;
        if (_currentStepIndex >= _steps.Count) return;
        Step step = _steps[_currentStepIndex];
        if (step.interactablesToEnable != null)
        {
            foreach (var interactable in step.interactablesToEnable) interactable.enabled = true;
        }
        step.onStartAction?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if user has looked at the fusebox
        if (!_HasUserLookedAtFusebox) HasLookedAtFusebox();
        // Check if user is at the workbench
        if (!_IsUserAtWorkbench) IsUserAtWorkbench();
        
        if (_currentStepIndex >= 0 && _currentStepIndex < _steps.Count)
        {
                
            if (_steps[_currentStepIndex].completionCondition())
            {
                CompleteCurrentStep();
                StartNextStep();
            }
        }
    }

    // Initialize the steps of the tutorial
    private void InitializeSteps()
    {
        _steps = new List<Step>
        {
            // Step 0: Initial instruction and teleport
            new Step
            {
                name = "StartInstruction",
                //instructionAudio = _instructionClips[0],
                // Complete when user has looked at the fusebox
                completionCondition = () => _HasUserLookedAtFusebox,
                //onCompleteAction = () => _goodSFX.Play(),
            },
            new Step
            {
                name = "Teleport To workbench",
                interactablesToEnable = new List<XRGrabInteractable>(){_LidInteractable} ,
                //instructionAudio = _instructionClips[0],
                // Complete when user is at the workbench
                completionCondition = () => _IsUserAtWorkbench,
                //onCompleteAction = () => _goodSFX.Play(),
            },
        };
    }
    
    private void CompleteCurrentStep()
    {
        var step = _steps[_currentStepIndex];
        step.onCompleteAction?.Invoke();
    }
    
    // Check if user has looked to the zekeringskast/Fusebox
    private bool HasLookedAtFusebox()
    {
        // Check if the user enters the trigger area of the teleportation trigger
        if (_teleportTrigger.IsTriggerd)
        {
            _HasUserLookedAtFusebox = true;
            _teleportTrigger.IsTriggerd = true;
            Debug.Log("GameManager: User has looked at the fusebox.");
        }
        return _HasUserLookedAtFusebox;
    }

    private bool IsUserAtWorkbench()
    {
        // Check if the user enters the trigger area of the teleportation trigger
        if (_teleportTriggerWorkbench.IsTriggerd)
        {
            _IsUserAtWorkbench = true;
            _teleportTriggerWorkbench.IsTriggerd = true;
            Debug.Log("GameManager: User is at the workbench.");
        }
        return _teleportTriggerWorkbench;
    }
}
