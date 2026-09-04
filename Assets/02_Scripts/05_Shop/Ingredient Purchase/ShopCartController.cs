//**재료 상점의 장바구니 데이터와 수량을 관리하는 Controller**
using System;                       //Action 이벤트
using System.Collections.Generic;   //Dictionary
using UnityEngine;

//*장바구니 안의 재료 하나를 나타내는 데이터*
//예) 빵 x3
//
//Data   → 빵 IngredientData
//Amount → 3
public class ShopCartItemData
{
    public IngredientData Data { get; private set; }    //장바구니에 들어있는 실제 재료 데이터

    public int Amount { get; private set; }             //현재 장바구니에 담긴 수량

    //새로운 장바구니 아이템을 만들 때 호출
    public ShopCartItemData(IngredientData data, int amount)
    {
        Data = data;        //어떤 재료인지 저장
        Amount = amount;    //몇 개 담겨있는지 저장
    }

    //같은 재료를 한 번 더 눌렀을 때 수량 +1
    public void AddOne()
    {
        Amount++;
    }

    //장바구니에 있는 재료를 눌렀을 때 수량 -1
    public void RemoveOne()
    {
        Amount--;
    }
}


//*장바구니 전체 관리*
public class ShopCartController : MonoBehaviour
{
    //*장바구니 데이터*
    //아이템 ID를 Key로 사용해서 현재 장바구니의 재료들을 관리하는 Dictionary
    //
    //예)
    //"IG_01" → 빵 x3
    //"IG_02" → 계란 x1
    //
    //같은 재료가 여러 슬롯으로 생기는 것을 방지하기 위해
    //재료 ID를 Key로 사용
    private readonly Dictionary<string, ShopCartItemData>
        cartItems =
        new Dictionary<string, ShopCartItemData>(
            StringComparer.Ordinal
            );

    //*이벤트*
    //장바구니 내용이 변경될 때 실행되는 이벤트
    //
    //나중에 ShopUI가 이 이벤트를 구독하면
    //재료 추가 / 재료 감소 / 재료 삭제
    //될 때마다 장바구니 UI와 총 가격을 자동 갱신할 수 있음
    public event Action OnCartChanged;

    //*프로퍼티*
    public bool IsEmpty => cartItems.Count == 0;    //현재 장바구니가 비어 있는지 확인
    public IEnumerable<ShopCartItemData> CartItems  //외부에서 장바구니 목록을 읽을 수 있도록 제공
        => cartItems.Values;

    //*재료 추가*
    //상점에서 재료를 클릭했을 때 호출
    public bool AddIngredient(IngredientData ingredientData)
    {
        //IngredientData가 없는 경우
        if (ingredientData == null) return false;
        
        //재료의 고유 ID 가져오기
        string itemId = ingredientData.ID;

        //ID가 비어있는 데이터는
        //장바구니에서 구분할 수 없기 때문에 추가하지 않음
        if (string.IsNullOrWhiteSpace(itemId)) return false;
        
        //이미 장바구니에 같은 재료가 있는지 확인
        if (cartItems.TryGetValue(itemId, out ShopCartItemData cartItem))
        {
            //*이미 존재한다면 새로운 슬롯을 만들지 않고
            //기존 수량만 +1*
            cartItem.AddOne();
        }
        else
        {
            //장바구니에 처음 들어오는 재료라면 수량 1의 새로운 장바구니 데이터를 생성
            ShopCartItemData newCartItem = new ShopCartItemData(ingredientData, 1);

            //재료 ID를 Key로 Dictionary에 추가
            cartItems.Add(itemId, newCartItem);
        }

        //*장바구니가 변경되었다는 이벤트 발생*
        OnCartChanged?.Invoke();

        return true;
    }

    //*재료 감소*
    //장바구니 슬롯을 클릭했을 때 호출
    //예)
    //빵 x3 → 빵 x2
    //빵 x1 → 0 → 장바구니에서 제거
    public bool RemoveIngredient(string itemId)
    {
        //잘못된 ID라면 처리하지 않음
        if (string.IsNullOrWhiteSpace(itemId)) return false;

        //해당 재료가 장바구니에 있는지 검색
        if (!cartItems.TryGetValue(itemId, out ShopCartItemData cartItem)) return false;

        cartItem.RemoveOne();   //수량 -1

        //수량이 0 이하가 되었다면 장바구니에서 해당 재료 자체를 제거
        if (cartItem.Amount <= 0)
        {
            cartItems.Remove(itemId);
        }

        OnCartChanged?.Invoke();    //장바구니 내용이 변경되었다는 이벤트 발생

        return true;
    }

    //*총 가격*
    //현재 장바구니 전체 가격 계산
    public int GetTotalPrice()
    {
        //총 가격을 0부터 시작
        int totalPrice = 0;

        //*현재 장바구니에 들어있는
        //모든 재료를 순서대로 확인*
        foreach (ShopCartItemData cartItem in cartItems.Values)
        {
            //혹시 잘못된 데이터가 들어있는 경우 방어
            if (cartItem == null || cartItem.Data == null) continue;

            //재료 단가 × 현재 수량
            int itemTotalPrice = cartItem.Data.Price * cartItem.Amount;

            //전체 가격에 더하기
            totalPrice += itemTotalPrice;
        }

        return totalPrice;  //최종 가격 반환
    }

    //*장바구니 초기화*
    //결제가 성공했거나 장바구니를 전체 비워야 할 때 호출
    public void ClearCart()
    {
        if (cartItems.Count == 0) return;   //이미 비어 있다면 할 작업 없음

        cartItems.Clear();                  //모든 장바구니 데이터 삭제

        OnCartChanged?.Invoke();            //장바구니 변경 이벤트 발생
    }
}
