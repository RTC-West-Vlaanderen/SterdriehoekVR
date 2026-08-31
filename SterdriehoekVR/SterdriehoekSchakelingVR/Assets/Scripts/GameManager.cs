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
    [SerializeField] private List<AudioClip> _instructionClipsFR = new List<AudioClip>();

    // List of steps in the tutorial
    [SerializeField] private List<Step> _steps = new List<Step>();
    [SerializeField] private List<AudioClip> _instructionClips = new List<AudioClip>();
    private int _currentStepIndex = -1;
    [SerializeField] private AudioSource _audioSource;
    // Koppelt een pagina (GameObject) aan de index in _instructionClipsVL / _instructionClipsFR
    // zodat de juiste narratie-audio automatisch afspeelt zodra die pagina actief wordt.
    private Dictionary<GameObject, int> _pageNarrationIndex;



    [Header("XR Grab Interactable References")] [SerializeField]
    private XRGrabInteractable _LidInteractable;

    [SerializeField] private XRGrabInteractable _ScrewdriverInteractable;

    [Header("Params pick a language")] 
    [SerializeField] bool _IsFrench = false;
    private bool _HasUserPickedALanguage = false;
    
    
    [Header("Params Step1")] 
    [SerializeField] private bool _IsUserAtWorkbench = false;
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
    [SerializeField] private List<XRGrabInteractable> _CablesGrab = new List<XRGrabInteractable>(); 
    [Header("Params Step12")] [SerializeField]
    private bool _AreConnectorsCorrect =false;
    [SerializeField] private List<XRGrabInteractable> _connectorsGrabInteractables = new List<XRGrabInteractable>();
    
    [SerializeField]private List<GameObject> _ConnectorTriggers = new List<GameObject>();


    [SerializeField] private List<ConnectorScript> _CorrectConnectors = new List<ConnectorScript>();
    [SerializeField] private GameObject _ConnectorPage;
    
    
    [Header("Params Step 13")]
    [SerializeField] private GameObject _PlaceLidPage;
    [SerializeField] private bool _IsLidPlaced = false;
    [SerializeField] private GameObject _LidTrigger;
    [SerializeField] private LidTriggerScript _lidTriggerScript;
    [Header("Params Step 14")]
    [SerializeField] private List<GameObject> _Screws = new List<GameObject>();

    //[SerializeField] private GameObject _RestartButton;
    [SerializeField] private GameObject _FinalPage;
    [SerializeField] private TextMeshPro _FinalCode;

    protected List<int> _WinningCodes = new List<int>()
    {
        673627,
        489106,
        876074,
        114704,
        406575,
        556993,
        474335,
        932124,
        533358,
        654672
    };
