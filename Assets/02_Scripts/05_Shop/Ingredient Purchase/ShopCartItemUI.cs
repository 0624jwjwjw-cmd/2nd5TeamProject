//**재료 상점 장바구니에서 재료 하나의 아이콘과 수량을 표시하고
//클릭 시 해당 재료의 장바구니 수량을 1 감소시키는 UI**
using TMPro;            //TMP_Text 사용
using UnityEngine;
using UnityEngine.UI;   //Image, Button 사용

public class ShopCartItemUI : MonoBehaviour
{
    //*UI*
    [SerializeField] private Image itemIconImage;   //현재 장바구니 재료의 아이콘을 표시하는 Image
    [SerializeField] private TMP_Text amountText;   //현재 장바구니에 담겨있는 재료 수량을 표시하는 TMP Text
    [SerializeField] private Button cartItemButton; //장바구니 슬롯 전체를 클릭하기 위한 Button

    //*데이터*
    //현재 이 ShopCartItemUI가 표시하고 있는 장바구니 데이터
    private ShopCartItemData cartItemData;

    //*장바구니 Controller*
    //현재 장바구니의 실제 데이터를 관리하는 Controller
    //
    //ShopCartItemUI가 직접 Amount를 수정하지 않고
    //ShopCartController에게 감소 요청을 전달하기 위해 사용
    private ShopCartController shopCartController;

    private void Awake()
    {
        //장바구니 슬롯 버튼이 Inspector에서 연결되지 않은 경우
        if (cartItemButton == null)
        {
            Debug.LogError("[ShopCartItemUI] CartItemButton이 연결되어 있지 않습니다.");
            return;
        }

        //장바구니 슬롯을 클릭했을 때 OnClickCartItem()이 실행되도록 이벤트 등록
        cartItemButton.onClick.AddListener(OnClickCartItem);

        //아직 어떤 장바구니 재료를 표시할지 모르기 때문에
        //Initialize()가 호출되기 전까지 버튼 비활성화
        cartItemButton.interactable = false;
    }

    private void OnDestroy()
    {
        //버튼이 존재하는 경우에만 실행
        if (cartItemButton != null)
        {
            //Awake()에서 등록했던 버튼 이벤트 제거
            cartItemButton.onClick.RemoveListener(OnClickCartItem);
        }
    }

    //*초기화*
    //이 ShopCartItemUI가
    //어떤 장바구니 재료를 표시할지 설정하는 메서드
    //
    //data
    //→ 표시할 ShopCartItemData
    //
    //cartController
    //→ 클릭했을 때 수량 감소 요청을 전달할 장바구니 Controller
    public void Initialize(ShopCartItemData data, ShopCartController cartController)
    {
        //전달받은 장바구니 데이터가 없는 경우
        if (data == null)
        {
            Debug.LogError("[ShopCartItemUI] ShopCartItemData가 null입니다.");
            return;
        }

        //ShopCartItemData 안의 IngredientData가 없는 경우
        if (data.Data == null)
        {
            Debug.LogError("[ShopCartItemUI] IngredientData가 null입니다.");
            return;
        }

        //장바구니 Controller가 없는 경우
        if (cartController == null)
        {
            Debug.LogError("[ShopCartItemUI] ShopCartController가 null입니다.");
            return;
        }

        //현재 이 UI가 담당할 장바구니 데이터 저장
        cartItemData = data;

        //수량 감소 요청을 전달할 Controller 저장
        shopCartController = cartController;

        //현재 장바구니 데이터를 이용해
        //아이콘과 수량을 화면에 표시
        RefreshUI();

        //정상적으로 초기화되었으므로
        //장바구니 슬롯 클릭 가능하도록 활성화
        if (cartItemButton != null)
        {
            cartItemButton.interactable = true;
        }
    }

    //*UI 갱신*
    //현재 ShopCartItemData를 기준으로
    //아이콘과 수량을 다시 표시
    public void RefreshUI()
    {
        //표시할 장바구니 데이터가 없다면 종료
        if (cartItemData == null) return;

        //장바구니 데이터 안에 IngredientData가 없다면 종료
        if (cartItemData.Data == null) return;

        //수량 표시
        if (amountText != null)
        {
            //예)
            //Amount = 3
            //화면: x3
            amountText.text = $"x{cartItemData.Amount}";
        }

        //아이콘 표시
        if (itemIconImage != null)
        {
            //아이콘 정보를 관리하는
            //ItemVisualRepository가 존재하는지 확인
            if (ItemVisualRepository.Instance == null)
            {
                Debug.LogError("[ShopCartItemUI] ItemVisualRepository가 존재하지 않습니다.");

                itemIconImage.sprite = null;

                return;
            }

            //현재 장바구니 재료의 ID를 이용해
            //ItemVisualRepository에서 아이콘 검색
            bool iconFound = ItemVisualRepository.Instance.TryGetIcon(cartItemData.Data.ID, out Sprite icon);

            //아이콘 검색 성공
            if (iconFound)
            {
                //찾은 Sprite를 Image에 적용
                itemIconImage.sprite = icon;
            }
            //아이콘 검색 실패
            else
            {
                //잘못된 이전 Sprite가 남지 않도록 비우기
                itemIconImage.sprite = null;

                Debug.LogWarning(
                    $"[ShopCartItemUI] 아이콘을 찾을 수 없습니다. " +
                    $"ID: {cartItemData.Data.ID}"
                    );
            }
        }
    }

    //*장바구니 재료 클릭*
    //사용자가 장바구니에 표시된 재료를 클릭했을 때 실행
    //
    //예)
    //빵 x3 → 클릭 → 빵 x2
    private void OnClickCartItem()
    {
        //현재 슬롯이 어떤 재료인지 모르는 경우
        if (cartItemData == null || cartItemData.Data == null)
        {
            Debug.LogWarning("[ShopCartItemUI] 감소시킬 장바구니 데이터가 없습니다.");
            return;
        }

        //장바구니 Controller가 없는 경우
        if (shopCartController == null)
        {
            Debug.LogError("[ShopCartItemUI] ShopCartController가 연결되어 있지 않습니다.");
            return;
        }

        //현재 재료 ID 저장
        string itemId = cartItemData.Data.ID;

        //로그에서 사용할 재료 이름 저장
        string ingredientName = cartItemData.Data.IngredientName;

        //ShopCartController에게 현재 재료 수량을 1 감소해달라고 요청
        bool removeSucceeded = shopCartController.RemoveIngredient(itemId);

        //수량 감소 실패
        if (!removeSucceeded)
        {
            Debug.LogWarning(
                $"[ShopCartItemUI] 장바구니 수량 감소 실패 | " +
                $"{ingredientName}"
                );
            return;
        }
    }
}
