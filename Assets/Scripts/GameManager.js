// static instance for singleton pattern
var _instance = null;
function Instance() {
    return _instance;
}

// Resources and Economy
// Resource and Economy variables
var gold = 0;
var experience = 0;

// Encapsulated getters and setters for resources
function setGold(amount) {
    gold = Math.max(0, amount);
}

function getGold() {
    return gold;
}

function addGold(amount) {
    setGold(gold + amount);
}

function spendGold(amount) {
    if (gold >= amount) {
        setGold(gold - amount);
        return true;
    }
    return false;
}

function setExperience(amount) {
    experience = Math.max(0, amount);
}

function getExperience() {
    return experience;
}

function addExperience(amount) {
    setExperience(experience + amount);
    CheckLevelUp();
}

// Game State
var GameState = {
    MainMenu: 0,
    Playing: 1,
    Paused: 2,
    GameOver: 3,
    Victory: 4
};
var currentGameState;

// Player Stats
var playerLevel = 1;
var baseHealth = 1000;
var baseDefense = 1.0;

// Lists for managing game entities
var activeUnits = new Array();
var playerBuildings = new Array();

// Game Settings
var startingGold = 100;
var startingBaseHealth = 1000;
var baseDefenseMultiplier = 1.0;
var baseDefenseMultiplier = 1.0;

function InitializeGame() {
    gold = startingGold;
    experience = 0;
    playerLevel = 1;
    baseHealth = startingBaseHealth;
    baseDefense = baseDefenseMultiplier;
    currentGameState = GameState.MainMenu;
}

function Start() {
    _instance = this;
    Debug.Log("Age of War - GameManager initialized successfully!");
}

function Update() {
    if (currentGameState == GameState.Playing) {
        UpdateGameState();
    }
}

function UpdateGameState() {
    // Update all active units
    for (var i = activeUnits.length - 1; i >= 0; i--) {
        if (activeUnits[i] != null) {
            activeUnits[i].UpdateUnit();
        } else {
            activeUnits.splice(i, 1);
        }
    }
    
    // Check victory/defeat conditions
    CheckGameConditions();
}

// Resource Management
function SpendGold(amount) {
    if (gold >= amount) {
        gold -= amount;
        return true;
    }
    return false;
}

function AddGold(amount) {
    gold += amount;
}

function GetGold() {
    return gold;
}

function AddExperience(amount) {
    experience += amount;
    CheckLevelUp();
}

function CheckLevelUp() {
    var experienceNeeded = playerLevel * 100; // Simple level up formula
    if (experience >= experienceNeeded) {
        playerLevel++;
        experience -= experienceNeeded;
        OnLevelUp();
    }
}

function OnLevelUp() {
    baseHealth += 100;
    baseDefense += 0.1;
    Debug.Log("Level Up! New Level: " + playerLevel);
}

// Unit Management
function RegisterUnit(unit) {
    if (activeUnits.indexOf(unit) == -1) {
        activeUnits.push(unit);
    }
}

function UnregisterUnit(unit) {
    var index = activeUnits.indexOf(unit);
    if (index != -1) {
        activeUnits.splice(index, 1);
    }
}

// Building Management
function RegisterBuilding(building) {
    if (playerBuildings.indexOf(building) == -1) {
        playerBuildings.push(building);
    }
}

function UnregisterBuilding(building) {
    var index = playerBuildings.indexOf(building);
    if (index != -1) {
        playerBuildings.splice(index, 1);
    }
}

// Game State Management
function SetGameState(newState) {
    currentGameState = newState;
    switch (newState) {
        case GameState.Playing:
            Time.timeScale = 1.0;
            break;
        case GameState.Paused:
            Time.timeScale = 0.0;
            break;
        case GameState.GameOver:
        case GameState.Victory:
            OnGameEnd(newState);
            break;
    }
}

function CheckGameConditions() {
    if (baseHealth <= 0) {
        SetGameState(GameState.GameOver);
    }
    // Add your victory conditions here
}

function OnGameEnd(endState) {
    Time.timeScale = 0.0;
    // Implement game end logic (show UI, save stats, etc.)
    Debug.Log("Game Ended with state: " + endState);
}

// Save/Load System
function SaveGame() {
    PlayerPrefs.SetInt("Gold", gold);
    PlayerPrefs.SetInt("Experience", experience);
    PlayerPrefs.SetInt("PlayerLevel", playerLevel);
    PlayerPrefs.SetInt("BaseHealth", baseHealth);
    PlayerPrefs.SetFloat("BaseDefense", baseDefense);
    PlayerPrefs.Save();
}

function LoadGame() {
    gold = PlayerPrefs.GetInt("Gold", startingGold);
    experience = PlayerPrefs.GetInt("Experience", 0);
    playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
    baseHealth = PlayerPrefs.GetInt("BaseHealth", startingBaseHealth);
    baseDefense = PlayerPrefs.GetFloat("BaseDefense", baseDefenseMultiplier);
}

function OnDestroy() {
    if (_instance == this) {
        SaveGame();
    }
}
