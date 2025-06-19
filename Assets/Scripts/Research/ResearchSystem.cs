using UnityEngine;
using System.Collections.Generic;

public class ResearchSystem : MonoBehaviour
{
    public static ResearchSystem Instance { get; private set; }

    [System.Serializable]
    public class Technology
    {
        public string id;
        public string name;
        public string description;
        public string[] prerequisites;
        public Dictionary<string, int> cost;
        public float researchTime;
        public Dictionary<string, float> effects;
        public Sprite icon;
    }

    [System.Serializable]
    public class ResearchProgress
    {
        public string technologyId;
        public float progress;
        public bool isComplete;
    }

    private Dictionary<string, Technology> availableTechnologies = new Dictionary<string, Technology>();
    private Dictionary<string, ResearchProgress> researchProgress = new Dictionary<string, ResearchProgress>();
    private List<string> activeResearch = new List<string>();
    private int maxSimultaneousResearch = 1;

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

        LoadTechnologies();
    }

    private void LoadTechnologies()
    {
        // Load from JSON configuration
        TextAsset techJson = Resources.Load<TextAsset>("Data/Technologies");
        if (techJson != null)
        {
            // Parse JSON and populate availableTechnologies
            TechnologyData techData = JsonUtility.FromJson<TechnologyData>(techJson.text);
            foreach (var tech in techData.technologies)
            {
                availableTechnologies[tech.id] = tech;
            }
        }
    }

    public bool CanResearch(string techId)
    {
        if (!availableTechnologies.ContainsKey(techId))
            return false;

        Technology tech = availableTechnologies[techId];

        // Check if already researched
        if (IsResearched(techId))
            return false;

        // Check prerequisites
        foreach (string prereq in tech.prerequisites)
        {
            if (!IsResearched(prereq))
                return false;
        }

        // Check resources
        foreach (var costPair in tech.cost)
        {
            if (ResourceManager.Instance.GetResource(costPair.Key) < costPair.Value)
                return false;
        }

        // Check if we can start new research
        if (activeResearch.Count >= maxSimultaneousResearch)
            return false;

        return true;
    }

    public void StartResearch(string techId)
    {
        if (!CanResearch(techId))
            return;

        Technology tech = availableTechnologies[techId];

        // Deduct resources
        foreach (var costPair in tech.cost)
        {
            ResourceManager.Instance.SpendResource(costPair.Key, costPair.Value);
        }

        // Start research progress
        ResearchProgress progress = new ResearchProgress
        {
            technologyId = techId,
            progress = 0f,
            isComplete = false
        };

        researchProgress[techId] = progress;
        activeResearch.Add(techId);

        // Notify UI
        UIManager.Instance.UpdateResearchUI();
    }

    private void Update()
    {
        // Update research progress
        for (int i = activeResearch.Count - 1; i >= 0; i--)
        {
            string techId = activeResearch[i];
            if (!researchProgress.ContainsKey(techId))
                continue;

            ResearchProgress progress = researchProgress[techId];
            Technology tech = availableTechnologies[techId];

            progress.progress += Time.deltaTime / tech.researchTime;

            if (progress.progress >= 1f)
            {
                CompleteResearch(techId);
                activeResearch.RemoveAt(i);
            }
        }
    }

    private void CompleteResearch(string techId)
    {
        if (!researchProgress.ContainsKey(techId))
            return;

        ResearchProgress progress = researchProgress[techId];
        progress.isComplete = true;
        progress.progress = 1f;

        // Apply technology effects
        Technology tech = availableTechnologies[techId];
        foreach (var effect in tech.effects)
        {
            ApplyTechnologyEffect(effect.Key, effect.Value);
        }

        // Notify UI and other systems
        UIManager.Instance.UpdateResearchUI();
        GameManager.Instance.OnTechnologyResearched(techId);
    }

    private void ApplyTechnologyEffect(string effectType, float value)
    {
        switch (effectType)
        {
            case "unit_damage":
                // Increase unit damage
                break;
            case "unit_health":
                // Increase unit health
                break;
            case "resource_gathering":
                // Improve resource gathering rate
                break;
            case "building_health":
                // Increase building health
                break;
            // Add more effect types as needed
        }
    }

    public bool IsResearched(string techId)
    {
        return researchProgress.ContainsKey(techId) && researchProgress[techId].isComplete;
    }

    public float GetResearchProgress(string techId)
    {
        if (researchProgress.ContainsKey(techId))
            return researchProgress[techId].progress;
        return 0f;
    }

    public List<Technology> GetAvailableTechnologies()
    {
        List<Technology> available = new List<Technology>();
        foreach (var tech in availableTechnologies.Values)
        {
            if (CanResearch(tech.id))
                available.Add(tech);
        }
        return available;
    }

    public List<Technology> GetResearchedTechnologies()
    {
        List<Technology> researched = new List<Technology>();
        foreach (var progress in researchProgress.Values)
        {
            if (progress.isComplete)
                researched.Add(availableTechnologies[progress.technologyId]);
        }
        return researched;
    }
}
