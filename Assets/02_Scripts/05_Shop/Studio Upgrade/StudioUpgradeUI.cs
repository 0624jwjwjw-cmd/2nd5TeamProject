using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StudioUpgradeUI : MonoBehaviour
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
        StudioUpgradeManager.Instance.OnStudioUpgradeChanged += SetData;
    }
    private void OnDisable()
    {
        StudioUpgradeManager.Instance.OnStudioUpgradeChanged -= SetData;
    }
    private void Start()
    {
        SetData();
    }
    private void SetData()
    {
        currentLevelText.text = "Lv. " + StudioUpgradeManager.Instance.CurrentData.Level.ToString();
        currentRateText.text = "x " + StudioUpgradeManager.Instance.CurrentData.SubscriberBonus.ToString();

        if(StudioUpgradeManager.Instance.NextData != null)
        {
            nextLevelText.text = "Lv. " + StudioUpgradeManager.Instance.NextData.Level.ToString();
            nextRateText.text = "x " + StudioUpgradeManager.Instance.NextData.SubscriberBonus.ToString();
            priceText.text = StudioUpgradeManager.Instance.NextData.Price.ToString() + "원";
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
    public void OnClickUpgradeButton()
    {
        StudioUpgradeManager.Instance.LevelUp();
    }
}
