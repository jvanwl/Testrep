using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [System.Serializable]
    public class TutorialStep
    {
        public string message;
        public string objective;
        public string triggerCondition;
        public GameObject highlightObject;
    }

    [SerializeField] private TutorialStep[] tutorialSteps;
    private int currentStep = -1;
    private bool tutorialActive = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (tutorialActive)
        {
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        currentStep = -1;
        NextStep();
    }

    public void NextStep()
    {
        currentStep++;
        if (currentStep < tutorialSteps.Length)
        {
            ShowCurrentStep();
        }
        else
        {
            CompleteTutorial();
        }
    }

    private void ShowCurrentStep()
    {
        var step = tutorialSteps[currentStep];
        UIManager.Instance.UpdateObjective(step.objective);
        
        if (step.highlightObject != null)
        {
            HighlightObject(step.highlightObject);
        }

        // Show tutorial message
        ShowMessage(step.message);
    }

    private void HighlightObject(GameObject obj)
    {
        // Add highlight effect to object
        var highlighter = obj.GetComponent<Highlighter>() ?? obj.AddComponent<Highlighter>();
        highlighter.StartHighlight();
    }

    private void ShowMessage(string message)
    {
        // Show tutorial message in UI
        UIManager.Instance.ShowTutorialMessage(message);
    }

    public void CheckTutorialProgress(string condition)
    {
        if (!tutorialActive || currentStep >= tutorialSteps.Length) return;

        if (tutorialSteps[currentStep].triggerCondition == condition)
        {
            NextStep();
        }
    }

    private void CompleteTutorial()
    {
        tutorialActive = false;
        UIManager.Instance.HideTutorialUI();
        
        // Save tutorial completion
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
    }

    // Tutorial step triggers
    public void OnResourceCollected(string resourceType)
    {
        CheckTutorialProgress($"collect_{resourceType}");
    }

    public void OnBuildingConstructed(string buildingType)
    {
        CheckTutorialProgress($"build_{buildingType}");
    }

    public void OnUnitCreated(string unitType)
    {
        CheckTutorialProgress($"create_{unitType}");
    }

    public void OnUnitMoved()
    {
        CheckTutorialProgress("move_unit");
    }

    public void OnBattleCompleted()
    {
        CheckTutorialProgress("complete_battle");
    }
}
