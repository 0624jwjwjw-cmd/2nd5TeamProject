using TMPro;
using UnityEngine;

public class HeartUI : MonoBehaviour
{
    [SerializeField] private Transform heartParent;
    [SerializeField] private GameObject heartPrefab;

    [SerializeField] private TMP_Text extraHeartText;

    //구독알림설정
    private void Start()
    {
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("CurrencyManager가 아직 생성되지 않음");
            return;
        }
        CurrencyManager.Instance.OnHeartChanged += RefreshUI;
        RefreshUI();
    }
    private void OnDisable()
    {
        if(CurrencyManager.Instance !=null)
        {
            CurrencyManager.Instance.OnHeartChanged -= RefreshUI;
        }
    }

    private void RefreshUI()
    {
        ClearHeart();
        int heart = CurrencyManager.Instance.Heart;
        int displayCount = Mathf.Min(heart, 5);
        for (int i = 0; i < displayCount; i++)
        {
            Instantiate(heartPrefab,heartParent);
        }

        int extra = heart - 5;

        if (extra > 0)
        {
            extraHeartText.gameObject.SetActive(true);
            extraHeartText.text = $"+{extra}";
        }
        else
        {
            extraHeartText.gameObject.SetActive(false);
        }
    }
    private void ClearHeart()
    {
        foreach (Transform child in heartParent)
        {
            Destroy(child.gameObject);
        }
    }
}
