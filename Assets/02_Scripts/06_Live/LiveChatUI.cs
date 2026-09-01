using TMPro;
using UnityEngine;

public class LiveChatUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _chatBoxes;
    [SerializeField] private float _chatSpacing = 10f;

    private TMP_Text[] _chatTexts;
    private RectTransform[] _chatRects;

    private const float ChatWidth = 560f;
    private const float MinChatHeight = 80f;
    private const float TextPadding = 10f;

    private void Awake()
    {
        int count = _chatBoxes.Length;

        _chatTexts = new TMP_Text[count];
        _chatRects = new RectTransform[count];

        for (int i = 0; i < count; i++)
        {
            _chatRects[i] = _chatBoxes[i].GetComponent<RectTransform>();
            _chatTexts[i] = _chatBoxes[i].GetComponentInChildren<TMP_Text>();

            _chatRects[i].anchorMin = new Vector2(0.5f, 0f);
            _chatRects[i].anchorMax = new Vector2(0.5f, 0f);
            _chatRects[i].pivot = new Vector2(0.5f, 0f);

            _chatRects[i].sizeDelta = new Vector2(ChatWidth, MinChatHeight);

            ClearChat(i);
        }
    }

    public void AddChat(string message)
    {
        // 기존 채팅을 한 칸씩 위로 이동
        for (int i = _chatBoxes.Length - 1; i > 0; i--)
        {
            _chatTexts[i].text = _chatTexts[i - 1].text;

            if (string.IsNullOrEmpty(_chatTexts[i].text))
                ClearChat(i);
            else
                _chatBoxes[i].SetActive(true);
        }

        // 새 채팅을 가장 아래에 표시
        _chatTexts[0].text = message;
        _chatBoxes[0].SetActive(true);

        UpdateChatLayout();
    }

    public void ClearChats()
    {
        for (int i = 0; i < _chatBoxes.Length; i++)
            ClearChat(i);
    }

    private void ClearChat(int index)
    {
        _chatTexts[index].text = "";
        _chatBoxes[index].SetActive(false);

        _chatRects[index].sizeDelta = new Vector2(
            ChatWidth,
            MinChatHeight
        );
    }

    private void UpdateChatLayout()
    {
        float currentY = 0f;

        for (int i = 0; i < _chatBoxes.Length; i++)
        {
            if (!_chatBoxes[i].activeSelf)
                continue;

            TMP_Text text = _chatTexts[i];

            text.ForceMeshUpdate();

            // 실제 560 너비에서 필요한 텍스트 높이 계산
            float textHeight = text.GetPreferredValues(
                text.text,
                ChatWidth - 20f,
                0f
            ).y;

            float boxHeight = Mathf.Max(
                MinChatHeight,
                textHeight + TextPadding
            );

            _chatRects[i].sizeDelta = new Vector2(
                ChatWidth,
                boxHeight
            );

            // 아래에서부터 위로 쌓기
            _chatRects[i].anchoredPosition = new Vector2(
                0f,
                currentY
            );

            currentY += boxHeight + _chatSpacing;
        }
    }
}