//**인벤토리 슬롯 데이터**
using System;
using UnityEngine;

//이 클래스를 Unity Inspector, JSON 저장, List 직렬화 등에 사용 가능
[Serializable]

//다른 클래스가 상속할 필요 없으므로 sealed 사용해 상속 막기
public sealed class InventorySlotData
{
    [SerializeField] private string itemId;     //슬롯에 들어 있는 아이템의 고유 ID
    [SerializeField] private int amount;        //현재 슬롯에 들어있는 아이템 개수 (예: 빵 5개는 amount = 5)
    [SerializeField] private int acquiredOrder; //해당 아이템을 처음 획득한 순서 저장(획득순 정렬용)
    
    //외부에서 읽게 프로퍼티
    public string ItemId => itemId;
    public int Amount => amount;
    public int AcquiredOrder => acquiredOrder;
    //아이템 ID가 없거나 수량이 0 이하라면 현재 슬롯을 빈 슬롯으로 판정
    public bool IsEmpty => string.IsNullOrWhiteSpace(itemId) || amount <= 0;
    
    //[직렬화 또는 저장 데이터 복원 과정에서 사용할 수 있는 기본 생성자]
    public InventorySlotData()
    {
        itemId = string.Empty;  //새로 생성된 빈 슬롯이므로 아이템 ID를 빈 문자열로 설정
        amount = 0;             //빈 슬롯이므로 수량 0으로 설정
        acquiredOrder = 0;      //아직 획득 못했으므로 획득 순서 0으로 설정
    }

    //[새로운 인벤토리 슬롯을 생성할 때 사용하는 생성자]
    //InventoryManager에서 아이템을 처음 획득했을 때 호출
    public InventorySlotData(string itemId, int amount, int acquiredOrder)
    {
        //전달받은 아이템 ID가 비어 있는지 검사
        if (string.IsNullOrWhiteSpace(itemId))
        {
            //아이템 ID가 없으면 어떤 아이템인지 구분할 수 없으므로
            //throw로 잘못된 값이라는 예외 발생시키기
            throw new ArgumentException("아이템 ID는 비어 있을 수 없습니다.", nameof(itemId));
        }

        //전달받은 아이템 수량이 1개 미만인지 검사
        if (amount <= 0)
        {
            //새로운 아이템 슬롯은 최소 1개 이상이어야함
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "아이템 수량은 1개 이상이어야 합니다."
                );
        }

        //전달받은 획득 순서가 0보다 작은지 검사
        if (acquiredOrder < 0)
        {
            //획득 순서는 음수가 될 수 없으므로 예외 발생시키기
            throw new ArgumentOutOfRangeException(
                nameof(acquiredOrder),
                acquiredOrder,
                "획득 순서는 0 이상이어야 합니다."
                );
        }

        this.itemId = itemId;               //전달받은 아이템 ID를 현재 슬롯의 itemId에 저장
        this.amount = amount;               //전달받은 아이템 수량을 현재 슬롯의 amount에 저장
        this.acquiredOrder = acquiredOrder; //전달받은 획득 순서를 현재 슬롯에 저장
    }

    //[이 슬롯이 전달받은 아이템 ID와 같은 아이템인지 검사 하는 메서드]
    //InventoryManager가 동일 아이템 슬롯을 찾을 때 사용
    public bool IsSameItem(string targetItemId)
    {
        //현재 슬롯의 itemId와 전달받은 targetItemId를 비교
        //StringComparison.Ordinal을 사용해 정확한 문자열 값으로 비교
        return string.Equals(itemId, targetItemId, StringComparison.Ordinal);
    }

    //[현재 슬롯의 아이템 수량을 증가시키는 메서드]
    //동일 아이템 추가시 InventoryManager가 호출
    public void AddAmount(int value)
    {
        //추가하려는 수량이 1개 미만인지 검사
        if (value <= 0)
        {
            //잘못된 수량이 전달되면 게임이 멈추지 않도록
            //경고 메시지만 출력하고 메서드 종료
            Debug.LogWarning($"[InventorySlotData] 추가 수량은 1 이상이어야 합니다. 입력값: {value}");

            return; //아래의 수량 증가 코드가 실행되지 않도록 메서드 종료
        }

        //현재 아이템 수량에 전달받은 수량 더하기
        amount += value;
    }

    //[현재 슬롯에서 아이템을 제거하는 메서드]
    //제거 성공 여부 bool로 반환
    public bool TryRemoveAmount(int value)
    {
        //제거하려는 수량이 1개 미만인지 검사
        if (value <= 0)
        {
            //잘못된 제거 요청이므로 경고 메시지 출력 후
            Debug.LogWarning($"[InventorySlotData] 제거 수량은 1 이상이어야 합니다. 입력값: {value}");
            //제거에 실패했으므로 false 반환
            return false;
        }

        //현재 보유 수량보다 제거하려는 수량이 많은지 검사
        if (amount < value)
        {
            //수량이 부족하면 아무것도 제거하지 않고 false 반환
            return false;
        }

        amount -= value;    //현재 수량에서 전달받은 수량만큼 차감
        if (amount == 0)
        {
            Clear();        //수량 차감 후 0개가 되었다면 슬롯 전체를 비움
        }
        return true;        //제거에 성공했으므로 true 반환
    }

    //[현재 수량을 지정한 값으로 변경하기 위한 메서드]
    //저장데이터 불러오거나 디버그 기능 만들 때 사용
    public void SetAmount(int value)
    {
        //수량이 0 이하라면 슬롯 전체를 비움
        if (value <= 0)
        {
            Clear();
            return;
        }

        //수량이 1 이상이면 전달받은 값으로 변경
        amount = value;
    }

    //[아이템 수량이 0이 된 슬롯을 완전히 비우는 메서드]
    //InventoryManager가 빈 슬롯을 정리할 때 사용 가능
    public void Clear()
    {
        itemId = string.Empty;  //슬롯에 들어 있던 아이템 ID 제거
        amount = 0;             //슬롯의 아이템 수량을 0으로 변경
        acquiredOrder = 0;      //획득 순서도 초기값인 0으로 변경
    }
}
