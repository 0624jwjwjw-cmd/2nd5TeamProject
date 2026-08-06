using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LiveChat : MonoBehaviour
{
    [SerializeField] private LiveManager _liveManager;
    [SerializeField] private TMP_Text _chatText;

    [SerializeField] private float _chatInterval = 2f;
    [SerializeField] private int _maxChatCount = 5;

    private readonly List<string> _currentChats = new();

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

    private Coroutine _chatCoroutine;

    private void OnEnable()
    {
        if (_liveManager == null)
            return;

        _liveManager.OnLiveStarted += StartChat;
        _liveManager.OnLiveEnded += StopChat;
    }

    private void OnDisable()
    {
        if (_liveManager == null)
            return;

        _liveManager.OnLiveStarted -= StartChat;
        _liveManager.OnLiveEnded -= StopChat;
    }

    private void StartChat()
    {
        if (_chatCoroutine != null)
        {
            StopCoroutine(_chatCoroutine);
        }

        _chatCoroutine = StartCoroutine(ChatRoutine());
    }

    private void StopChat()
    {
        if (_chatCoroutine != null)
        {
            StopCoroutine(_chatCoroutine);
            _chatCoroutine = null;
        }

        _currentChats.Clear();
        _chatText.text = "";
    }

    private IEnumerator ChatRoutine()
    {
        while (true)
        {
            AddChat();

            yield return new WaitForSeconds(_chatInterval);
        }
    }

    private void AddChat()
    {
        int randomIndex = Random.Range(0, _chatMessages.Length);

        _currentChats.Add(_chatMessages[randomIndex]);

        if (_currentChats.Count > _maxChatCount)
        {
            _currentChats.RemoveAt(0);
        }

        UpdateChatUI();
    }

    private void UpdateChatUI()
    {
        _chatText.text = string.Join("\n\n", _currentChats);
    }
}