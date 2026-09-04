using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIController : MonoBehaviour
{
    [SerializeField] private Button openButton;
    [SerializeField] private GameObject upgradePanel;

    public void OnClickOpenButton()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        openButton.gameObject.SetActive(false);
        upgradePanel.gameObject.SetActive(true);
    }
    public void OnClickExitButton()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        openButton.gameObject.SetActive(true);
        upgradePanel.gameObject.SetActive(false);
    }
}
