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

    private void OnEnable()
    {
        InventoryUpgradeManager.Instance.OnInventoryUpgradeChanged += SetData;
    }
    private void OnDisable()
    {
        InventoryUpgradeManager.Instance.OnInventoryUpgradeChanged -= SetData;
    }
    private void SetData()
    {
        currentLevelText.text = "Lv. " + InventoryUpgradeManager.Instance.CurrentData.Level.ToString();
        currentRateText.text = InventoryUpgradeManager.Instance.CurrentData.Stack.ToString() + "°³";

        if (InventoryUpgradeManager.Instance.NextData != null)
        {
            nextLevelText.text = "Lv. " + InventoryUpgradeManager.Instance.NextData.Level.ToString();
            nextRateText.text = "x " + InventoryUpgradeManager.Instance.NextData.Stack.ToString() + "°³";
        }
        else
        {
            nextLevelText.text = "";
            nextRateText.text = "";
            upgradeButton.interactable = false;
        }
    }
}
