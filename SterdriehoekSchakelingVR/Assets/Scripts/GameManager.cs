using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Linq;
using UnityEngine.UI;

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
        public List<GameObject> objectsToEnable;
    }

    // Vlaams
    [SerializeField] private List<AudioClip> _instructionClipsVL = new List<AudioClip>();

    // English
    [SerializeField] private List<AudioClip> _instructionClipsEN = new List<AudioClip>();

    // List of steps in the tutorial
    [SerializeField] private List<Step> _steps = new List<Step>();
    [SerializeField] private List<AudioClip> _instructionClips = new List<AudioClip>();
    private int _currentStepIndex = -1;
    [SerializeField] private AudioSource _audioSource;


    [Header("XR Grab Interactable References")] [SerializeField]
    private XRGrabInteractable _LidInteractable;

    [SerializeField] private XRGrabInteractable _ScrewdriverInteractable;

    [Header("Params Step1")] [SerializeField]
    private bool _IsUserAtWorkbench = false;

    [SerializeField] private WorkBenchTriggerScript _teleportTriggerWorkbench;

    [Header("Params step2")] 
    [SerializeField] private bool _HasUserStartedTheApp = false;
    [SerializeField] private bool _HasUserPressedStartButton = false;
    [SerializeField] private GameObject _StartPage;
    
    [Header("Params step3")] 
    [SerializeField] private bool _CorrectButtonPressed = false;
    //[SerializeField] private bool _WrongButtonPressed = false;
    [SerializeField] private GameObject _FirstQuestionPage;
    [Header("Params Step3")] 
    [SerializeField] private bool _HasUserLookedAtFusebox = false;

    [SerializeField] private FuseBoxCheckTrigger _teleportTrigger;

    [SerializeField] private List<GameObject> _Screwdrivers = new List<GameObject>();

    [Header("Params Step4")] 
    [SerializeField] private XRScrewUnscrew[] _screwUnscrews;


    [SerializeField]private bool _HasUserUnscrewedLid = false;

    [Header("Params Step5")] [SerializeField]
    private bool _IsLidRemoved = false;

    [Header("Params Step5")] [SerializeField]
    private List<XRGrabInteractable> _CablesToPlace = new List<XRGrabInteractable>();

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

        if (step.objectsToEnable != null)
        {
            foreach (var obj in step.objectsToEnable) obj.SetActive(true);
        }

        step.onStartAction?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if user has started the app
        if (!_HasUserStartedTheApp) HasUserStartedTheApp();
        
        // Check if user has looked at the fusebox
        if (!_HasUserLookedAtFusebox) HasLookedAtFusebox();
        // Check if user is at the workbench
        if (!_IsUserAtWorkbench) IsUserAtWorkbench();
        // Check if user has unscrewed the lid
        if (!_HasUserUnscrewedLid) HasUnscrewedLid();
        // Check if lid is removed
        if (!_IsLidRemoved) IsLidRemoved();

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
            new Step
            {
                name = "Teleport To workbench",
                //instructionAudio = _instructionClips[0],
                // Complete when user is at the workbench

                completionCondition = () => _IsUserAtWorkbench,
                //onCompleteAction = () => _goodSFX.Play(),
            },
            new Step
            {
              name ="Start the app",
              completionCondition = () => _HasUserStartedTheApp,
            },
            new Step
            {
                name ="Answer question 1",
                completionCondition = () => _CorrectButtonPressed,
            },
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
                name = "Unscrew the lid",
                objectsToEnable = _Screwdrivers,
                // Complete when user has unscrewed the lid
                completionCondition = () => _HasUserUnscrewedLid,
                //onCompleteAction = () => _goodSFX.Play(),
            },
            new Step
            {
                name = "Lift the lid",
                interactablesToEnable = new List<XRGrabInteractable>() { _LidInteractable },
                //instructionAudio = _instructionClips[0],
                // Complete when user is at the workbench
                completionCondition = () => _IsLidRemoved,
                //onCompleteAction = () => _goodSFX.Play(),
            },
            new Step
            {
                name = "Place the cables",
                interactablesToEnable = _CablesToPlace,
                //instructionAudio = _instructionClips[0],
                // Complete when user is at the workbench
                completionCondition = () => _IsLidRemoved,
                //onCompleteAction = () => _goodSFX.Play(),
            }
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

    private bool HasUnscrewedLid()
    {
        // Check if all lid screw triggers are triggered
        if (_screwUnscrews.All(trigger => trigger.IsScrewRemoved))
        {
            _HasUserUnscrewedLid = true;
            Debug.Log("GameManager: User has unscrewed the lid.");
        }

        return _HasUserUnscrewedLid;
    }

    private bool IsLidRemoved()
    {
        // Check if the lid interactable is being held
        if (_LidInteractable.isSelected)
        {
            _IsLidRemoved = true;
            Debug.Log("GameManager: Lid has been removed.");
        }

        return _IsLidRemoved;
    }

    public void BtnHasUserStartedTheApp()
    {
        _HasUserPressedStartButton = true;
        _StartPage.SetActive(false);
        _FirstQuestionPage.SetActive(true);
        Debug.Log("GameManager: User has started the app.");
    }

    private bool HasUserStartedTheApp()
    {
        if (_HasUserPressedStartButton)
        {
            _HasUserStartedTheApp = true;    
        }
        return _HasUserStartedTheApp;
    }

    public void OnCorrectAnswerpressedQ1(Button btn)
    {
        _CorrectButtonPressed = true;
        // Change button color to green
        // Verander kleur naar rood
        SetButtonColors(btn,Color.green);
        
        Debug.Log("GameManager: User has pressed the correct answer.");
    }

    private void SetButtonColors(Button btn, Color color)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        cb.highlightedColor = color;
        btn.colors = cb;
    }
    public void onNotCorrectButtonPressed(Button btn)
    {
        // Change the button color to red
        Debug.Log("GameManager: User has pressed the wrong answer.");
        // Get the pressed button
        
        Debug.Log("GameManager: User pressed the wrong answer: " + btn.name);
        SetButtonColors(btn,Color.red);
    }
}
