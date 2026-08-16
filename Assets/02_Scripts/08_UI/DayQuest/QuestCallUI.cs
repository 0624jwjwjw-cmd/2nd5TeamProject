using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class QuestCallUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject darkPanel;

    [Header("Result")]
    [SerializeField] private Image resultImage;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;
    [SerializeField] private TMP_Text resultText;

    private void Start()
    {
        DayQuest.Instance.OnQuestResult += ShowResult;
    }

    private void OnDestroy()
    {
        if (DayQuest.Instance != null)
        {
            DayQuest.Instance.OnQuestResult -= ShowResult;
        }
    }
    private void ShowResult(bool isSuccess)
    {
        if (isSuccess)
        {
            ShowSuccess();
        }
        else
        {
            ShowFail();
        }
    }
    public void ShowSuccess()
    {
        resultImage.sprite = successSprite;
        resultText.text = "Äù½ºÆ® ¼º°ø!";
        SoundManager.Instance.PlaySFX(SFXType.Win);
        OpenPanel();
    }

    public void ShowFail()
    {
        resultImage.sprite = failSprite;
        resultText.text = "Äù½ºÆ® ½ÇÆÐ!";
        SoundManager.Instance.PlaySFX(SFXType.Lose);
        OpenPanel();
    }

    private void OpenPanel()
    {
        resultPanel.SetActive(true);
        
        if (darkPanel != null)
        {
            darkPanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        resultPanel.SetActive(false);

        if (darkPanel != null)
        {
            darkPanel.SetActive(false);
        }
    }
}
