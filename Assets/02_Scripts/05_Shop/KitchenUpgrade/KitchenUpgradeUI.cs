using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenUpgradeUI : MonoBehaviour
{
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeButtonText;
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text currentRateText;
    [SerializeField] private TMP_Text nextRateText;

    private void OnEnable()
    {
        KitchenUpgradeManager.Instance.OnKitchenUpgradeChanged += SetData;
    }
    private void OnDisable()
    {
        KitchenUpgradeManager.Instance.OnKitchenUpgradeChanged -= SetData;
    }
    private void SetData()
    {
        currentLevelText.text = "Lv. " + KitchenUpgradeManager.Instance.CurrentData.Level.ToString();
        currentRateText.text = KitchenUpgradeManager.Instance.CurrentData.SpecialFoodRate.ToString() + "%";

        if (KitchenUpgradeManager.Instance.NextData != null)
        {
            nextLevelText.text = "Lv. " + KitchenUpgradeManager.Instance.NextData.Level.ToString();
            nextRateText.text = "x " + KitchenUpgradeManager.Instance.NextData.SpecialFoodRate.ToString() + "%";
        }
        else
        {
            nextLevelText.text = "";
            nextRateText.text = "";
            upgradeButton.interactable = false;
        }
    }
}
