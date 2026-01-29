using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
using UnityEngine.XR.Interaction.Toolkit.UI;

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
    [SerializeField] private Button _startButton;
    [SerializeField] private WorkBenchTriggerScript _teleportTriggerWorkbench;

    [Header("Params step2")] 
    [SerializeField] private bool _HasUserStartedTheApp = false;
    [SerializeField] private bool _HasUserPressedStartButton = false;
    [SerializeField] private GameObject _StartPage;
    
    [Header("Params step3")] 
    [SerializeField] private bool _CorrectButtonPressed = false;
    //[SerializeField] private bool _WrongButtonPressed = false;
    [SerializeField] private GameObject _FirstQuestionPage;
    
    [Header("Params Step4")]
    [SerializeField] private bool _SecondCorrectQuestion = false;
    [SerializeField] private GameObject _SecondQuestionPage;
    [SerializeField] private List<TMP_InputField> _SecondQuestionText;
    
    [Header("Params Step5")]
    [SerializeField] private bool _ThirdCorrectQuestion = false;
    [SerializeField] private GameObject _ThirdQuestionPage;
    [SerializeField] private List<TMP_InputField> _ThirdQuestionText;

    [Header("Params Step6")]
    [SerializeField] private bool _ForthCorrectQuestion = false;
    [SerializeField] private GameObject _FourthQuestionPage;
    
    [Header("Params Step7")]
    [SerializeField] private bool _FifthCorrectQuestion = false;
    [SerializeField] private GameObject _FifthQuestionPage;

    [Header("Params Step8")] 
    [SerializeField]private bool _SixthCorrectQuestion;
    [SerializeField] private GameObject _SixthQuestionPage;
    [SerializeField] private List<TMP_InputField> _SixthQuestionText;
    
    [Header("Keyboard")]
    [SerializeField] private GlobalNonNativeKeyboard _keyboardManager;
    [SerializeField] private TMP_InputField _activeInputField;
    [SerializeField] private GameObject _KeyBoardCoding;
    [SerializeField] private Transform _PlayerRoot;
    [SerializeField] private GameObject _keyb;
    [Header("Params Step3")] 
    [SerializeField] private bool _HasUserLookedAtFusebox = false;

    [SerializeField] private FuseBoxCheckTrigger _teleportTrigger;

    [SerializeField] private List<GameObject> _Screwdrivers = new List<GameObject>();
    [SerializeField] private List<Button> _QuestionButtons = new List<Button>();
    
    [Header("Params Step9")] 
    [SerializeField] private XRScrewUnscrew[] _screwUnscrews;
    

    [SerializeField]private bool _HasUserUnscrewedLid = false;
    [SerializeField] private GameObject _SeventhQuestionPage;

    
    
    
    [Header("Params Step10")] [SerializeField]
    private bool _IsLidRemoved = false;
    [SerializeField] private GameObject _EightQuestionPage;
    [Header("Params Step11")] [SerializeField]
    private List<XRGrabInteractable> _CablesToPlace = new List<XRGrabInteractable>();

    [Header("Params Step11")] [SerializeField]
    private bool _AreCablesCorrect = false;

    [SerializeField] private GameObject _CablePage;
    [SerializeField] private List<CableTriggerScript> _CableSockets = new List<CableTriggerScript>();

    [Header("Params Step12")] [SerializeField]
    private bool _AreConnectorsCorrect =false;
    [SerializeField] private List<XRGrabInteractable> _connectorsGrabInteractables = new List<XRGrabInteractable>();

    [SerializeField]private List<GameObject> _ConnectorTriggers = new List<GameObject>();
    
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

        if (!_AreCablesCorrect)_AreCablesPlacedCorrectly();
        
        

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
                name ="Answer question 1",
                completionCondition = () => _CorrectButtonPressed,
            },
            new Step
            {
                name ="Answer question 2",
                completionCondition = () => _SecondCorrectQuestion,
                
            },
            new Step
            {
                name ="Answer question 3",
                completionCondition = () => _ThirdCorrectQuestion,
            },
            new Step
            {
                name ="Answer question 4",
                completionCondition = () => _ForthCorrectQuestion,
            },
            new Step
            {
                name ="Answer question 5",
                completionCondition = () => _FifthCorrectQuestion,
            },
            new Step
            {
                name ="Answer question 6",
                completionCondition = () => _SixthCorrectQuestion,
            },
            new Step
            {
                name = "Unscrew the lid",
                objectsToEnable = _Screwdrivers,
                // Complete when user has unscrewed the lid
                completionCondition = () => _HasUserUnscrewedLid,
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
                completionCondition = () => _AreCablesCorrect,
                //onCompleteAction = () => _goodSFX.Play(),
            },
            new Step
            {
                name = "Place the connectors",
                objectsToEnable = _ConnectorTriggers,
                interactablesToEnable = _connectorsGrabInteractables,
                completionCondition = () => _AreConnectorsCorrect,
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
            foreach (Button btn in _QuestionButtons) btn.interactable = true;
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
            _startButton.interactable = true;
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
            if (_SixthCorrectQuestion) StartCoroutine(WaitForUserReading(_EightQuestionPage, _SeventhQuestionPage));
            Debug.Log("GameManager: User has unscrewed the lid.");
        }
        
        return _HasUserUnscrewedLid;
    }
    
    
    private bool _AreCablesPlacedCorrectly()
    {
        // Check if all cable sockets are triggered
        if (_CableSockets.All(socket => socket.IsTriggerd))
        {
            _AreCablesCorrect = true;
            Debug.Log("GameManager: All cables are placed correctly.");
            StartCoroutine(WaitForUserReading(_CablePage, _EightQuestionPage));
        }

        return _AreCablesCorrect;
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
        // Wait a little
        StartCoroutine(WaitForUserReading(_SecondQuestionPage, _FirstQuestionPage));
        
        Debug.Log("GameManager: User has pressed the correct answer.");
    }
    public void OnCorrectAnswerpressedQ4(Button btn)
    {
        _ForthCorrectQuestion = true;
        // Change button color to green
        // Verander kleur naar rood
        SetButtonColors(btn,Color.green);
        // Wait a little
        StartCoroutine(WaitForUserReading(_FifthQuestionPage, _FourthQuestionPage));

        Debug.Log("GameManager: User has pressed the correct answer.");
    }
    public void OnCorrectAnswerpressedQ5(Button btn)
    {
        _FifthCorrectQuestion = true;
        // Change button color to green
        // Verander kleur naar rood
        SetButtonColors(btn,Color.green);
        // Wait a little
        
        StartCoroutine(WaitForUserReading(_SixthQuestionPage, _FifthQuestionPage));
        //_keyboard.keyboard = _KeyBoardCoding;
        
        Debug.Log("GameManager: User has pressed the correct answer. Answer Q5");
    }
    
    
    
    
    
    public void CheckSecondpageAnswers()
    {
        bool isCorrect = true;

        foreach (var input in _SecondQuestionText)
        {
            string raw = input.text.Trim();

            if (int.TryParse(raw, out int numberFilled) && numberFilled == 230)
            {
                input.textComponent.color = Color.green;
            }
            else
            {
                input.textComponent.color = Color.red;
                isCorrect = false;
            }
        }

        _SecondCorrectQuestion = isCorrect;
        if (_SecondCorrectQuestion)StartCoroutine(WaitForUserReading(_ThirdQuestionPage,_SecondQuestionPage));
    }

    public void CheckThirdpageAnswers()
    {
        bool isCorrect = true;

        foreach (var input in _ThirdQuestionText)
        {
            string raw = input.text.Trim();

            if (int.TryParse(raw, out int numberFilled) && numberFilled == 400)
            {
                input.textComponent.color = Color.green;
            }
            else
            {
                input.textComponent.color = Color.red;
                isCorrect = false;
            }
        }

        _ThirdCorrectQuestion = isCorrect;
        if (_ThirdCorrectQuestion) StartCoroutine(WaitForUserReading(_FourthQuestionPage, _ThirdQuestionPage));
    }
    
    public void CheckSixthpageAnswers()
    {
        bool isCorrect = true;

        foreach (var input in _SixthQuestionText)
        {
            string raw = input.text.Trim();

            if (raw.ToLower() ==input.name.ToLower())
            {
                input.textComponent.color = Color.green;
            }
            else
            {
                input.textComponent.color = Color.red;
                isCorrect = false;
            }
        }

        _SixthCorrectQuestion = isCorrect;
        if (_SixthCorrectQuestion) StartCoroutine(WaitForUserReading(_SeventhQuestionPage, _SixthQuestionPage));
        Debug.Log("GameManager: User has pressed the correct answer.");
    }

    
    
    private IEnumerator WaitForUserReading(GameObject page1,GameObject page2)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
        page1.SetActive(true);
        page2.SetActive(false);
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
