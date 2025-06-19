using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }
    
    // Events for ad callbacks
    public event UnityAction OnRewardedAdCompleted;
    public event UnityAction OnInterstitialAdCompleted;
    public event UnityAction OnAdError;
    
    // Configurable parameters
    [SerializeField] private float minTimeBetweenInterstitials = 180f; // 3 minutes
    [SerializeField] private int playSessionsBeforeFirstAd = 3;
    
    // State tracking
    private DateTime lastInterstitialTime;
    private int currentSessionCount;
    private bool isAdInitialized;
    private bool isPremiumUser;

    // Royalty system
    [System.Serializable]
    public class RoyaltyTier
    {
        public int monthlyActiveUsers;
        public float retentionRate;
        public float royaltyPercentage;
    }

    [SerializeField] private float baseRoyaltyPercentage = 0.05f; // 5% base
    [SerializeField] private float maxRoyaltyPercentage = 0.15f; // 15% max
    
    private List<RoyaltyTier> royaltyTiers = new List<RoyaltyTier>
    {
        new RoyaltyTier { monthlyActiveUsers = 100000, retentionRate = 0.3f, royaltyPercentage = 0.02f },
        new RoyaltyTier { monthlyActiveUsers = 500000, retentionRate = 0.4f, royaltyPercentage = 0.03f },
        new RoyaltyTier { monthlyActiveUsers = 1000000, retentionRate = 0.5f, royaltyPercentage = 0.04f }
    };

    private float currentRoyaltyPercentage;
    private float monthlyAdRevenue;
    private float monthlyRoyaltyAmount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadPlayerAdPreferences();
        StartCoroutine(PreloadAds());
    }

    private void InitializeAds()
    {
        try
        {
            // TODO: Initialize your ad SDK here (AdMob, Unity Ads, etc.)
            isAdInitialized = true;
            Debug.Log("Ad system initialized successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize ad system: {e.Message}");
            OnAdError?.Invoke();
        }
    }

    private void LoadPlayerAdPreferences()
    {
        currentSessionCount = PlayerPrefs.GetInt("AdSessionCount", 0);
        isPremiumUser = PlayerPrefs.GetInt("PremiumUser", 0) == 1;
        
        // Increment session count
        currentSessionCount++;
        PlayerPrefs.SetInt("AdSessionCount", currentSessionCount);
        PlayerPrefs.Save();
    }

    private IEnumerator PreloadAds()
    {
        if (!isAdInitialized || isPremiumUser) yield break;

        try
        {
            // TODO: Implement ad preloading logic here
            yield return new WaitForSeconds(1f);
            Debug.Log("Ads preloaded successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to preload ads: {e.Message}");
            OnAdError?.Invoke();
        }
    }

    public bool CanShowInterstitial()
    {
        if (!isAdInitialized || isPremiumUser) return false;
        if (currentSessionCount < playSessionsBeforeFirstAd) return false;
        
        TimeSpan timeSinceLastAd = DateTime.Now - lastInterstitialTime;
        return timeSinceLastAd.TotalSeconds >= minTimeBetweenInterstitials;
    }

    public void ShowRewardedAd(Action onComplete = null)
    {
        if (!isAdInitialized)
        {
            Debug.LogWarning("Ad system not initialized");
            OnAdError?.Invoke();
            return;
        }

        try
        {
            // TODO: Implement rewarded ad display logic
            Debug.Log("Showing rewarded ad");
            onComplete?.Invoke();
            OnRewardedAdCompleted?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to show rewarded ad: {e.Message}");
            OnAdError?.Invoke();
        }
    }

    public void ShowInterstitial(Action onComplete = null)
    {
        if (!CanShowInterstitial())
        {
            onComplete?.Invoke();
            return;
        }

        try
        {
            // TODO: Implement interstitial ad display logic
            Debug.Log("Showing interstitial ad");
            lastInterstitialTime = DateTime.Now;
            onComplete?.Invoke();
            OnInterstitialAdCompleted?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to show interstitial ad: {e.Message}");
            OnAdError?.Invoke();
            onComplete?.Invoke();
        }
    }

    public void SetPremiumUser(bool isPremium)
    {
        isPremiumUser = isPremium;
        PlayerPrefs.SetInt("PremiumUser", isPremium ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            StartCoroutine(PreloadAds());
        }
    }

    // Track ad revenue
    public void TrackAdRevenue(float revenue)
    {
        monthlyAdRevenue += revenue;
        UpdateRoyaltyAmount();
    }

    // Calculate current royalty percentage based on performance
    private void UpdateRoyaltyPercentage()
    {
        float percentage = baseRoyaltyPercentage;
        
        // Get current MAU and retention
        int currentMAU = AnalyticsManager.Instance.GetMonthlyActiveUsers();
        float currentRetention = AnalyticsManager.Instance.GetRetentionRate();

        // Add bonuses based on tiers
        foreach (var tier in royaltyTiers)
        {
            if (currentMAU >= tier.monthlyActiveUsers)
                percentage += tier.royaltyPercentage;
            
            if (currentRetention >= tier.retentionRate)
                percentage += tier.royaltyPercentage;
        }

        // Cap at maximum
        currentRoyaltyPercentage = Mathf.Min(percentage, maxRoyaltyPercentage);
    }

    // Calculate royalty amount
    private void UpdateRoyaltyAmount()
    {
        UpdateRoyaltyPercentage();
        monthlyRoyaltyAmount = monthlyAdRevenue * currentRoyaltyPercentage;
    }

    // Get current royalty info
    public (float revenue, float percentage, float amount) GetCurrentRoyaltyInfo()
    {
        return (monthlyAdRevenue, currentRoyaltyPercentage, monthlyRoyaltyAmount);
    }

    // Reset monthly tracking
    private void ResetMonthlyTracking()
    {
        // Save previous month's data for records
        SaveRoyaltyData();
        
        // Reset current month
        monthlyAdRevenue = 0;
        monthlyRoyaltyAmount = 0;
        UpdateRoyaltyPercentage();
    }

    // Save royalty data for records
    private void SaveRoyaltyData()
    {
        var data = new RoyaltyData
        {
            timestamp = DateTime.Now,
            revenue = monthlyAdRevenue,
            percentage = currentRoyaltyPercentage,
            amount = monthlyRoyaltyAmount
        };

        GameManager.Instance.SaveRoyaltyData(data);
    }
}
