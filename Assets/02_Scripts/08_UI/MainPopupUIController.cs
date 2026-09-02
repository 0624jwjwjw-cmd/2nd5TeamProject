//**거실씬에서 상점과 인벤토리 팝업의 열기/닫기를 공통으로 관리하는 스크립트**
using UnityEngine;
using UnityEngine.UI;   //Button 사용

//같은 GameObject에 MainPopupUIController가 여러 개 붙는 것을 방지
[DisallowMultipleComponent]
public class MainPopupUIController : MonoBehaviour
{
    //*공통 UI*
    [Header("공통 UI")]
    [SerializeField] private GameObject dimOverlay; //UI열렸을때 배경 어둡게 하는 애

    //*상점 UI*
    [Header("상점 UI")]
    [SerializeField] private Button shopButton;     //메인 화면의 상점 버튼

    //상점 버튼에 추가해둔 Canvas
    //팝업이 열렸을 때 Sort Order를 높여
    //DimOverlay보다 위에 보이도록 사용
    //(상점 눌러서 배경 어두워져도 버튼은 그대로)
    [SerializeField] private Canvas shopButtonCanvas;

    //상점 UI 전체를 담고 있는 Root
    //켜고 끄면서 상점 화면을 표시한다
    [SerializeField] private GameObject shopUIRoot;

    //상점 팝업 내부의 닫기 버튼
    [SerializeField] private Button shopCloseButton;

    //*인벤토리 UI*
    //메인 화면의 가방 버튼
    [Header("인벤토리 UI")]
    [SerializeField] private Button inventoryButton;

    //가방 버튼에 추가해둔 Canvas
    //인벤토리가 열렸을 때 DimOverlay보다 위로 올린다
    [SerializeField] private Canvas inventoryButtonCanvas;

    //인벤토리 UI 전체를 담고 있는 Root
    [SerializeField] private GameObject inventoryUIRoot;

    //인벤토리 팝업 내부의 닫기 버튼
    [SerializeField] private Button inventoryCloseButton;

    //*Sorting 설정*
    //평소 ShopButton / InventoryButton이 사용하는 Sort Order
    //현재 BottomPanel보다 앞에 보이기 위해 1을 사용
    [Header("Sorting")]
    [SerializeField] private int normalButtonSortOrder = 1;

    //현재 팝업을 열어둔 버튼의 Sort Order
    //DimOverlay(50), Popup(60)보다 높아야 함
    [SerializeField] private int selectedButtonSortOrder = 100;

    //게임 시작 시 아무것도 열려 있지 않음
    private PopupType currentPopup = PopupType.None;

    private void Awake()
    {
        //Inspector에서 연결한 버튼에 클릭 이벤트를 코드로 등록
        if (shopButton != null)
        {
            shopButton.onClick.AddListener(ToggleShop);
        }

        //상점 팝업 내부 닫기 버튼 이벤트 등록
        if (shopCloseButton != null)
        {
            shopCloseButton.onClick.AddListener(CloseShop);
        }

        if (inventoryButton != null)
        {
            inventoryButton.onClick.AddListener(ToggleInventory);
        }

        //인벤토리 팝업 내부 닫기 버튼 이벤트 등록
        if (inventoryCloseButton != null)
        {
            inventoryCloseButton.onClick.AddListener(CloseInventory);
        }

        //씬이 시작될 때 모든 팝업을 닫힌 상태로 맞춤
        SetInitialState();
    }

    private void OnDestroy()
    {
        //이 오브젝트가 제거될 때 등록했던 버튼 이벤트도 제거
        //중복 이벤트가 남는 걸 방지
        if (shopButton != null)
        {
            shopButton.onClick.RemoveListener(ToggleShop);
        }

        //상점 닫기 버튼 이벤트 제거
        if (shopCloseButton != null)
        {
            shopCloseButton.onClick.RemoveListener(CloseShop);
        }

        if (inventoryButton != null)
        {
            inventoryButton.onClick.RemoveListener(ToggleInventory);
        }

        //인벤토리 닫기 버튼 이벤트 제거
        if (inventoryCloseButton != null)
        {
            inventoryCloseButton.onClick.RemoveListener(CloseInventory);
        }
    }

