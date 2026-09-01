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

    private void OnEnable()
    {
        StudioUpgradeManager.Instance.OnStudioUpgradeChanged += SetData;
    }
    private void OnDisable()
    {
        StudioUpgradeManager.Instance.OnStudioUpgradeChanged -= SetData;
    }
    private void SetData()
    {
        currentLevelText.text = "Lv. " + StudioUpgradeManager.Instance.CurrentData.Level.ToString();
        currentRateText.text = "x " + StudioUpgradeManager.Instance.CurrentData.SubscriberBonus.ToString();

        if(StudioUpgradeManager.Instance.NextData != null)
        {
            nextLevelText.text = "Lv. " + StudioUpgradeManager.Instance.NextData.Level.ToString();
            nextRateText.text = "x " + StudioUpgradeManager.Instance.NextData.SubscriberBonus.ToString();
        }
        else
        {
            nextLevelText.text = "";
            nextRateText.text = "";
            upgradeButton.interactable = false;
        }
    }
}
