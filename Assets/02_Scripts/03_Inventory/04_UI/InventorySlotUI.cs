//**인벤토리 한 칸을 담당하는 슬롯 UI**
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//같은 게임 오브젝트에 해당 스크립트가 여러개 붙는거 방지
[DisallowMultipleComponent]

//이 스크립트가 붙는 오브젝트에는 반드시 Button 컨포넌트가 존재하도록 하기
[RequireComponent(typeof(Button))]

public sealed class InventorySlotUI : MonoBehaviour
{
    [Header("슬롯 클릭 버튼")]
    [SerializeField] private Button slotButton;         //인벤토리 슬롯 클릭 감지

    [Header("아이템 표시 UI")]
    [SerializeField] private Image itemIconImage;       //재료 또는 요리 아이콘 표시
    [SerializeField] private TMP_Text itemNameText;     //아이템 이름 텍스트
    [SerializeField] private TMP_Text amountText;       //보유 수량 텍스트

    [Header("선택 표시")]
    [SerializeField] private GameObject selectedFrame;  //현재 선택된 슬롯임을 표시하는 테두리 오브젝트

    [Header("인벤토리 특별 요리 배지")]
    [SerializeField] private GameObject specialBadge;

    private string itemId = string.Empty;               //현재 이 슬롯에 연결된 아이템 ID (예: IG_01, DS_03 등)
    private Action<string> onClickCallback;             //슬롯이 클릭됐을 때 실행할 외부 메서드 저장

    //프로퍼티
    public string ItemId => itemId;
    public bool IsEmpty => string.IsNullOrWhiteSpace(itemId);

    private void Awake()
    {
        //Inspector에 버튼을 연결하지 않았다면
        //현재 오브젝트에 붙어 있는 Button을 자동으로 가져옴
        if (slotButton == null) slotButton = GetComponent<Button>();

        //혹시 같은 클릭 이벤트고 중복 등록되어 있다면 제거
        slotButton.onClick.RemoveListener(HandleSlotClicked);

        //슬롯 버튼 클릭 시 HandleSlotClicked가 실행되도록 연결
        slotButton.onClick.AddListener(HandleSlotClicked);

        //처음에는 선택 표시 끄기
        SetSelected(false);
    }

    //오브젝트 제거 시 호출
    private void OnDestroy()
    {
        //버튼이 존재한다면 등록했던 클릭 이벤트 제거
        if (slotButton != null) slotButton.onClick.RemoveListener(HandleSlotClicked);
    }

    //[슬롯에 표시할 아이템 정보를 설정하는 메서드]
    public void Setup(
        string targetItemId,
        Sprite icon,
        string displayName,
        int amount,
        Action<string> clickCallback
        )
    {
        //아이템 ID가 비어 있다면 정상적인 아이템 슬롯이 아니므로 슬롯을 비우고 메서드 종료
        if (string.IsNullOrWhiteSpace(targetItemId))
        {
            Debug.LogWarning("[InventorySlotUI] 아이템 ID가 비어 있어 슬롯을 표시할 수 없습니다.");

            ClearSlot();
            return;
        }

        //보유 수량이 0 이하라면 표시할 아이템이 없으므로 슬롯을 비우고 메서드 종료
        if (amount <= 0)
        {
            ClearSlot();
            return;
        }

        itemId = targetItemId;              //현재 슬롯에 표시할 아이템 ID 저장
        onClickCallback = clickCallback;    //슬롯 클릭 시 실행할 외부 메서드 저장

        //아이콘 Image가 연결되어 있는지 확인
        if (itemIconImage != null)
        {
            itemIconImage.sprite = icon;            //전달받은 Sprite를 아이콘으로 설정
            itemIconImage.enabled = icon != null;   //아이콘이 존재할 때만 Image 컴포넌트를 보이게 함
        }

        //이름 텍스트가 연결되어 있다면 아이템 이름을 표시
        if (itemNameText != null) itemNameText.text = displayName;

        //수량 텍스트가 연결되어 있다면 보유 수량을 표시
        if (amountText != null) amountText.text = amount.ToString();

        //버튼이 연결되어 있다면 클릭할 수 있는 상태로 만듦
        if (slotButton != null) slotButton.interactable = true;

        //새 아이템을 설정할 때는 선택 표시 초기화
        SetSelected(false);
    }

    //[아이템 종류는 그대로이고 수량만 바꼈을 때 사용하는 메서드]
    public void UpdateAmount(int amount)
    {
        //0 이하의 수량은 이 메서드에서 처리하지 않음
        //
        //아이템이 모두 소모되어 Slot 자체가 사라지는 처리는
        //InventoryUIController의 RemoveSlot()이 담당함
        //
        //여기서 ClearSlot()을 해버리면
        //Controller의 slotLookup에는 Slot이 남아 있는데
        //UI만 비어버리는 상태가 될 수 있음
        if (amount <= 0)
        {
            Debug.LogWarning($"[InventorySlotUI] UpdateAmount에는 1 이상의 수량이 필요합니다. 전달값: {amount}");

            return;
        }

        //아이템 종류는 그대로 유지하고 화면에 표시되는 수량만 변경
        if (amountText != null)
        {
            amountText.text = amount.ToString();
        }
    }

    //[현재 슬롯의 선택 여부를 표시하는 메서드]
    public void SetSelected(bool isSelected)
    {
        //선택 테두리 오브젝트가 연결되어있는지 확인(true: 테두리 표시, false: 테두리 숨김)
        if (selectedFrame != null) selectedFrame.SetActive(isSelected);
    }

    //[특별 요리 여부에 따라 S 배지 표시를 변경]
    public void SetSpecialBadge(bool isSpecial)
    {
        //Inspector에서 SpecialBadge가 연결되지 않은 경우 NullReferenceException을 방지하고 종료
        if (specialBadge == null) return;

        //특별 요리라면 ON / 재료나 일반 요리라면 OFF
        specialBadge.SetActive(isSpecial);
    }

    //[슬롯에 연결된 아이템 정보를 모두 제거하는 메서드]
    public void ClearSlot()
    {
        itemId = string.Empty;  //저장된 아이템 ID를 비움
        onClickCallback = null; //저장된 클릭 콜백을 제거

        //아이콘이 연결되어 있다면 초기화
        if (itemIconImage != null)
        {
            itemIconImage.sprite = null;    //기존 아이콘 Sprite 제거
            itemIconImage.enabled = false;  //빈 아이콘이 화면에 나타나지 않도록 비활성화
        }

        //이름 텍스트 비우기
        if (itemNameText != null) itemNameText.text = string.Empty;

        //수량 텍스트 비우기
        if (amountText != null) amountText.text = string.Empty;

        //빈 슬롯 클릭 안되도록 버튼 비활성화
        if (slotButton != null) slotButton.interactable = false;

        //선택 표시도 제거
        SetSelected(false);

        //특별 요리 배지도 초기 상태로 복구
        SetSpecialBadge(false);
    }

    //[실제로 슬롯 버튼을 클릭했을 때 실행되는 메서드]
    private void HandleSlotClicked()
    {
        if (IsEmpty) return;                    //빈 슬롯을 클릭하면 아무 작업도 하지 않음
        if (onClickCallback == null) return;    //외부에서 연결한 클릭 메서드가 없다면 종료

        onClickCallback.Invoke(itemId);         //아이템 ID 외부 메서드에 전달
    }
}
