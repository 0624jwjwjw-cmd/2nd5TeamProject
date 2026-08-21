//**ScrollView가 움직이는 동안에만 세로 Scrollbar를 표시하는 UI**
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;     //IBeginDragHandler, IEndDragHandler 사용
using UnityEngine.UI;               //ScrollRect 사용

//같은 GameObject에 같은 스크립트가 여러 개 붙는 것을 방지
[DisallowMultipleComponent]

public sealed class ScrollbarAutoHideUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IScrollHandler
{
    //*Scroll UI*
    [Header("Scroll UI")]

    [SerializeField] private ScrollRect scrollRect;             //실제 인벤토리 스크롤을 담당하는 ScrollRect

    //Scrollbar Vertical에 붙어있는 CanvasGroup
    //Alpha를 이용해서 표시 / 숨김 처리
    [SerializeField] private CanvasGroup scrollbarCanvasGroup;

    //*Auto Hide 설정*
    [Header("Auto Hide")]
    //스크롤 움직임이 멈춘 뒤
    //Scrollbar가 사라지기 전까지 기다리는 시간
    [Min(0f)][SerializeField] private float hideDelay = 0.05f;  //스크롤 움직임 멈추고 스크롤바가 사라지기 전까지 시간
    [Min(0f)][SerializeField] private float fadeSpeed = 8f;     //스크롤바가 서서히 사라지는 속도
    
    //*Runtime 상태*
    private bool isDragging;                //현재 사용자가 직접 화면을 드래그하고 있는지 여부
    private float hideTimer;                //스크롤바를 계속 보여줄 남은 시간

    private void OnEnable()
    {
        HideImmediately();      //UI가 처음 켜졌을 때는 Scrollbar를 숨김
    }

    private void Update()
    {
        //CanvasGroup이 없으면 표시 상태를 제어할 수 없으므로 종료
        if (scrollbarCanvasGroup == null) return;

        //스크롤할 만큼 Content가 크지 않다면
        if (!CanScrollVertically())
        {
            HideImmediately();  //스크롤바 계속 숨겨둠
            return;
        }

        //현재 화면을 직접 드래그 중이라면
        if (isDragging)
        {
            ShowImmediately();  //스크롤바 계속 보여줌
            return;
        }

        //최근 스크롤 이동 이후 아직 HideDelay가 남아 있다면 스크롤바 계속 표시
        if (hideTimer > 0f)
        {
            hideTimer -= Time.unscaledDeltaTime;

            ShowImmediately();
            return;
        }

        //숨김 시간이 끝났다면 Alpha를 0 방향으로 빠르게 감소
        scrollbarCanvasGroup.alpha =
            Mathf.MoveTowards(
                scrollbarCanvasGroup.alpha,
                0f,
                fadeSpeed * Time.unscaledDeltaTime
                );
    }

    //*Drag 이벤트*
    //마우스 클릭 드래그 또는 모바일 터치를 시작했을 때 호출
    public void OnBeginDrag(PointerEventData eventData)
    {
        //스크롤할 내용이 없다면 아무것도 하지 않음
        if (!CanScrollVertically()) return;

        //현재 드래그 중이라고 기록
        isDragging = true;

        //이전 숨김 타이머 제거
        hideTimer = 0f;

        //스크롤바 즉시 표시
        ShowImmediately();
    }

    //*사용자가 손가락 또는 마우스를 뗐을 때 호출*
    public void OnEndDrag(PointerEventData eventData)
    {
        //드래그 종료
        isDragging = false;

        //손을 뗀 순간 바로 사라지지 않고
        //관성 움직임이 끝날 시간을 주기 위해 타이머 설정
        hideTimer = hideDelay;
    }

    //*마우스 휠 입력*
    public void OnScroll(PointerEventData eventData)
    {
        //실제로 스크롤 가능한 상태인지 확인
        if (!CanScrollVertically()) return;
        
        //휠을 움직이는 순간 Scrollbar 표시
        ShowImmediately();

        //마지막 휠 입력 이후 잠시 뒤 숨김
        hideTimer = hideDelay;
    }

    //*Scroll 가능 여부*
    private bool CanScrollVertically()
    {
        //필수 참조가 없다면 스크롤 가능 여부를 계산할 수 없음
        if (scrollRect == null) return false;
        if (scrollRect.content == null) return false;
        if (scrollRect.viewport == null) return false;

        //Content 높이가 Viewport 높이보다 클 때만
        //실제로 세로 스크롤이 필요한 상태
        return scrollRect.content.rect.height > scrollRect.viewport.rect.height;
    }

    //*표시 / 숨김*
    private void ShowImmediately()
    {
        if (scrollbarCanvasGroup == null) return;   //CanvasGroup이 없다면 종료

        scrollbarCanvasGroup.alpha = 1f;            //완전히 표시
    }

    private void HideImmediately()
    {
        if (scrollbarCanvasGroup == null) return;   //CanvasGroup이 없다면 종료

        scrollbarCanvasGroup.alpha = 0f;            //완전히 숨김

        //Runtime 상태 초기화
        isDragging = false;                         //드래그 초기화
        hideTimer = 0f;                             //숨김 타이머 초기화
    }
}
