using UnityEngine;

public class PanelReset : MonoBehaviour
{
    [SerializeField] private GameObject resetPanel;
    [SerializeField] private GameObject darkPanel;
    private void Start()
    {
        if (SaveLoadManager.Instance == null)
            return;

        if (SaveLoadManager.Instance.IsGameReset)
        {
            OpenPanel();
            SaveLoadManager.Instance.ClearGameResetFlag();
        }
    }


    private void OpenPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        resetPanel.SetActive(true);
        darkPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        resetPanel.SetActive(false);
        darkPanel.SetActive(false);
    }
}
