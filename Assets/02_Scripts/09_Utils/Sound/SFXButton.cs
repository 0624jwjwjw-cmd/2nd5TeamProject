using UnityEngine;

public class SFXButton : MonoBehaviour
{
    public void ClickSound()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    public void CookSound()
    {
        SoundManager.Instance.PlaySFX(SFXType.Cooking);
    }
    public void CoinSound()
    {
        SoundManager.Instance.PlaySFX(SFXType.Coin);
    }
    public void HeartSound()
    {
        SoundManager.Instance.PlaySFX(SFXType.Heart);
    }
    public void WinSound()
    {
        SoundManager.Instance.PlaySFX(SFXType.Win);
    }
    public void LoseSound()
    {
        SoundManager.Instance.PlaySFX(SFXType.Lose);
    }

}
