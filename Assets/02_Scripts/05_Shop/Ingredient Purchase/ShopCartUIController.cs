using TMPro;        //TMP_Text 사용
using UnityEngine;

public class ShopCartUIController : MonoBehaviour
{
    //*장바구니*
    //실제 장바구니 데이터를 관리하는 Controller
    //
    //재료 추가 / 감소 / 삭제 등의 실제 데이터 처리는
    //ShopCartController가 담당
    [SerializeField] private ShopCartController shopCartController;

    //*UI*
    //장바구니 슬롯들이 생성될 부모 Transform
    [SerializeField] private Transform cartContent;

    //장바구니 재료 하나를 표시할 Prefab
    [SerializeField] private ShopCartItemUI cartItemPrefab;

    //현재 장바구니의 전체 가격을 표시하는 TMP Text
    [SerializeField] private TMP_Text totalPriceText;       

    private void OnEnable()
    {
        //ShopCartController가 연결되지 않은 경우
        if (shopCartController == null) return;
        
        //장바구니 내용이 변경될 때 RefreshCartUI()가 실행되도록 이벤트 구독
        shopCartController.OnCartChanged += RefreshCartUI;

        //상점 UI가 처음 활성화됐을 때도 현재 장바구니 상태를 한 번 표시
        RefreshCartUI();
    }

    private void OnDisable()
    {
        //ShopCartController가 존재하는 경우
        if (shopCartController != null)
        {
            //OnEnable()에서 등록했던 이벤트 구독 해제
            shopCartController.OnCartChanged -= RefreshCartUI;
        }
    }

    //*장바구니 전체 UI 갱신*
    private void RefreshCartUI()
    {
        //필요한 참조가 연결되어 있는지 확인
        if (shopCartController == null) return;     

        if (cartContent == null) return;
        
        if (cartItemPrefab == null) return;
        
        //현재 화면에 표시되어 있는 기존 장바구니 슬롯들을 모두 제거
        ClearCartItemUI();

        //ShopCartController에 저장되어 있는 현재 장바구니 데이터를 하나씩 확인
        foreach (ShopCartItemData cartItemData in shopCartController.CartItems)
        {
            //잘못된 데이터가 있다면 건너뜀
            if (cartItemData == null || cartItemData.Data == null) continue;

            //ShopCartItemUI Prefab 생성
            //
            //부모를 cartContent로 설정하기 때문에
            //ScrollView의 Content 안에 자동으로 들어감
            ShopCartItemUI newCartItemUI = Instantiate(cartItemPrefab, cartContent);

            //생성된 UI에 현재 장바구니 재료 데이터 전달
            newCartItemUI.Initialize(cartItemData, shopCartController);
        }

        //장바구니 전체 가격 갱신
        RefreshTotalPrice();
    }

    //기존 장바구니 슬롯 UI 제거
    private void ClearCartItemUI()
    {
        //Content 안에 들어있는 모든 자식 UI를 확인
        for (int i = cartContent.childCount - 1; i >= 0; i--)
        {
            //현재 자식 GameObject 제거
            Destroy(cartContent.GetChild(i).gameObject);
        }
    }

    //총 가격 UI 갱신
    private void RefreshTotalPrice()
    {
        //총 가격 Text가 연결되지 않았다면 종료
        if (totalPriceText == null) return;

        //ShopCartController에서 현재 총 가격 계산
        int totalPrice = shopCartController.GetTotalPrice();

        //총 가격 화면 표시
        totalPriceText.text = $"총 가격 : {totalPrice} G";
    }
}
