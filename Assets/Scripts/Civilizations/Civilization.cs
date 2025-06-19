using UnityEngine;
using System.Collections.Generic;
using System;

public class Civilization : MonoBehaviour
{
    // Basic Information
    public string CivName { get; private set; }
    public CivilizationType Type { get; private set; }
    public float Population { get; private set; }
    public float Currency { get; private set; }

    // Resources and Economy
    private Dictionary<string, float> resources = new Dictionary<string, float>();
    private Dictionary<string, float> productionRates = new Dictionary<string, float>();
    private Dictionary<string, float> consumptionRates = new Dictionary<string, float>();

    // Technology and Development
    private Dictionary<string, float> technologyLevels = new Dictionary<string, float>();
    private List<Building> buildings = new List<Building>();
    private List<Unit> military = new List<Unit>();

    // Diplomacy
    private Dictionary<string, DiplomaticStatus> diplomaticRelations = new Dictionary<string, DiplomaticStatus>();

    // Cultural
    private float culturalPower;
    private float religiousInfluence;
    private List<string> culturalAchievements = new List<string>();

    // Events
    public event Action<float> OnPopulationChanged;
    public event Action<float> OnCurrencyChanged;
    public event Action<string, float> OnResourceChanged;
    public event Action<string, float> OnTechnologyAdvanced;

    [SerializeField] private float basePopulationGrowthRate = 0.001f;
    [SerializeField] private float baseCurrencyIncomeRate = 1f;
    [SerializeField] private float baseResourceProductionRate = 1f;
    [SerializeField] private float baseTechnologyProgressRate = 0.1f;

    public void Initialize(string name, CivilizationType type, Vector2 position)
    {
        CivName = name;
        Type = type;
        transform.position = position;
        
        InitializeStartingConditions();
    }

    private void InitializeStartingConditions()
    {
        Population = 100f;
        Currency = 1000f;
        culturalPower = 0f;
        religiousInfluence = 0f;

        // Initialize resources based on civilization type
        InitializeResources();
        InitializeTechnologies();
        ApplyCivilizationTraits();
    }

    private void InitializeResources()
    {
        // Add starting resources based on civilization type
        foreach (string resourceName in EconomicSystem.Instance.GetAllResourceNames())
        {
            resources[resourceName] = 0f;
            productionRates[resourceName] = baseResourceProductionRate;
            consumptionRates[resourceName] = 0f;
        }

        // Add starting resources based on civilization type
        switch (Type.name)
        {
            case "Mesopotamian":
                AddResource("Wheat", 100f);
                AddResource("Clay", 100f);
                break;
            case "Egyptian":
                AddResource("Stone", 100f);
                AddResource("Gold", 50f);
                break;
            // Add more civilization-specific starting resources
        }
    }

    private void InitializeTechnologies()
    {
        // Initialize basic technologies
        technologyLevels["Agriculture"] = 1f;
        technologyLevels["Construction"] = 1f;
        technologyLevels["Military"] = 1f;
        technologyLevels["Culture"] = 1f;
        technologyLevels["Economy"] = 1f;
    }

    private void ApplyCivilizationTraits()
    {
        foreach (string trait in Type.traits)
        {
            ApplyTrait(trait);
        }
    }

    private void ApplyTrait(string trait)
    {
        switch (trait)
        {
            case "Agriculture Focus":
                productionRates["Wheat"] *= 1.5f;
                productionRates["Livestock"] *= 1.5f;
                break;
            case "River Valley Bonus":
                basePopulationGrowthRate *= 1.2f;
                break;
            case "Early Writing System":
                technologyLevels["Culture"] += 0.5f;
                break;
            // Add more trait implementations
        }
    }

    private void Update()
    {
        UpdatePopulation();
        UpdateResources();
        UpdateEconomy();
        UpdateTechnology();
        UpdateDiplomacy();
    }

    private void UpdatePopulation()
    {
        float foodAvailable = GetTotalFood();
        float populationChange = CalculatePopulationChange(foodAvailable);
        
        Population += populationChange * Time.deltaTime;
        OnPopulationChanged?.Invoke(Population);
    }

    private float GetTotalFood()
    {
        float total = 0f;
        string[] foodResources = { "Wheat", "Livestock", "Fish" };
        foreach (string food in foodResources)
        {
            if (resources.ContainsKey(food))
            {
                total += resources[food];
            }
        }
        return total;
    }

    private float CalculatePopulationChange(float foodAvailable)
    {
        float foodPerPerson = foodAvailable / Population;
        float growthModifier = Mathf.Clamp(foodPerPerson - 1f, -0.5f, 1f);
        return Population * basePopulationGrowthRate * growthModifier;
    }

    private void UpdateResources()
    {
        foreach (var resource in resources.Keys)
        {
            float production = CalculateResourceProduction(resource);
            float consumption = CalculateResourceConsumption(resource);
            
            resources[resource] += (production - consumption) * Time.deltaTime;
            resources[resource] = Mathf.Max(0f, resources[resource]);
            
            OnResourceChanged?.Invoke(resource, resources[resource]);
        }
    }

    private float CalculateResourceProduction(string resource)
    {
        if (!productionRates.ContainsKey(resource)) return 0f;

        float baseProduction = productionRates[resource];
        float techBonus = GetTechnologyBonus(resource);
        float buildingBonus = GetBuildingProductionBonus(resource);
        
        return baseProduction * (1f + techBonus + buildingBonus);
    }

    private float CalculateResourceConsumption(string resource)
    {
        if (!consumptionRates.ContainsKey(resource)) return 0f;

        float baseConsumption = consumptionRates[resource];
        float populationConsumption = Population * 0.01f; // Basic consumption per person
        float militaryConsumption = military.Count * 0.05f; // Additional consumption for military

        return baseConsumption + populationConsumption + militaryConsumption;
    }

