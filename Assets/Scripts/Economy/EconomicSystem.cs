using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Resource
{
    public string name;
    public ResourceType type;
    public float baseValue;
    public float quantity;
    public float productionRate;
    public float consumptionRate;
    public float tradeValue;
    public bool isStrategic;
    public List<string> requiredTechs;
    
    public Resource(string name, ResourceType type, float baseValue, bool isStrategic = false)
    {
        this.name = name;
        this.type = type;
        this.baseValue = baseValue;
        this.isStrategic = isStrategic;
        this.quantity = 0;
        this.productionRate = 0;
        this.consumptionRate = 0;
        this.tradeValue = baseValue;
        this.requiredTechs = new List<string>();
    }
}

public enum ResourceType
{
    Natural,      // Wood, Stone, Iron, etc.
    Agricultural, // Wheat, Livestock, etc.
    Manufactured, // Tools, Weapons, etc.
    Luxury,       // Gold, Spices, Art, etc.
    Cultural,     // Knowledge, Religion, etc.
    Currency,     // Gold coins, paper money
    Strategic     // Military supplies, etc.
}

public class EconomicSystem : MonoBehaviour
{
    private static EconomicSystem _instance;
    public static EconomicSystem Instance => _instance;

    [SerializeField] private float inflationRate = 0.01f;
    [SerializeField] private float marketVolatility = 0.1f;
    [SerializeField] private float tradeMultiplier = 1.5f;

    private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
    private Dictionary<string, float> marketPrices = new Dictionary<string, float>();
    private Dictionary<string, List<TradeAgreement>> tradeAgreements = new Dictionary<string, List<TradeAgreement>>();
    
    public event Action<string, float> OnResourcePriceChanged;
    public event Action<string, float> OnResourceQuantityChanged;
    public event Action<TradeAgreement> OnTradeAgreementCreated;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeResources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeResources()
    {
        // Natural Resources
        AddResource("Wood", ResourceType.Natural, 10);
        AddResource("Stone", ResourceType.Natural, 15);
        AddResource("Iron", ResourceType.Natural, 30);
        AddResource("Coal", ResourceType.Natural, 25);
        AddResource("Oil", ResourceType.Natural, 50, true);

        // Agricultural Resources
        AddResource("Wheat", ResourceType.Agricultural, 8);
        AddResource("Livestock", ResourceType.Agricultural, 20);
        AddResource("Cotton", ResourceType.Agricultural, 15);
        AddResource("Spices", ResourceType.Luxury, 100);

        // Manufactured Resources
        AddResource("Tools", ResourceType.Manufactured, 40);
        AddResource("Weapons", ResourceType.Manufactured, 60, true);
        AddResource("Textiles", ResourceType.Manufactured, 35);
        AddResource("Pottery", ResourceType.Manufactured, 25);

        // Luxury Resources
        AddResource("Gold", ResourceType.Luxury, 200);
        AddResource("Jewelry", ResourceType.Luxury, 150);
        AddResource("Art", ResourceType.Luxury, 180);
        
        // Cultural Resources
        AddResource("Knowledge", ResourceType.Cultural, 100);
        AddResource("Religion", ResourceType.Cultural, 150);

        // Currency
        AddResource("Coins", ResourceType.Currency, 1);
        AddResource("Paper Money", ResourceType.Currency, 1);
    }

    private void AddResource(string name, ResourceType type, float baseValue, bool isStrategic = false)
    {
        Resource resource = new Resource(name, type, baseValue, isStrategic);
        resources[name] = resource;
        marketPrices[name] = baseValue;
    }

    private void Update()
    {
        UpdateEconomy();
        UpdateMarketPrices();
        HandleTradeAgreements();
    }

    private void UpdateEconomy()
    {
        foreach (var resource in resources.Values)
        {
            // Update production
            float production = CalculateProduction(resource);
            resource.quantity += production * Time.deltaTime;

            // Update consumption
            float consumption = CalculateConsumption(resource);
            resource.quantity -= consumption * Time.deltaTime;

            // Ensure non-negative quantities
            resource.quantity = Mathf.Max(0, resource.quantity);

            OnResourceQuantityChanged?.Invoke(resource.name, resource.quantity);
        }
    }

