using UnityEngine;

public class SaveTest : MonoBehaviour
{
    public void AddGold()
    {
        CalculateGold.GetDonation(100, 2);
    }


    public void SpendGold()
    {
        CurrencyManager.Instance.SpendGold(100);
    }


    public void AddHeart()
    {
        CurrencyManager.Instance.AddHeart(1);
    }


    public void SpendHeart()
    {
        CurrencyManager.Instance.SpendHeart(1);
    }


    public void AddSubscriber()
    {
        CurrencyManager.Instance.AddSubscriber(100);
    }
}