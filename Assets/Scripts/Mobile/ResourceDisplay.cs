using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image trendIndicator;
    [SerializeField] private CanvasGroup canvasGroup;

    private Resource resource;
    private float previousPrice;
    private float displayDuration = 3f;
    private float currentDisplayTime;

    public void Initialize(Resource resource)
    {
        this.resource = resource;
        UpdateQuantity(resource.quantity);
        UpdatePrice(resource.tradeValue);
        LoadResourceIcon(resource.name);
    }

    private void LoadResourceIcon(string resourceName)
    {
        // Load the resource icon from Resources folder
        Sprite icon = Resources.Load<Sprite>($"Icons/Resources/{resourceName}");
        if (icon != null)
        {
            resourceIcon.sprite = icon;
        }
    }

    public void UpdateQuantity(float quantity)
    {
        quantityText.text = quantity.ToString("N1");
        currentDisplayTime = displayDuration;
        ShowDisplay();
    }

    public void UpdatePrice(float price)
    {
        priceText.text = price.ToString("N2");
        
        // Update trend indicator
        if (price > previousPrice)
        {
            trendIndicator.color = Color.green;
            trendIndicator.transform.rotation = Quaternion.Euler(0, 0, 45);
        }
        else if (price < previousPrice)
        {
            trendIndicator.color = Color.red;
            trendIndicator.transform.rotation = Quaternion.Euler(0, 0, -45);
        }
        
        previousPrice = price;
        currentDisplayTime = displayDuration;
        ShowDisplay();
    }

    private void Update()
    {
        if (currentDisplayTime > 0)
        {
            currentDisplayTime -= Time.deltaTime;
            if (currentDisplayTime <= 0)
            {
                HideDisplay();
            }
        }
    }

    private void ShowDisplay()
    {
        canvasGroup.alpha = 1f;
    }

    private void HideDisplay()
    {
        canvasGroup.LeanAlpha(0f, 0.5f);
    }

    public void OnPointerEnter()
    {
        ShowResourceDetails();
    }

    public void OnPointerExit()
    {
        MobileUIManager.Instance.HideTooltip();
    }

    private void ShowResourceDetails()
    {
        string tooltipText = $"{resource.name}\n" +
                           $"Type: {resource.type}\n" +
                           $"Quantity: {resource.quantity:N1}\n" +
                           $"Price: {resource.tradeValue:N2}\n" +
                           $"Production: {resource.productionRate:N1}/s\n" +
                           $"Consumption: {resource.consumptionRate:N1}/s";

        MobileUIManager.Instance.ShowTooltip(tooltipText, transform.position);
    }
}