    private void UpdateEconomy()
    {
        float income = CalculateIncome();
        float expenses = CalculateExpenses();
        
        Currency += (income - expenses) * Time.deltaTime;
        OnCurrencyChanged?.Invoke(Currency);
    }

    private float CalculateIncome()
    {
        float tradeIncome = CalculateTradeIncome();
        float taxIncome = Population * 0.1f; // Basic tax per person
        return baseCurrencyIncomeRate + tradeIncome + taxIncome;
    }

    private float CalculateTradeIncome()
    {
        float income = 0f;
        foreach (var resource in resources)
        {
            float price = EconomicSystem.Instance.GetResourcePrice(resource.Key);
            income += resource.Value * price * 0.01f; // 1% of resource value as trade income
        }
        return income;
    }

    private float CalculateExpenses()
    {
        float buildingMaintenance = buildings.Count * 5f;
        float militaryMaintenance = military.Count * 10f;
        return buildingMaintenance + militaryMaintenance;
    }

    private void UpdateTechnology()
    {
        foreach (var tech in technologyLevels.Keys.ToList())
        {
            float progress = CalculateTechnologyProgress(tech);
            technologyLevels[tech] += progress * Time.deltaTime;
            OnTechnologyAdvanced?.Invoke(tech, technologyLevels[tech]);
        }
    }

    private float CalculateTechnologyProgress(string technology)
    {
        float culturalModifier = 1f + (culturalPower * 0.1f);
        float resourceModifier = GetResourceTechnologyModifier(technology);
        return baseTechnologyProgressRate * culturalModifier * resourceModifier;
    }

    private float GetResourceTechnologyModifier(string technology)
    {
        switch (technology)
        {
            case "Agriculture":
                return (resources.ContainsKey("Wheat") ? resources["Wheat"] : 0f) * 0.01f;
            case "Military":
                return (resources.ContainsKey("Iron") ? resources["Iron"] : 0f) * 0.01f;
            default:
                return 1f;
        }
    }

    private void UpdateDiplomacy()
    {
        foreach (var relation in diplomaticRelations)
        {
            UpdateDiplomaticRelation(relation.Key);
        }
    }

    private void UpdateDiplomaticRelation(string otherCiv)
    {
        if (!diplomaticRelations.ContainsKey(otherCiv)) return;

        var currentStatus = diplomaticRelations[otherCiv];
        var tradeFactor = CalculateTradeFactor(otherCiv);
        var culturalFactor = CalculateCulturalFactor(otherCiv);
        var militaryFactor = CalculateMilitaryFactor(otherCiv);

        DiplomaticStatus newStatus = DetermineNewStatus(tradeFactor, culturalFactor, militaryFactor);
        if (newStatus != currentStatus)
        {
            diplomaticRelations[otherCiv] = newStatus;
            OnDiplomaticStatusChanged?.Invoke(this, otherCiv);
        }
    }

    private float CalculateTradeFactor(string otherCiv)
    {
        // Implementation of trade relations calculation
        return 0f;
    }

    private float CalculateCulturalFactor(string otherCiv)
    {
        // Implementation of cultural influence calculation
        return 0f;
    }

    private float CalculateMilitaryFactor(string otherCiv)
    {
        // Implementation of military tension calculation
        return 0f;
    }

    private DiplomaticStatus DetermineNewStatus(float tradeFactor, float culturalFactor, float militaryFactor)
    {
        float relations = tradeFactor + culturalFactor - militaryFactor;
        
        if (relations < -0.5f) return DiplomaticStatus.War;
        if (relations < 0f) return DiplomaticStatus.Hostile;
        if (relations < 0.3f) return DiplomaticStatus.Neutral;
        if (relations < 0.7f) return DiplomaticStatus.Friendly;
        return DiplomaticStatus.Allied;
    }

    // Public Methods for Resource Management
    public void AddResource(string resourceName, float amount)
    {
        if (resources.ContainsKey(resourceName))
        {
            resources[resourceName] += amount;
            OnResourceChanged?.Invoke(resourceName, resources[resourceName]);
        }
    }

    public bool RemoveResource(string resourceName, float amount)
    {
        if (resources.ContainsKey(resourceName) && resources[resourceName] >= amount)
        {
            resources[resourceName] -= amount;
            OnResourceChanged?.Invoke(resourceName, resources[resourceName]);
            return true;
        }
        return false;
    }

    public void AddCurrency(float amount)
    {
        Currency += amount;
        OnCurrencyChanged?.Invoke(Currency);
    }

    public bool SpendCurrency(float amount)
    {
        if (Currency >= amount)
        {
            Currency -= amount;
            OnCurrencyChanged?.Invoke(Currency);
            return true;
        }
        return false;
    }

    // Getters for various bonuses
    public float GetTechnologyBonus(string resourceName)
    {
        float bonus = 0f;
        if (technologyLevels.ContainsKey("Economy"))
            bonus += technologyLevels["Economy"] * 0.1f;
        return bonus;
    }

    public float GetBuildingProductionBonus(string resourceName)
    {
        return buildings.Count * 0.05f; // 5% bonus per building
    }

    public float GetResourceProductionBonus(string resourceName)
    {
        return productionRates.ContainsKey(resourceName) ? productionRates[resourceName] - baseResourceProductionRate : 0f;
    }

    public float GetPopulationConsumption(string resourceName)
    {
        return Population * 0.01f; // Basic consumption per person
    }

    public float GetMilitaryConsumption(string resourceName)
    {
        return military.Count * 0.05f; // Consumption per military unit
    }
}

public enum DiplomaticStatus
{
    War,
    Hostile,
    Neutral,
    Friendly,
    Allied
}
