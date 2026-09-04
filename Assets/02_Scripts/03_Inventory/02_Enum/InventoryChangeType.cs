//**인벤토리에서 어떤 종류의 변경이 발생했는지 구분하는 열거형**
public enum InventoryChangeType
{
    Added,              //새로운 종류의 아이템이 인벤토리에 추가됨
    AmountChanged,      //이미 존재하는 아이템의 수량만 변경됨
    Removed,            //아이템 수량이 0이 되어 슬롯 자체가 제거됨
    Cleared,            //인벤토리 전체가 비워짐
    Loaded              //저장 데이터를 불러와 인벤토리 전체가 교체됨
}