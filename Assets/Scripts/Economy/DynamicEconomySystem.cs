using UnityEngine;
using System.Collections.Generic;
using System;

public class DynamicEconomySystem : MonoBehaviour
{
    public static DynamicEconomySystem Instance { get; private set; }

    [System.Serializable]
    public class Resource
    {
        public string id;
        public string name;
        public float baseValue;
        public float currentValue;
        public float supply;
        public float demand;
        public float productionRate;
        public float consumptionRate;
        public bool isLuxury;
        public bool isStrategic;
        public string[] productionRequirements;
    }

    [System.Serializable]
    public class Market
    {
        public string id;
        public string name;
        public Dictionary<string, float> resourcePrices;
        public Dictionary<string, float> tradeVolume;
        public float marketStrength;
        public float economicStability;
        public List<string> connectedMarkets;
    }

    [System.Serializable]
    public class TradeRoute
    {
        public string id;
        public string sourceMarketId;
        public string destinationMarketId;
        public Dictionary<string, float> resourceFlow;
        public float efficiency;
        public float risk;
        public float cost;
        public bool isActive;
    }

    private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
    private Dictionary<string, Market> markets = new Dictionary<string, Market>();
    private List<TradeRoute> tradeRoutes = new List<TradeRoute>();
    
    [SerializeField] private float economicCycleTime = 10f; // Seconds per economic cycle
    [SerializeField] private float marketVolatility = 0.1f;
    [SerializeField] private float priceChangeThreshold = 0.05f;
    
    private float lastUpdateTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEconomy();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEconomy()
    {
        InitializeResources();
        InitializeMarkets();
        SetupInitialTradeRoutes();
    }

    private void InitializeResources()
    {
        // Basic resources
        AddResource(new Resource
        {
            id = "food",
            name = "Food",
            baseValue = 10f,
            currentValue = 10f,
            supply = 1000f,
            demand = 1000f,
            productionRate = 100f,
            consumptionRate = 100f,
            isLuxury = false,
            isStrategic = true,
            productionRequirements = new string[] { "fertile_land", "labor" }
        });

        // Luxury resources
        AddResource(new Resource
        {
            id = "silk",
            name = "Silk",
            baseValue = 50f,
            currentValue = 50f,
            supply = 100f,
            demand = 200f,
            productionRate = 10f,
            consumptionRate = 15f,
            isLuxury = true,
            isStrategic = false,
            productionRequirements = new string[] { "mulberry_trees", "skilled_labor" }
        });

        // Strategic resources
        AddResource(new Resource
        {
            id = "iron",
            name = "Iron",
            baseValue = 30f,
            currentValue = 30f,
            supply = 500f,
            demand = 600f,
            productionRate = 50f,
            consumptionRate = 60f,
            isLuxury = false,
            isStrategic = true,
            productionRequirements = new string[] { "iron_deposits", "labor", "tools" }
        });
    }

    private void InitializeMarkets()
    {
        var civilizations = FindObjectsOfType<Civilization>();
        foreach (var civ in civilizations)
        {
            CreateMarket(civ);
        }
    }

    private void CreateMarket(Civilization civ)
    {
        var market = new Market
        {
            id = civ.CivilizationId + "_market",
            name = civ.CivilizationName + " Market",
            resourcePrices = new Dictionary<string, float>(),
            tradeVolume = new Dictionary<string, float>(),
            marketStrength = 1f,
            economicStability = 1f,
            connectedMarkets = new List<string>()
        };

        // Initialize prices for all resources
        foreach (var resource in resources.Values)
        {
            market.resourcePrices[resource.id] = resource.currentValue;
            market.tradeVolume[resource.id] = 0f;
        }

        markets[market.id] = market;
    }

