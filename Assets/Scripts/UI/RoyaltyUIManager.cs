using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoyaltyUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI revenueText;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI mauText;
    [SerializeField] private TextMeshProUGUI retentionText;

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        var (revenue, percentage, amount) = AdManager.Instance.GetCurrentRoyaltyInfo();
        
        revenueText.text = $"Monthly Revenue: ${revenue:F2}";
        percentageText.text = $"Current Royalty: {percentage * 100:F1}%";
        amountText.text = $"Royalty Amount: ${amount:F2}";
        
        int mau = AnalyticsManager.Instance.GetMonthlyActiveUsers();
        float retention = AnalyticsManager.Instance.GetRetentionRate();
        
        mauText.text = $"Monthly Active Users: {mau:N0}";
        retentionText.text = $"30-Day Retention: {retention * 100:F1}%";
    }
}
