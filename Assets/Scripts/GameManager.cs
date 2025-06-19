using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance 
    { 
        get { return _instance; }
        private set { _instance = value; }
    }

    // Resources
    [SerializeField] private int startingGold = 100;
    private int gold;
    private int experience;
    
    // Game State
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
    public GameState CurrentGameState { get; private set; }

    // Player Stats
    public int PlayerLevel { get; private set; }
    public int BaseHealth { get; private set; }
    public float BaseDefense { get; private set; }

    // Lists for managing game entities
    private List<Unit> activeUnits = new List<Unit>();
    private List<Building> playerBuildings = new List<Building>();

    // Game Settings
    [SerializeField] private int startingGold = 100;
    [SerializeField] private int startingBaseHealth = 1000;
    [SerializeField] private float baseDefenseMultiplier = 1.0f;

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
        Gold = startingGold;
        Experience = 0;
        PlayerLevel = 1;
        BaseHealth = startingBaseHealth;
        BaseDefense = baseDefenseMultiplier;
        CurrentGameState = GameState.MainMenu;
    }

    void Start()
    {
        Debug.Log("Age of War - GameManager initialized successfully!");
    }

    void Update()
    {
        if (CurrentGameState == GameState.Playing)
        {
            UpdateGameState();
        }
    }

    private void UpdateGameState()
    {
        // Update all active units
        for (int i = activeUnits.Count - 1; i >= 0; i--)
        {
            if (activeUnits[i] != null)
            {
                activeUnits[i].UpdateUnit();
            }
            else
            {
                activeUnits.RemoveAt(i);
            }
        }

        // Check victory/defeat conditions
        CheckGameConditions();
    }

    // Resource Management
    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            return true;
        }
        return false;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
    }

    public void AddExperience(int amount)
    {
        Experience += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int experienceNeeded = PlayerLevel * 100; // Simple level up formula
        if (Experience >= experienceNeeded)
        {
            PlayerLevel++;
            Experience -= experienceNeeded;
            OnLevelUp();
        }
    }

    private void OnLevelUp()
    {
        BaseHealth += 100;
        BaseDefense += 0.1f;
        Debug.Log($"Level Up! New Level: {PlayerLevel}");
    }

    // Unit Management
    public void RegisterUnit(Unit unit)
    {
        if (!activeUnits.Contains(unit))
        {
            activeUnits.Add(unit);
        }
    }

    public void UnregisterUnit(Unit unit)
    {
        activeUnits.Remove(unit);
    }

    // Building Management
    public void RegisterBuilding(Building building)
    {
        if (!playerBuildings.Contains(building))
        {
            playerBuildings.Add(building);
        }
    }

    public void UnregisterBuilding(Building building)
    {
        playerBuildings.Remove(building);
    }

    // Game State Management
    public void SetGameState(GameState newState)
    {
        CurrentGameState = newState;
        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
            case GameState.Victory:
                OnGameEnd(newState);
                break;
        }
    }

    private void CheckGameConditions()
    {
        if (BaseHealth <= 0)
        {
            SetGameState(GameState.GameOver);
        }
        // Add your victory conditions here
    }

    private void OnGameEnd(GameState endState)
    {
        Time.timeScale = 0f;
        // Implement game end logic (show UI, save stats, etc.)
        Debug.Log($"Game Ended with state: {endState}");
    }

    // Save/Load System
    public void SaveGame()
    {
        PlayerPrefs.SetInt("Gold", Gold);
        PlayerPrefs.SetInt("Experience", Experience);
        PlayerPrefs.SetInt("PlayerLevel", PlayerLevel);
        PlayerPrefs.SetInt("BaseHealth", BaseHealth);
        PlayerPrefs.SetFloat("BaseDefense", BaseDefense);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        Gold = PlayerPrefs.GetInt("Gold", startingGold);
        Experience = PlayerPrefs.GetInt("Experience", 0);
        PlayerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        BaseHealth = PlayerPrefs.GetInt("BaseHealth", startingBaseHealth);
        BaseDefense = PlayerPrefs.GetFloat("BaseDefense", baseDefenseMultiplier);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SaveGame();
        }
    }
}