    private void SetupInitialTradeRoutes()
    {
        // Create trade routes between neighboring markets
        foreach (var market1 in markets.Values)
        {
            foreach (var market2 in markets.Values)
            {
                if (market1.id == market2.id) continue;

                if (ShouldCreateTradeRoute(market1, market2))
                {
                    CreateTradeRoute(market1, market2);
                }
            }
        }
    }

    private bool ShouldCreateTradeRoute(Market market1, Market market2)
    {
        // Implementation would check distance, political relations, etc.
        return true; // Placeholder
    }

    private void CreateTradeRoute(Market source, Market destination)
    {
        var route = new TradeRoute
        {
            id = Guid.NewGuid().ToString(),
            sourceMarketId = source.id,
            destinationMarketId = destination.id,
            resourceFlow = new Dictionary<string, float>(),
            efficiency = CalculateRouteEfficiency(source, destination),
            risk = CalculateRouteRisk(source, destination),
            cost = CalculateRouteCost(source, destination),
            isActive = true
        };

        foreach (var resource in resources.Values)
        {
            route.resourceFlow[resource.id] = 0f;
        }

        tradeRoutes.Add(route);
        source.connectedMarkets.Add(destination.id);
        destination.connectedMarkets.Add(source.id);
    }

    private void Update()
    {
        if (Time.time - lastUpdateTime >= economicCycleTime)
        {
            UpdateEconomy();
            lastUpdateTime = Time.time;
        }
    }

    private void UpdateEconomy()
    {
        UpdateResourceProduction();
        UpdateResourceConsumption();
        UpdateMarkets();
        UpdateTradeRoutes();
        UpdatePrices();
    }

    private void UpdateResourceProduction()
    {
        foreach (var resource in resources.Values)
        {
            // Calculate actual production based on requirements and conditions
            float actualProduction = CalculateActualProduction(resource);
            resource.supply += actualProduction;
        }
    }

    private float CalculateActualProduction(Resource resource)
    {
        float production = resource.productionRate;

        // Check production requirements
        foreach (var requirement in resource.productionRequirements)
        {
            production *= GetRequirementEfficiency(requirement);
        }

        // Apply random events and modifiers
        production *= GetProductionModifier(resource);

        return production * Time.deltaTime;
    }

    private void UpdateResourceConsumption()
    {
        foreach (var resource in resources.Values)
        {
            // Calculate actual consumption based on population and needs
            float actualConsumption = CalculateActualConsumption(resource);
            resource.supply -= actualConsumption;
            resource.demand = CalculateResourceDemand(resource);
        }
    }

    private void UpdateMarkets()
    {
        foreach (var market in markets.Values)
        {
            UpdateMarketStrength(market);
            UpdateMarketStability(market);
            UpdateTradeVolumes(market);
        }
    }

    private void UpdateTradeRoutes()
    {
        foreach (var route in tradeRoutes)
        {
            if (!route.isActive) continue;

            UpdateTradeRouteFlow(route);
            UpdateRouteEfficiency(route);
            UpdateRouteRisk(route);
        }
    }

    private void UpdatePrices()
    {
        foreach (var market in markets.Values)
        {
            foreach (var resource in resources.Values)
            {
                UpdateResourcePrice(market, resource);
            }
        }
    }

    private void UpdateResourcePrice(Market market, Resource resource)
    {
        float oldPrice = market.resourcePrices[resource.id];
        float supplyDemandRatio = resource.supply / Mathf.Max(resource.demand, 0.01f);
        float marketFactor = market.marketStrength * market.economicStability;
        
        // Calculate new price based on supply/demand and market conditions
        float newPrice = resource.baseValue * supplyDemandRatio * marketFactor;
        
        // Add some randomness within volatility range
        newPrice *= 1f + (UnityEngine.Random.value * 2f - 1f) * marketVolatility;
        
        // Only update if change is significant
        if (Mathf.Abs(newPrice - oldPrice) / oldPrice > priceChangeThreshold)
        {
            market.resourcePrices[resource.id] = newPrice;
        }
    }

