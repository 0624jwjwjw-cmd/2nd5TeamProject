using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUpgradeUI : MonoBehaviour
{
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeButtonText;
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text currentRateText;
    [SerializeField] private TMP_Text nextRateText;
    [SerializeField] private TMP_Text priceText;

    [SerializeField] private string unMaxLevel = "업그레이드";
    [SerializeField] private string maxLevel = "최대 레벨";
    private void OnEnable()
    {
        InventoryUpgradeManager.Instance.OnInventoryUpgradeChanged += SetData;
    }
    private void OnDisable()
    {
        InventoryUpgradeManager.Instance.OnInventoryUpgradeChanged -= SetData;
    }
    private void Start()
    {
        SetData();
    }
    private void SetData()
    {
        currentLevelText.text = "Lv. " + InventoryUpgradeManager.Instance.CurrentData.Level.ToString();
        currentRateText.text = InventoryUpgradeManager.Instance.CurrentData.Stack.ToString() + "개";

        if (InventoryUpgradeManager.Instance.NextData != null)
        {
            nextLevelText.text = "Lv. " + InventoryUpgradeManager.Instance.NextData.Level.ToString();
            nextRateText.text = "x " + InventoryUpgradeManager.Instance.NextData.Stack.ToString() + "개";
            priceText.text = InventoryUpgradeManager.Instance.NextData.Price.ToString() + "원";
            upgradeButtonText.text = unMaxLevel;

        }
        else
        {
            nextLevelText.text = "";
            nextRateText.text = "";
            priceText.text = "";
            upgradeButton.interactable = false;
            upgradeButtonText.text = maxLevel;
        }
    }
    public void OnclickUpgradeButton()
    {
        InventoryUpgradeManager.Instance.LevelUp();
    }
}
