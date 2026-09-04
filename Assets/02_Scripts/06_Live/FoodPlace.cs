//**스튜디오의 접시 한 칸에 배치된 음식 정보와 화면 표시를 관리**
using UnityEngine;
using UnityEngine.UI; //Image 사용

public class FoodPlace : MonoBehaviour
{
    //*UI 참조*
    //이 접시에 놓인 음식 이미지를 표시할 자식 Image
    [SerializeField] private Image _foodImage;

    //*현재 배치된 음식 정보*
    //ItemId가 비어 있지 않으면 현재 접시에 음식이 있다는 뜻임
    public bool IsFilled => !string.IsNullOrWhiteSpace(ItemId);

    //현재 접시에 배치된 음식 ID
    public string ItemId { get; private set; }

    private void Awake()
    {
        //게임 시작 시 접시에 남아 있는 음식 표시 제거
        HideFoodImage();
    }

    public bool TryPlace(string itemId)
    {
        //이미 음식이 놓인 접시라면 추가로 배치하지 않음
        if (IsFilled) return false;

        //전달받은 음식 ID가 비어 있다면 배치하지 않음
        if (string.IsNullOrWhiteSpace(itemId)) return false;

        //음식을 표시할 Image가 연결되지 않았다면 배치 불가능
        if (_foodImage == null)
        {
            return false;
        }

        //음식 아이콘을 관리하는 Repository가 존재하는지 확인
        if (ItemVisualRepository.Instance == null)
        {
            return false;
        }

        //ItemId에 해당하는 음식 Sprite 검색
        bool foundIcon = ItemVisualRepository.Instance.TryGetIcon(itemId, out Sprite foodSprite);

        //등록된 음식 Sprite를 찾지 못했다면 배치하지 않음
        if (!foundIcon || foodSprite == null)
        {
            return false;
        }

        //현재 접시에 배치된 음식 ID 저장
        ItemId = itemId;

        //찾은 음식 Sprite를 자식 FoodImage에 적용
        _foodImage.sprite = foodSprite;

        //원본 음식 이미지 비율 유지
        _foodImage.preserveAspect = true;

        //숨겨져 있던 음식 Image 표시
        _foodImage.enabled = true;

        return true;
    }

    //*접시에서 음식 제거*
    public void RemoveFood()
    {
        ItemId = null;      //현재 배치된 음식 ID 제거
        HideFoodImage();    //접시 위 음식 이미지 숨김
    }

    //*외부에서 접시 전체를 초기화할 때 사용*
    public void Clear()
    {
        RemoveFood();
    }

    //*음식 Image를 비우고 숨김*
    private void HideFoodImage()
    {
        //FoodImage가 연결되지 않았다면 처리할 작업 없음
        if (_foodImage == null) return;        

        //이전에 표시하던 음식 Sprite 제거
        _foodImage.sprite = null;

        //빈 Image가 하얀 사각형으로 보이지 않도록 비활성화
        _foodImage.enabled = false;
    }
}