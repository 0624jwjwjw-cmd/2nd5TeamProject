using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // 재화
    public int gold = 5000;
    public int heart = 10;
    public int subscriber = 0;
    // 마지막 하트 회복 시간
    public long lastHeartRecoverTime = 0;

    //퀘스트 데이터
    public int questStartGold = 5000;

    //날짜
    public int dateCount = 0;

    // 사운드 볼륨
    public float bgmVolume = 0.5f;
    public float sfxVolume = 0.5f;

    // 업그레이드
    public int kitchenLevel = 0;
    public int studioLevel = 0;
    public int inventoryLevel = 0;
    //public List<string> unlockedRecipeIDs = new();
    public HashSet<string> unlockedRecipeIDs = new();


    //인벤토리 안에 음식,재료들
    public List<InventorySlotData> inventory = new();
}
