//**게임 실행 중 ID를 이용해 재료와 요리 데이터를 검색하는 Repository**
using System;                       //StringComparer 사용
using System.Collections.Generic;   //Dictionary와 HashSet 사용
using UnityEngine;

//같은 게임 오브젝트에 해당 스크립트가 여러개 붙는거 방지
[DisallowMultipleComponent]

public class GameDataRepository : MonoBehaviour//, IGameDataRepository, IInitializable
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