    // Helper methods
    private float GetRequirementEfficiency(string requirement)
    {
        // Implementation would check availability of requirements
        return 1f; // Placeholder
    }

    private float GetProductionModifier(Resource resource)
    {
        // Implementation would calculate modifiers based on conditions
        return 1f; // Placeholder
    }

    private float CalculateActualConsumption(Resource resource)
    {
        // Implementation would calculate consumption based on population and needs
        return resource.consumptionRate * Time.deltaTime; // Placeholder
    }

    private float CalculateResourceDemand(Resource resource)
    {
        // Implementation would calculate demand based on population and needs
        return resource.demand; // Placeholder
    }

    private void UpdateMarketStrength(Market market)
    {
        // Implementation would update market strength based on various factors
    }

    private void UpdateMarketStability(Market market)
    {
        // Implementation would update market stability based on various factors
    }

    private void UpdateTradeVolumes(Market market)
    {
        // Implementation would update trade volumes based on active trade routes
    }

    private void UpdateTradeRouteFlow(TradeRoute route)
    {
        // Implementation would update resource flow along trade route
    }

    private void UpdateRouteEfficiency(TradeRoute route)
    {
        // Implementation would update route efficiency based on conditions
    }

    private void UpdateRouteRisk(TradeRoute route)
    {
        // Implementation would update route risk based on conditions
    }

    private float CalculateRouteEfficiency(Market source, Market destination)
    {
        // Implementation would calculate initial route efficiency
        return 1f; // Placeholder
    }

    private float CalculateRouteRisk(Market source, Market destination)
    {
        // Implementation would calculate initial route risk
        return 0f; // Placeholder
    }

    private float CalculateRouteCost(Market source, Market destination)
    {
        // Implementation would calculate initial route cost
        return 100f; // Placeholder
    }

    private void AddResource(Resource resource)
    {
        resources[resource.id] = resource;
    }

    // Public API
    public float GetResourcePrice(string marketId, string resourceId)
    {
        if (markets.TryGetValue(marketId, out Market market) &&
            market.resourcePrices.TryGetValue(resourceId, out float price))
        {
            return price;
        }
        return 0f;
    }

    public bool TryTrade(string buyerMarketId, string sellerMarketId, string resourceId, float amount)
    {
        if (!ValidateTradeParameters(buyerMarketId, sellerMarketId, resourceId, amount))
            return false;

        var route = FindTradeRoute(buyerMarketId, sellerMarketId);
        if (route == null || !route.isActive)
            return false;

        float totalCost = CalculateTradeCost(sellerMarketId, resourceId, amount, route);
        
        // Execute trade if possible
        return ExecuteTrade(buyerMarketId, sellerMarketId, resourceId, amount, totalCost);
    }

    private bool ValidateTradeParameters(string buyerMarketId, string sellerMarketId, string resourceId, float amount)
    {
        return markets.ContainsKey(buyerMarketId) &&
               markets.ContainsKey(sellerMarketId) &&
               resources.ContainsKey(resourceId) &&
               amount > 0;
    }

    private TradeRoute FindTradeRoute(string marketId1, string marketId2)
    {
        return tradeRoutes.Find(r => 
            (r.sourceMarketId == marketId1 && r.destinationMarketId == marketId2) ||
            (r.sourceMarketId == marketId2 && r.destinationMarketId == marketId1));
    }

    private float CalculateTradeCost(string marketId, string resourceId, float amount, TradeRoute route)
    {
        float basePrice = GetResourcePrice(marketId, resourceId);
        float routeCost = route.cost * route.efficiency;
        float riskPremium = basePrice * route.risk;
        
        return (basePrice + routeCost + riskPremium) * amount;
    }

    private bool ExecuteTrade(string buyerMarketId, string sellerMarketId, string resourceId, float amount, float totalCost)
    {
        // Implementation would handle the actual trade execution
        return true; // Placeholder
    }
}
