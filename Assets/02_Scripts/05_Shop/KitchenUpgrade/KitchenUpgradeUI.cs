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

    [SerializeField] private TMP_Text warningText;
    private void OnEnable()
    {
        KitchenUpgradeManager.Instance.OnKitchenUpgradeChanged += SetData;
        warningText.gameObject.SetActive(false);
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
        if (!CurrencyManager.Instance.SpendGold(KitchenUpgradeManager.Instance.NextData.Price))
        {
            warningText.text = (KitchenUpgradeManager.Instance.NextData.Price - CurrencyManager.Instance.Gold).ToString() + "원 부족합니다.";
            warningText.gameObject.SetActive(true);
            SoundManager.Instance?.PlaySFX(SFXType.Lose);
        }
        else
        {
            warningText.gameObject.SetActive(false);
            SoundManager.Instance?.PlaySFX(SFXType.Coin);
            KitchenUpgradeManager.Instance.LevelUp();
        }
    }
}
