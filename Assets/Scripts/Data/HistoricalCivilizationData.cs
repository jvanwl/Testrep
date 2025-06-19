using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "HistoricalData", menuName = "Game/Historical Data")]
public class HistoricalCivilizationData : ScriptableObject
{
    public List<CivilizationData> civilizations = new List<CivilizationData>();
}

[System.Serializable]
public class CivilizationData
{
    public string name;
    public string era;
    public Vector2Int timeSpan; // Start and end year
    public List<string> historicalEvents;
    public CulturalTraits culturalTraits;
    public MilitaryTraits militaryTraits;
    public EconomicTraits economicTraits;
    public DiplomaticTraits diplomaticTraits;
    public ReligiousTraits religiousTraits;
    public List<string> famousLeaders;
    public List<string> significantInventions;
    public List<GeographicalPreference> geographicalPreferences;
    public List<string> historicalRivals;
    public List<string> historicalAllies;
}

[System.Serializable]
public class CulturalTraits
{
    public List<string> values;
    public List<string> customs;
    public List<string> socialStructures;
    public List<string> artForms;
    public List<string> architecture;
    public List<string> clothing;
    public List<string> cuisine;
    public List<string> taboos;
    public List<string> festivals;
    public float individualismIndex; // 0-1
    public float powerDistanceIndex; // 0-1
    public float uncertaintyAvoidanceIndex; // 0-1
    public float longTermOrientationIndex; // 0-1
}

[System.Serializable]
public class MilitaryTraits
{
    public List<string> preferredUnits;
    public List<string> tactics;
    public List<string> weapons;
    public List<string> armor;
    public float aggression; // 0-1
    public float discipline; // 0-1
    public float innovation; // 0-1
    public List<string> specialAbilities;
}

[System.Serializable]
public class EconomicTraits
{
    public List<string> mainResources;
    public List<string> tradeGoods;
    public List<string> specializations;
    public float merchantTendency; // 0-1
    public float innovationIndex; // 0-1
    public List<string> economicPolicies;
    public List<string> traditionalIndustries;
}

[System.Serializable]
public class DiplomaticTraits
{
    public float trustworthiness; // 0-1
    public float aggressiveness; // 0-1
    public float isolationism; // 0-1
    public List<string> diplomaticPolicies;
    public List<string> preferredAlliances;
}

[System.Serializable]
public class ReligiousTraits
{
    public List<string> mainReligions;
    public List<string> religiousBeliefs;
    public float religiousTolerance; // 0-1
    public float religiousFervor; // 0-1
    public List<string> sacredSites;
    public List<string> rituals;
}

[System.Serializable]
public class GeographicalPreference
{
    public string terrainType;
    public float preference; // 0-1
    public List<string> adaptations;
}
