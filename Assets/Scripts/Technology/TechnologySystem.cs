using UnityEngine;
using System.Collections.Generic;
using System;

public class TechnologySystem : MonoBehaviour
{
    private static TechnologySystem _instance;
    public static TechnologySystem Instance => _instance;

    private Dictionary<string, Technology> technologies = new Dictionary<string, Technology>();
    private Dictionary<string, List<string>> technologyTrees = new Dictionary<string, List<string>>();

    public event Action<string> OnTechnologyDiscovered;
    public event Action<string, float> OnResearchProgress;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTechnologies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTechnologies()
    {
        // Ancient Era Technologies
        AddTechnology("Basic Agriculture", "Ancient", new[]
        {
            "Increased food production",
            "Enable farming buildings",
            "Population growth bonus"
        }, null);

        AddTechnology("Basic Tools", "Ancient", new[]
        {
            "Resource gathering efficiency",
            "Enable basic workshops",
            "Production speed increase"
        }, null);

        AddTechnology("Writing", "Ancient", new[]
        {
            "Enable libraries",
            "Cultural development boost",
            "Administrative efficiency"
        }, null);

        // Classical Era Technologies
        AddTechnology("Advanced Agriculture", "Classical", new[]
        {
            "Advanced farming techniques",
            "New crop types",
            "Irrigation systems"
        }, new[] { "Basic Agriculture" });

        AddTechnology("Metal Working", "Classical", new[]
        {
            "Better tools and weapons",
            "Enable metal workshops",
            "Military unit upgrades"
        }, new[] { "Basic Tools" });

        // Medieval Era Technologies
        AddTechnology("Feudalism", "Medieval", new[]
        {
            "Manor system",
            "Knight units",
            "Castle buildings"
        }, new[] { "Advanced Agriculture" });

        AddTechnology("Banking", "Medieval", new[]
        {
            "Advanced economy",
            "Trade improvements",
            "New financial buildings"
        }, new[] { "Writing" });

        // Renaissance Era Technologies
        AddTechnology("Printing Press", "Renaissance", new[]
        {
            "Cultural spread bonus",
            "Research speed increase",
            "Enable universities"
        }, new[] { "Writing" });

        // Industrial Era Technologies
        AddTechnology("Steam Power", "Industrial", new[]
        {
            "Factory production bonus",
            "New transportation options",
            "Resource extraction bonus"
        }, new[] { "Metal Working" });

        AddTechnology("Mass Production", "Industrial", new[]
        {
            "Factory efficiency",
            "Resource conversion bonus",
            "Population growth"
        }, new[] { "Steam Power" });

        // Modern Era Technologies
        AddTechnology("Electronics", "Modern", new[]
        {
            "Advanced production methods",
            "Communication bonus",
            "Research bonus"
        }, new[] { "Steam Power" });

        AddTechnology("Nuclear Power", "Modern", new[]
        {
            "Massive energy production",
            "Advanced military options",
            "Industry bonus"
        }, new[] { "Electronics" });

        // Information Era Technologies
        AddTechnology("Computers", "Information", new[]
        {
            "Research super bonus",
            "Production efficiency",
            "Enable digital infrastructure"
        }, new[] { "Electronics" });

        AddTechnology("Internet", "Information", new[]
        {
            "Cultural super spread",
            "Trade bonus",
            "Information warfare"
        }, new[] { "Computers" });
    }

    private void AddTechnology(string name, string era, string[] effects, string[] prerequisites)
    {
        var tech = new Technology
        {
            name = name,
            era = era,
            effects = new List<string>(effects),
            prerequisites = prerequisites != null ? new List<string>(prerequisites) : new List<string>(),
            researchProgress = 0f,
            isDiscovered = false
        };

        technologies[name] = tech;

        if (!technologyTrees.ContainsKey(era))
        {
            technologyTrees[era] = new List<string>();
        }
        technologyTrees[era].Add(name);
    }

    public bool CanResearch(string techName, Civilization civ)
    {
        if (!technologies.ContainsKey(techName)) return false;
        var tech = technologies[techName];

        if (tech.isDiscovered) return false;

        foreach (var prerequisite in tech.prerequisites)
        {
            if (!IsTechnologyDiscovered(prerequisite, civ))
            {
                return false;
            }
        }

        return true;
    }

    public void ResearchTechnology(string techName, Civilization civ, float researchPoints)
    {
        if (!technologies.ContainsKey(techName)) return;
        var tech = technologies[techName];

        if (!CanResearch(techName, civ)) return;

        tech.researchProgress += researchPoints;
        OnResearchProgress?.Invoke(techName, tech.researchProgress);

        if (tech.researchProgress >= GetRequiredResearchPoints(tech))
        {
            CompleteTechnology(techName, civ);
        }
    }

    private float GetRequiredResearchPoints(Technology tech)
    {
        float basePoints = 100f;
        switch (tech.era)
        {
            case "Ancient": return basePoints;
            case "Classical": return basePoints * 2f;
            case "Medieval": return basePoints * 4f;
            case "Renaissance": return basePoints * 8f;
            case "Industrial": return basePoints * 16f;
            case "Modern": return basePoints * 32f;
            case "Information": return basePoints * 64f;
            default: return basePoints;
        }
    }

    private void CompleteTechnology(string techName, Civilization civ)
    {
        if (!technologies.ContainsKey(techName)) return;
        var tech = technologies[techName];

        tech.isDiscovered = true;
        ApplyTechnologyEffects(tech, civ);
        OnTechnologyDiscovered?.Invoke(techName);
    }

    private void ApplyTechnologyEffects(Technology tech, Civilization civ)
    {
        foreach (var effect in tech.effects)
        {
            ApplyEffect(effect, civ);
        }
    }

    private void ApplyEffect(string effect, Civilization civ)
    {
        // Implementation of various technology effects
        if (effect.Contains("food production"))
        {
            // Increase food production
        }
        else if (effect.Contains("research"))
        {
            // Increase research speed
        }
        // Add more effect implementations
    }

    public bool IsTechnologyDiscovered(string techName, Civilization civ)
    {
        return technologies.ContainsKey(techName) && technologies[techName].isDiscovered;
    }

    public List<string> GetAvailableTechnologies(Civilization civ)
    {
        List<string> available = new List<string>();
        foreach (var tech in technologies.Values)
        {
            if (CanResearch(tech.name, civ))
            {
                available.Add(tech.name);
            }
        }
        return available;
    }

    public List<string> GetTechnologiesInEra(string era)
    {
        return technologyTrees.ContainsKey(era) ? technologyTrees[era] : new List<string>();
    }

    public float GetTechnologyProgress(string techName)
    {
        return technologies.ContainsKey(techName) ? technologies[techName].researchProgress : 0f;
    }
}

[System.Serializable]
public class Technology
{
    public string name;
    public string era;
    public List<string> effects;
    public List<string> prerequisites;
    public float researchProgress;
    public bool isDiscovered;
}
