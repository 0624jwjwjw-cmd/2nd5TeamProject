using UnityEngine;

public class SaveTest : MonoBehaviour
{
    public void AddGold()
    {
        CalculateGold.GetDonation(4500, 2);
    }


    public void SpendGold()
    {
        CurrencyManager.Instance.SpendGold(5000);
    }


    public void AddHeart()
    {
        CurrencyManager.Instance.AddHeart(1);
    }


    public void SpendHeart()
    {
        HeartManager.Instance.UseHeart();
    }

    public void AddDate()
    {
        GameDateManager.Instance.AddDateCount();
    }
    public void SoundLive()
    {
        SoundManager.Instance.PlayBGM(BGMType.Studio);
    }
}