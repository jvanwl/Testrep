using UnityEngine;
using System.Collections.Generic;
using System;

public class AdvancedDiplomacySystem : MonoBehaviour
{
    public static AdvancedDiplomacySystem Instance { get; private set; }

    [System.Serializable]
    public class DiplomaticAction
    {
        public string id;
        public string name;
        public string description;
        public float baseSuccess;
        public float relationshipImpact;
        public float culturalImpact;
        public float economicCost;
        public string[] prerequisites;
        public Dictionary<string, float> modifiers;
    }

    [System.Serializable]
    public class DiplomaticRelation
    {
        public string civId1;
        public string civId2;
        public float relationshipScore; // -100 to 100
        public Dictionary<string, float> modifiers;
        public List<string> activeAgreements;
        public List<DiplomaticIncident> recentIncidents;
    }

    [System.Serializable]
    public class DiplomaticIncident
    {
        public string id;
        public string type;
        public float impact;
        public float decayRate;
        public DateTime timestamp;
    }

    [System.Serializable]
    public class DynamicAgreement
    {
        public string id;
        public string type;
        public string[] participants;
        public Dictionary<string, float> terms;
        public DateTime startDate;
        public DateTime endDate;
        public bool isActive;
        public float complianceScore;
    }

    private Dictionary<string, List<DiplomaticRelation>> diplomacyNetwork = new Dictionary<string, List<DiplomaticRelation>>();
    private List<DynamicAgreement> activeAgreements = new List<DynamicAgreement>();
    private Dictionary<string, DiplomaticAction> availableActions = new Dictionary<string, DiplomaticAction>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDiplomacy();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDiplomacy()
    {
        LoadDiplomaticActions();
        InitializeRelationships();
    }

    private void LoadDiplomaticActions()
    {
        // Example diplomatic actions
        var actions = new List<DiplomaticAction>
        {
            new DiplomaticAction
            {
                id = "trade_agreement",
                name = "Establish Trade Route",
                description = "Propose a formal trade agreement",
                baseSuccess = 0.7f,
                relationshipImpact = 10f,
                culturalImpact = 5f,
                economicCost = 100f,
                prerequisites = new[] { "market", "roads" },
                modifiers = new Dictionary<string, float>
                {
                    { "existing_trade", 0.2f },
                    { "cultural_similarity", 0.1f },
                    { "military_strength", -0.05f }
                }
            },
            new DiplomaticAction
            {
                id = "cultural_exchange",
                name = "Cultural Exchange Program",
                description = "Establish cultural exchange between civilizations",
                baseSuccess = 0.8f,
                relationshipImpact = 15f,
                culturalImpact = 20f,
                economicCost = 150f,
                prerequisites = new[] { "writing", "cultural_center" },
                modifiers = new Dictionary<string, float>
                {
                    { "cultural_development", 0.3f },
                    { "shared_religion", 0.2f },
                    { "recent_conflicts", -0.4f }
                }
            }
        };

        foreach (var action in actions)
        {
            availableActions[action.id] = action;
        }
    }

    private void InitializeRelationships()
    {
        var civilizations = FindObjectsOfType<Civilization>();
        foreach (var civ1 in civilizations)
        {
            if (!diplomacyNetwork.ContainsKey(civ1.CivilizationId))
            {
                diplomacyNetwork[civ1.CivilizationId] = new List<DiplomaticRelation>();
            }

            foreach (var civ2 in civilizations)
            {
                if (civ1 == civ2) continue;

                var relation = new DiplomaticRelation
                {
                    civId1 = civ1.CivilizationId,
                    civId2 = civ2.CivilizationId,
                    relationshipScore = CalculateInitialRelationship(civ1, civ2),
                    modifiers = new Dictionary<string, float>(),
                    activeAgreements = new List<string>(),
                    recentIncidents = new List<DiplomaticIncident>()
                };

                diplomacyNetwork[civ1.CivilizationId].Add(relation);
            }
        }
    }

    private float CalculateInitialRelationship(Civilization civ1, Civilization civ2)
    {
        float baseScore = 0f;

        // Cultural similarity bonus
        float culturalSimilarity = CalculateCulturalSimilarity(civ1, civ2);
        baseScore += culturalSimilarity * 20f;

        // Geographic proximity penalty/bonus
        float distance = Vector3.Distance(civ1.transform.position, civ2.transform.position);
        float proximityFactor = Mathf.Lerp(1f, -0.5f, distance / 100f);
        baseScore += proximityFactor * 10f;

        // Historical relations
        float historicalFactor = GetHistoricalRelations(civ1, civ2);
        baseScore += historicalFactor * 15f;

        return Mathf.Clamp(baseScore, -100f, 100f);
    }

    public bool ProposeDiplomaticAction(string fromCivId, string toCivId, string actionId)
    {
        if (!availableActions.TryGetValue(actionId, out DiplomaticAction action))
            return false;

        var relation = GetRelation(fromCivId, toCivId);
        if (relation == null)
            return false;

        float successChance = CalculateActionSuccess(action, relation);
        bool success = UnityEngine.Random.value < successChance;

        if (success)
        {
            ExecuteDiplomaticAction(action, relation);
        }

        RecordDiplomaticIncident(relation, actionId, success);
        return success;
    }

    private void ExecuteDiplomaticAction(DiplomaticAction action, DiplomaticRelation relation)
    {
        // Update relationship score
        relation.relationshipScore = Mathf.Clamp(
            relation.relationshipScore + action.relationshipImpact,
            -100f, 100f
        );

        // Create new agreement if applicable
        if (action.id.Contains("agreement"))
        {
            CreateDynamicAgreement(action, relation);
        }

        // Apply cultural impact
        ApplyCulturalImpact(action, relation);

        // Update economic status
        UpdateEconomicStatus(action, relation);
    }

