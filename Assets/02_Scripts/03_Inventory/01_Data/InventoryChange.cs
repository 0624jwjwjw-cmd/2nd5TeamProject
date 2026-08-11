//**InventoryManager에서 발생한 변경 정보를 다른 시스템에 전달하기 위한 데이터 구조체**
public readonly struct InventoryChange
{
    //어떤 종류의 변경이 발생했는지
    public InventoryChangeType ChangeType { get; }

    public string ItemId { get; }       //변경된 아이템의 고유 ID
    public int PreviousAmount { get; }  //변경되기 전 아이템 수량
    public int CurrentAmount { get; }   //변경된 후 아이템 수량

    //생성자
    //InventoryManager가 변경 내용을 생성할 때 사용
    public InventoryChange(
        InventoryChangeType changeType,
        string itemId,
        int previousAmount,
        int currentAmount)
    {
        ChangeType = changeType;
        ItemId = itemId;
        PreviousAmount = previousAmount;
        CurrentAmount = currentAmount;
    }
}

/*
 위 네 가지를 저장하는 이유: 
 예)
 인벤토리에서 빵이 4개 있었는데 하나 구매시
 InventoryChange

 ChangeType     = AmountChanged
 ItemId         = IG_01
 PreviousAmount = 4
 CurrentAmount  = 5
 이렇게 전달됨
 
 이후 InventoryUIController가
 "IG_01만 바뀌었구나"
       ↓
 IG_01 슬롯 찾기
       ↓
 수량 텍스트
    4 → 5
 만 하면 됨
 
 반대로 기존 이벤트는 "인벤토리 변경됨!" 밖에 모르니까 UI 입장에서 무엇이 바뀌었는지 알 수 없었음
 */
