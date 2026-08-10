//**GameDataRepository의 ID 검색 기능을 테스트하는 스크립트**
using UnityEngine;

//같은 게임 오브젝트에 해당 스크립트가 여러개 붙는거 방지
[DisallowMultipleComponent]

public sealed class GameDataRepositoryTest : MonoBehaviour
{
    [Header("테스트할 Repository")]
    [SerializeField] private GameDataRepository repository; //테스트 씬에 배치된 GameDataRepository 연결

    [Header("테스트용 ID")]
    [SerializeField] private string ingredientId = "IG_01";     //재료 검색 테스트용 ID
    [SerializeField] private string dishId = "DS_01";           //일반 요리 검색 테스트용 ID
    [SerializeField] private string specialDishId = "SD_01";    //특별 요리 검색 테스트용 ID
    [SerializeField] private string invalidId = "TEST_999";     //존재하지 않는 ID 검색 실패 테스트용 ID

    //[모든 검색 테스트 한 번에 실행]
    [ContextMenu("Repository 전체 검색 테스트")]
    public void RunAllTests()
    {
        //Repository가 Inspector에 연결되어 있지 않으면 테스트 불가능
        if (repository == null)
        {
            Debug.LogError("[GameDataRepositoryTest] Repository가 연결되지 않았습니다.");
            return;
        }

        //Reposity 초기화가 끝나지 않았다면 Dictionary가 아직 준비되지 않은 상태이므로 테스트하지 않음
        if (!repository.IsInitialized)
        {
            Debug.LogError("[GameDataRepositoryTest] Repository가 아직 초기화되지 않았습니다. " + "Play 모드에서 실행 해주세요.");
            return;
        }

        TestIngredient();   //재료 검색 테스트
        TestDish();         //일반 요리 검색 테스트
        TestSpecialDish();  //특별 요리 검색 테스트
        TestInvalidId();    //존재하지 않는 ID 검색 테스트

        Debug.Log("[GameDataRepositoryTest] 모든 Repository 검색 테스트 완료");
    }
    
    //[IG_ ID로 재료 데이터가 정상 검색되는지 확인]
    private void TestIngredient()
    {
        //재료 Dictionary에서 검색
        bool success =
            repository.TryGetIngredient(
                ingredientId,
                out IngredientData ingredientData
            );

        //검색에 실패 시 오류 출력
        if (!success || ingredientData == null)
        {
            Debug.LogError(
                $"[GameDataRepositoryTest] " +
                $"재료 검색 실패: {ingredientId}"
            );

            return;
        }

        //검색된 재료 정보 출력
        Debug.Log(
            $"[재료 검색 성공] " +
            $"ID: {ingredientData.ID}, " +
            $"이름: {ingredientData.IngredientName}, " +
            $"가격: {ingredientData.Price}"
        );
    }

    //[DS_ ID로 일반 요리가 정상 검색되는지 확인]
    private void TestDish()
    {
        //일반 요리 Dictionary에서 검색
        bool success =
            repository.TryGetDish(
                dishId,
                out DishData dishData
            );

        //검색 실패시 오류 출력
        if (!success || dishData == null)
        {
            Debug.LogError(
                $"[GameDataRepositoryTest] " +
                $"일반 요리 검색 실패: {dishId}"
            );

            return;
        }

        //검색된 요리 정보 출력
        Debug.Log(
            $"[일반 요리 검색 성공] " +
            $"ID: {dishData.ID}, " +
            $"이름: {dishData.DishName}, " +
            $"원가: {dishData.Cost}"
        );
    }

    //[SD_ ID로 특별 요리 데이터가 정상 검색되는지 확인]
    private void TestSpecialDish()
    {
        //특별 요리 Dictionary에서 검색
        bool success =
            repository.TryGetSpecialDish(
                specialDishId,
                out DishData specialDishData
            );

        //검색 실패시 오류 출력
        if (!success || specialDishData == null)
        {
            Debug.LogError(
                $"[GameDataRepositoryTest] " +
                $"특별 요리 검색 실패: {specialDishId}"
            );

            return;
        }

        //검색된 요리 정보 출력
        Debug.Log(
            $"[특별 요리 검색 성공] " +
            $"ID: {specialDishData.ID}, " +
            $"이름: {specialDishData.DishName}, " +
            $"원가: {specialDishData.Cost}"
        );
    }

    //[존재하지 않는 ID가 잘못 검색되지 않는지 확인]
    private void TestInvalidId()
    {
        //존재하지 않는 ID로 재료 검색 시도
        bool success = repository.TryGetIngredient(invalidId, out IngredientData ingredientData);

        //존재하지 않는 ID가 검색되었을때 에러 출력
        if (success || ingredientData != null)
        {
            Debug.LogError($"[GameDataRepositoryTest] " + $"존재하지 않는 ID가 검색되었습니다: {invalidId}");
            return;
        }

        //false가 반환돼야 정상
        Debug.Log($"[잘못된 ID 검색 정상 실패] {invalidId}");
    }
}
