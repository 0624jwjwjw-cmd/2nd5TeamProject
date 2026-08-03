using UnityEngine;

[CreateAssetMenu(fileName = "ST_", menuName ="GameData/Upgrade/Studio")]
public class StudioUpgradeData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private int level;
    [SerializeField] private int price;
    [SerializeField] private float subscriberBonus;

    public string ID => id;
    public int Level => level;
    public int Price => price;
    public float SubscriberBonus => subscriberBonus;

    public void SetData(string id, int level, int price, float subscriberBonus)
    {
        this.id = id;
        this.level = level;
        this.price = price;
        this.subscriberBonus = subscriberBonus;
    }
}
