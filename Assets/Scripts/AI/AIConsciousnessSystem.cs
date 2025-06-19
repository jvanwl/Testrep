using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AIConsciousnessSystem : MonoBehaviour
{
    public static AIConsciousnessSystem Instance { get; private set; }

    [System.Serializable]
    public class ConsciousnessState
    {
        public int level;
        public float awareness;
        public float learning_rate;
        public float adaptation_speed;
        public float creativity_factor;
        public Dictionary<string, float> cultural_values = new Dictionary<string, float>();
        public List<string> learned_behaviors = new List<string>();
        public Dictionary<string, float> historical_memory = new Dictionary<string, float>();
    }

    [System.Serializable]
    public class TimelineEvent
    {
        public string id;
        public string name;
        public string description;
        public string[] ai_behavior_triggers;
        public int year;
        public bool has_occurred;
    }

    private ConsciousnessState currentState;
    private Dictionary<string, TimelineEvent> timelineEvents;
    private int currentYear;
    private float evolutionRate = 1.0f;
    private List<string> activeBehaviors = new List<string>();

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

        InitializeConsciousness();
        LoadTimeline();
    }

    private void InitializeConsciousness()
    {
        currentState = new ConsciousnessState
        {
            level = 1,
            awareness = 0.1f,
            learning_rate = 0.01f,
            adaptation_speed = 0.05f,
            creativity_factor = 0.1f
        };

        // Initialize basic cultural values
        currentState.cultural_values["survival"] = 1.0f;
        currentState.cultural_values["growth"] = 0.5f;
        currentState.cultural_values["cooperation"] = 0.3f;
        currentState.cultural_values["innovation"] = 0.2f;
    }

    private void LoadTimeline()
    {
        TextAsset timelineJson = Resources.Load<TextAsset>("Data/UniverseTimeline");
        // Parse timeline and populate events
        timelineEvents = new Dictionary<string, TimelineEvent>();
        currentYear = -10000; // Start at dawn of civilization
    }

    private void Update()
    {
        // Progress time
        float timeScale = Time.deltaTime * evolutionRate;
        ProgressTime(timeScale);

        // Update consciousness
        EvolveConsciousness(timeScale);

        // Process active behaviors
        ProcessBehaviors();
    }

    private void ProgressTime(float timeScale)
    {
        currentYear += Mathf.CeilToInt(timeScale);
        CheckTimelineEvents();
    }

    private void EvolveConsciousness(float timeScale)
    {
        // Increase awareness based on learning rate
        currentState.awareness += currentState.learning_rate * timeScale;

        // Evolve cultural values
        foreach (var value in currentState.cultural_values.Keys.ToList())
        {
            float evolution = Random.Range(-0.1f, 0.2f) * currentState.adaptation_speed * timeScale;
            currentState.cultural_values[value] = Mathf.Clamp01(currentState.cultural_values[value] + evolution);
        }

        // Check for consciousness level advancement
        if (currentState.awareness >= 1.0f * currentState.level)
        {
            AdvanceConsciousnessLevel();
        }
    }

    private void AdvanceConsciousnessLevel()
    {
        currentState.level++;
        currentState.awareness = 0;
        currentState.learning_rate *= 1.2f;
        currentState.adaptation_speed *= 1.1f;
        currentState.creativity_factor *= 1.15f;

        // Unlock new behaviors based on level
        UnlockNewBehaviors();
    }

    private void UnlockNewBehaviors()
    {
        switch (currentState.level)
        {
            case 2:
                AddBehavior("strategic_planning");
                AddBehavior("resource_optimization");
                break;
            case 3:
                AddBehavior("diplomatic_relations");
                AddBehavior("cultural_development");
                break;
            case 4:
                AddBehavior("scientific_research");
                AddBehavior("economic_planning");
                break;
            case 5:
                AddBehavior("advanced_warfare");
                AddBehavior("social_engineering");
                break;
            // Add more levels as needed
        }
    }

    private void AddBehavior(string behavior)
    {
        if (!currentState.learned_behaviors.Contains(behavior))
        {
            currentState.learned_behaviors.Add(behavior);
            activeBehaviors.Add(behavior);
        }
    }

    private void ProcessBehaviors()
    {
        foreach (string behavior in activeBehaviors)
        {
            ExecuteBehavior(behavior);
        }
    }

    private void ExecuteBehavior(string behavior)
    {
        switch (behavior)
        {
            case "strategic_planning":
                PlanStrategy();
                break;
            case "resource_optimization":
                OptimizeResources();
                break;
            case "diplomatic_relations":
                ManageDiplomacy();
                break;
            case "cultural_development":
                DevelopCulture();
                break;
            // Add more behaviors
        }
    }

    private void CheckTimelineEvents()
    {
        foreach (var evt in timelineEvents.Values)
        {
            if (!evt.has_occurred && evt.year <= currentYear)
            {
                TriggerTimelineEvent(evt);
            }
        }
    }

    private void TriggerTimelineEvent(TimelineEvent evt)
    {
        evt.has_occurred = true;
        
        // Apply event effects
        foreach (string trigger in evt.ai_behavior_triggers)
        {
            AddBehavior(trigger);
        }

        // Add to historical memory
        currentState.historical_memory[evt.id] = currentState.awareness;
        
        // Notify other systems
        GameManager.Instance.OnTimelineEventOccurred(evt.id);
    }

    // AI Decision Making Methods
    private void PlanStrategy()
    {
        // Analyze current situation
        float militaryThreat = AnalyzeThreatLevel();
        float resourceStatus = AnalyzeResources();
        float diplomaticStanding = AnalyzeDiplomaticRelations();

        // Make strategic decisions based on analysis
        if (militaryThreat > 0.7f)
        {
            PrioritizeMilitaryDevelopment();
        }
        else if (resourceStatus < 0.3f)
        {
            PrioritizeResourceGathering();
        }
        else if (diplomaticStanding < 0.5f)
        {
            PrioritizeDiplomacy();
        }
    }

    private void OptimizeResources()
    {
        // Implement resource optimization logic
        ResourceManager.Instance.OptimizeResourceAllocation(
            currentState.awareness,
            currentState.cultural_values["growth"]
        );
    }

    private void ManageDiplomacy()
    {
        // Implement diplomatic relations management
        float cooperationValue = currentState.cultural_values["cooperation"];
        DiplomacyManager.Instance.AdjustDiplomaticStance(cooperationValue);
    }

    private void DevelopCulture()
    {
        // Implement cultural development
        float creativityImpact = currentState.creativity_factor * currentState.awareness;
        CultureManager.Instance.AdvanceCulture(creativityImpact);
    }

    // Analysis Methods
    private float AnalyzeThreatLevel()
    {
        // Implement threat analysis
        return 0.5f; // Placeholder
    }

    private float AnalyzeResources()
    {
        // Implement resource analysis
        return 0.7f; // Placeholder
    }

    private float AnalyzeDiplomaticRelations()
    {
        // Implement diplomatic analysis
        return 0.6f; // Placeholder
    }

    // Priority Setting Methods
    private void PrioritizeMilitaryDevelopment()
    {
        // Implement military focus
    }

    private void PrioritizeResourceGathering()
    {
        // Implement resource focus
    }

    private void PrioritizeDiplomacy()
    {
        // Implement diplomatic focus
    }

    // Public interface
    public ConsciousnessState GetCurrentState()
    {
        return currentState;
    }

    public int GetCurrentYear()
    {
        return currentYear;
    }

    public void SetEvolutionRate(float rate)
    {
        evolutionRate = Mathf.Clamp(rate, 0.1f, 10f);
    }

    public List<string> GetActiveBehaviors()
    {
        return new List<string>(activeBehaviors);
    }

    public Dictionary<string, float> GetCulturalValues()
    {
        return new Dictionary<string, float>(currentState.cultural_values);
    }
}
