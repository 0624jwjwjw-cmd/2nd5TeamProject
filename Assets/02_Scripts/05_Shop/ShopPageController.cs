//**상점 내부 페이지 전환과 장바구니 초기화를 관리하는 Controller**
using UnityEngine;
using UnityEngine.UI;

//같은 GameObject에 중복으로 붙는 것을 방지
[DisallowMultipleComponent]

public sealed class ShopPageController : MonoBehaviour
{
    //*상점 페이지*
    [Header("Shop Pages")]

    //상점에 처음 들어왔을 때 보이는 메인 페이지
    //재료 구매 / 레시피 구매 / 스튜디오 업그레이드 버튼이 있는 화면
    [SerializeField] private GameObject shopMainPage;

    //재료 구매 페이지
    //재료 목록 / 장바구니 / 구매하기 버튼이 있는 화면
    [SerializeField] private GameObject ingredientPurchasePage;

    //*Header*
    [Header("Header")]
    //상점 서브 페이지에서 메인 상점으로 돌아가는 버튼
    [SerializeField] private Button backButton;

    //*상점 메인 메뉴 버튼*
    [Header("Main Menu Buttons")]
    //상점 메인 페이지의 재료 구매 버튼
    [SerializeField] private Button ingredientShopButton;

    //*장바구니*
    [Header("Cart")]
    //현재 재료 구매 장바구니를 관리하는 Controller
    [SerializeField] private ShopCartController shopCartController;

    //*상점 UI가 활성화될 때*
    private void OnEnable()
    {
        //재료 구매 버튼 이벤트 등록
        if (ingredientShopButton != null)
        {
            //중복 등록 방지
            ingredientShopButton.onClick.RemoveListener(OpenIngredientPurchasePage);

            //재료 구매 페이지 열기 이벤트 등록
            ingredientShopButton.onClick.AddListener(OpenIngredientPurchasePage);
        }

        //뒤로가기 버튼 이벤트 등록
        if (backButton != null)
        {
            //중복 등록 방지
            backButton.onClick.RemoveListener(HandleBackButtonClicked);

            //뒤로가기 이벤트 등록
            backButton.onClick.AddListener(HandleBackButtonClicked);
        }

        //상점을 새로 열었을 때는
        //항상 상점 메인 페이지부터 보여줌
        ShowMainPage();
    }

    //*상점 UI가 비활성화될 때*
    private void OnDisable()
    {
        //재료 구매 버튼 이벤트 제거
        if (ingredientShopButton != null)
        {
            ingredientShopButton.onClick.RemoveListener(OpenIngredientPurchasePage);
        }

        //뒤로가기 버튼 이벤트 제거
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(HandleBackButtonClicked);
        }

        //상점 자체를 닫은 경우
        //남아있던 장바구니도 초기화
        ClearCart();
    }

    //*재료 구매 페이지 열기*
    private void OpenIngredientPurchasePage()
    {
        //상점 메인 페이지 숨김
        if (shopMainPage != null)
        {
            shopMainPage.SetActive(false);
        }

        //재료 구매 페이지 표시
        if (ingredientPurchasePage != null)
        {
            ingredientPurchasePage.SetActive(true);
        }

        //서브 페이지이므로 뒤로가기 버튼 표시
        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
        }
    }

    //*뒤로가기 버튼 클릭*
    private void HandleBackButtonClicked()
    {
        //재료 구매 페이지에서 나가므로
        //현재 장바구니 초기화
        ClearCart();

        //상점 메인 화면으로 돌아감
        ShowMainPage();
    }

    //*상점 메인 페이지 표시*
    private void ShowMainPage()
    {
        //상점 메인 페이지 표시
        if (shopMainPage != null)
        {
            shopMainPage.SetActive(true);
        }

        //재료 구매 페이지 숨김
        if (ingredientPurchasePage != null)
        {
            ingredientPurchasePage.SetActive(false);
        }

        //메인 페이지에서는 뒤로가기 버튼이 필요 없으므로 숨김
        if (backButton != null)
        {
            backButton.gameObject.SetActive(false);
        }
    }

    //*장바구니 초기화*
    private void ClearCart()
    {
        //ShopCartController가 없다면 처리할 수 없음
        if (shopCartController == null) return;

        //장바구니 전체 초기화
        shopCartController.ClearCart();
    }
}
