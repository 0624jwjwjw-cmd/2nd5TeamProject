//**상점에서 재료 하나의 정보를 표시하고
//ShopItem 전체 클릭 시 해당 재료를 장바구니에 추가하도록 전달하는 UI**
using System;           //Action 사용
using TMPro;            //TMP_Text 사용
using UnityEngine;      
using UnityEngine.UI;   //Image, Button 사용

public class ShopItemUI : MonoBehaviour
{
    //*UI*
    [SerializeField] private Image itemIconImage;       //현재 재료의 아이콘을 표시하는 Image
    [SerializeField] private TMP_Text itemNameText;     //현재 재료의 이름을 표시하는 TMP Text
    [SerializeField] private TMP_Text priceText;        //현재 재료의 가격을 표시하는 TMP Text
    [SerializeField] private Button addButton;          //현재 재료를 장바구니에 추가할 때 사용

    //*데이터*
    //현재 이 ShopItemUI가 담당하고 있는 재료 데이터
    //Inspector에서 직접 넣지 않고
    //ShopUIManager가 Initialize()를 통해 전달해줄 예정
    private IngredientData ingredientData;

    //*이벤트*
    //ShopItem을 클릭했을 때 실행할 기능
    //클릭된 IngredientData를 외부 시스템에 전달하기 위해 사용
    private Action<IngredientData> onItemClicked;

    //ShopItem에 사용할 재료 데이터와
    //클릭했을 때 실행할 기능을 전달받아 초기화하는 메서드
    public void Initialize(IngredientData data, Sprite icon, Action<IngredientData> clickAction)
    {
        ingredientData = data;          //전달받은 재료 데이터를 현재 ShopItem의 데이터로 저장
        onItemClicked = clickAction;    //ShopItem 클릭 시 실행할 기능을 저장

        //UI 갱신
        itemIconImage.sprite = icon;                        //현재 재료의 아이콘을 ItemIcon에 표시
        itemNameText.text = ingredientData.IngredientName;  //현재 재료의 이름을 ItemNameText에 표시
        priceText.text = $"{ingredientData.Price}원";        //현재 재료의 가격을 표시

        //클릭 이벤트 연결
        //Initialize()가 여러 번 호출될 경우
        //같은 이벤트가 중복 등록되는 것을 방지
        addButton.onClick.RemoveListener(OnAddButtonClicked);

        //ShopItem 전체를 클릭하면
        //OnAddButtonClicked()가 실행되도록 등록
        addButton.onClick.AddListener(OnAddButtonClicked);
    }

    //ShopItem 전체를 클릭했을 때 실행되는 메서드
    private void OnAddButtonClicked()
    {
        //현재 ShopItem이 가지고 있는 IngredientData를
        //장바구니를 관리하는 외부 시스템으로 전달
        onItemClicked?.Invoke(ingredientData);
    }

    //이 ShopItem GameObject가 파괴될 때 호출
    private void OnDestroy()
    {
        //Button에 등록한 이벤트 제거
        //파괴된 객체에 대한 불필요한 이벤트 참조가 남는 것을 방지
        addButton.onClick.RemoveListener(OnAddButtonClicked);
    }
}
