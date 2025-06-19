using UnityEngine;

public class HistoricalDataInitializer : MonoBehaviour
{
    public HistoricalCivilizationData data;

    private void Awake()
    {
        InitializeHistoricalData();
    }

    private void InitializeHistoricalData()
    {
        // Ancient Egypt
        var egypt = new CivilizationData
        {
            name = "Ancient Egypt",
            era = "Ancient",
            timeSpan = new Vector2Int(-3150, -30),
            famousLeaders = new List<string> { "Ramesses II", "Tutankhamun", "Cleopatra", "Thutmose III" },
            culturalTraits = new CulturalTraits
            {
                values = new List<string> { "Afterlife", "Divine Kingship", "Ma'at (Order and Justice)" },
                customs = new List<string> { "Mummification", "Hieroglyphic Writing", "Temple Worship" },
                socialStructures = new List<string> { "Pharaonic System", "Priesthood", "Scribes" },
                artForms = new List<string> { "Monumental Architecture", "Wall Paintings", "Sculptures" },
                architecture = new List<string> { "Pyramids", "Temples", "Obelisks" },
                clothing = new List<string> { "Linen Garments", "Royal Headdresses", "Ceremonial Dress" },
                cuisine = new List<string> { "Bread", "Beer", "Fish", "Dates" },
                taboos = new List<string> { "Dishonoring the Gods", "Disturbing the Dead" },
                festivals = new List<string> { "Opet Festival", "Heb Sed Festival" },
                individualismIndex = 0.2f,
                powerDistanceIndex = 0.9f,
                uncertaintyAvoidanceIndex = 0.8f,
                longTermOrientationIndex = 0.7f
            },
            militaryTraits = new MilitaryTraits
            {
                preferredUnits = new List<string> { "Chariots", "Archers", "Spearmen" },
                tactics = new List<string> { "Chariot Warfare", "Fortress Defense", "River Naval Combat" },
                weapons = new List<string> { "Khopesh", "Compound Bow", "Spear" },
                armor = new List<string> { "Scale Armor", "Shield", "Leather Armor" },
                aggression = 0.6f,
                discipline = 0.8f,
                innovation = 0.7f,
                specialAbilities = new List<string> { "River Warfare", "Desert Warfare" }
            }
        };

        // Roman Empire
        var rome = new CivilizationData
        {
            name = "Roman Empire",
            era = "Classical",
            timeSpan = new Vector2Int(-27, 476),
            famousLeaders = new List<string> { "Augustus", "Trajan", "Marcus Aurelius", "Constantine" },
            culturalTraits = new CulturalTraits
            {
                values = new List<string> { "Law and Order", "Military Glory", "Civic Virtue" },
                customs = new List<string> { "Gladiatorial Games", "Public Baths", "Client System" },
                socialStructures = new List<string> { "Senate", "Patricians", "Plebeians" },
                artForms = new List<string> { "Sculpture", "Mosaics", "Literature" },
                architecture = new List<string> { "Arches", "Domes", "Aqueducts" },
                clothing = new List<string> { "Toga", "Tunic", "Military Uniform" },
                cuisine = new List<string> { "Bread", "Wine", "Olive Oil", "Garum" },
                taboos = new List<string> { "Tyranny", "Dishonoring Family" },
                festivals = new List<string> { "Saturnalia", "Lupercalia" },
                individualismIndex = 0.4f,
                powerDistanceIndex = 0.7f,
                uncertaintyAvoidanceIndex = 0.6f,
                longTermOrientationIndex = 0.6f
            },
            militaryTraits = new MilitaryTraits
            {
                preferredUnits = new List<string> { "Legionaries", "Cavalry", "Siege Engines" },
                tactics = new List<string> { "Legion Formation", "Siege Warfare", "Naval Warfare" },
                weapons = new List<string> { "Gladius", "Pilum", "Scorpio" },
                armor = new List<string> { "Lorica Segmentata", "Scutum", "Galea" },
                aggression = 0.8f,
                discipline = 0.9f,
                innovation = 0.8f,
                specialAbilities = new List<string> { "Road Building", "Siege Warfare" }
            }
        };

        // Imperial China (Han Dynasty)
        var china = new CivilizationData
        {
            name = "Imperial China",
            era = "Classical",
            timeSpan = new Vector2Int(-206, 220),
            famousLeaders = new List<string> { "Emperor Wu", "Emperor Guangwu", "Wang Mang" },
            culturalTraits = new CulturalTraits
            {
                values = new List<string> { "Harmony", "Filial Piety", "Scholarship" },
                customs = new List<string> { "Ancestor Worship", "Calligraphy", "Tea Ceremony" },
                socialStructures = new List<string> { "Imperial Court", "Scholar-Officials", "Peasantry" },
                artForms = new List<string> { "Calligraphy", "Poetry", "Painting" },
                architecture = new List<string> { "Great Wall", "Pagodas", "Imperial Palaces" },
                clothing = new List<string> { "Hanfu", "Silk Robes", "Imperial Dress" },
                cuisine = new List<string> { "Rice", "Tea", "Soy", "Noodles" },
                taboos = new List<string> { "Disrespecting Elders", "Disrupting Harmony" },
                festivals = new List<string> { "Spring Festival", "Mid-Autumn Festival" },
                individualismIndex = 0.2f,
                powerDistanceIndex = 0.8f,
                uncertaintyAvoidanceIndex = 0.7f,
                longTermOrientationIndex = 0.9f
            },
            militaryTraits = new MilitaryTraits
            {
                preferredUnits = new List<string> { "Crossbowmen", "Cavalry", "Infantry" },
                tactics = new List<string> { "Great Wall Defense", "Cavalry Raids", "Siege Warfare" },
                weapons = new List<string> { "Crossbow", "Dao", "Ji" },
                armor = new List<string> { "Lamellar Armor", "Bronze Armor" },
                aggression = 0.5f,
                discipline = 0.8f,
                innovation = 0.9f,
                specialAbilities = new List<string> { "Gunpowder Weapons", "Wall Building" }
            }
        };

        // Add more civilizations here...

        // Add all civilizations to the data
        data.civilizations.Add(egypt);
        data.civilizations.Add(rome);
        data.civilizations.Add(china);
    }
}
