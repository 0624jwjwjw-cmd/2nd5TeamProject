using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "RE_",menuName ="GameData/Upgrade/Reciepe")]
public class ReciepePurchaseData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string reciepeName;
    [SerializeField] private string foodID;
    [SerializeField] private string grade;
    [SerializeField] private int price;
    [SerializeField] private string info;
    [SerializeField] private string reciepeInfo;
    [SerializeField] private bool isUnlocked;

    public string ID => id;
    public string ReciepeName => reciepeName;
    public string FoodID => foodID;
    public string Grade => grade;
    public int Price => price;
    public string Info => info;
    public string ReciepeInfo => reciepeInfo;
    public bool IsUnlocked => isUnlocked;

    public void SetData(string id, string reciepeName, string foodID, string grade, int price, string info, string reciepeInfo)
    {
        this.id = id;
        this.reciepeName = reciepeName;
        this.foodID = foodID;
        this.grade = grade;
        this.price = price;
        this.info = info;
        this.reciepeInfo = reciepeInfo;
    }
    public void UnlockReciepe(bool isUnlocked)
    {
        this.isUnlocked = isUnlocked;
    }

}
