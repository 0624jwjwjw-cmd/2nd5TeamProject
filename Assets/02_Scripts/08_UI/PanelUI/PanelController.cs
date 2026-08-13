using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject targetPanel;

    public void OpenPanel()
    {
        targetPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        targetPanel.SetActive(false);
    }
}
