using UnityEngine;
using System.Collections.Generic;
using System;

public class CulturalSystem : MonoBehaviour
{
    private static CulturalSystem _instance;
    public static CulturalSystem Instance => _instance;

    private Dictionary<string, Culture> cultures = new Dictionary<string, Culture>();
    private Dictionary<string, Religion> religions = new Dictionary<string, Religion>();
    private Dictionary<string, List<CulturalAchievement>> achievements = new Dictionary<string, List<CulturalAchievement>>();

    public event Action<string, Culture> OnCultureSpread;
    public event Action<string, Religion> OnReligionSpread;
    public event Action<string, CulturalAchievement> OnAchievementUnlocked;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCultures();
            InitializeReligions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCultures()
    {
        // Ancient Cultures
        AddCulture("Mesopotamian", new[]
        {
            "Cuneiform Writing",
            "Ziggurats",
            "Code of Laws"
        });

        AddCulture("Egyptian", new[]
        {
            "Pyramids",
            "Hieroglyphs",
            "Mummification"
        });

        // Classical Cultures
        AddCulture("Greek", new[]
        {
            "Philosophy",
            "Democracy",
            "Olympics"
        });

        AddCulture("Roman", new[]
        {
            "Engineering",
            "Law System",
            "Road Networks"
        });

        // Medieval Cultures
        AddCulture("Chinese", new[]
        {
            "Bureaucracy",
            "Confucianism",
            "Great Wall"
        });

        AddCulture("Islamic Golden Age", new[]
        {
            "Mathematics",
            "Astronomy",
            "Architecture"
        });

        // Renaissance Cultures
        AddCulture("Italian Renaissance", new[]
        {
            "Art Revolution",
            "Banking System",
            "Humanism"
        });
    }

    private void InitializeReligions()
    {
        AddReligion("Polytheism", new[]
        {
            "Multiple Deities",
            "Nature Worship",
            "Ritual Practices"
        });

        AddReligion("Monotheism", new[]
        {
            "Single Deity",
            "Sacred Texts",
            "Prophetic Tradition"
        });

        AddReligion("Eastern Philosophy", new[]
        {
            "Meditation",
            "Karma",
            "Reincarnation"
        });
    }

    private void AddCulture(string name, string[] traits)
    {
        var culture = new Culture
        {
            name = name,
            traits = new List<string>(traits),
            influence = 0f,
            spreadRate = 1f
        };
        cultures[name] = culture;
    }

    private void AddReligion(string name, string[] beliefs)
    {
        var religion = new Religion
        {
            name = name,
            beliefs = new List<string>(beliefs),
            followers = 0,
            conversionRate = 1f
        };
        religions[name] = religion;
    }

    public void AddCulturalAchievement(string civName, string name, string description, float culturalValue)
    {
        var achievement = new CulturalAchievement
        {
            name = name,
            description = description,
            culturalValue = culturalValue,
            dateAchieved = DateTime.Now
        };

        if (!achievements.ContainsKey(civName))
        {
            achievements[civName] = new List<CulturalAchievement>();
        }
        achievements[civName].Add(achievement);
        OnAchievementUnlocked?.Invoke(civName, achievement);
    }

    public void UpdateCulturalInfluence(Civilization civ)
    {
        foreach (var culture in cultures.Values)
        {
            float baseInfluence = CalculateBaseInfluence(civ);
            float modifiers = CalculateCulturalModifiers(civ, culture);
            
            culture.influence += baseInfluence * modifiers * Time.deltaTime;
            OnCultureSpread?.Invoke(civ.CivName, culture);
        }
    }

    private float CalculateBaseInfluence(Civilization civ)
    {
        float population = civ.Population;
        float technology = civ.GetTechnologyLevel("Culture");
        float achievements = GetTotalAchievementValue(civ.CivName);
        
        return (population * 0.001f + technology + achievements) * Time.deltaTime;
    }

    private float CalculateCulturalModifiers(Civilization civ, Culture culture)
    {
        float modifier = 1f;
        
        // Technology bonus
        if (civ.HasTechnology("Writing")) modifier *= 1.2f;
        if (civ.HasTechnology("Printing Press")) modifier *= 1.5f;
        if (civ.HasTechnology("Internet")) modifier *= 2f;

        // Trade routes bonus
        float tradeBonus = civ.GetTradeRouteCount() * 0.1f;
        modifier += tradeBonus;

        // Cultural buildings bonus
        float buildingBonus = civ.GetCulturalBuildingCount() * 0.15f;
        modifier += buildingBonus;

        return modifier;
    }

    public void UpdateReligion(Civilization civ)
    {
        foreach (var religion in religions.Values)
        {
            float baseConversion = CalculateBaseConversion(civ);
            float modifiers = CalculateReligiousModifiers(civ, religion);
            
            religion.followers += baseConversion * modifiers * Time.deltaTime;
            OnReligionSpread?.Invoke(civ.CivName, religion);
        }
    }

    private float CalculateBaseConversion(Civilization civ)
    {
        return civ.Population * 0.001f * Time.deltaTime;
    }

    private float CalculateReligiousModifiers(Civilization civ, Religion religion)
    {
        float modifier = 1f;
        
        // Religious buildings bonus
        float buildingBonus = civ.GetReligiousBuildingCount() * 0.2f;
        modifier += buildingBonus;

        // Holy sites bonus
        float holySitesBonus = civ.GetHolySiteCount() * 0.3f;
        modifier += holySitesBonus;

        return modifier;
    }

    private float GetTotalAchievementValue(string civName)
    {
        if (!achievements.ContainsKey(civName)) return 0f;
        
        float total = 0f;
        foreach (var achievement in achievements[civName])
        {
            total += achievement.culturalValue;
        }
        return total;
    }

    public Culture GetCulture(string name)
    {
        return cultures.ContainsKey(name) ? cultures[name] : null;
    }

    public Religion GetReligion(string name)
    {
        return religions.ContainsKey(name) ? religions[name] : null;
    }

    public List<CulturalAchievement> GetAchievements(string civName)
    {
        return achievements.ContainsKey(civName) ? achievements[civName] : new List<CulturalAchievement>();
    }
}

[System.Serializable]
public class Culture
{
    public string name;
    public List<string> traits;
    public float influence;
    public float spreadRate;
}

[System.Serializable]
public class Religion
{
    public string name;
    public List<string> beliefs;
    public float followers;
    public float conversionRate;
}

[System.Serializable]
public class CulturalAchievement
{
    public string name;
    public string description;
    public float culturalValue;
    public DateTime dateAchieved;
}
