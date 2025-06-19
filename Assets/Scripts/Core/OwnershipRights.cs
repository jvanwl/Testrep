using UnityEngine;
using System;

[Serializable]
public class OwnershipRights : ScriptableObject
{
    [Header("Equity Settings")]
    public float equityPercentage = 0.10f; // 10% ownership
    public bool hasAntidilutionProtection = true;
    public int antidilutionYears = 5;
    
    [Header("Voting Rights")]
    public bool hasVotingRights = true;
    public bool hasBoardObserverRights = true;
    
    [Header("Protection Periods")]
    public int lockupPeriodMonths = 12;
    public int guaranteedLiquidityYears = 5;
    
    [Header("Special Rights")]
    public bool hasTagAlongRights = true;
    public bool hasRightOfFirstRefusal = true;
    public bool hasProRataRights = true;
    
    [Header("Current Valuation")]
    public decimal companyValuation = 0;
    public decimal equityValue = 0;
    
    // Calculate current equity value
    public void UpdateEquityValue()
    {
        equityValue = companyValuation * (decimal)equityPercentage;
    }
    
    // Check if rights are currently exercisable
    public bool CanExerciseLiquidityOption()
    {
        DateTime foundingDate = GameManager.Instance.GetCompanyFoundingDate();
        TimeSpan timeSinceFounding = DateTime.Now - foundingDate;
        return timeSinceFounding.TotalDays > guaranteedLiquidityYears * 365;
    }
    
    // Validate anti-dilution protection
    public bool HasValidAntidilutionProtection()
    {
        DateTime foundingDate = GameManager.Instance.GetCompanyFoundingDate();
        TimeSpan timeSinceFounding = DateTime.Now - foundingDate;
        return hasAntidilutionProtection && timeSinceFounding.TotalDays <= antidilutionYears * 365;
    }
    
    // Calculate voting power
    public float GetVotingPower()
    {
        return hasVotingRights ? equityPercentage : 0f;
    }
}
