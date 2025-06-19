using UnityEngine;
using System.Collections.Generic;
using System;

public class PremiumFeaturesManager : MonoBehaviour
{
    private static PremiumFeaturesManager _instance;
    public static PremiumFeaturesManager Instance => _instance;

    [Header("Premium Features")]
    [SerializeField] private List<SpecialUnit> premiumUnits = new List<SpecialUnit>();
    [SerializeField] private List<SpecialBuilding> premiumBuildings = new List<SpecialBuilding>();
    [SerializeField] private List<SpecialWonder> premiumWonders = new List<SpecialWonder>();

    [Header("Special Effects")]
    [SerializeField] private List<ParticleSystem> premiumEffects = new List<ParticleSystem>();
    [SerializeField] private List<AudioClip> premiumSoundEffects = new List<AudioClip>();

    [Header("Boosts")]
    [SerializeField] private float productionSpeedBoost = 1.5f;
    [SerializeField] private float resourceGatheringBoost = 1.5f;
    [SerializeField] private float experienceBoost = 1.5f;
    [SerializeField] private float goldBoost = 1.5f;

    private Dictionary<string, SpecialOffer> activeOffers = new Dictionary<string, SpecialOffer>();
    private Dictionary<string, List<PremiumReward>> dailyRewards = new Dictionary<string, List<PremiumReward>>();

    public event Action<SpecialOffer> OnSpecialOfferAvailable;
    public event Action<PremiumReward> OnRewardClaimed;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFeatures();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFeatures()
    {
        InitializeSpecialOffers();
        InitializeDailyRewards();
        StartCoroutine(CheckForNewOffers());
    }

    private void InitializeSpecialOffers()
    {
        // First Time Purchase Bonus
        AddSpecialOffer(new SpecialOffer
        {
            id = "first_purchase_bonus",
            name = "First Purchase Special",
            description = "Double gems on your first purchase!",
            discount = 0f,
            bonusItems = new List<PremiumReward>
            {
                new PremiumReward { type = RewardType.Gems, amount = 200 },
                new PremiumReward { type = RewardType.SpecialUnit, unitType = "Elite Warrior" }
            },
            duration = TimeSpan.FromDays(7),
            isFirstPurchaseOnly = true
        });

        // Weekend Special
        AddSpecialOffer(new SpecialOffer
        {
            id = "weekend_special",
            name = "Weekend Warriors Pack",
            description = "50% more gems + exclusive unit!",
            discount = 0.2f,
            bonusItems = new List<PremiumReward>
            {
                new PremiumReward { type = RewardType.Gems, amount = 150 },
                new PremiumReward { type = RewardType.SpecialUnit, unitType = "Weekend Champion" }
            },
            duration = TimeSpan.FromDays(2),
            isWeekendOnly = true
        });

        // Holiday Special
        AddSpecialOffer(new SpecialOffer
        {
            id = "holiday_special",
            name = "Holiday Celebration Pack",
            description = "Special holiday themed items and bonuses!",
            discount = 0.3f,
            bonusItems = new List<PremiumReward>
            {
                new PremiumReward { type = RewardType.Gems, amount = 300 },
                new PremiumReward { type = RewardType.SpecialBuilding, buildingType = "Festival Hall" },
                new PremiumReward { type = RewardType.SpecialEffect, effectType = "Holiday Fireworks" }
            },
            duration = TimeSpan.FromDays(14),
            isSeasonalOnly = true
        });
    }

    private void InitializeDailyRewards()
    {
        // Premium Daily Rewards
        var premiumDailyRewards = new List<PremiumReward>
        {
            new PremiumReward { type = RewardType.Gems, amount = 100 },
            new PremiumReward { type = RewardType.Gold, amount = 1000 },
            new PremiumReward { type = RewardType.SpecialUnit, unitType = "Daily Champion" },
            new PremiumReward { type = RewardType.ProductionBoost, duration = TimeSpan.FromHours(4) },
            new PremiumReward { type = RewardType.ResourceBoost, duration = TimeSpan.FromHours(4) }
        };

        dailyRewards["premium"] = premiumDailyRewards;
    }

    public void AddSpecialOffer(SpecialOffer offer)
    {
        if (!activeOffers.ContainsKey(offer.id))
        {
            activeOffers[offer.id] = offer;
            OnSpecialOfferAvailable?.Invoke(offer);
        }
    }

    public bool IsOfferAvailable(string offerId)
    {
        if (activeOffers.TryGetValue(offerId, out SpecialOffer offer))
        {
            return IsOfferValid(offer);
        }
        return false;
    }

