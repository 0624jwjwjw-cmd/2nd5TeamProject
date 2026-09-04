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
        resultText.text = "퀘스트 성공! \n구독자20%증가!!";
        SoundManager.Instance.PlaySFX(SFXType.Win);
        OpenPanel();
    }

    public void ShowFail()
    {
        resultImage.sprite = failSprite;
        resultText.text = "퀘스트 실패!\n구독자20%감소...";
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
