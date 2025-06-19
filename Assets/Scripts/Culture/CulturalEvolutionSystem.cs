using UnityEngine;
using System.Collections.Generic;
using System;

public class CulturalEvolutionSystem : MonoBehaviour
{
    private static CulturalEvolutionSystem _instance;
    public static CulturalEvolutionSystem Instance => _instance;

    [SerializeField] private float evolutionRate = 0.1f;
    [SerializeField] private float culturalDiffusionRate = 0.05f;
    [SerializeField] private float tradeInfluenceMultiplier = 1.2f;
    [SerializeField] private float warInfluenceMultiplier = 0.8f;

    private Dictionary<string, CultureState> culturalStates = new Dictionary<string, CultureState>();
    private Dictionary<string, List<CulturalInnovation>> innovations = new Dictionary<string, List<CulturalInnovation>>();

    public event Action<string, CulturalInnovation> OnCulturalInnovation;
    public event Action<string, string, float> OnCulturalDiffusion;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        foreach (var civilization in CivilizationManager.Instance.GetAllCivilizations())
        {
            UpdateCulturalState(civilization);
            ProcessCulturalEvolution(civilization);
            ProcessCulturalDiffusion(civilization);
        }
    }

    private void UpdateCulturalState(Civilization civ)
    {
        if (!culturalStates.ContainsKey(civ.CivName))
        {
            culturalStates[civ.CivName] = new CultureState();
        }

        var state = culturalStates[civ.CivName];
        
        // Update cultural metrics
        state.stability = CalculateCulturalStability(civ);
        state.innovation = CalculateInnovationPotential(civ);
        state.influence = CalculateCulturalInfluence(civ);
        state.resistance = CalculateCulturalResistance(civ);
        
        // Update cultural traits
        state.UpdateTraits(civ);
    }

    private float CalculateCulturalStability(Civilization civ)
    {
        float stability = 0.5f; // Base stability

        // Population happiness affects stability
        stability += civ.GetPopulationHappiness() * 0.2f;

        // Economic prosperity affects stability
        stability += civ.GetEconomicProsperity() * 0.2f;

        // Cultural buildings and monuments increase stability
        stability += civ.GetCulturalBuildingCount() * 0.05f;

        return Mathf.Clamp01(stability);
    }

    private float CalculateInnovationPotential(Civilization civ)
    {
        float potential = 0.3f; // Base potential

        // Education level affects innovation
        potential += civ.GetEducationLevel() * 0.3f;

        // Research facilities increase potential
        potential += civ.GetResearchFacilities() * 0.1f;

        // Trade connections boost innovation
        potential += civ.GetTradeConnectionCount() * 0.05f;

        return Mathf.Clamp01(potential);
    }

    private float CalculateCulturalInfluence(Civilization civ)
    {
        float influence = 0.2f; // Base influence

        // Population size affects influence
        influence += Mathf.Log10(civ.Population) * 0.1f;

        // Cultural achievements boost influence
        influence += civ.GetCulturalAchievements().Count * 0.05f;

        // Diplomatic relations affect influence
        influence += civ.GetDiplomaticInfluence() * 0.2f;

        return Mathf.Clamp01(influence);
    }

    private float CalculateCulturalResistance(Civilization civ)
    {
        float resistance = 0.3f; // Base resistance

        // Cultural stability affects resistance
        resistance += culturalStates[civ.CivName].stability * 0.3f;

        // Isolation increases resistance
        resistance += civ.GetIsolationLevel() * 0.2f;

        // Traditional buildings increase resistance
        resistance += civ.GetTraditionalBuildingCount() * 0.05f;

        return Mathf.Clamp01(resistance);
    }

    private void ProcessCulturalEvolution(Civilization civ)
    {
        var state = culturalStates[civ.CivName];
        
        // Check for new innovations
        if (UnityEngine.Random.value < state.innovation * evolutionRate)
        {
            GenerateCulturalInnovation(civ);
        }

        // Process cultural adaptation
        ProcessCulturalAdaptation(civ);

        // Update cultural values
        UpdateCulturalValues(civ);
    }

    private void GenerateCulturalInnovation(Civilization civ)
    {
        var innovation = new CulturalInnovation
        {
            name = GenerateInnovationName(civ),
            type = DetermineCulturalInnovationType(civ),
            impact = CalculateInnovationImpact(),
            timeToSpread = UnityEngine.Random.Range(10f, 30f)
        };

        if (!innovations.ContainsKey(civ.CivName))
        {
            innovations[civ.CivName] = new List<CulturalInnovation>();
        }

        innovations[civ.CivName].Add(innovation);
        OnCulturalInnovation?.Invoke(civ.CivName, innovation);
    }

    private string GenerateInnovationName(Civilization civ)
    {
        // Generate based on civilization's current state and focus
        var focus = civ.GetCurrentFocus();
        var era = civ.GetCurrentEra();

        switch (focus)
        {
            case "Military":
                return $"New {era} Military Tactics";
            case "Economic":
                return $"Advanced {era} Trade Methods";
            case "Cultural":
                return $"{era} Artistic Revolution";
            case "Scientific":
                return $"{era} Scientific Discovery";
            default:
                return $"New {era} Development";
        }
    }

    private CulturalInnovationType DetermineCulturalInnovationType(Civilization civ)
    {
        // Determine based on civilization's strengths and current needs
        float random = UnityEngine.Random.value;
        var state = culturalStates[civ.CivName];

        if (random < state.innovation * 0.4f)
            return CulturalInnovationType.Technological;
        else if (random < 0.6f)
            return CulturalInnovationType.Social;
        else if (random < 0.8f)
            return CulturalInnovationType.Artistic;
        else
            return CulturalInnovationType.Religious;
    }

    private float CalculateInnovationImpact()
    {
        return UnityEngine.Random.Range(0.1f, 0.5f);
    }

    private void ProcessCulturalAdaptation(Civilization civ)
    {
        var state = culturalStates[civ.CivName];
        
        // Adapt to environmental pressures
        AdaptToEnvironment(civ);

        // Adapt to technological changes
        AdaptToTechnology(civ);

        // Adapt to social changes
        AdaptToSocialChanges(civ);
    }

    private void AdaptToEnvironment(Civilization civ)
    {
        var environment = civ.GetEnvironment();
        var state = culturalStates[civ.CivName];

        // Modify cultural traits based on environment
        foreach (var trait in state.traits)
        {
            trait.Value.adaptationLevel += 
                CalculateEnvironmentalPressure(environment, trait.Key) * evolutionRate;
        }
    }

    private float CalculateEnvironmentalPressure(Environment env, string trait)
    {
        // Calculate how much environmental pressure exists to change this trait
        return 0f; // Implement specific calculation
    }

    private void AdaptToTechnology(Civilization civ)
    {
        var techLevel = civ.GetTechnologyLevel();
        var state = culturalStates[civ.CivName];

        // Modify cultural values based on technology
        state.values.traditionalism -= techLevel * evolutionRate * 0.1f;
        state.values.innovation += techLevel * evolutionRate * 0.15f;
    }

    private void AdaptToSocialChanges(Civilization civ)
    {
        var state = culturalStates[civ.CivName];
        
        // Update social structures based on population and economy
        float urbanization = civ.GetUrbanizationLevel();
        state.values.collectivism -= urbanization * evolutionRate * 0.1f;
        state.values.individualism += urbanization * evolutionRate * 0.1f;
    }

    private void ProcessCulturalDiffusion(Civilization civ)
    {
        var neighbors = CivilizationManager.Instance.GetNeighboringCivilizations(civ);
        
        foreach (var neighbor in neighbors)
        {
            float diffusionStrength = CalculateDiffusionStrength(civ, neighbor);
            DiffuseCulture(civ, neighbor, diffusionStrength);
        }
    }

    private float CalculateDiffusionStrength(Civilization source, Civilization target)
    {
        float strength = culturalDiffusionRate;

        // Trade increases diffusion
        if (source.HasTradeWith(target))
        {
            strength *= tradeInfluenceMultiplier;
        }

        // War affects diffusion
        if (source.IsAtWarWith(target))
        {
            strength *= warInfluenceMultiplier;
        }

        // Cultural resistance reduces diffusion
        strength *= (1f - culturalStates[target.CivName].resistance);

        return strength;
    }

    private void DiffuseCulture(Civilization source, Civilization target, float strength)
    {
        var sourceState = culturalStates[source.CivName];
        var targetState = culturalStates[target.CivName];

        // Diffuse cultural traits
        foreach (var trait in sourceState.traits)
        {
            if (!targetState.traits.ContainsKey(trait.Key))
            {
                targetState.traits[trait.Key] = new CulturalTrait();
            }

            float diffusion = strength * trait.Value.strength;
            targetState.traits[trait.Key].strength += diffusion;

            OnCulturalDiffusion?.Invoke(source.CivName, target.CivName, diffusion);
        }
    }
}

public class CultureState
{
    public float stability;
    public float innovation;
    public float influence;
    public float resistance;
    
    public Dictionary<string, CulturalTrait> traits = new Dictionary<string, CulturalTrait>();
    public CulturalValues values = new CulturalValues();

    public void UpdateTraits(Civilization civ)
    {
        // Update existing traits
        foreach (var trait in traits)
        {
            trait.Value.Update();
        }

        // Add new traits based on civilization's current state
        // Implementation depends on specific trait system
    }
}

public class CulturalTrait
{
    public float strength;
    public float adaptationLevel;
    public float resistance;

    public void Update()
    {
        // Update trait strength based on adaptation and resistance
        strength = Mathf.Lerp(strength, adaptationLevel, 1f - resistance);
    }
}

public class CulturalValues
{
    public float individualism;
    public float collectivism;
    public float traditionalism;
    public float innovation;
    public float hierarchy;
    public float egalitarianism;
}

public class CulturalInnovation
{
    public string name;
    public CulturalInnovationType type;
    public float impact;
    public float timeToSpread;
    public bool isActive;
}

public enum CulturalInnovationType
{
    Technological,
    Social,
    Artistic,
    Religious
}
