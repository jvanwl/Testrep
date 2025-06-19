using UnityEngine;

public class RoyaltySystem : MonoBehaviour
{
    private const float MIN_ROYALTY = 0.05f; // 5%
    private const float MAX_ROYALTY = 0.15f; // 15%
    
    private float currentRoyaltyPercentage;
    private float totalRevenue;
    
    void Start()
    {
        currentRoyaltyPercentage = MIN_ROYALTY;
        LoadRoyaltyPolicy();
    }

    private void LoadRoyaltyPolicy()
    {
        // Load and verify royalty policy
        Debug.Log("Royalty Policy Loaded - Base Rate: 5%");
    }

    public void TrackRevenue(float amount)
    {
        totalRevenue += amount;
        float royalty = amount * currentRoyaltyPercentage;
        Debug.Log($"Revenue: ${amount:F2}, Royalty: ${royalty:F2} ({currentRoyaltyPercentage:P})");
    }
}