using System.Collections;
using UnityEngine;

public class LiveChat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LiveManager _liveManager;
    [SerializeField] private LiveChatUI _liveChatUI;
    [SerializeField] private LiveChatAI _liveChatAI;

    [Header("Basic Chat")]
    [SerializeField] private float _chatInterval = 2f;

    [Header("AI Chat Nicknames")]
    [SerializeField]
    private string[] _aiChatNicknames =
    {
        "맛집탐방러",
        "먹방매니아",
        "야식헌터",
        "배고픈사람",
        "한입만요",
        "오늘도먹자",
        "치팅데이",
        "대식가팬",
        "먹는게최고",
        "위대한먹방",
        "밥친구",
        "배부른시청자",
        "음식평론가",
        "먹방구경꾼",
        "오늘뭐먹지",
        "야무진한입",
        "식욕폭발",
        "맛있는거최고",
        "먹방보는중",
        "배고파죽음"
    };

    private Coroutine _chatCoroutine;

    private readonly string[] _chatMessages =
    {
        "오즈중학생: 안녕하세요. 스트리머님. 저는 도덕 수업에서 선플달기 활동중인 오즈중학교 1학년 학생입니다.",
        "대한외국인: Anyone 2026?, [대식가키우기TV] Bring me here~~~~~~~~",
        "스폰지밥: 모두 밥상준비!!",
        "할머니사랑꾼: 할머니가 제일 좋아하는 손주 1위",
        "버거조아: 햄최몇 기원 132일차",
        "조문객: 구독 2번 눌렀어요 ㅎㅎ",
        "에너그램: 겠있맛당....",
        "영리한젊은이: 배부름 청년.",
        "현대자동차N: 라이브 달려왔어요",
        "LG울트라기어: 모니터 핥을뻔",
        "삼성비스포크: 식욕 폭발해서 냉장고 털고 오는 중",
        "식사합시다: 오늘 메뉴 뭐에요?",
        "인정TV: 대식가 인정",
        "지나가던나그네: 우연히 왔는데 재밌네",
        "웃음전도사: ㅋㅋㅋㅋㅋㅋㅋㅋㅋ",
        "대리만족: 제가 먹는 것도 아닌데 왜 배가 부르죠?",
        "성장형시청자: 점점 엄청나게 드시는데ㅋㅋㅋㅋㅋ",
        "yoyoyo3456: ㅠㅠㅠ개맛있어보인다 진짜...",
        "와쿠와쿠7: 와구와구와구와구와구와구",
        "정준하: 야무지게 먹어야징~~~",
        "반더린드갱단: 오늘도 그릇 싹 비워씀ㅋㅋㅋ",
        "매니저봇: 타인 비하, 욕설, 과도한 스포일러는 매니저에 의해 차단될 수 있습니다."
    };

    private void OnEnable()
    {
        if (_liveManager == null)
            return;

        _liveManager.OnLiveStarted += StartChat;
        _liveManager.OnLiveStopped += StopChat;
        _liveManager.OnLiveEnded += StopChat;

        _liveManager.OnFoodEaten += GenerateAIChat;
    }

    private void OnDisable()
    {
        if (_liveManager == null)
            return;

        _liveManager.OnLiveStarted -= StartChat;
        _liveManager.OnLiveStopped -= StopChat;
        _liveManager.OnLiveEnded -= StopChat;

        _liveManager.OnFoodEaten -= GenerateAIChat;
    }

    private void StartChat()
    {
        StopChat();

        if (_liveChatUI != null)
            _liveChatUI.ClearChats();

        _chatCoroutine = StartCoroutine(ChatRoutine());
    }

    private void StopChat()
    {
        if (_chatCoroutine != null)
        {
            StopCoroutine(_chatCoroutine);
            _chatCoroutine = null;
        }

        if (_liveChatUI != null)
            _liveChatUI.ClearChats();
    }

    private IEnumerator ChatRoutine()
    {
        while (true)
        {
            AddBasicChat();

            yield return new WaitForSeconds(_chatInterval);
        }
    }

    private void AddBasicChat()
    {
        if (_liveChatUI == null)
            return;

        if (_chatMessages.Length == 0)
            return;

        int randomIndex =
            Random.Range(0, _chatMessages.Length);

        _liveChatUI.AddChat(
            _chatMessages[randomIndex]
        );
    }

    private void GenerateAIChat(string foodName)
    {
        if (_liveManager == null || !_liveManager.IsLive)
            return;

        if (_liveChatAI == null)
        {
            Debug.LogWarning(
                "[LiveChat] LiveChatAI가 연결되지 않았습니다."
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(foodName))
        {
            Debug.LogWarning(
                "[LiveChat] 음식 이름이 비어 있습니다."
            );
            return;
        }

        Debug.Log(
            $"[LiveChat] 음식 섭취 감지 → AI 채팅 요청: {foodName}"
        );

        _liveChatAI.GenerateChat(
            foodName,
            OnAIChatGenerated
        );
    }

    private void OnAIChatGenerated(string chat)
    {
        if (_liveManager == null || !_liveManager.IsLive)
            return;

        if (string.IsNullOrWhiteSpace(chat))
            return;

        if (_liveChatUI == null)
            return;

        if (_aiChatNicknames == null ||
            _aiChatNicknames.Length == 0)
        {
            Debug.LogWarning(
                "[LiveChat] AI 채팅 닉네임이 없습니다."
            );
            return;
        }

        int nicknameIndex =
            Random.Range(0, _aiChatNicknames.Length);

        string finalChat =
            _aiChatNicknames[nicknameIndex] +
            ": " +
            chat;

        _liveChatUI.AddChat(finalChat);

        Debug.Log(
            $"[LiveChat] AI 채팅 출력: {finalChat}"
        );
    }
}