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
    [SerializeField] private TMP_Text priceText;

    [SerializeField] private string unMaxLevel = "업그레이드";
    [SerializeField] private string maxLevel = "최대 레벨";
    private void OnEnable()
    {
        KitchenUpgradeManager.Instance.OnKitchenUpgradeChanged += SetData;
    }
    private void OnDisable()
    {
        KitchenUpgradeManager.Instance.OnKitchenUpgradeChanged -= SetData;
    }
    private void Start()
    {
        SetData();
    }
    private void SetData()
    {
        currentLevelText.text = "Lv. " + KitchenUpgradeManager.Instance.CurrentData.Level.ToString();
        currentRateText.text = KitchenUpgradeManager.Instance.CurrentData.SpecialFoodRate.ToString() + "%";

        if (KitchenUpgradeManager.Instance.NextData != null)
        {
            nextLevelText.text = "Lv. " + KitchenUpgradeManager.Instance.NextData.Level.ToString();
            nextRateText.text = "x " + KitchenUpgradeManager.Instance.NextData.SpecialFoodRate.ToString() + "%";
            priceText.text = KitchenUpgradeManager.Instance.NextData.Price.ToString() + "원";
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
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        KitchenUpgradeManager.Instance.LevelUp();
    }
}
