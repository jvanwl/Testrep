using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private GameConfig gameConfig;
    private Dictionary<string, CivilizationData> civilizations;
    private AdManager adManager;
    private RoyaltyTracker royaltyTracker;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        civilizations = new Dictionary<string, CivilizationData>();
        adManager = gameObject.AddComponent<AdManager>();
        royaltyTracker = gameObject.AddComponent<RoyaltyTracker>();
        LoadGameConfig();
    }

    private void LoadGameConfig()
    {
        if (gameConfig == null)
        {
            Debug.LogError("GameConfig not assigned!");
            return;
        }
        // Initialize game systems with config
    }
}