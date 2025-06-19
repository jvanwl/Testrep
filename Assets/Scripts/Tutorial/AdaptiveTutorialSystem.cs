using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class AdaptiveTutorialSystem : MonoBehaviour
{
    public static AdaptiveTutorialSystem Instance { get; private set; }

    [System.Serializable]
    public class TutorialStep
    {
        public string id;
        public string title;
        public string description;
        public string[] objectives;
        public string[] requiredSteps;
        public float importance; // 0-1, affects when this is introduced
        public bool isOptional;
        public GameObject visualAid; // Prefab for visual demonstration
    }

    [System.Serializable]
    public class PlayerMetric
    {
        public string metricName;
        public float value; // 0-1, representing proficiency
        public float weight; // How important this metric is
    }

    private Dictionary<string, TutorialStep> allTutorialSteps = new Dictionary<string, TutorialStep>();
    private List<string> completedSteps = new List<string>();
    private List<PlayerMetric> playerMetrics = new List<PlayerMetric>();
    private Queue<TutorialStep> activeSteps = new Queue<TutorialStep>();

    [SerializeField] private float tutorialAggressiveness = 0.5f; // How often to suggest tutorials
    [SerializeField] private int maxConcurrentTutorials = 3;
    [SerializeField] private float learningSpeedMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTutorialSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTutorialSystem()
    {
        LoadTutorialSteps();
        InitializePlayerMetrics();
        StartInitialTutorials();
    }

    private void LoadTutorialSteps()
    {
        // Example tutorial steps - in production, these would be loaded from data files
        var basicTutorials = new[]
        {
            new TutorialStep
            {
                id = "basic_controls",
                title = "Basic Controls",
                description = "Learn how to navigate your empire",
                objectives = new[] { "Move the camera", "Select units", "Issue basic commands" },
                importance = 1f,
                isOptional = false
            },
            new TutorialStep
            {
                id = "resource_management",
                title = "Resource Management",
                description = "Learn how to manage your empire's resources",
                objectives = new[] { "Check resource levels", "Build resource gatherers", "Trade resources" },
                requiredSteps = new[] { "basic_controls" },
                importance = 0.9f,
                isOptional = false
            },
            new TutorialStep
            {
                id = "military_basics",
                title = "Military Basics",
                description = "Learn basic military operations",
                objectives = new[] { "Create military units", "Form armies", "Basic combat" },
                requiredSteps = new[] { "basic_controls" },
                importance = 0.8f,
                isOptional = false
            }
            // Add more tutorials
        };

        foreach (var tutorial in basicTutorials)
        {
            allTutorialSteps[tutorial.id] = tutorial;
        }
    }

    private void InitializePlayerMetrics()
    {
        playerMetrics = new List<PlayerMetric>
        {
            new PlayerMetric { metricName = "navigation", value = 0f, weight = 1f },
            new PlayerMetric { metricName = "resource_management", value = 0f, weight = 0.9f },
            new PlayerMetric { metricName = "military", value = 0f, weight = 0.8f },
            new PlayerMetric { metricName = "diplomacy", value = 0f, weight = 0.7f },
            new PlayerMetric { metricName = "technology", value = 0f, weight = 0.6f }
        };
    }

    private void StartInitialTutorials()
    {
        // Queue up initial mandatory tutorials
        foreach (var tutorial in allTutorialSteps.Values)
        {
            if (tutorial.requiredSteps == null || tutorial.requiredSteps.Length == 0)
            {
                QueueTutorial(tutorial);
            }
        }
    }

    public void UpdatePlayerMetric(string metricName, float delta)
    {
        var metric = playerMetrics.Find(m => m.metricName == metricName);
        if (metric != null)
        {
            metric.value = Mathf.Clamp01(metric.value + delta * learningSpeedMultiplier);
            CheckForNewTutorials();
        }
    }

    private void CheckForNewTutorials()
    {
        if (activeSteps.Count >= maxConcurrentTutorials)
            return;

        foreach (var tutorial in allTutorialSteps.Values)
        {
            if (!completedSteps.Contains(tutorial.id) && !activeSteps.Contains(tutorial))
            {
                if (ShouldOfferTutorial(tutorial))
                {
                    QueueTutorial(tutorial);
                }
            }
        }
    }

    private bool ShouldOfferTutorial(TutorialStep tutorial)
    {
        // Check prerequisites
        if (tutorial.requiredSteps != null)
        {
            foreach (var required in tutorial.requiredSteps)
            {
                if (!completedSteps.Contains(required))
                    return false;
            }
        }

        // Check if the tutorial is relevant based on player metrics
        float relevance = CalculateTutorialRelevance(tutorial);
        float threshold = tutorial.isOptional ? 0.7f : 0.4f;

        return relevance > threshold;
    }

    private float CalculateTutorialRelevance(TutorialStep tutorial)
    {
        // Calculate how relevant this tutorial is based on:
        // - Player's current metrics
        // - Tutorial importance
        // - Time since last tutorial
        // - Player's recent performance

        float baseRelevance = tutorial.importance;
        float metricFactor = GetRelevantMetricValue(tutorial);
        float timeFactor = GetTimeSinceLastTutorial() * 0.1f;

        return (baseRelevance + metricFactor + timeFactor) / 3f;
    }

    private float GetRelevantMetricValue(TutorialStep tutorial)
    {
        // Match tutorial to relevant metrics and return average proficiency
        // This is a simplified version - would need more sophisticated matching in production
        return 0.5f;
    }

    private float GetTimeSinceLastTutorial()
    {
        // Return normalized time since last tutorial completion
        // This would be implemented based on your game's time scale
        return 0.5f;
    }

    private void QueueTutorial(TutorialStep tutorial)
    {
        if (activeSteps.Count < maxConcurrentTutorials)
        {
            activeSteps.Enqueue(tutorial);
            StartTutorial(tutorial);
        }
    }

    private void StartTutorial(TutorialStep tutorial)
    {
        // Initialize tutorial UI and systems
        InitializeTutorialUI(tutorial);
        
        // Set up objective tracking
        SetupObjectiveTracking(tutorial);
        
        // Spawn visual aids if needed
        if (tutorial.visualAid != null)
        {
            SpawnVisualAid(tutorial);
        }
    }

    private void InitializeTutorialUI(TutorialStep tutorial)
    {
        // Implementation to show tutorial UI
        Debug.Log($"Starting tutorial: {tutorial.title}");
    }

    private void SetupObjectiveTracking(TutorialStep tutorial)
    {
        // Implementation to track tutorial objectives
    }

    private void SpawnVisualAid(TutorialStep tutorial)
    {
        // Implementation to show visual aids
    }

    public void CompleteTutorialStep(string tutorialId, string objectiveId)
    {
        var currentTutorial = activeSteps.Peek();
        if (currentTutorial.id == tutorialId)
        {
            // Mark objective as complete
            // If all objectives are complete, finish tutorial
            if (AreAllObjectivesComplete(currentTutorial))
            {
                CompleteTutorial(currentTutorial);
            }
        }
    }

    private bool AreAllObjectivesComplete(TutorialStep tutorial)
    {
        // Implementation to check if all objectives are complete
        return true; // Placeholder
    }

    private void CompleteTutorial(TutorialStep tutorial)
    {
        completedSteps.Add(tutorial.id);
        activeSteps.Dequeue();
        
        // Update player metrics
        UpdateMetricsOnCompletion(tutorial);
        
        // Save progress
        SaveTutorialProgress();
        
        // Check for new tutorials
        CheckForNewTutorials();
    }

    private void UpdateMetricsOnCompletion(TutorialStep tutorial)
    {
        // Implementation to update relevant metrics
    }

    private void SaveTutorialProgress()
    {
        // Implementation to save progress
        PlayerPrefs.SetString("CompletedTutorials", string.Join(",", completedSteps));
        PlayerPrefs.Save();
    }

    public void LoadTutorialProgress()
    {
        string saved = PlayerPrefs.GetString("CompletedTutorials", "");
        if (!string.IsNullOrEmpty(saved))
        {
            completedSteps = new List<string>(saved.Split(','));
        }
    }
}
