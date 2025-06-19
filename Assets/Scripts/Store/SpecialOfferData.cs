using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SpecialOfferData", menuName = "Game/Store/Special Offer Data")]
public class SpecialOfferData : ScriptableObject
{
    [System.Serializable]
    public class CivilizationBundle
    {
        public string civilizationId;
        public string displayName;
        public string description;
        public float price;
        public string[] includedUnits;
        public string[] includedBuildings;
        public string[] includedTechnologies;
        public Sprite civilizationIcon;
    }

    [System.Serializable]
    public class TimedBundle
    {
        public string bundleId;
        public string displayName;
        public string description;
        public float originalPrice;
        public float discountedPrice;
        public int durationHours;
        public string[] includedItems;
        public int[] itemQuantities;
        public bool isRecurring;
        public Sprite bundleIcon;
    }

    [System.Serializable]
    public class DailyDeal
    {
        public string dealId;
        public string displayName;
        public string itemId;
        public int quantity;
        public float discountPercentage;
        public bool requiresAd;
        public Sprite dealIcon;
    }

    // Example civilization bundles
    public CivilizationBundle[] civilizationBundles = new CivilizationBundle[]
    {
        new CivilizationBundle
        {
            civilizationId = "ancient_egypt_premium",
            displayName = "Ancient Egypt Premium Pack",
            description = "Unlock the mighty civilization of Ancient Egypt with exclusive units and wonders!",
            price = 4.99f,
            includedUnits = new string[] { "royal_guard", "war_chariot", "egyptian_archer" },
            includedBuildings = new string[] { "great_pyramid", "sphinx", "temple_of_ra" },
            includedTechnologies = new string[] { "hieroglyphics", "pyramid_construction", "mummification" }
        },
        new CivilizationBundle
        {
            civilizationId = "rome_premium",
            displayName = "Roman Empire Premium Pack",
            description = "Command the mighty Roman legions and build magnificent structures!",
            price = 4.99f,
            includedUnits = new string[] { "praetorian_guard", "roman_legion", "ballista" },
            includedBuildings = new string[] { "colosseum", "pantheon", "roman_forum" },
            includedTechnologies = new string[] { "roman_architecture", "roman_engineering", "roman_law" }
        }
    };

    // Example timed bundles
    public TimedBundle[] timedBundles = new TimedBundle[]
    {
        new TimedBundle
        {
            bundleId = "starter_pack",
            displayName = "Starter Empire Pack",
            description = "Kickstart your empire with essential resources!",
            originalPrice = 9.99f,
            discountedPrice = 4.99f,
            durationHours = 48,
            includedItems = new string[] { "gold", "wood", "stone", "food" },
            itemQuantities = new int[] { 1000, 500, 500, 500 },
            isRecurring = false
        },
        new TimedBundle
        {
            bundleId = "premium_resources",
            displayName = "Weekly Premium Resources",
            description = "Get premium resources delivered weekly!",
            originalPrice = 4.99f,
            discountedPrice = 3.99f,
            durationHours = 168, // 1 week
            includedItems = new string[] { "gems", "scrolls", "premium_currency" },
            itemQuantities = new int[] { 100, 10, 500 },
            isRecurring = true
        }
    };

    // Example daily deals
    public DailyDeal[] dailyDeals = new DailyDeal[]
    {
        new DailyDeal
        {
            dealId = "daily_gold_boost",
            displayName = "Daily Gold Boost",
            itemId = "gold",
            quantity = 500,
            discountPercentage = 50f,
            requiresAd = true
        },
        new DailyDeal
        {
            dealId = "daily_premium_unit",
            displayName = "Premium Unit of the Day",
            itemId = "random_premium_unit",
            quantity = 1,
            discountPercentage = 30f,
            requiresAd = false
        }
    };
}
