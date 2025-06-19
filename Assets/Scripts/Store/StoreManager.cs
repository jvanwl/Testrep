using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Purchasing;

public class StoreManager : MonoBehaviour, IStoreListener
{
    private static StoreManager _instance;
    public static StoreManager Instance => _instance;

    private IStoreController storeController;
    private IExtensionProvider storeExtensionProvider;

    // Product IDs
    public const string PREMIUM_CURRENCY_PACK_1 = "com.ageofwar.gems_100";
    public const string PREMIUM_CURRENCY_PACK_2 = "com.ageofwar.gems_500";
    public const string PREMIUM_CURRENCY_PACK_3 = "com.ageofwar.gems_1000";
    public const string PREMIUM_SUBSCRIPTION = "com.ageofwar.premium_monthly";
    public const string CIVILIZATION_PACK_1 = "com.ageofwar.civpack_ancient";
    public const string CIVILIZATION_PACK_2 = "com.ageofwar.civpack_medieval";
    public const string NO_ADS = "com.ageofwar.remove_ads";

    [SerializeField] private float gemsToGoldRate = 10f;
    
    public event Action<string> OnPurchaseComplete;
    public event Action<string> OnPurchaseFailed;
    public event Action<int> OnGemsAdded;

    private Dictionary<string, StoreItem> storeItems = new Dictionary<string, StoreItem>();
    private Dictionary<string, SubscriptionInfo> subscriptionStatus = new Dictionary<string, SubscriptionInfo>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeStore()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Premium Currency
        builder.AddProduct(PREMIUM_CURRENCY_PACK_1, ProductType.Consumable, new IDs
        {
            {"gems_100_google", GooglePlay.Name},
            {"gems_100_apple", AppleAppStore.Name}
        });
        builder.AddProduct(PREMIUM_CURRENCY_PACK_2, ProductType.Consumable);
        builder.AddProduct(PREMIUM_CURRENCY_PACK_3, ProductType.Consumable);

        // Subscription
        builder.AddProduct(PREMIUM_SUBSCRIPTION, ProductType.Subscription);

        // DLC Packs
        builder.AddProduct(CIVILIZATION_PACK_1, ProductType.NonConsumable);
        builder.AddProduct(CIVILIZATION_PACK_2, ProductType.NonConsumable);

        // Remove Ads
        builder.AddProduct(NO_ADS, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);

