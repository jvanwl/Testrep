using UnityEngine;
using System.Collections.Generic;
using System;

public class CivilizationAI : MonoBehaviour
{
    private Civilization civilization;
    private Dictionary<string, float> personalityTraits = new Dictionary<string, float>();
    private List<AIDecision> decisionHistory = new List<AIDecision>();
    
    [SerializeField] private float decisionInterval = 5f;
    private float nextDecisionTime;

    // Personality traits ranges (0-1)
    private float aggression;
    private float expansion;
    private float trading;
    private float cultural;
    private float technological;
    private float diplomatic;
    private float religious;
    private float traditionalism;

    // Historical focus areas
    private Dictionary<string, float> historicalFocus = new Dictionary<string, float>();
    
    // Cultural preferences
    private List<string> preferredResources = new List<string>();
    private List<string> culturalTaboos = new List<string>();
    private Dictionary<string, float> relationshipMemory = new Dictionary<string, float>();

    public void Initialize(Civilization civ, string civilizationType)
    {
        civilization = civ;
        InitializePersonality(civilizationType);
        InitializeHistoricalFocus(civilizationType);
        InitializeCulturalPreferences(civilizationType);
    }

    private void InitializePersonality(string civType)
    {
        switch (civType)
        {
            case "Mongol":
                aggression = UnityEngine.Random.Range(0.7f, 0.9f);
                expansion = UnityEngine.Random.Range(0.8f, 1.0f);
                trading = UnityEngine.Random.Range(0.3f, 0.5f);
                cultural = UnityEngine.Random.Range(0.2f, 0.4f);
                technological = UnityEngine.Random.Range(0.4f, 0.6f);
                diplomatic = UnityEngine.Random.Range(0.2f, 0.4f);
                religious = UnityEngine.Random.Range(0.3f, 0.5f);
                traditionalism = UnityEngine.Random.Range(0.6f, 0.8f);
                break;

            case "Roman":
                aggression = UnityEngine.Random.Range(0.6f, 0.8f);
                expansion = UnityEngine.Random.Range(0.7f, 0.9f);
                trading = UnityEngine.Random.Range(0.5f, 0.7f);
                cultural = UnityEngine.Random.Range(0.6f, 0.8f);
                technological = UnityEngine.Random.Range(0.6f, 0.8f);
                diplomatic = UnityEngine.Random.Range(0.5f, 0.7f);
                religious = UnityEngine.Random.Range(0.4f, 0.6f);
                traditionalism = UnityEngine.Random.Range(0.7f, 0.9f);
                break;

            case "Chinese":
                aggression = UnityEngine.Random.Range(0.3f, 0.5f);
                expansion = UnityEngine.Random.Range(0.4f, 0.6f);
                trading = UnityEngine.Random.Range(0.7f, 0.9f);
                cultural = UnityEngine.Random.Range(0.8f, 1.0f);
                technological = UnityEngine.Random.Range(0.7f, 0.9f);
                diplomatic = UnityEngine.Random.Range(0.6f, 0.8f);
                religious = UnityEngine.Random.Range(0.5f, 0.7f);
                traditionalism = UnityEngine.Random.Range(0.8f, 1.0f);
                break;

            case "Persian":
                aggression = UnityEngine.Random.Range(0.4f, 0.6f);
                expansion = UnityEngine.Random.Range(0.5f, 0.7f);
                trading = UnityEngine.Random.Range(0.8f, 1.0f);
                cultural = UnityEngine.Random.Range(0.7f, 0.9f);
                technological = UnityEngine.Random.Range(0.6f, 0.8f);
                diplomatic = UnityEngine.Random.Range(0.7f, 0.9f);
                religious = UnityEngine.Random.Range(0.6f, 0.8f);
                traditionalism = UnityEngine.Random.Range(0.6f, 0.8f);
                break;

            case "Greek":
                aggression = UnityEngine.Random.Range(0.4f, 0.6f);
                expansion = UnityEngine.Random.Range(0.5f, 0.7f);
                trading = UnityEngine.Random.Range(0.6f, 0.8f);
                cultural = UnityEngine.Random.Range(0.8f, 1.0f);
                technological = UnityEngine.Random.Range(0.8f, 1.0f);
                diplomatic = UnityEngine.Random.Range(0.6f, 0.8f);
                religious = UnityEngine.Random.Range(0.5f, 0.7f);
                traditionalism = UnityEngine.Random.Range(0.5f, 0.7f);
                break;

            // Add more historical civilizations...
        }
    }

