using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MobileUIManager : MonoBehaviour
{
    private static MobileUIManager _instance;
    public static MobileUIManager Instance => _instance;

    [Header("Main UI Panels")]
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private RectTransform resourcePanel;
    [SerializeField] private RectTransform buildPanel;
    [SerializeField] private RectTransform unitPanel;
    [SerializeField] private RectTransform techPanel;
    [SerializeField] private RectTransform diplomaticPanel;

    [Header("Resource Display")]
    [SerializeField] private Transform resourceDisplayContainer;
    [SerializeField] private GameObject resourceDisplayPrefab;

    [Header("Bottom Bar")]
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button unitsButton;
    [SerializeField] private Button techButton;
    [SerializeField] private Button diplomacyButton;

    [Header("Top Bar")]
    [SerializeField] private TextMeshProUGUI populationText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI eraText;

    private Dictionary<string, ResourceDisplay> resourceDisplays = new Dictionary<string, ResourceDisplay>();
    private List<RectTransform> allPanels = new List<RectTransform>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeUI()
    {
        // Initialize panels list
        allPanels.Add(buildPanel);
        allPanels.Add(unitPanel);
        allPanels.Add(techPanel);
        allPanels.Add(diplomaticPanel);

        // Hide all panels initially
        foreach (var panel in allPanels)
        {
            panel.gameObject.SetActive(false);
        }

        // Setup button listeners
        buildButton.onClick.AddListener(() => TogglePanel(buildPanel));
        unitsButton.onClick.AddListener(() => TogglePanel(unitPanel));
        techButton.onClick.AddListener(() => TogglePanel(techPanel));
        diplomacyButton.onClick.AddListener(() => TogglePanel(diplomaticPanel));

        // Initialize resource displays
        InitializeResourceDisplays();

        // Subscribe to events
        SubscribeToEvents();
    }

    private void InitializeResourceDisplays()
    {
        var resources = EconomicSystem.Instance.GetAllResources();
        foreach (var resource in resources)
        {
            CreateResourceDisplay(resource);
        }
    }

    private void CreateResourceDisplay(Resource resource)
    {
        GameObject displayObj = Instantiate(resourceDisplayPrefab, resourceDisplayContainer);
        ResourceDisplay display = displayObj.GetComponent<ResourceDisplay>();
        display.Initialize(resource);
        resourceDisplays[resource.name] = display;
    }

    private void SubscribeToEvents()
    {
        if (EconomicSystem.Instance != null)
        {
            EconomicSystem.Instance.OnResourceQuantityChanged += UpdateResourceDisplay;
            EconomicSystem.Instance.OnResourcePriceChanged += UpdateResourcePrice;
        }

        if (CivilizationManager.Instance != null)
        {
            CivilizationManager.Instance.OnPopulationChanged += UpdatePopulation;
            CivilizationManager.Instance.OnCurrencyChanged += UpdateGold;
            CivilizationManager.Instance.OnEraChanged += UpdateEra;
        }
    }

    private void TogglePanel(RectTransform panelToToggle)
    {
        bool isActive = panelToToggle.gameObject.activeSelf;

        // Hide all panels
        foreach (var panel in allPanels)
        {
            panel.gameObject.SetActive(false);
        }

        // Show the selected panel if it wasn't active
        if (!isActive)
        {
            panelToToggle.gameObject.SetActive(true);
            AnimatePanel(panelToToggle);
        }
    }

    private void AnimatePanel(RectTransform panel)
    {
        // Add animation here (can use DOTween or other animation system)
        panel.localScale = Vector3.zero;
        panel.LeanScale(Vector3.one, 0.3f).setEaseOutBack();
    }

    private void UpdateResourceDisplay(string resourceName, float quantity)
    {
        if (resourceDisplays.TryGetValue(resourceName, out ResourceDisplay display))
        {
            display.UpdateQuantity(quantity);
        }
    }

    private void UpdateResourcePrice(string resourceName, float price)
    {
        if (resourceDisplays.TryGetValue(resourceName, out ResourceDisplay display))
        {
            display.UpdatePrice(price);
        }
    }

    private void UpdatePopulation(float population)
    {
        populationText.text = $"Population: {Mathf.FloorToInt(population):N0}";
    }

    private void UpdateGold(float gold)
    {
        goldText.text = $"Gold: {Mathf.FloorToInt(gold):N0}";
    }

    private void UpdateEra(string era)
    {
        eraText.text = era;
    }

    public void ShowTooltip(string text, Vector2 position)
    {
        // Implement tooltip display
    }

    public void HideTooltip()
    {
        // Implement tooltip hiding
    }

    public void ShowNotification(string message, NotificationType type)
    {
        // Implement notification system
    }
}

public enum NotificationType
{
    Info,
    Warning,
    Error,
    Achievement
}
