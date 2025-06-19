using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Resource Display")]
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Building Menu")]
    [SerializeField] private GameObject buildingMenu;
    [SerializeField] private Button townCenterButton;
    [SerializeField] private Button barracksButton;
    [SerializeField] private Button resourceBuildingButton;

    [Header("Unit Menu")]
    [SerializeField] private GameObject unitMenu;
    [SerializeField] private Button workerButton;
    [SerializeField] private Button soldierButton;

    [Header("Game State")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private TextMeshProUGUI objectiveText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeUI();
        SubscribeToEvents();
    }

    private void InitializeUI()
    {
        // Initialize resource display
        UpdateResourceDisplay();

        // Initialize building buttons
        townCenterButton.onClick.AddListener(() => BuildingManager.Instance.StartPlacingBuilding("townCenter"));
        barracksButton.onClick.AddListener(() => BuildingManager.Instance.StartPlacingBuilding("barracks"));
        resourceBuildingButton.onClick.AddListener(() => BuildingManager.Instance.StartPlacingBuilding("resourceBuilding"));

        // Initialize unit buttons
        workerButton.onClick.AddListener(() => UnitManager.Instance.CreateUnit("worker"));
        soldierButton.onClick.AddListener(() => UnitManager.Instance.CreateUnit("soldier"));

        // Hide victory/defeat panels
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
    }

    public void UpdateResourceDisplay()
    {
        var resources = ResourceManager.Instance.GetResources();
        foodText.text = $"Food: {resources["food"]}";
        woodText.text = $"Wood: {resources["wood"]}";
        stoneText.text = $"Stone: {resources["stone"]}";
        goldText.text = $"Gold: {resources["gold"]}";
    }

    public void ShowBuildingMenu(bool show)
    {
        buildingMenu.SetActive(show);
        unitMenu.SetActive(!show);
    }

    public void ShowUnitMenu(bool show)
    {
        unitMenu.SetActive(show);
        buildingMenu.SetActive(!show);
    }

    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ShowDefeat()
    {
        defeatPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void UpdateObjective(string objective)
    {
        objectiveText.text = objective;
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        ResourceManager.Instance.OnResourceChanged += UpdateResourceDisplay;
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Victory:
                ShowVictory();
                break;
            case GameManager.GameState.GameOver:
                ShowDefeat();
                break;
        }
    }
}