    private bool IsOfferValid(SpecialOffer offer)
    {
        if (offer.isFirstPurchaseOnly && HasMadeAnyPurchase())
            return false;

        if (offer.isWeekendOnly && !IsWeekend())
            return false;

        if (offer.isSeasonalOnly && !IsHolidaySeason())
            return false;

        return DateTime.Now < offer.startTime + offer.duration;
    }

    public void ClaimDailyReward(string rewardType)
    {
        if (dailyRewards.TryGetValue(rewardType, out var rewards))
        {
            foreach (var reward in rewards)
            {
                ApplyReward(reward);
                OnRewardClaimed?.Invoke(reward);
            }
        }
    }

    private void ApplyReward(PremiumReward reward)
    {
        switch (reward.type)
        {
            case RewardType.Gems:
                AddGems(reward.amount);
                break;
            case RewardType.Gold:
                AddGold(reward.amount);
                break;
            case RewardType.SpecialUnit:
                UnlockSpecialUnit(reward.unitType);
                break;
            case RewardType.SpecialBuilding:
                UnlockSpecialBuilding(reward.buildingType);
                break;
            case RewardType.ProductionBoost:
                ApplyProductionBoost(reward.duration);
                break;
            case RewardType.ResourceBoost:
                ApplyResourceBoost(reward.duration);
                break;
            case RewardType.SpecialEffect:
                UnlockSpecialEffect(reward.effectType);
                break;
        }
    }

    private void AddGems(int amount)
    {
        PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems", 0) + amount);
        PlayerPrefs.Save();
    }

    private void AddGold(int amount)
    {
        if (CivilizationManager.Instance != null)
        {
            CivilizationManager.Instance.GetPlayerCivilization()?.AddGold(amount);
        }
    }

    private void UnlockSpecialUnit(string unitType)
    {
        PlayerPrefs.SetInt($"SpecialUnit_{unitType}", 1);
        PlayerPrefs.Save();
    }

    private void UnlockSpecialBuilding(string buildingType)
    {
        PlayerPrefs.SetInt($"SpecialBuilding_{buildingType}", 1);
        PlayerPrefs.Save();
    }

    private void UnlockSpecialEffect(string effectType)
    {
        PlayerPrefs.SetInt($"SpecialEffect_{effectType}", 1);
        PlayerPrefs.Save();
    }

    private void ApplyProductionBoost(TimeSpan duration)
    {
        // Implement production boost
    }

    private void ApplyResourceBoost(TimeSpan duration)
    {
        // Implement resource boost
    }

    private bool HasMadeAnyPurchase()
    {
        return PlayerPrefs.GetInt("HasMadePurchase", 0) == 1;
    }

    private bool IsWeekend()
    {
        DayOfWeek today = DateTime.Now.DayOfWeek;
        return today == DayOfWeek.Saturday || today == DayOfWeek.Sunday;
    }

    private bool IsHolidaySeason()
    {
        // Implement holiday season check
        return false;
    }

    public float GetProductionBoost()
    {
        return IsSubscribed() ? productionSpeedBoost : 1f;
    }

    public float GetResourceBoost()
    {
        return IsSubscribed() ? resourceGatheringBoost : 1f;
    }

    public float GetExperienceBoost()
    {
        return IsSubscribed() ? experienceBoost : 1f;
    }

    public float GetGoldBoost()
    {
        return IsSubscribed() ? goldBoost : 1f;
    }

    private bool IsSubscribed()
    {
        return StoreManager.Instance.IsProductPurchased(StoreManager.PREMIUM_SUBSCRIPTION);
    }
}

[System.Serializable]
public class SpecialOffer
{
    public string id;
    public string name;
    public string description;
    public float discount;
    public List<PremiumReward> bonusItems;
    public DateTime startTime;
    public TimeSpan duration;
    public bool isFirstPurchaseOnly;
    public bool isWeekendOnly;
    public bool isSeasonalOnly;
}

[System.Serializable]
public class PremiumReward
{
    public RewardType type;
    public int amount;
    public string unitType;
    public string buildingType;
    public string effectType;
    public TimeSpan duration;
}

[System.Serializable]
public class SpecialUnit
{
    public string name;
    public string description;
    public GameObject prefab;
    public int gemCost;
}

[System.Serializable]
public class SpecialBuilding
{
    public string name;
    public string description;
    public GameObject prefab;
    public int gemCost;
}

[System.Serializable]
public class SpecialWonder
{
    public string name;
    public string description;
    public GameObject prefab;
    public int gemCost;
}

public enum RewardType
{
    Gems,
    Gold,
    SpecialUnit,
    SpecialBuilding,
    ProductionBoost,
    ResourceBoost,
    SpecialEffect
}
