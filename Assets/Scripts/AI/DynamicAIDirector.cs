using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DynamicAIDirector : MonoBehaviour
{
    public static DynamicAIDirector Instance { get; private set; }

    [System.Serializable]
    public class DifficultyParameters
    {
        public float resourceGainMultiplier;
        public float aiAggressiveness;
        public float eventFrequency;
        public float tradeWillingness;
        public float techProgressSpeed;
    }

    [SerializeField] private DifficultyParameters easySettings;
    [SerializeField] private DifficultyParameters mediumSettings;
    [SerializeField] private DifficultyParameters hardSettings;
    
    private Dictionary<string, float> playerPerformanceMetrics = new Dictionary<string, float>();
    private float overallDifficultyRating = 0.5f; // 0 = easiest, 1 = hardest
    private readonly float adaptationRate = 0.1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMetrics();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMetrics()
    {
        playerPerformanceMetrics["economicStrength"] = 0.5f;
        playerPerformanceMetrics["militaryStrength"] = 0.5f;
        playerPerformanceMetrics["technologicalProgress"] = 0.5f;
        playerPerformanceMetrics["territorialControl"] = 0.5f;
        playerPerformanceMetrics["populationGrowth"] = 0.5f;
    }

    public void UpdatePlayerMetric(string metricName, float value)
    {
        if (playerPerformanceMetrics.ContainsKey(metricName))
        {
            playerPerformanceMetrics[metricName] = Mathf.Clamp01(value);
            RecalculateDifficulty();
        }
    }

    private void RecalculateDifficulty()
    {
        float averagePerformance = playerPerformanceMetrics.Values.Average();
        
        // Smoothly adapt difficulty
        overallDifficultyRating = Mathf.Lerp(overallDifficultyRating, 
            averagePerformance, adaptationRate);

        // Apply new difficulty settings
        ApplyDynamicDifficulty();
    }

    public DifficultyParameters GetCurrentDifficultySettings()
    {
        DifficultyParameters current = new DifficultyParameters();
        
        // Interpolate between difficulty settings based on overall rating
        if (overallDifficultyRating <= 0.33f)
        {
            float t = overallDifficultyRating / 0.33f;
            current = LerpDifficultySettings(easySettings, mediumSettings, t);
        }
        else if (overallDifficultyRating <= 0.66f)
        {
            float t = (overallDifficultyRating - 0.33f) / 0.33f;
            current = LerpDifficultySettings(mediumSettings, hardSettings, t);
        }
        else
        {
            current = hardSettings;
        }

        return current;
    }

    private DifficultyParameters LerpDifficultySettings(DifficultyParameters a, DifficultyParameters b, float t)
    {
        return new DifficultyParameters
        {
            resourceGainMultiplier = Mathf.Lerp(a.resourceGainMultiplier, b.resourceGainMultiplier, t),
            aiAggressiveness = Mathf.Lerp(a.aiAggressiveness, b.aiAggressiveness, t),
            eventFrequency = Mathf.Lerp(a.eventFrequency, b.eventFrequency, t),
            tradeWillingness = Mathf.Lerp(a.tradeWillingness, b.tradeWillingness, t),
            techProgressSpeed = Mathf.Lerp(a.techProgressSpeed, b.techProgressSpeed, t)
        };
    }

    private void ApplyDynamicDifficulty()
    {
        var settings = GetCurrentDifficultySettings();
        
        // Apply to AI civilizations
        var aiCivs = FindObjectsOfType<CivilizationAI>();
        foreach (var ai in aiCivs)
        {
            ai.UpdateDifficultySettings(settings);
        }

        // Trigger dynamic events based on current difficulty
        TriggerDynamicEvents(settings);
    }

    private void TriggerDynamicEvents(DifficultyParameters settings)
    {
        // Calculate event probability based on player performance
        float eventChance = settings.eventFrequency * Time.deltaTime;
        
        if (Random.value < eventChance)
        {
            // Generate appropriate event based on current game state
            GenerateDynamicEvent();
        }
    }

    private void GenerateDynamicEvent()
    {
        // Choose event type based on player's weakest area
        var weakestMetric = playerPerformanceMetrics
            .OrderBy(kvp => kvp.Value)
            .First();

        switch (weakestMetric.Key)
        {
            case "economicStrength":
                TriggerEconomicEvent();
                break;
            case "militaryStrength":
                TriggerMilitaryEvent();
                break;
            case "technologicalProgress":
                TriggerTechnologyEvent();
                break;
            case "territorialControl":
                TriggerTerritorialEvent();
                break;
            case "populationGrowth":
                TriggerPopulationEvent();
                break;
        }
    }

    private void TriggerEconomicEvent()
    {
        // Implement economic events like trade opportunities or resource discoveries
        Debug.Log("Triggering economic event based on current game state");
    }

    private void TriggerMilitaryEvent()
    {
        // Implement military events like barbarian invasions or ally requests
        Debug.Log("Triggering military event based on current game state");
    }

    private void TriggerTechnologyEvent()
    {
        // Implement technology events like discoveries or research opportunities
        Debug.Log("Triggering technology event based on current game state");
    }

    private void TriggerTerritorialEvent()
    {
        // Implement territorial events like natural disasters or new land discoveries
        Debug.Log("Triggering territorial event based on current game state");
    }

    private void TriggerPopulationEvent()
    {
        // Implement population events like migrations or cultural developments
        Debug.Log("Triggering population event based on current game state");
    }
}