    private void CreateDynamicAgreement(DiplomaticAction action, DiplomaticRelation relation)
    {
        var agreement = new DynamicAgreement
        {
            id = Guid.NewGuid().ToString(),
            type = action.id,
            participants = new[] { relation.civId1, relation.civId2 },
            terms = new Dictionary<string, float>(),
            startDate = DateTime.Now,
            endDate = DateTime.Now.AddMonths(6),
            isActive = true,
            complianceScore = 1f
        };

        activeAgreements.Add(agreement);
        relation.activeAgreements.Add(agreement.id);
    }

    private float CalculateActionSuccess(DiplomaticAction action, DiplomaticRelation relation)
    {
        float chance = action.baseSuccess;

        // Relationship modifier
        chance += relation.relationshipScore * 0.002f;

        // Apply action-specific modifiers
        foreach (var modifier in action.modifiers)
        {
            float modifierValue = GetModifierValue(relation, modifier.Key);
            chance += modifierValue * modifier.Value;
        }

        // Recent incidents impact
        chance += CalculateRecentIncidentsImpact(relation);

        return Mathf.Clamp01(chance);
    }

    private void RecordDiplomaticIncident(DiplomaticRelation relation, string actionId, bool success)
    {
        var incident = new DiplomaticIncident
        {
            id = Guid.NewGuid().ToString(),
            type = actionId,
            impact = success ? 1f : -1f,
            decayRate = 0.1f,
            timestamp = DateTime.Now
        };

        relation.recentIncidents.Add(incident);
        CleanupOldIncidents(relation);
    }

    private void CleanupOldIncidents(DiplomaticRelation relation)
    {
        var now = DateTime.Now;
        relation.recentIncidents.RemoveAll(i => 
            (now - i.timestamp).TotalDays > 30 || 
            Math.Abs(i.impact) < 0.1f
        );
    }

    public void UpdateRelationships()
    {
        foreach (var relations in diplomacyNetwork.Values)
        {
            foreach (var relation in relations)
            {
                UpdateRelation(relation);
            }
        }

        UpdateAgreements();
    }

    private void UpdateRelation(DiplomaticRelation relation)
    {
        // Natural relationship decay/improvement
        float naturalChange = CalculateNaturalChange(relation);
        relation.relationshipScore = Mathf.Clamp(
            relation.relationshipScore + naturalChange,
            -100f, 100f
        );

        // Update incidents
        foreach (var incident in relation.recentIncidents)
        {
            incident.impact *= (1f - incident.decayRate);
        }

        // Check for automatic actions
        CheckForAutomaticActions(relation);
    }

    private void UpdateAgreements()
    {
        var now = DateTime.Now;
        var expiredAgreements = new List<DynamicAgreement>();

        foreach (var agreement in activeAgreements)
        {
            if (now > agreement.endDate)
            {
                expiredAgreements.Add(agreement);
                continue;
            }

            // Update compliance
            agreement.complianceScore = CalculateComplianceScore(agreement);

            // Handle breaches
            if (agreement.complianceScore < 0.3f)
            {
                HandleAgreementBreach(agreement);
            }
        }

        // Remove expired agreements
        foreach (var agreement in expiredAgreements)
        {
            RemoveAgreement(agreement);
        }
    }

    private float CalculateComplianceScore(DynamicAgreement agreement)
    {
        // Implementation would check if all parties are meeting agreement terms
        return 1f; // Placeholder
    }

    private void HandleAgreementBreach(DynamicAgreement agreement)
    {
        // Implementation would handle diplomatic consequences of breaking agreements
    }

    private void RemoveAgreement(DynamicAgreement agreement)
    {
        agreement.isActive = false;
        activeAgreements.Remove(agreement);

        // Remove from relations
        foreach (var civId in agreement.participants)
        {
            if (diplomacyNetwork.TryGetValue(civId, out var relations))
            {
                foreach (var relation in relations)
                {
                    relation.activeAgreements.Remove(agreement.id);
                }
            }
        }
    }

    // Helper methods
    private float CalculateCulturalSimilarity(Civilization civ1, Civilization civ2)
    {
        // Implementation would compare cultural traits and values
        return 0.5f; // Placeholder
    }

    private float GetHistoricalRelations(Civilization civ1, Civilization civ2)
    {
        // Implementation would check historical interactions
        return 0f; // Placeholder
    }

    private float GetModifierValue(DiplomaticRelation relation, string modifierKey)
    {
        // Implementation would calculate specific modifier values
        return 0f; // Placeholder
    }

    private float CalculateRecentIncidentsImpact(DiplomaticRelation relation)
    {
        float impact = 0f;
        var now = DateTime.Now;

        foreach (var incident in relation.recentIncidents)
        {
            float age = (float)(now - incident.timestamp).TotalDays;
            impact += incident.impact * Mathf.Exp(-age / 30f);
        }

        return impact;
    }

    private float CalculateNaturalChange(DiplomaticRelation relation)
    {
        // Implementation would calculate natural relationship changes
        return 0f; // Placeholder
    }

    private void CheckForAutomaticActions(DiplomaticRelation relation)
    {
        // Implementation would trigger automatic diplomatic actions
    }

    private void ApplyCulturalImpact(DiplomaticAction action, DiplomaticRelation relation)
    {
        // Implementation would apply cultural changes
    }

    private void UpdateEconomicStatus(DiplomaticAction action, DiplomaticRelation relation)
    {
        // Implementation would update economic status
    }

    private DiplomaticRelation GetRelation(string fromCivId, string toCivId)
    {
        if (!diplomacyNetwork.TryGetValue(fromCivId, out var relations))
            return null;

        return relations.Find(r => r.civId2 == toCivId);
    }
}
