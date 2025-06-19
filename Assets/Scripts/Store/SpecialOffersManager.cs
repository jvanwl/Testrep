using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SpecialOffer
{
    public string offerId;
    public string displayName;
    public string description;
    public float originalPrice;
    public float discountedPrice;
    public DateTime startTime;
    public DateTime endTime;
    public string[] itemIds;
    public int[] itemQuantities;
    public bool requiresAd;
    public bool isTimeLimited;
    public Sprite offerIcon;
}

public class SpecialOffersManager : MonoBehaviour
{
    public static SpecialOffersManager Instance { get; private set; }

    [SerializeField] private List<SpecialOffer> availableOffers = new List<SpecialOffer>();
    private Dictionary<string, SpecialOffer> activeOffers = new Dictionary<string, SpecialOffer>();
    
    public event System.Action<SpecialOffer> OnOfferActivated;
    public event System.Action<SpecialOffer> OnOfferExpired;
    public event System.Action<SpecialOffer> OnOfferPurchased;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeOffers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeOffers()
    {
        // Load any saved offer state
        foreach (var offer in availableOffers)
        {
            if (ShouldActivateOffer(offer))
            {
                ActivateOffer(offer);
            }
        }
    }

    private bool ShouldActivateOffer(SpecialOffer offer)
    {
        // Check if the offer is within its valid time range
        if (offer.isTimeLimited)
        {
            if (DateTime.Now < offer.startTime || DateTime.Now > offer.endTime)
            {
                return false;
            }
        }

        // Add additional activation logic here (e.g., player level requirements)
        return true;
    }

    private void ActivateOffer(SpecialOffer offer)
    {
        if (!activeOffers.ContainsKey(offer.offerId))
        {
            activeOffers.Add(offer.offerId, offer);
            OnOfferActivated?.Invoke(offer);
        }
    }

    public void PurchaseOffer(string offerId)
    {
        if (!activeOffers.TryGetValue(offerId, out SpecialOffer offer))
        {
            Debug.LogWarning($"Attempted to purchase non-existent offer: {offerId}");
            return;
        }

        // If the offer requires watching an ad first
        if (offer.requiresAd)
        {
            AdManager.Instance.ShowRewardedAd(() => CompletePurchase(offer));
        }
        else
        {
            CompletePurchase(offer);
        }
    }

    private void CompletePurchase(SpecialOffer offer)
    {
        try
        {
            // Verify purchase with StoreManager
            StoreManager.Instance.ProcessPurchase(offer.discountedPrice, () =>
            {
                // Grant items to player
                for (int i = 0; i < offer.itemIds.Length; i++)
                {
                    string itemId = offer.itemIds[i];
                    int quantity = offer.itemQuantities[i];
                    // TODO: Grant items through your game's inventory system
                    Debug.Log($"Granting {quantity}x {itemId} from special offer");
                }

                OnOfferPurchased?.Invoke(offer);

                // Remove time-limited offers after purchase
                if (offer.isTimeLimited)
                {
                    activeOffers.Remove(offer.offerId);
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to complete offer purchase: {e.Message}");
        }
    }

    private void Update()
    {
        List<string> expiredOffers = new List<string>();

        // Check for expired offers
        foreach (var kvp in activeOffers)
        {
            var offer = kvp.Value;
            if (offer.isTimeLimited && DateTime.Now > offer.endTime)
            {
                expiredOffers.Add(offer.offerId);
                OnOfferExpired?.Invoke(offer);
            }
        }

        // Remove expired offers
        foreach (var offerId in expiredOffers)
        {
            activeOffers.Remove(offerId);
        }
    }

    public List<SpecialOffer> GetActiveOffers()
    {
        return new List<SpecialOffer>(activeOffers.Values);
    }

    public bool IsOfferActive(string offerId)
    {
        return activeOffers.ContainsKey(offerId);
    }

    public float GetDiscountPercentage(SpecialOffer offer)
    {
        return (1f - (offer.discountedPrice / offer.originalPrice)) * 100f;
    }
}
