//**인벤토리 음식 슬롯이 Dish에 드롭됐을 때 배치를 요청하는 스크립트**
using UnityEngine;
using UnityEngine.EventSystems; //IDropHandler, PointerEventData 사용
using UnityEngine.UI;           //Image 사용

public class FoodPlaceDropHandler : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    //현재 드롭 영역이 담당하는 접시
    [SerializeField] private FoodPlace _foodPlace;

    //실제 음식 배치 처리를 담당하는 컨트롤러
    [SerializeField] private FoodPlaceController _controller;

    //접시 3개의 상태와 방송 시작 버튼을 관리
    [SerializeField] private FoodArea _foodArea;

    //*드롭 안내 화살표*

    //드래그 중 빈 접시에 표시할 화살표 Image
    //Dish 본체의 Image를 연결
    [SerializeField] private Image _dropGuideImage;

    //화살표가 보일 때 적용할 투명도
    [Range(0f, 1f)][SerializeField] private float _dropGuideVisibleAlpha = 1f;

    private void Awake()
    {
        //Inspector에서 연결하지 않았다면
        //현재 Dish 오브젝트의 FoodPlace를 자동으로 검색
        if (_foodPlace == null)
            _foodPlace = GetComponent<FoodPlace>();

        //Inspector에서 연결하지 않았다면 부모에서 검색
        //부모에 없을 수 있으므로 Inspector 직접 연결을 권장
        if (_controller == null)
            _controller = GetComponentInParent<FoodPlaceController>();

        //Inspector에서 연결하지 않았다면
        //Dish의 부모에 있는 FoodArea 검색
        if (_foodArea == null)
            _foodArea = GetComponentInParent<FoodArea>();

        //Inspector에서 연결하지 않았다면
        //Dish 본체의 Image를 자동으로 사용
        if (_dropGuideImage == null)
        {
            _dropGuideImage = GetComponent<Image>();
        }

        //게임 시작 시 화살표 숨김
        SetDropGuideVisible(false);
    }

    private void OnEnable()
    {
        //음식 슬롯의 드래그 시작·종료 이벤트 구독
        LiveInventorySlotUI.OnFoodDragStateChanged += HandleFoodDragStateChanged;
    }

    private void OnDisable()
    {
        //비활성화될 때 이벤트 구독 해제
        LiveInventorySlotUI.OnFoodDragStateChanged -= HandleFoodDragStateChanged;

        //비활성화될 때 화살표가 남지 않도록 숨김
        SetDropGuideVisible(false);
    }

    //*음식 슬롯의 드래그 상태가 바뀌었을 때 호출*
    private void HandleFoodDragStateChanged(bool isDragging)
    {
        //현재 방송 중인지 확인
        bool isLive = LiveManager.Instance != null && LiveManager.Instance.IsLive;

        //음식을 받을 수 있는 빈 접시인지 확인
        bool isEmptyFoodPlace = _foodPlace != null && !_foodPlace.IsFilled;

        //다음 조건을 모두 만족할 때만 화살표 표시
        //1. 음식을 드래그 중
        //2. 현재 접시가 비어 있음
        //3. 라이브 방송 중이 아님
        bool shouldShow = isDragging && isEmptyFoodPlace && !isLive;

        SetDropGuideVisible(shouldShow);
    }

    //*드롭 안내 화살표 표시 또는 숨김*
    private void SetDropGuideVisible(bool visible)
    {
        //화살표 Image가 없다면 처리할 작업 없음
        if (_dropGuideImage == null) return;
        
        //현재 Image 색상 가져오기
        Color guideColor = _dropGuideImage.color;

        //표시할 때는 지정된 알파값,
        //숨길 때는 완전히 투명한 0 적용
        guideColor.a = visible ? _dropGuideVisibleAlpha : 0f;

        //변경된 색상 적용
        _dropGuideImage.color = guideColor;

        //Image 컴포넌트는 비활성화하지 않음
        //투명한 상태에서도 드롭과 클릭 Raycast를 받아야 함
    }

    //*음식 슬롯을 현재 Dish 위에 놓았을 때 호출*
    public void OnDrop(PointerEventData eventData)
    {
        //현재 Dish의 FoodPlace가 없다면 배치할 수 없음
        if (_foodPlace == null)
        {
            Debug.LogError($"[FoodPlaceDropHandler] {name}의 FoodPlace가 없습니다.");
            return;
        }

        //배치를 처리할 컨트롤러가 없다면 진행할 수 없음
        if (_controller == null)
        {
            Debug.LogError($"[FoodPlaceDropHandler] {name}의 FoodPlaceController가 연결되지 않았습니다.");
            return;
        }

        //현재 드래그 중인 원본 UI 오브젝트 가져오기
        GameObject draggedObject = eventData.pointerDrag;

        //드래그된 오브젝트가 없다면 처리하지 않음
        if (draggedObject == null) return;

        //드래그된 오브젝트에서 음식 슬롯 스크립트 검색
        LiveInventorySlotUI draggedSlot = draggedObject.GetComponent<LiveInventorySlotUI>();

        //드래그된 것이 음식 인벤토리 슬롯이 아니라면 처리하지 않음
        if (draggedSlot == null) return;

        //드래그한 슬롯에 저장된 음식 ID 가져오기
        string itemId = draggedSlot.ItemId;

        //음식 ID가 비어 있다면 배치하지 않음
        if (string.IsNullOrWhiteSpace(itemId)) return;
        
        //현재 슬롯의 보유 수량이 없다면 배치하지 않음
        if (draggedSlot.Amount <= 0) return;     

        //컨트롤러에 음식 배치를 요청
        bool placed = _controller.TryPlaceFood(_foodPlace, itemId);

        //이미 음식이 있는 접시 등으로 인해 배치에 실패했다면 종료
        if (!placed) return;

        //음식이 배치됐으므로 해당 Dish의 화살표 즉시 숨김
        SetDropGuideVisible(false);

        //배치에 성공했으므로 드래그 중 표시되던 임시 아이콘 제거
        draggedSlot.CancelDragVisual();

        //접시 3개가 모두 채워졌는지 다시 확인
        if (_foodArea != null)
        {
            _foodArea.CheckFoodPlaces();
        }
    }

    //*접시를 마우스 또는 터치로 눌렀을 때 호출*
    public void OnPointerClick(PointerEventData eventData)
    {
        //PC에서는 왼쪽 클릭만 처리
        //모바일 터치도 Left 입력으로 전달됨
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        //접시 또는 컨트롤러가 없다면 반환할 수 없음
        if (_foodPlace == null || _controller == null) return;
        
        //접시 음식을 인벤토리로 돌려달라고 컨트롤러에 요청
        bool returned = _controller.TryReturnFood(_foodPlace);

        //빈 접시 또는 방송 중이라서 반환에 실패했다면 종료
        if (!returned) return;

        //음식을 반환해도 현재 드래그 중이 아니므로
        //화살표는 숨겨진 상태로 유지
        SetDropGuideVisible(false);

        //음식이 제거됐으므로 방송 시작 버튼 상태 다시 확인
        if (_foodArea != null) _foodArea.CheckFoodPlaces();        
    }
}