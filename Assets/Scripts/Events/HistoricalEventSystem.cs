using UnityEngine;
using System.Collections.Generic;
using System;

public class HistoricalEventSystem : MonoBehaviour
{
    public static HistoricalEventSystem Instance { get; private set; }

    [System.Serializable]
    public class HistoricalEvent
    {
        public string id;
        public string name;
        public string description;
        public string[] triggers;
        public string[] effects;
        public string[] prerequisites;
        public float probability;
        public int minYear;
        public int maxYear;
        public bool isRepeatable;
        public float cooldown;
        public Dictionary<string, float> civilizationModifiers;
    }

    [System.Serializable]
    public class EventChain
    {
        public string id;
        public string name;
        public List<string> eventSequence;
        public float progressionSpeed;
        public bool isActive;
        public int currentStep;
        public Dictionary<string, float> progressModifiers;
    }

    [System.Serializable]
    public class WorldCondition
    {
        public string id;
        public string name;
        public float severity;
        public float duration;
        public Dictionary<string, float> effects;
        public bool isActive;
    }

    private Dictionary<string, HistoricalEvent> events = new Dictionary<string, HistoricalEvent>();
    private List<EventChain> activeChains = new List<EventChain>();
    private List<WorldCondition> activeConditions = new List<WorldCondition>();
    
    [SerializeField] private float eventCheckInterval = 1f;
    private float lastEventCheck;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEventSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEventSystem()
    {
        LoadHistoricalEvents();
        InitializeEventChains();
        SetupWorldConditions();
    }

    private void LoadHistoricalEvents()
    {
        // Example historical events
        var historicalEvents = new List<HistoricalEvent>
        {
            new HistoricalEvent
            {
                id = "plague_outbreak",
                name = "Great Plague Outbreak",
                description = "A devastating plague spreads through the civilized world",
                triggers = new[] { "poor_sanitation", "trade_routes", "high_population_density" },
                effects = new[] { "population_decline", "economic_crisis", "social_upheaval" },
                prerequisites = new[] { "urbanization" },
                probability = 0.1f,
                minYear = -1000,
                maxYear = 1700,
                isRepeatable = true,
                cooldown = 50f,
                civilizationModifiers = new Dictionary<string, float>
                {
                    { "population_growth", -0.5f },
                    { "trade_efficiency", -0.3f },
                    { "research_progress", -0.2f }
                }
            },
            new HistoricalEvent
            {
                id = "golden_age",
                name = "Cultural Golden Age",
                description = "A flourishing of art, science, and culture",
                triggers = new[] { "cultural_development", "economic_prosperity", "political_stability" },
                effects = new[] { "cultural_renaissance", "technological_advancement", "economic_boom" },
                prerequisites = new[] { "writing_system", "cultural_center" },
                probability = 0.05f,
                minYear = -800,
                maxYear = 2000,
                isRepeatable = true,
                cooldown = 100f,
                civilizationModifiers = new Dictionary<string, float>
                {
                    { "research_speed", 1.5f },
                    { "cultural_influence", 2.0f },
                    { "economic_growth", 1.3f }
                }
            }
        };

        foreach (var evt in historicalEvents)
        {
            events[evt.id] = evt;
        }
    }

    private void InitializeEventChains()
    {
        // Example event chains
        activeChains.Add(new EventChain
        {
            id = "rise_and_fall",
            name = "Rise and Fall of Empires",
            eventSequence = new List<string>
            {
                "cultural_unification",
                "imperial_expansion",
                "golden_age",
                "decadence",
                "civil_war",
                "collapse"
            },
            progressionSpeed = 1f,
            isActive = false,
            currentStep = 0,
            progressModifiers = new Dictionary<string, float>()
        });
    }

    private void SetupWorldConditions()
    {
        // Example world conditions
        activeConditions.Add(new WorldCondition
        {
            id = "climate_change",
            name = "Climate Fluctuation",
            severity = 0.5f,
            duration = 100f,
            effects = new Dictionary<string, float>
            {
                { "food_production", -0.2f },
                { "population_growth", -0.1f },
                { "migration_rate", 0.3f }
            },
            isActive = false
        });
    }

