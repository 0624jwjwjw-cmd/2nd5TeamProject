//**스튜디오의 인벤토리에서 음식 하나를 표시하는 슬롯 UI**
using System;                   //Action 이벤트 사용
using UnityEngine;
using UnityEngine.EventSystems; //IBeginDragHandler, PointerEventData 사용
using UnityEngine.UI;           //Image 사용
using TMPro;                    //TMP_Text 사용

public class LiveInventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler //(새로 추가)
{
    [SerializeField] private Image _icon;           //현재 음식 아이콘 이미지
    [SerializeField] private TMP_Text _nameText;    //현재 음식의 이름 표시
    [SerializeField] private TMP_Text _amountText;  //현재 보유 수량

    private string _itemId; //현재 슬롯이 표시하는 음식 ID
    private int _amount;    //현재 슬롯에 표시할 음식 보유 수량

    //(새로 추가)
    private GameObject _dragIconObject;             //드래그 중 화면 최상단에 생성되는 음식 아이콘 오브젝트
    private RectTransform _dragIconRectTransform;   //드래그 아이콘의 위치와 크기를 조절하는 RectTransform
    private Canvas _rootCanvas;                     //현재 UI가 들어 있는 최상위 Canvas

    //*음식 드래그 상태 알림*
    //음식 드래그가 시작되거나 종료됐음을 Dish UI에 알리는 이벤트
    public static event Action<bool> OnFoodDragStateChanged;
    //현재 이 슬롯이 실제로 드래그 중인지 저장
    private bool _isDragging;

    //프로퍼티
    public string ItemId => _itemId;
    public int Amount => _amount;

    //*슬롯에 음식 정보를 표시하는 메서드*
    //LiveInventoryUI가 보유 중인 음식을 찾았을 때 호출
    public void Setup(string itemId, Sprite icon, string itemName, int amount)
    {
        //Clear()에서 비활성화했던 슬롯을 다시 화면에 표시 (새로 추가)
        gameObject.SetActive(true);

        _itemId = itemId;   //현재 슬롯이 담당할 음식 ID 저장
        _amount = amount;   //현재 음식 보유 수량 저장

        //아이콘 Image가 정상적으로 연결되어 있다면 음식 아이콘 표시
        if (_icon != null) _icon.sprite = icon;

        //이름 Text가 정상적으로 연결되어 있다면 음식 이름 표시
        if (_nameText != null) _nameText.text = itemName;

        //수량 Text가 정상적으로 연결되어 있다면 음식 수량 표시
        if (_amountText != null) _amountText.text = amount.ToString();
    }

    //*슬롯을 비우고 화면에서 숨기는 메서드*
    //현재 인벤토리에 표시할 음식이 없거나 슬롯을 재사용할 때 호출
    public void Clear()
    {
        //슬롯이 비워지는 도중 드래그 아이콘이 남지 않도록 제거
        CancelDragVisual();

        _itemId = null;     //기존 음식 ID 제거
        _amount = 0;        //기존 수량 제거

        //이전 음식 아이콘이 남지 않도록 제거
        if (_icon != null) _icon.sprite = null;

        //이전 음식 이름이 남지 않도록 제거
        if (_nameText != null) _nameText.text = string.Empty;

        //이전 수량이 남지 않도록 제거
        if (_amountText != null) _amountText.text = string.Empty;

        //빈 슬롯은 가로 인벤토리에 표시하지 않음 (새로 추가)
        gameObject.SetActive(false);
    }

    //*음식 슬롯을 터치하고 드래그하기 시작할 때 호출* (새로 추가)
    public void OnBeginDrag(PointerEventData eventData)
    {
        //빈 슬롯은 드래그할 수 없음
        if (string.IsNullOrWhiteSpace(_itemId)) return;

        //보유 수량이 없다면 드래그할 수 없음
        if (_amount <= 0) return;

        //아이콘이 연결되지 않았거나 Sprite가 없다면 표시할 수 없음
        if (_icon == null || _icon.sprite == null) return;

        //기존 드래그 표시가 남아 있다면 먼저 제거
        CancelDragVisual();

        //드래그 중 보여줄 음식 아이콘 생성
        CreateDragVisual();

        //Canvas를 찾지 못하는 등의 이유로
        //드래그 아이콘 생성에 실패했다면 드래그 시작으로 처리하지 않음
        if (_dragIconRectTransform == null) return;
        
        //현재 슬롯이 드래그 중임을 저장
        _isDragging = true;

        //이 이벤트를 구독 중인 Dish들에게
        //음식 드래그가 시작됐다고 알림
        OnFoodDragStateChanged?.Invoke(true);

        //생성된 아이콘을 현재 터치 위치로 이동
        UpdateDragVisualPosition(eventData);
    }

    //*사용자가 손가락이나 마우스를 움직이는 동안 계속 호출* (새로 추가)
    public void OnDrag(PointerEventData eventData)
    {
        //드래그 아이콘이 생성되지 않았다면 처리할 작업 없음
        if (_dragIconRectTransform == null) return;

        //드래그 아이콘을 현재 포인터 위치로 이동
        UpdateDragVisualPosition(eventData);
    }

