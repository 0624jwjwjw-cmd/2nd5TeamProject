//**인벤토리 슬롯 데이터를 UI에 표시할 정보로 바꿔주는 애**
using UnityEngine;

public static class InventoryItemDisplayDataProvider
{
    //슬롯 데이터와 두 Repository를 받아서
    //UI 표시용 이름, 설명, 아이콘, 특별 요리 여부를 만들어 주는 메서드
    public static bool TryGet(
        InventorySlotData slotData,                 //표시할 인벤토리 슬롯 데이터
        GameDataRepository gameDataRepository,      //이름·설명을 찾을 데이터 Repository
        ItemVisualRepository itemVisualRepository,  //아이콘을 찾을 Visual Repository
        out string displayName,                     //밖으로 돌려줄 아이템 이름
        out string description,                     //밖으로 돌려줄 아이템 설명
        out Sprite icon,                            //밖으로 돌려줄 아이콘
        out bool isSpecial)                         //밖으로 돌려줄 특별 요리 여부
    {
        //실패했을 때도 out 변수에 안전한 기본값이 들어가도록 먼저 초기화
        displayName = string.Empty;
        description = string.Empty;
        icon = null;
        isSpecial = false;

        //슬롯 데이터나 필요한 Repository가 없으면 표시 정보를 만들 수 없음
        if (slotData == null ||
            slotData.IsEmpty ||
            gameDataRepository == null ||
            itemVisualRepository == null)
        {
            return false;
        }

        //이후에 여러 번 쓸 ID를 변수에 저장
        string itemId = slotData.ItemId;

        //슬롯에 저장된 ItemType에 따라
        //알맞은 데이터 종류를 한 번만 찾아서 표시 정보를 채움
        switch (slotData.ItemType)
        {
            //재료인 경우
            case ItemType.Ingredient:

                //재료 데이터 찾기 실패 시 표시 불가
                if (!gameDataRepository.TryGetIngredient(itemId, out IngredientData ingredientData))
                {
                    return false;
                }

                //재료 이름만 표시하고, 설명은 기존처럼 빈 문자열 유지
                displayName = ingredientData.IngredientName;
                break;

            //일반 요리인 경우
            case ItemType.Dish:

                //일반 요리 데이터 찾기 실패 시 표시 불가
                if (!gameDataRepository.TryGetDish(itemId, out DishData dishData))
                {
                    return false;
                }

                //요리 이름과 설명 저장
                displayName = dishData.DishName;
                description = dishData.Info;
                break;

            //특별 요리인 경우
            case ItemType.SpecialDish:

                //특별 요리 데이터 찾기 실패 시 표시 불가
                if (!gameDataRepository.TryGetSpecialDish(itemId, out DishData specialDishData))
                {
                    return false;
                }

                //특별 요리의 이름과 설명 저장
                displayName = specialDishData.DishName;
                description = specialDishData.Info;

                //특별 요리이므로 슬롯의 S 배지 표시용 값을 true로 변경
                isSpecial = true;
                break;

            //정의되지 않은 타입은 표시하지 않음
            default:
                return false;
        }

        //아이콘이 없더라도 이름과 설명은 표시할 수 있으므로
        //기존 코드와 똑같이 아이콘 조회 실패 자체는 실패 처리하지 않음
        itemVisualRepository.TryGetIcon(itemId, out icon);

        //필요한 표시 정보를 모두 준비했으므로 성공
        return true;
    }
}
