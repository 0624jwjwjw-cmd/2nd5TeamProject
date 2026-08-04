using System;

[Serializable]
public class SaveData
{
    // 재화
    public int gold;
    public int heart;
    public int subscriber;
    // 마지막 하트 회복 시간
    public long lastHeartRecoverTime;

    // 업그레이드
    public int kitchenLevel;
    public int studioLevel;
    public int recipeLevel;

    //인벤토리 안에 음식,재료들
    public int[] foodInventory;
    public int[] ingredientsInventory;
}
