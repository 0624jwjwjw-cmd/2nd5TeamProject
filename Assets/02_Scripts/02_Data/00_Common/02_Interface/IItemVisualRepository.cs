//**아이템 ID를 이용해 아이콘과 프리팹을 검색하기 위한 Visual Repository 규칙**
using UnityEngine;

public interface IItemVisualRepository
{
    //Repository가 정상적으로 초기화되었는지 확인
    //
    //Dictionary가 만들어지기 전에
    //다른 시스템이 검색을 시도하는 것을 방지하는 데 사용
    bool IsInitialized { get; }

    //==================================================
    //아이콘 검색
    //==================================================
    //itemId에 해당하는 Sprite 검색
    //성공: true 반환 icon에 Sprite 전달
    //실패: false 반환 icon은 null
    bool TryGetIcon(string itemId, out Sprite icon);

    //==================================================
    //프리팹 검색
    //==================================================
    //itemId에 해당하는 월드 Prefab 검색
    //inventroy에서는 주로 Icon을 사용하고,
    //Cooking / Live 등에서 실제 오브젝트가 필요하면 Prefab 사용
    //(최적화를 위해 inventroy에선 실제 오브젝트를 만들지 않음)
    bool TryGetPrefab(string itemId, out GameObject prefab);
}
