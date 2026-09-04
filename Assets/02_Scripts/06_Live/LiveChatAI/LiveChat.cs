using System.Collections;
using System.Collections.Generic;
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
        "맛집탐험대",
        "qwen12234",
        "식신로드",
        "배고픔청년",
        "한입만",
        "winnerChicken7799",
        "얼음꿀차",
        "전우애시간",
        "와라비모찌",
        "whitefox0815",
        "밥친구",
        "스윗중붕",
        "음식평론가",
        "응애에요345",
        "오늘뭐먹지",
        "dwwodujoj5007",
        "privatecorotine",
        "약과에소주",
        "레몬청과",
        "건더기99"
    };

    private Coroutine _chatCoroutine;
    private Coroutine _aiChatCoroutine;

    private readonly Queue<string> _aiFoodQueue =
        new Queue<string>();

    private bool _isProcessingAIChat;

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
        {
            return;
        }

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

        _aiFoodQueue.Clear();
        _isProcessingAIChat = false;

        _chatCoroutine = StartCoroutine(ChatRoutine());
    }


    private void StopChat()
    {
        if (_chatCoroutine != null)
        {
            StopCoroutine(_chatCoroutine);
            _chatCoroutine = null;
        }

        if (_aiChatCoroutine != null)
        {
            StopCoroutine(_aiChatCoroutine);
            _aiChatCoroutine = null;
        }

        _aiFoodQueue.Clear();
        _isProcessingAIChat = false;

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

        if (_chatMessages == null ||
            _chatMessages.Length == 0)
            return;

        int randomIndex =
            Random.Range(
                0,
                _chatMessages.Length
            );

        _liveChatUI.AddChat(
            _chatMessages[randomIndex]
        );
    }


    private void GenerateAIChat(string foodName)
    {
        if (_liveManager == null ||
            !_liveManager.IsLive)
            return;

        if (string.IsNullOrWhiteSpace(foodName))
        {
            AddBasicChat();
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR

        AddBasicChat();

        return;

#endif

        if (_liveChatAI == null)
        {
            AddBasicChat();

            return;
        }

        _aiFoodQueue.Enqueue(foodName);

        if (!_isProcessingAIChat)
        {
            _aiChatCoroutine =
                StartCoroutine(
                    ProcessAIChatQueue()
                );
        }
    }


    private IEnumerator ProcessAIChatQueue()
    {
        _isProcessingAIChat = true;

        while (_aiFoodQueue.Count > 0)
        {
            if (_liveManager == null ||
                !_liveManager.IsLive)
            {
                _aiFoodQueue.Clear();
                break;
            }

            string foodName =
                _aiFoodQueue.Dequeue();

            yield return StartCoroutine(
                GenerateMultipleAIChats(
                    foodName
                )
            );

            if (_aiFoodQueue.Count > 0)
            {
                yield return new WaitForSeconds(
                    Random.Range(
                        0.3f,
                        0.7f
                    )
                );
            }
        }

        _isProcessingAIChat = false;
        _aiChatCoroutine = null;
    }


    private IEnumerator GenerateMultipleAIChats(
        string foodName)
    {
        int chatCount = 3;

        List<string> usedNicknames =
            new List<string>();

        for (int i = 0; i < chatCount; i++)
        {
            if (_liveManager == null ||
                !_liveManager.IsLive)
                yield break;

            bool completed = false;
            string generatedChat = null;

            _liveChatAI.GenerateChat(
                foodName,
                chat =>
                {
                    generatedChat = chat;
                    completed = true;
                }
            );

            while (!completed)
            {
                if (_liveManager == null ||
                    !_liveManager.IsLive)
                    yield break;

                yield return null;
            }

            if (!string.IsNullOrWhiteSpace(generatedChat))
            {
                AddAIChat(
                    generatedChat,
                    usedNicknames
                );
            }
            else
            {
                AddBasicChat();
            }

            if (i < chatCount - 1)
            {
                yield return new WaitForSeconds(
                    Random.Range(
                        0.6f,
                        1.2f
                    )
                );
            }
        }
    }


    private void AddAIChat(
        string chat,
        List<string> usedNicknames)
    {
        if (_liveManager == null ||
            !_liveManager.IsLive)
            return;

        if (_liveChatUI == null)
            return;

        if (_aiChatNicknames == null ||
            _aiChatNicknames.Length == 0)
        {
            AddBasicChat();

            return;
        }

        List<string> availableNicknames =
            new List<string>();

        for (int i = 0;
             i < _aiChatNicknames.Length;
             i++)
        {
            string nickname =
                _aiChatNicknames[i];

            if (!usedNicknames.Contains(nickname))
            {
                availableNicknames.Add(nickname);
            }
        }

        if (availableNicknames.Count == 0)
        {
            availableNicknames.AddRange(
                _aiChatNicknames
            );
        }

        int nicknameIndex =
            Random.Range(
                0,
                availableNicknames.Count
            );

        string selectedNickname =
            availableNicknames[nicknameIndex];

        usedNicknames.Add(
            selectedNickname
        );

        string finalChat =
            selectedNickname +
            ": " +
            chat;

        _liveChatUI.AddChat(
            finalChat
        );
    }
}
