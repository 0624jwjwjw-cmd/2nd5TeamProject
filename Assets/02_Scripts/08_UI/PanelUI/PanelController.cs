using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject targetPanel;

    public void OpenPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        targetPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        targetPanel.SetActive(false);
    }
}
