using System;
using UnityEngine;

[Serializable]
public class RoyaltyData
{
    public DateTime timestamp;
    public float revenue;
    public float percentage;
    public float amount;
    public int monthlyActiveUsers;
    public float retentionRate;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static RoyaltyData FromJson(string json)
    {
        return JsonUtility.FromJson<RoyaltyData>(json);
    }
}