    private void InitializeHistoricalFocus(string civType)
    {
        switch (civType)
        {
            case "Mongol":
                historicalFocus.Add("Cavalry", 0.9f);
                historicalFocus.Add("Military", 0.9f);
                historicalFocus.Add("HorseBreeding", 0.8f);
                historicalFocus.Add("Archery", 0.8f);
                historicalFocus.Add("Conquest", 0.9f);
                break;

            case "Roman":
                historicalFocus.Add("Infrastructure", 0.8f);
                historicalFocus.Add("Law", 0.9f);
                historicalFocus.Add("Engineering", 0.8f);
                historicalFocus.Add("Military", 0.8f);
                historicalFocus.Add("Administration", 0.9f);
                break;

            case "Chinese":
                historicalFocus.Add("Philosophy", 0.9f);
                historicalFocus.Add("Bureaucracy", 0.9f);
                historicalFocus.Add("Agriculture", 0.8f);
                historicalFocus.Add("Writing", 0.9f);
                historicalFocus.Add("Innovation", 0.8f);
                break;

            // Add more historical focuses...
        }
    }

    private void InitializeCulturalPreferences(string civType)
    {
        switch (civType)
        {
            case "Mongol":
                preferredResources.AddRange(new[] { "Horses", "Meat", "Leather", "Gold" });
                culturalTaboos.AddRange(new[] { "Pork", "Settled Life" });
                break;

            case "Chinese":
                preferredResources.AddRange(new[] { "Rice", "Tea", "Silk", "Jade" });
                culturalTaboos.AddRange(new[] { "Barbaric Customs", "Foreign Religion" });
                break;

            case "Islamic":
                preferredResources.AddRange(new[] { "Spices", "Cotton", "Gold", "Books" });
                culturalTaboos.AddRange(new[] { "Alcohol", "Pork" });
                break;

            // Add more cultural preferences...
        }
    }

    private void Update()
    {
        if (Time.time >= nextDecisionTime)
        {
            MakeDecisions();
            nextDecisionTime = Time.time + decisionInterval;
        }
    }

    private void MakeDecisions()
    {
        // Analyze current situation
        var situation = AnalyzeCurrentSituation();
        
        // Make strategic decisions based on personality and situation
        List<AIDecision> decisions = new List<AIDecision>();

        // Military decisions
        if (ShouldFocus(AIFocus.Military, situation))
        {
            decisions.AddRange(MakeMilitaryDecisions(situation));
        }

        // Economic decisions
        if (ShouldFocus(AIFocus.Economic, situation))
        {
            decisions.AddRange(MakeEconomicDecisions(situation));
        }

        // Cultural decisions
        if (ShouldFocus(AIFocus.Cultural, situation))
        {
            decisions.AddRange(MakeCulturalDecisions(situation));
        }

        // Diplomatic decisions
        if (ShouldFocus(AIFocus.Diplomatic, situation))
        {
            decisions.AddRange(MakeDiplomaticDecisions(situation));
        }

        // Execute decisions
        foreach (var decision in decisions)
        {
            ExecuteDecision(decision);
        }
    }

    private SituationAnalysis AnalyzeCurrentSituation()
    {
        return new SituationAnalysis
        {
            militaryStrength = CalculateMilitaryStrength(),
            economicStrength = CalculateEconomicStrength(),
            culturalInfluence = CalculateCulturalInfluence(),
            diplomaticStanding = CalculateDiplomaticStanding(),
            threats = IdentifyThreats(),
            opportunities = IdentifyOpportunities(),
            resourceNeeds = AnalyzeResourceNeeds()
        };
    }

    private bool ShouldFocus(AIFocus focus, SituationAnalysis situation)
    {
        float threshold = 0.6f;

        switch (focus)
        {
            case AIFocus.Military:
                return aggression > threshold || situation.threats.Count > 0;

            case AIFocus.Economic:
                return trading > threshold || situation.resourceNeeds.Count > 0;

            case AIFocus.Cultural:
                return cultural > threshold && situation.militaryStrength > 0.5f;

            case AIFocus.Diplomatic:
                return diplomatic > threshold || situation.threats.Count > 0;

            default:
                return false;
        }
    }

    private List<AIDecision> MakeMilitaryDecisions(SituationAnalysis situation)
    {
        List<AIDecision> decisions = new List<AIDecision>();

        // Defensive decisions
        if (situation.threats.Count > 0)
        {
            foreach (var threat in situation.threats)
            {
                if (threat.threatLevel > 0.7f)
                {
                    decisions.Add(new AIDecision
                    {
                        type = DecisionType.Military,
                        action = "BuildDefenses",
                        target = threat.source,
                        priority = threat.threatLevel
                    });
                }
            }
        }

        // Offensive decisions
        if (aggression > 0.7f && situation.militaryStrength > 0.8f)
        {
            var bestTarget = IdentifyBestMilitaryTarget();
            if (bestTarget != null)
            {
                decisions.Add(new AIDecision
                {
                    type = DecisionType.Military,
                    action = "PrepareInvasion",
                    target = bestTarget,
                    priority = 0.9f
                });
            }
        }

        return decisions;
    }

