//**ItemVisualRepository의 ID 기반 Sprite / Prefab 검색 기능을 테스트하는 스크립트**
using UnityEngine;

//같은 GameObject에 테스트 스크립트가 여러 개 붙는 것 방지
[DisallowMultipleComponent]

public class ItemVisualRepositoryTest : MonoBehaviour
{
    //Repository 연결
    //씬에 존재하는 ItemVisualRepository를 Inspector에서 연결
    [Header("테스트할 Visual Repository")]
    [SerializeField] private ItemVisualRepository repository;

    [Header("테스트용 ID")]
    [SerializeField] private string ingredientId = "IG_01";     //재료 테스트
    [SerializeField] private string dishId = "DS_01";           //일반 요리 테스트
    [SerializeField] private string specialDishId = "SD_01";    //특별 요리 테스트
    [SerializeField] private string invalidId = "TEST_999";     //존재하지 않는 ID 테스트

    //[전체 테스트]
    //컴포넌트 메뉴에서 직접 실행하기 위한 ContextMenu
    [ContextMenu("Visual Repository 전체 검색 테스트")]
    public void RunAllTests()
    {
        //Repository가 Inspector에서 연결되지 않았다면 테스트 불가능
        if (repository == null)
        {
            Debug.LogError("[ItemVisualRepositoryTest] Repository가 연결되지 않았습니다.");
            return;
        }

        //BootstrapManager가 Repository를 아직 초기화하지 않았다면
        //Dictionary가 준비되지 않은 상태이므로 테스트 중단
        if (!repository.IsInitialized)
        {
            Debug.LogError(
                "[ItemVisualRepositoryTest] " +
                "Repository가 아직 초기화되지 않았습니다. " +
                "Play 모드에서 실행해주세요."
            );
            return;
        }

        //모든 테스트가 성공했는지 마지막에 확인하기 위한 변수
        bool allPassed = true;

        //재료 Sprite / Prefab 테스트
        if (!TestVisual(ingredientId, "재료"))
        {
            allPassed = false;
        }

        //일반 요리 Sprite / Prefab 테스트
        if (!TestVisual(dishId, "일반 요리"))
        {
            allPassed = false;
        }

        //특별 요리 Sprite / Prefab 테스트
        if (!TestVisual(specialDishId, "특별 요리"))
        {
            allPassed = false;
        }

        //존재하지 않는 ID 검색 실패 테스트
        if (!TestInvalidId())
        {
            allPassed = false;
        }

        //모든 테스트가 성공했을 때만 PASS 출력
        if (allPassed)
        {
            Debug.Log("[ItemVisualRepositoryTest] TEST PASS");
        }
        else
        {
            Debug.LogError("[ItemVisualRepositoryTest] TEST FAIL");
        }
    }

    //[정상 아이템 Visual 테스트]
    //재료 / 일반 요리 / 특별 요리는
    //VisualRepository 입장에서는 모두
    //
    //ID → Sprite
    //ID → Prefab
    //으로 방식이 같기 때문에 하나의 공통 메서드로 테스트
    private bool TestVisual(string itemId, string categoryName)
    {
        //현재 테스트가 성공했는지 기록
        bool passed = true;

        //Sprite검색 테스트
        bool iconSuccess = repository.TryGetIcon(itemId, out Sprite icon);

        //검색 실패했거나 Sprite가 null이면 실패
        if (!iconSuccess || icon == null)
        {
            Debug.LogError(
                $"[ItemVisualRepositoryTest] " +
                $"{categoryName} Icon 검색 실패 | ID: {itemId}"
                );
            passed = false;
        }
        else
        {
            Debug.Log(
                $"[{categoryName} Icon 검색 성공] " +
                $"ID: {itemId}, " +
                $"Sprite: {icon.name}"
                );
        }

        //Prefab 검색 테스트
        bool prefabSuccess = repository.TryGetPrefab(itemId, out GameObject prefab);

        //검색에 실패했거나 Prefab이 null이면 실패
        if (!prefabSuccess || prefab == null)
        {
            Debug.LogError(
                $"[ItemVisualRepositoryTest] " +
                $"{categoryName} Prefab 검색 실패 | ID: {itemId}"
                );
            passed = false;
        }
        else
        {
            Debug.Log(
                $"[{categoryName} Prefab 검색 성공] " +
                $"ID: {itemId}, " +
                $"Prefab: {prefab.name}"
                );
        }
        return passed;
    }

    //[존재하지 않는 ID 테스트]
    private bool TestInvalidId()
    {
        //존재하지 않는 ID로 Sprite 검색
        bool iconSuccess = repository.TryGetIcon(invalidId, out Sprite icon);

        //없는 ID인데 검색에 성공하면 Repository 문제가 있는 것
        if (iconSuccess || icon != null)
        {
            Debug.LogError(
                $"[ItemVisualRepositoryTest] " +
                $"존재하지 않는 Icon이 검색되었습니다: {invalidId}"
                );
            return false;
        }

        //존재하지 않는 ID로 Prefab 검색
        bool prefabSuccess = repository.TryGetPrefab(invalidId, out GameObject prefab);

        if (prefabSuccess || prefab != null)
        {
            Debug.LogError(
                $"[ItemVisualRepositoryTest] " +
                $"존재하지 않는 Prefab이 검색되었습니다: {invalidId}"
                );
            return false;
        }

        //Icon과 Prefab 모두 정상적으로 검색 실패
        Debug.Log($"[잘못된 Visual ID검색 정상 실패] {invalidId}");

        return true;
    }
}
