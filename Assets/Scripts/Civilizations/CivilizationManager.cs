using UnityEngine;
using System.Collections.Generic;
using System;

public class CivilizationManager : MonoBehaviour
{
    private static CivilizationManager _instance;
    public static CivilizationManager Instance => _instance;

    private Dictionary<string, Civilization> civilizations = new Dictionary<string, Civilization>();
    private Dictionary<string, CivilizationType> civilizationTypes = new Dictionary<string, CivilizationType>();

    public event Action<Civilization> OnCivilizationCreated;
    public event Action<Civilization> OnCivilizationDestroyed;
    public event Action<Civilization, string> OnDiplomaticStatusChanged;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCivilizationTypes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCivilizationTypes()
    {
        // Ancient Civilizations
        AddCivilizationType("Mesopotamian", new[]
        {
            "Agriculture Focus",
            "River Valley Bonus",
            "Early Writing System"
        });

        AddCivilizationType("Egyptian", new[]
        {
            "Monument Builders",
            "Desert Adaptation",
            "Religious Power"
        });

        AddCivilizationType("Greek", new[]
        {
            "Philosophy Bonus",
            "Naval Trade",
            "Democratic Government"
        });

        // Medieval Civilizations
        AddCivilizationType("European", new[]
        {
            "Feudal System",
            "Castle Building",
            "Religious Unity"
        });

        AddCivilizationType("Islamic", new[]
        {
            "Trade Routes",
            "Scientific Knowledge",
            "Cultural Growth"
        });

        AddCivilizationType("Asian", new[]
        {
            "Rice Cultivation",
            "Bureaucratic Efficiency",
            "Technological Innovation"
        });

        // Modern Civilizations
        AddCivilizationType("Industrial", new[]
        {
            "Factory Production",
            "Resource Extraction",
            "Urban Development"
        });

        AddCivilizationType("Information", new[]
        {
            "Digital Technology",
            "Global Trade",
            "Research Focus"
        });
    }

    private void AddCivilizationType(string name, string[] traits)
    {
        var civType = new CivilizationType
        {
            name = name,
            traits = new List<string>(traits)
        };
        civilizationTypes[name] = civType;
    }

    public Civilization CreateCivilization(string name, string type, Vector2 startPosition)
    {
        if (civilizations.ContainsKey(name)) return null;

        GameObject civObject = new GameObject($"Civilization_{name}");
        Civilization newCiv = civObject.AddComponent<Civilization>();
        
        if (civilizationTypes.TryGetValue(type, out CivilizationType civType))
        {
            newCiv.Initialize(name, civType, startPosition);
            civilizations[name] = newCiv;
            OnCivilizationCreated?.Invoke(newCiv);
            return newCiv;
        }
        
        Destroy(civObject);
        return null;
    }

    public void DestroyCivilization(string name)
    {
        if (civilizations.TryGetValue(name, out Civilization civ))
        {
            civilizations.Remove(name);
            OnCivilizationDestroyed?.Invoke(civ);
            Destroy(civ.gameObject);
        }
    }

    public Civilization GetCivilization(string name)
    {
        return civilizations.TryGetValue(name, out Civilization civ) ? civ : null;
    }

    public float GetTechnologyBonus(string resourceName)
    {
        float totalBonus = 0f;
        foreach (var civ in civilizations.Values)
        {
            totalBonus += civ.GetTechnologyBonus(resourceName);
        }
        return totalBonus;
    }

    public float GetBuildingProductionBonus(string resourceName)
    {
        float totalBonus = 0f;
        foreach (var civ in civilizations.Values)
        {
            totalBonus += civ.GetBuildingProductionBonus(resourceName);
        }
        return totalBonus;
    }

    public float GetResourceProductionBonus(string resourceName)
    {
        float totalBonus = 0f;
        foreach (var civ in civilizations.Values)
        {
            totalBonus += civ.GetResourceProductionBonus(resourceName);
        }
        return totalBonus;
    }

    public float GetPopulationConsumption(string resourceName)
    {
        float totalConsumption = 0f;
        foreach (var civ in civilizations.Values)
        {
            totalConsumption += civ.GetPopulationConsumption(resourceName);
        }
        return totalConsumption;
    }

    public float GetMilitaryConsumption(string resourceName)
    {
        float totalConsumption = 0f;
        foreach (var civ in civilizations.Values)
        {
            totalConsumption += civ.GetMilitaryConsumption(resourceName);
        }
        return totalConsumption;
    }

    public float GetTotalPopulation()
    {
        float totalPopulation = 0f;
        foreach (var civ in civilizations.Values)
        {
            totalPopulation += civ.Population;
        }
        return totalPopulation;
    }
}

[System.Serializable]
public class CivilizationType
{
    public string name;
    public List<string> traits;
}