    private List<AIDecision> MakeEconomicDecisions(SituationAnalysis situation)
    {
        List<AIDecision> decisions = new List<AIDecision>();

        // Resource management
        foreach (var need in situation.resourceNeeds)
        {
            if (need.urgency > 0.7f)
            {
                decisions.Add(new AIDecision
                {
                    type = DecisionType.Economic,
                    action = "AcquireResource",
                    target = need.resourceType,
                    priority = need.urgency
                });
            }
        }

        // Trade decisions
        if (trading > 0.6f)
        {
            var tradingOpportunities = IdentifyTradingOpportunities();
            foreach (var opportunity in tradingOpportunities)
            {
                decisions.Add(new AIDecision
                {
                    type = DecisionType.Economic,
                    action = "EstablishTrade",
                    target = opportunity.partner,
                    priority = opportunity.value
                });
            }
        }

        return decisions;
    }

    private List<AIDecision> MakeCulturalDecisions(SituationAnalysis situation)
    {
        List<AIDecision> decisions = new List<AIDecision>();

        // Cultural development
        if (cultural > 0.6f && situation.economicStrength > 0.7f)
        {
            decisions.Add(new AIDecision
            {
                type = DecisionType.Cultural,
                action = "DevelopCulture",
                priority = cultural
            });
        }

        // Religious influence
        if (religious > 0.6f)
        {
            var target = IdentifyReligiousExpansionTarget();
            if (target != null)
            {
                decisions.Add(new AIDecision
                {
                    type = DecisionType.Cultural,
                    action = "SpreadReligion",
                    target = target,
                    priority = religious
                });
            }
        }

        return decisions;
    }

    private List<AIDecision> MakeDiplomaticDecisions(SituationAnalysis situation)
    {
        List<AIDecision> decisions = new List<AIDecision>();

        // Alliance decisions
        if (diplomatic > 0.6f && situation.threats.Count > 0)
        {
            var potentialAlly = IdentifyPotentialAlly();
            if (potentialAlly != null)
            {
                decisions.Add(new AIDecision
                {
                    type = DecisionType.Diplomatic,
                    action = "ProposeAlliance",
                    target = potentialAlly,
                    priority = 0.8f
                });
            }
        }

        // Peace decisions
        foreach (var threat in situation.threats)
        {
            if (situation.militaryStrength < 0.4f && threat.threatLevel > 0.8f)
            {
                decisions.Add(new AIDecision
                {
                    type = DecisionType.Diplomatic,
                    action = "ProposePeace",
                    target = threat.source,
                    priority = 0.9f
                });
            }
        }

        return decisions;
    }

    private void ExecuteDecision(AIDecision decision)
    {
        switch (decision.type)
        {
            case DecisionType.Military:
                ExecuteMilitaryDecision(decision);
                break;

            case DecisionType.Economic:
                ExecuteEconomicDecision(decision);
                break;

            case DecisionType.Cultural:
                ExecuteCulturalDecision(decision);
                break;

            case DecisionType.Diplomatic:
                ExecuteDiplomaticDecision(decision);
                break;
        }

        decisionHistory.Add(decision);
    }

    // Helper methods for decision execution...
    private void ExecuteMilitaryDecision(AIDecision decision)
    {
        switch (decision.action)
        {
            case "BuildDefenses":
                // Implement defense building logic
                break;

            case "PrepareInvasion":
                // Implement invasion preparation logic
                break;
        }
    }

    private void ExecuteEconomicDecision(AIDecision decision)
    {
        switch (decision.action)
        {
            case "AcquireResource":
                // Implement resource acquisition logic
                break;

            case "EstablishTrade":
                // Implement trade route establishment
                break;
        }
    }

    private void ExecuteCulturalDecision(AIDecision decision)
    {
        switch (decision.action)
        {
            case "DevelopCulture":
                // Implement cultural development
                break;

            case "SpreadReligion":
                // Implement religious spread
                break;
        }
    }

    private void ExecuteDiplomaticDecision(AIDecision decision)
    {
        switch (decision.action)
        {
            case "ProposeAlliance":
                // Implement alliance proposal
                break;

            case "ProposePeace":
                // Implement peace negotiation
                break;
        }
    }
}

public enum AIFocus
{
    Military,
    Economic,
    Cultural,
    Diplomatic
}

public enum DecisionType
{
    Military,
    Economic,
    Cultural,
    Diplomatic
}

public class AIDecision
{
    public DecisionType type;
    public string action;
    public object target;
    public float priority;
    public DateTime timestamp = DateTime.Now;
}

public class SituationAnalysis
{
    public float militaryStrength;
    public float economicStrength;
    public float culturalInfluence;
    public float diplomaticStanding;
    public List<Threat> threats = new List<Threat>();
    public List<Opportunity> opportunities = new List<Opportunity>();
    public List<ResourceNeed> resourceNeeds = new List<ResourceNeed>();
}

public class Threat
{
    public string source;
    public float threatLevel;
    public string type;
}

public class Opportunity
{
    public string type;
    public object target;
    public float value;
}

public class ResourceNeed
{
    public string resourceType;
    public float urgency;
    public float quantity;
}
