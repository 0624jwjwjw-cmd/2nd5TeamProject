//**게임 데이터 검색 기능의 공통 규칙을 정의하는 인터페이스**
//
//역할: 어떤 검색 기능이 있어야 하는지 규칙만 정의
using System.Collections.Generic;

public interface IGameDataRepository
{
    //Repository가 정상적으로 초기화되었는지 외부에서 확인할 수 있게
    bool IsInitialized { get; }

    //전달받은 재료 ID와 일치하는 IngredientData 검색
    //검색 성공: ingredientData에 검색 결과 저장(true)
    //검색 실패: ingredientData는 null(false)
    bool TryGetIngredient(string itemId, out IngredientData ingredientData);

    //GameDataRepository에 등록된 요리 데이터만 순회할 수 있도록 제공
    //IEnumerable<DishData> Dishes { get; }

    //전달받은 일반 요리 ID와 일치하는 DishData 검색
    bool TryGetDish(string itemId, out DishData dishData);

    //전달받은 특별 요리 ID와 일치하는 DishData 검색
    bool TryGetSpecialDish(string itemId, out DishData specialDishData);
}