    //*초기화*
    private void SetInitialState()
    {
        //어두운 배경 끄기
        if (dimOverlay != null)
        {
            dimOverlay.SetActive(false);
        }

        //상점 UI 끄기
        if (shopUIRoot != null)
        {
            shopUIRoot.SetActive(false);
        }

        //인벤토리 UI 끄기
        if (inventoryUIRoot != null)
        {
            inventoryUIRoot.SetActive(false);
        }

        //상점 버튼 Sorting을 평소 값으로 복구
        if (shopButtonCanvas != null)
        {
            shopButtonCanvas.sortingOrder = normalButtonSortOrder;
        }

        //가방 버튼 Sorting을 평소 값으로 복구
        if (inventoryButtonCanvas != null)
        {
            inventoryButtonCanvas.sortingOrder = normalButtonSortOrder;
        }

        //현재 열린 팝업 없음
        currentPopup = PopupType.None;
    }

    //*상점 버튼*
    private void ToggleShop()
    {
        //열린 상점을 다시 누른 경우
        if (currentPopup == PopupType.Shop)
        {
            PlayButtonClickSfx();
            CloseShop();
            return;
        }

        //아무 팝업도 없을 때만 상점을 열고 효과음 재생
        if (currentPopup == PopupType.None)
        {
            PlayButtonClickSfx();
            OpenShop();
        }
    }

    private void OpenShop()
    {
        //뒤쪽 화면을 어둡게 만듬
        if (dimOverlay != null)
        {
            dimOverlay.SetActive(true);
        }

        //상점 UI 표시
        if (shopUIRoot != null)
        {
            shopUIRoot.SetActive(true);
        }

        //상점 버튼만 DimOverlay보다 위로 올림(버튼은 안어둡게)
        if (shopButtonCanvas != null)
        {
            shopButtonCanvas.sortingOrder = selectedButtonSortOrder;
        }

        //현재 상점이 열려 있다고 기록
        currentPopup = PopupType.Shop;
    }

    private void CloseShop()
    {
        //상점 UI 숨김
        if (shopUIRoot != null)
        {
            shopUIRoot.SetActive(false);
        }

        //DimOverlay를 숨김(배경 다시 밝게)
        if (dimOverlay != null)
        {
            dimOverlay.SetActive(false);
        }

        //상점 버튼 Sorting을 원래 값으로 되돌림
        if (shopButtonCanvas != null)
        {
            shopButtonCanvas.sortingOrder = normalButtonSortOrder;
        }

        //현재 열린 팝업 없음
        currentPopup = PopupType.None;
    }

    //*인벤토리 버튼*
    private void ToggleInventory()
    {
        //열린 인벤토리를 다시 누른 경우
        if (currentPopup == PopupType.Inventory)
        {
            PlayButtonClickSfx();
            CloseInventory();
            return;
        }

        //아무 팝업도 없을 때만 인벤토리를 열고 효과음 재생
        if (currentPopup == PopupType.None)
        {
            PlayButtonClickSfx();
            OpenInventory();
        }
    }

    private void OpenInventory()
    {
        //뒤쪽 화면을 어둡게 만들기
        if (dimOverlay != null)
        {
            dimOverlay.SetActive(true);
        }

        //인벤토리 UI 표시
        if (inventoryUIRoot != null)
        {
            inventoryUIRoot.SetActive(true);
        }

        //가방 버튼만 DimOverlay보다 위로 올림(버튼은 안어둡게)
        if (inventoryButtonCanvas != null)
        {
            inventoryButtonCanvas.sortingOrder = selectedButtonSortOrder;
        }

        //현재 인벤토리가 열려 있다고 기록
        currentPopup = PopupType.Inventory;
    }


    private void CloseInventory()
    {
        //인벤토리 UI를 숨김
        if (inventoryUIRoot != null)
        {
            inventoryUIRoot.SetActive(false);
        }

        //DimOverlay를 숨긴다(다시 밝게ㄱㄱ)
        if (dimOverlay != null)
        {
            dimOverlay.SetActive(false);
        }

        //가방 버튼 Sorting을 원래 값으로 되돌림
        if (inventoryButtonCanvas != null)
        {
            inventoryButtonCanvas.sortingOrder = normalButtonSortOrder;
        }

        //현재 열린 팝업 없음
        currentPopup = PopupType.None;
    }

    //*일반 버튼 클릭 효과음 재생*
    private void PlayButtonClickSfx()
    {
        //SoundManager가 있는 경우에만
        //프로젝트 공통 버튼 효과음을 한 번 재생
        SoundManager.Instance?.PlaySFX(SFXType.ButtonClick);
    }
}