    private void Update()
    {
        if (Time.time - lastEventCheck >= eventCheckInterval)
        {
            CheckForEvents();
            UpdateEventChains();
            UpdateWorldConditions();
            lastEventCheck = Time.time;
        }
    }

    private void CheckForEvents()
    {
        var gameYear = GetCurrentGameYear();
        var civilizations = FindObjectsOfType<Civilization>();

        foreach (var evt in events.Values)
        {
            if (ShouldTriggerEvent(evt, gameYear))
            {
                foreach (var civ in civilizations)
                {
                    if (CanEventAffectCivilization(evt, civ))
                    {
                        TriggerHistoricalEvent(evt, civ);
                    }
                }
            }
        }
    }

    private bool ShouldTriggerEvent(HistoricalEvent evt, int currentYear)
    {
        if (currentYear < evt.minYear || currentYear > evt.maxYear)
            return false;

        if (!evt.isRepeatable && HasEventOccurred(evt.id))
            return false;

        if (!CheckEventPrerequisites(evt))
            return false;

        return UnityEngine.Random.value < evt.probability;
    }

    private bool CheckEventPrerequisites(HistoricalEvent evt)
    {
        foreach (var prerequisite in evt.prerequisites)
        {
            if (!IsPrerequisiteMet(prerequisite))
                return false;
        }
        return true;
    }

    private void TriggerHistoricalEvent(HistoricalEvent evt, Civilization civ)
    {
        // Apply event effects
        ApplyEventEffects(evt, civ);

        // Record event occurrence
        RecordEventOccurrence(evt.id);

        // Trigger potential chain reactions
        CheckForChainReactions(evt);

        // Notify the game system
        NotifyEventOccurrence(evt, civ);
    }

    private void UpdateEventChains()
    {
        foreach (var chain in activeChains)
        {
            if (!chain.isActive) continue;

            UpdateChainProgress(chain);
            CheckChainProgression(chain);
        }
    }

    private void UpdateChainProgress(EventChain chain)
    {
        float progress = CalculateChainProgress(chain);
        
        if (progress >= 1f)
        {
            ProgressChain(chain);
        }
    }

    private void CheckChainProgression(EventChain chain)
    {
        if (chain.currentStep >= chain.eventSequence.Count)
        {
            CompleteEventChain(chain);
        }
    }

    private void UpdateWorldConditions()
    {
        foreach (var condition in activeConditions)
        {
            if (!condition.isActive) continue;

            UpdateConditionEffects(condition);
            CheckConditionDuration(condition);
        }
    }

    // Helper methods
    private int GetCurrentGameYear()
    {
        // Implementation would get the current game year
        return 0; // Placeholder
    }

    private bool HasEventOccurred(string eventId)
    {
        // Implementation would check event history
        return false; // Placeholder
    }

    private bool IsPrerequisiteMet(string prerequisite)
    {
        // Implementation would check if prerequisite conditions are met
        return true; // Placeholder
    }

    private void ApplyEventEffects(HistoricalEvent evt, Civilization civ)
    {
        // Implementation would apply event effects to civilization
    }

    private void RecordEventOccurrence(string eventId)
    {
        // Implementation would record event occurrence
    }

    private void CheckForChainReactions(HistoricalEvent evt)
    {
        // Implementation would check for and trigger chain reactions
    }

    private void NotifyEventOccurrence(HistoricalEvent evt, Civilization civ)
    {
        // Implementation would notify game systems of event occurrence
    }

    private float CalculateChainProgress(EventChain chain)
    {
        // Implementation would calculate chain progress
        return 0f; // Placeholder
    }

    private void ProgressChain(EventChain chain)
    {
        // Implementation would progress the event chain
    }

    private void CompleteEventChain(EventChain chain)
    {
        // Implementation would handle chain completion
    }

    private void UpdateConditionEffects(WorldCondition condition)
    {
        // Implementation would update condition effects
    }

    private void CheckConditionDuration(WorldCondition condition)
    {
        // Implementation would check and update condition duration
    }

    private bool CanEventAffectCivilization(HistoricalEvent evt, Civilization civ)
    {
        // Implementation would check if civilization meets event conditions
        return true; // Placeholder
    }
}
