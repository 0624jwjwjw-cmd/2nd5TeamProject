//**Component를 상속받는 객체를 재사용하기 위한 공용 Generic Object Pool**
using System;
using System.Collections.Generic;
using UnityEngine;

//사용 예:
//InventorySlotUI  → 인벤토리 슬롯
//IngredientBase   → 요리 재료
//DishBase         → 완성 요리
//ChatMessageUI    → 라이브 채팅 UI
//
//각 시스템마다 풀링 코드를 새로 만드는 것이 아니라
//Get / Release / 생성 / 보관 같은 공통 기능을 이 클래스에서 재사용

//ComponentPool을 사용할 때 실제 타입 T가 뭔지는 나중에 정함, 대신 Unity의 Component를 상속한 타입만 받음 
public sealed class ComponentPool<T> where T : Component
{
    //새로운 객체가 필요할 때 생성할 원본 Prefab
    private readonly T prefab;

    //현재 사용하지 않는 객체들을 보관할 부모
    private readonly Transform poolRoot;

    //현재 사용하지 않고 Pool에 대기 중인 객체들
    //가장 최근에 반환된 객체를 가장 먼저 다시 사용할 수 있도록 Stack 사용
    private readonly Stack<T> availableObjects = new Stack<T>();

    //현재 Pool 안에 반환되어 대기 중인 객체들을 기록
    //같은 객체가 Release()로 두 번 들어오는 것을 방지하기 위해 사용
    //Stack:    실제 Get / Release 순서 관리
    //HashSet:  이 객체가 이미 Pool 안에 들어있는지 빠르게 확인
    private readonly HashSet<T> pooledObjects = new HashSet<T>();

    //이 Pool이 직접 생성한 모든 객체를 기록
    //다른 Pool의 객체가 잘못 Release되는 것을 방지하기 위해 사용
    private readonly HashSet<T> ownedObjects = new HashSet<T>();

    //*공용 Pool 생성*
    public ComponentPool(T prefab, Transform poolRoot)
    {
        //생성할 원본 Prefab이 없다면
        //Pool 자체가 정상적으로 동작할 수 없으므로 즉시 오류 처리
        if (prefab == null) throw new ArgumentNullException(nameof(prefab));

        //사용하지 않는 객체들을 보관할 부모가 없다면
        //Pool 내부 객체를 정리해서 관리할 수 없으므로 즉시 오류 처리
        if (poolRoot == null) throw new ArgumentNullException(nameof(poolRoot));

        //외부에서 전달받은 Prefab을 이 Pool이 계속 사용할 수 있도록 저장
        this.prefab = prefab;

        //사용하지 않는 객체들이 돌아갈 Pool 전용 부모 Transform 저장
        this.poolRoot = poolRoot;
    }

    //*새 Pool 객체 생성*
    private T Create()
    {
        //저장해둔 Prefab을 기준으로 새로운 객체를 Pool 전용 부모 아래에 생성
        T newObject = UnityEngine.Object.Instantiate(prefab, poolRoot);

        //이 Pool이 직접 생성한 객체라는 것을 기록
        //나중에 Release()할 때 다른 Pool에서 만들어진 객체가 잘못 들어오는 것을 구분하기 위해 사용
        ownedObjects.Add(newObject);

        //새로 생성된 객체의 초기 상태를 비활성화로 통일 Get()에서 사용 위치를 지정한 뒤 활성화함
        newObject.gameObject.SetActive(false);

        //생성된 객체 반환
        return newObject;
    }

    //*Pool에서 사용할 객체 가져오기*
    public T Get(Transform parent = null)
    {
        T targetObject = null;

        //Pool에 대기 중인 객체가 있다면
        //정상적인 객체를 찾을 때까지 하나씩 확인
        while (availableObjects.Count > 0)
        {
            targetObject = availableObjects.Pop();

            //Stack에서 꺼냈으므로
            //현재 Pool 내부에 있다는 기록에서도 제거
            pooledObjects.Remove(targetObject);

            //외부에서 Destroy된 객체라면
            //더 이상 이 Pool이 관리할 수 없는 객체이므로
            //소유 목록에서도 제거하고 다음 대기 객체를 확인
            if (targetObject == null)
            {
                ownedObjects.Remove(targetObject);
                continue;
            }

            //정상적으로 살아있는 객체라면 이 객체를 재사용하기 위해 반복 종료
            break;
        }

        //재사용 가능한 객체가 하나도 없다면 새로운 객체 생성
        if (targetObject == null)
        {
            targetObject = Create();
        }

        //생성까지 실패한 예외 상황 방어
        if (targetObject == null) return null;

        //사용 위치가 지정되어 있다면
        //해당 부모 아래로 이동
        if (parent != null)
        {
            targetObject.transform.SetParent(parent, false);
        }

        //실제로 사용할 수 있도록 활성화
        targetObject.gameObject.SetActive(true);

        return targetObject;
    }

    //*사용이 끝난 객체를 Pool에 반환*
    public void Release(T targetObject)
    {
        //반환하려는 객체가 없다면
        //Pool에 넣을 대상도 없으므로 아무 작업 없이 종료
        if (targetObject == null) return;

        //이 Pool이 직접 생성한 객체가 아니라면
        //잘못된 Pool로 반환하려는 것이므로 반환을 거부
        if (!ownedObjects.Contains(targetObject))
        {
            Debug.LogWarning(
                $"[ComponentPool<{typeof(T).Name}>] " +
                $"이 Pool이 생성하지 않은 객체는 반환할 수 없습니다: {targetObject.name}"
                );

            return;
        }

        //이미 Pool 안에 들어있는 객체라면
        //같은 객체를 Stack에 두 번 넣지 않도록 반환 중단
        if (!pooledObjects.Add(targetObject))
        {
            Debug.LogWarning(
                $"[ComponentPool<{typeof(T).Name}>] " +
                $"이미 Pool에 반환된 객체입니다: {targetObject.name}"
                );

            return;
        }

        //다음에 다시 꺼내기 전까지 비활성화
        targetObject.gameObject.SetActive(false);

        //실제 사용 위치에서 Pool 전용 보관 위치로 다시 이동
        targetObject.transform.SetParent(poolRoot, false);

        //사용이 끝난 객체를 Stack에 넣어
        //다음 Get() 호출 때 다시 사용할 수 있도록 보관
        availableObjects.Push(targetObject);
    }
}

/*
 Create()
→ Prefab 생성
→ ownedObjects에 소유권 기록

Get()
→ Stack에 대기 객체가 있으면 Pop
→ pooledObjects에서 대기 상태 제거
→ 파괴된 객체면 건너뜀
→ 없으면 Create()
→ 사용할 부모로 이동
→ 활성화

Release()
→ null 검사
→ 내 Pool이 만든 객체인지 검사
→ 이미 반환된 객체인지 검사
→ 비활성화
→ PoolRoot로 이동
→ Stack에 Push
 */