    //*사용자가 손가락이나 마우스를 놓았을 때 호출* (새로 추가)
    public void OnEndDrag(PointerEventData eventData)
    {
        //드래그가 끝났으므로 화면의 임시 아이콘 제거
        CancelDragVisual();
    }

    //*드래그 중 보여줄 임시 음식 아이콘 생성* (새로 추가)
    private void CreateDragVisual()
    {
        //현재 슬롯이 들어있는 Canvas 검색
        _rootCanvas = GetComponentInParent<Canvas>();

        //Canvas가 없다면 UI 아이콘을 생성할 수 없음
        if (_rootCanvas == null)
        {
            return;
        }

        //드래그 아이콘으로 사용할 새로운 UI 오브젝트 생성
        _dragIconObject = new GameObject(
            "FoodDragIcon",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(CanvasGroup)
            );

        //다른 UI보다 위에 표시되도록 Canvas의 자식으로 설정
        _dragIconObject.transform.SetParent(_rootCanvas.transform, false);

        //Canvas와 같은 UI Layer 사용
        _dragIconObject.layer = _rootCanvas.gameObject.layer;

        //드래그 아이콘 RectTransform 가져오기
        _dragIconRectTransform = _dragIconObject.GetComponent<RectTransform>();

        //드래그 아이콘의 중심점을 포인터 위치에 맞춤
        _dragIconRectTransform.anchorMin = new Vector2(0.5f, 0.5f); //anchorMin
        _dragIconRectTransform.anchorMax = new Vector2(0.5f, 0.5f); //anchorMax
        _dragIconRectTransform.pivot = new Vector2(0.5f, 0.5f);     //pivot

        //기존 슬롯 아이콘과 같은 크기로 설정
        _dragIconRectTransform.sizeDelta = _icon.rectTransform.rect.size;

        //드래그 아이콘 Image 가져오기
        Image dragImage = _dragIconObject.GetComponent<Image>();

        //현재 음식 Sprite를 드래그 아이콘에 적용
        dragImage.sprite = _icon.sprite;

        //음식 이미지 비율 유지
        dragImage.preserveAspect = true;

        //드래그 아이콘이 터치 입력을 가로채지 않도록 설정
        dragImage.raycastTarget = false;

        //드래그 아이콘의 CanvasGroup 가져오기
        CanvasGroup canvasGroup = _dragIconObject.GetComponent<CanvasGroup>();

        //아래에 있는 Dish 드롭 영역이 입력을 받을 수 있도록 설정
        canvasGroup.blocksRaycasts = false;

        //드래그 아이콘 자체는 입력 대상이 아님
        canvasGroup.interactable = false;

        //항상 다른 UI보다 위에 표시
        _dragIconObject.transform.SetAsLastSibling();
    }

    //*현재 터치 위치로 드래그 아이콘 이동* (새로 추가)
    private void UpdateDragVisualPosition(
        PointerEventData eventData)
    {
        //Canvas 또는 드래그 아이콘이 없다면 이동할 수 없음
        if (_rootCanvas == null || _dragIconRectTransform == null) return;
        
        //Canvas의 RectTransform 가져오기
        RectTransform canvasRectTransform = _rootCanvas.transform as RectTransform;

        //정상적인 Canvas RectTransform인지 확인
        if (canvasRectTransform == null) return;

        //화면 좌표를 Canvas 내부 좌표로 변환
        bool converted =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPosition
                );

        //좌표 변환에 실패했다면 이동하지 않음
        if (!converted) return;

        //변환된 위치를 드래그 아이콘에 적용
        _dragIconRectTransform.anchoredPosition = localPosition;
    }

    //*화면에 남아 있는 임시 드래그 아이콘 제거* (새로 추가)
    public void CancelDragVisual()
    {
        //이 슬롯이 실제로 드래그 중이었는지 먼저 저장
        bool wasDragging = _isDragging;

        //현재 드래그 상태 종료
        _isDragging = false;

        //드래그 아이콘 오브젝트가 존재하면 제거
        if (_dragIconObject != null)
        {
            //Destroy가 실제로 처리되기 전에도 보이지 않도록 비활성화
            _dragIconObject.SetActive(false);

            Destroy(_dragIconObject);
        }

        //기존 참조 초기화
        _dragIconObject = null;
        _dragIconRectTransform = null;
        _rootCanvas = null;

        //실제로 드래그 중이었던 슬롯만 종료 이벤트 발생
        //빈 슬롯 Clear 등으로 잘못된 종료 이벤트가 발생하는 것을 방지
        if (wasDragging)
        {
            OnFoodDragStateChanged?.Invoke(false);
        }
    }

    //*인벤토리 갱신으로 슬롯이 비활성화될 때 호출* (새로 추가)
    private void OnDisable()
    {
        //드래그 중 슬롯이 사라져도 아이콘이 화면에 남지 않도록 정리
        CancelDragVisual();
    }
}