private void Awake()
    {
        // Init everything
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();

        _pageNarrationIndex = new Dictionary<GameObject, int>();
        void MapPage(GameObject page, int index)
        {
            if (page != null) _pageNarrationIndex[page] = index;
        }
        MapPage(_FirstQuestionPage, 0);
        MapPage(_SecondQuestionPage, 1);
        MapPage(_ThirdQuestionPage, 1);
        MapPage(_FourthQuestionPage, 2);
        MapPage(_FifthQuestionPage, 3);
        MapPage(_SixthQuestionPage, 4);
        MapPage(_SeventhQuestionPage, 5);
        MapPage(_EightQuestionPage, 6);
        MapPage(_CablePage, 7);
        MapPage(_ConnectorPage, 8);
        MapPage(_PlaceLidPage, 9);
        MapPage(_FinalPage, 10);

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


    /// <summary>
    /// Speelt de instructie-audio af voor de meegegeven pagina, in de huidige gekozen taal
    /// (Vlaams of Frans), via de mapping die in Awake() is opgebouwd.
    /// </summary>
    private void PlayInstructionAudioForPage(GameObject page)
    {
        if (page != null && _pageNarrationIndex != null && _pageNarrationIndex.TryGetValue(page, out int index))
        {
            PlayInstructionAudio(index);
        }
    }

    /// <summary>
    /// Speelt de instructie-audio af op de gegeven index, in de huidige gekozen taal.
    /// index verwijst naar dezelfde positie in _instructionClipsVL en _instructionClipsFR.
    /// </summary>
    private void PlayInstructionAudio(int index)
    {
        if (index < 0 || _audioSource == null) return;

        bool isFrench = LanguageManager.LanguageManagerSingleton != null && LanguageManager.LanguageManagerSingleton.IsFrench;
        List<AudioClip> clips = isFrench ? _instructionClipsFR : _instructionClipsVL;

        if (clips == null || index >= clips.Count || clips[index] == null)
        {
            Debug.LogWarning($"GameManager: Geen instructie-audio gevonden voor index {index} (Frans={isFrench}).");
            return;
        }

        _audioSource.Stop();
        _audioSource.clip = clips[index];
        _audioSource.Play();
    }


    // Update is called once per frame
    void Update()
    {
        if (!_HasUserPickedALanguage) HasUserPickedALanguage();
        
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

        if (!_AreConnectorsCorrect) _AreConnectorsPlacedCorrectly();

        if (!_IsLidPlaced) IsLidPlaced();
        
        //if (!_HasUserScrewedLidBack) HasScrewedLidBack();

        if (_currentStepIndex >= 0 && _currentStepIndex < _steps.Count)
        {

            if (_steps[_currentStepIndex].completionCondition())
            {
                CompleteCurrentStep();
                StartNextStep();
            }
        }
    }

    private void HasUserPickedALanguage()
    {
        _HasUserPickedALanguage = LanguageManager.LanguageManagerSingleton.HasUserPickLanguage;
    }
    // Initialize the steps of the tutorial
    private void InitializeSteps()
    {
        _steps = new List<Step>
        {
            new Step
            {
                name = "Select Language",
                // Complete when user is at the workbench
                completionCondition = () => _HasUserPickedALanguage
                //onCompleteAction = () => _goodSFX.Play(),
            },
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
            },
            new Step
            {
                name = "Place the lid back in",
                objectsToEnable = new List<GameObject>(){_LidTrigger},
                completionCondition = () => _IsLidPlaced,
            },
            /*new Step
            {
                name = "Screw the lid back on",
                objectsToEnable = _ScrewBackScrewdrivers,
                completionCondition = () => _HasUserScrewedLidBack,
            }*/
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
            // Disable all there grab intecactables
            foreach (var socket in _CablesGrab)
            {
                if (socket != null)
                {
                    // Disable the grab interactable so it can't be picked up again
                    socket.enabled = false;
                    Debug.Log($"Disabled grab interactable for socket: {socket.name}");
                }
            }
            Debug.Log("GameManager: All cables are placed correctly.");
            StartCoroutine(WaitForUserReading(_ConnectorPage,_CablePage ));
        }

        return _AreCablesCorrect;
    }

    private bool _AreConnectorsPlacedCorrectly()
    {
        if (_CorrectConnectors.All(connector => connector.IsTriggerd))
        {
            _AreConnectorsCorrect = true;
            Debug.Log("GameManager: All connectors are placed correctly.");
            StartCoroutine(WaitForUserReading(_PlaceLidPage,_ConnectorPage));
        }

        return _AreConnectorsCorrect;
    }



    private bool IsLidPlaced()
    {
        if (_lidTriggerScript.IsTriggerd)
        {
            _IsLidPlaced = true;
            Debug.Log("GameManager: Lid has been placed.");
            //Setscrews back
            foreach (var screw in _Screws)
            {
                screw.SetActive(true);
            }
            // Set the final page active
            StartCoroutine(WaitForUserReading(_FinalPage,_PlaceLidPage));
            // Get one random code from the winning codes
            System.Random rand = new System.Random();
            int index = rand.Next(_WinningCodes.Count);
            int winningCode = _WinningCodes[index];
            _FinalCode.text = winningCode.ToString();
            //_RestartButton.SetActive(true);
            // End of tutorial
            Debug.Log("GameManager: Tutorial completed!");
            // Show last page with code and restart button
        }

        return _IsLidPlaced;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    

    private bool IsLidRemoved()
    {
        // Check if the lid interactable is being held
        if (_LidInteractable.isSelected)
        {
            _IsLidRemoved = true;
            Debug.Log("GameManager: Lid has been removed.");
            StartCoroutine(WaitForUserReading(_CablePage, _EightQuestionPage));
        }

        return _IsLidRemoved;
    }

public void BtnHasUserStartedTheApp()
    {
        _HasUserPressedStartButton = true;
        _StartPage.SetActive(false);
        _FirstQuestionPage.SetActive(true);
        PlayInstructionAudioForPage(_FirstQuestionPage);
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
        List<int> values = new List<int>();
    
        // First pass: collect all values and check if they're valid
        foreach (var input in _SecondQuestionText)
        {
            string raw = input.text.Trim();

            if (int.TryParse(raw, out int numberFilled) && (numberFilled == 230 || numberFilled == 240))
            {
                values.Add(numberFilled);
            }
            else
            {
                isCorrect = false;
                break;
            }
        }
    
        // Check if all values are the same
        if (isCorrect && values.Count > 0)
        {
            int firstValue = values[0];
            isCorrect = values.All(v => v == firstValue);
        }
    
        // Second pass: color the inputs based on result
        foreach (var input in _SecondQuestionText)
        {
            if (isCorrect)
            {
                input.textComponent.color = Color.green;
            }
            else
            {
                input.textComponent.color = Color.red;
            }
        }

        _SecondCorrectQuestion = isCorrect;
        if (_SecondCorrectQuestion) StartCoroutine(WaitForUserReading(_ThirdQuestionPage, _SecondQuestionPage));
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
        PlayInstructionAudioForPage(page1);
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