    private float CalculateProduction(Resource resource)
    {
        float baseProduction = resource.productionRate;
        
        // Apply modifiers based on technology, buildings, and civilization bonuses
        float techBonus = CivilizationManager.Instance.GetTechnologyBonus(resource.name);
        float buildingBonus = CivilizationManager.Instance.GetBuildingProductionBonus(resource.name);
        float civilizationBonus = CivilizationManager.Instance.GetResourceProductionBonus(resource.name);

        return baseProduction * (1 + techBonus + buildingBonus + civilizationBonus);
    }

    private float CalculateConsumption(Resource resource)
    {
        float baseConsumption = resource.consumptionRate;
        
        // Apply population and military consumption factors
        float populationConsumption = CivilizationManager.Instance.GetPopulationConsumption(resource.name);
        float militaryConsumption = CivilizationManager.Instance.GetMilitaryConsumption(resource.name);

        return baseConsumption + populationConsumption + militaryConsumption;
    }

    private void UpdateMarketPrices()
    {
        foreach (var resource in resources.Values)
        {
            float supplyDemandFactor = CalculateSupplyDemandFactor(resource);
            float randomFactor = UnityEngine.Random.Range(-marketVolatility, marketVolatility);
            float inflationFactor = 1 + (inflationRate * Time.deltaTime);

            float newPrice = resource.tradeValue * supplyDemandFactor * (1 + randomFactor) * inflationFactor;
            marketPrices[resource.name] = newPrice;

            OnResourcePriceChanged?.Invoke(resource.name, newPrice);
        }
    }

    private float CalculateSupplyDemandFactor(Resource resource)
    {
        float supply = resource.quantity;
        float demand = resource.consumptionRate * CivilizationManager.Instance.GetTotalPopulation();

        if (supply <= 0) return 2.0f; // Extreme scarcity
        return Mathf.Clamp(demand / supply, 0.5f, 2.0f);
    }

    public bool CreateTradeAgreement(string resourceName, float quantity, float price, string buyerCiv, string sellerCiv)
    {
        if (!resources.ContainsKey(resourceName)) return false;

        TradeAgreement agreement = new TradeAgreement
        {
            resourceName = resourceName,
            quantity = quantity,
            price = price,
            buyerCivilization = buyerCiv,
            sellerCivilization = sellerCiv,
            duration = 10f // 10 turns duration by default
        };

        if (!tradeAgreements.ContainsKey(buyerCiv))
        {
            tradeAgreements[buyerCiv] = new List<TradeAgreement>();
        }
        tradeAgreements[buyerCiv].Add(agreement);
        
        OnTradeAgreementCreated?.Invoke(agreement);
        return true;
    }

    private void HandleTradeAgreements()
    {
        foreach (var civAgreements in tradeAgreements)
        {
            for (int i = civAgreements.Value.Count - 1; i >= 0; i--)
            {
                var agreement = civAgreements.Value[i];
                agreement.duration -= Time.deltaTime;

                if (agreement.duration <= 0)
                {
                    civAgreements.Value.RemoveAt(i);
                    continue;
                }

                ExecuteTradeAgreement(agreement);
            }
        }
    }

    private void ExecuteTradeAgreement(TradeAgreement agreement)
    {
        if (!resources.ContainsKey(agreement.resourceName)) return;

        var sellerCiv = CivilizationManager.Instance.GetCivilization(agreement.sellerCivilization);
        var buyerCiv = CivilizationManager.Instance.GetCivilization(agreement.buyerCivilization);

        if (sellerCiv == null || buyerCiv == null) return;

        float tradeAmount = agreement.quantity * Time.deltaTime;
        float payment = tradeAmount * agreement.price;

        if (buyerCiv.SpendCurrency(payment))
        {
            sellerCiv.AddCurrency(payment);
            TransferResource(agreement.resourceName, tradeAmount, sellerCiv, buyerCiv);
        }
    }

    private void TransferResource(string resourceName, float amount, Civilization from, Civilization to)
    {
        if (from.RemoveResource(resourceName, amount))
        {
            to.AddResource(resourceName, amount);
        }
    }

    public float GetResourcePrice(string resourceName)
    {
        return marketPrices.ContainsKey(resourceName) ? marketPrices[resourceName] : 0f;
    }

    public Resource GetResource(string resourceName)
    {
        return resources.ContainsKey(resourceName) ? resources[resourceName] : null;
    }
}

[System.Serializable]
public class TradeAgreement
{
    public string resourceName;
    public float quantity;
    public float price;
    public string buyerCivilization;
    public string sellerCivilization;
    public float duration;
}