        InitializeStoreItems();
    }

    private void InitializeStoreItems()
    {
        // Premium Currency Packs
        storeItems.Add(PREMIUM_CURRENCY_PACK_1, new StoreItem
        {
            id = PREMIUM_CURRENCY_PACK_1,
            name = "Small Gems Pack",
            description = "100 Premium Gems",
            gemAmount = 100,
            bonusGems = 0,
            type = StoreItemType.Currency
        });

        storeItems.Add(PREMIUM_CURRENCY_PACK_2, new StoreItem
        {
            id = PREMIUM_CURRENCY_PACK_2,
            name = "Medium Gems Pack",
            description = "500 Premium Gems + 50 Bonus",
            gemAmount = 500,
            bonusGems = 50,
            type = StoreItemType.Currency
        });

        storeItems.Add(PREMIUM_CURRENCY_PACK_3, new StoreItem
        {
            id = PREMIUM_CURRENCY_PACK_3,
            name = "Large Gems Pack",
            description = "1000 Premium Gems + 150 Bonus",
            gemAmount = 1000,
            bonusGems = 150,
            type = StoreItemType.Currency
        });

        // Civilization Packs
        storeItems.Add(CIVILIZATION_PACK_1, new StoreItem
        {
            id = CIVILIZATION_PACK_1,
            name = "Ancient Civilizations Pack",
            description = "Unlock 5 new ancient civilizations with unique units and buildings",
            civilizations = new List<string> { "Sumerian", "Phoenician", "Carthaginian", "Assyrian", "Hittite" },
            type = StoreItemType.CivilizationPack
        });

        storeItems.Add(CIVILIZATION_PACK_2, new StoreItem
        {
            id = CIVILIZATION_PACK_2,
            name = "Medieval Civilizations Pack",
            description = "Unlock 5 new medieval civilizations with unique units and buildings",
            civilizations = new List<string> { "Byzantine", "Viking", "Mongol", "Arab", "Frankish" },
            type = StoreItemType.CivilizationPack
        });

        // Premium Subscription
        storeItems.Add(PREMIUM_SUBSCRIPTION, new StoreItem
        {
            id = PREMIUM_SUBSCRIPTION,
            name = "Premium Subscription",
            description = "Monthly benefits: Daily gems, exclusive items, and special events",
            type = StoreItemType.Subscription,
            subscriptionBenefits = new List<string>
            {
                "100 Gems Daily",
                "Exclusive Units",
                "Special Events Access",
                "Priority Support",
                "No Ads"
            }
        });

        // No Ads
        storeItems.Add(NO_ADS, new StoreItem
        {
            id = NO_ADS,
            name = "Remove Ads",
            description = "Remove all advertisements permanently",
            type = StoreItemType.Feature
        });
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;

        // Update prices from store
        foreach (var product in controller.products.all)
        {
            if (storeItems.ContainsKey(product.definition.id))
            {
                storeItems[product.definition.id].price = product.metadata.localizedPrice;
                storeItems[product.definition.id].priceString = product.metadata.localizedPriceString;
            }
        }

        Debug.Log("Store initialized successfully");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"Store initialization failed: {error}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var product = args.purchasedProduct;
        Debug.Log($"Processing purchase: {product.definition.id}");

        if (storeItems.ContainsKey(product.definition.id))
        {
            var item = storeItems[product.definition.id];
            
            switch (item.type)
            {
                case StoreItemType.Currency:
                    ProcessCurrencyPurchase(item);
                    break;
                case StoreItemType.CivilizationPack:
                    ProcessCivilizationPackPurchase(item);
                    break;
                case StoreItemType.Subscription:
                    ProcessSubscriptionPurchase(item, product);
                    break;
                case StoreItemType.Feature:
                    ProcessFeaturePurchase(item);
                    break;
            }

            OnPurchaseComplete?.Invoke(product.definition.id);
            SavePurchase(product.definition.id);
        }
        else
        {
            Debug.LogError($"Unhandled product: {product.definition.id}");
        }

        return PurchaseProcessingResult.Complete;
    }

    private void ProcessCurrencyPurchase(StoreItem item)
    {
        int totalGems = item.gemAmount + item.bonusGems;
        AddGems(totalGems);
        
        // Add bonus gold based on gem amount
        float goldBonus = totalGems * gemsToGoldRate;
        if (CivilizationManager.Instance != null)
        {
            CivilizationManager.Instance.GetPlayerCivilization()?.AddGold(goldBonus);
        }
    }

    private void ProcessCivilizationPackPurchase(StoreItem item)
    {
        foreach (var civName in item.civilizations)
        {
            UnlockCivilization(civName);
        }
    }

    private void ProcessSubscriptionPurchase(StoreItem item, Product product)
    {
        var subscriptionManager = storeExtensionProvider.GetExtension<ISubscriptionExtension>();
        var info = subscriptionManager.GetSubscriptionInfo(product.definition.id);
        
        subscriptionStatus[product.definition.id] = new SubscriptionInfo
        {
            isSubscribed = true,
            expirationDate = info.getExpireDate(),
            isAutoRenewing = info.isAutoRenewing()
        };

        // Apply subscription benefits
        ApplySubscriptionBenefits(item);
    }

    private void ProcessFeaturePurchase(StoreItem item)
    {
        switch (item.id)
        {
            case NO_ADS:
                DisableAds();
                break;
            // Add other feature implementations
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase failed: {product.definition.id}, reason: {failureReason}");
        OnPurchaseFailed?.Invoke(product.definition.id);
    }

    public void PurchaseProduct(string productId)
    {
        try
        {
            if (storeController == null)
            {
                Debug.LogError("Store not initialized");
                return;
            }

            var product = storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.LogError($"Product not available: {productId}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Purchase failed: {e.Message}");
        }
    }

    private void AddGems(int amount)
    {
        PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems", 0) + amount);
        PlayerPrefs.Save();
        OnGemsAdded?.Invoke(amount);
    }

    private void UnlockCivilization(string civName)
    {
        PlayerPrefs.SetInt($"Civ_Unlocked_{civName}", 1);
        PlayerPrefs.Save();
    }

    private void DisableAds()
    {
        PlayerPrefs.SetInt("AdsDisabled", 1);
        PlayerPrefs.Save();
    }

    private void SavePurchase(string productId)
    {
        PlayerPrefs.SetInt($"Purchase_{productId}", 1);
        PlayerPrefs.Save();
    }

    public bool IsProductPurchased(string productId)
    {
        return PlayerPrefs.GetInt($"Purchase_{productId}", 0) == 1;
    }

    public bool IsCivilizationUnlocked(string civName)
    {
        return PlayerPrefs.GetInt($"Civ_Unlocked_{civName}", 0) == 1;
    }

    public bool AreAdsDisabled()
    {
        return PlayerPrefs.GetInt("AdsDisabled", 0) == 1;
    }

    public StoreItem GetStoreItem(string productId)
    {
        return storeItems.ContainsKey(productId) ? storeItems[productId] : null;
    }

    public List<StoreItem> GetAllStoreItems()
    {
        return new List<StoreItem>(storeItems.Values);
    }

    public int GetGemBalance()
    {
        return PlayerPrefs.GetInt("Gems", 0);
    }

    private void ApplySubscriptionBenefits(StoreItem subscription)
    {
        // Implement subscription benefits
        DisableAds();
        // Add other benefits
    }
}

[System.Serializable]
public class StoreItem
{
    public string id;
    public string name;
    public string description;
    public decimal price;
    public string priceString;
    public StoreItemType type;
    
    // Currency specific
    public int gemAmount;
    public int bonusGems;
    
    // Civilization pack specific
    public List<string> civilizations;
    
    // Subscription specific
    public List<string> subscriptionBenefits;
}

[System.Serializable]
public class SubscriptionInfo
{
    public bool isSubscribed;
    public DateTime expirationDate;
    public bool isAutoRenewing;
}

public enum StoreItemType
{
    Currency,
    CivilizationPack,
    Subscription,
    Feature
}
