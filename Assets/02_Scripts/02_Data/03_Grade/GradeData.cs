using UnityEngine;

[CreateAssetMenu(fileName = "BT_", menuName = "GameData/Grade")]
public class GradeData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string gradeName;
    [SerializeField] private int requiredSubscribers;
    [SerializeField] private float donationBonus;

    public string ID => id;
    public string GradeName => gradeName;
    public int RequiredSubscribers => requiredSubscribers;
    public float DonationBonus => donationBonus;

    public void SetData(string id, string gradeName, int requiredSubscribers, float donationBonus)
    {
        this.id = id;
        this.gradeName = gradeName;
        this.requiredSubscribers = requiredSubscribers;
        this.donationBonus = donationBonus;
    }
}
