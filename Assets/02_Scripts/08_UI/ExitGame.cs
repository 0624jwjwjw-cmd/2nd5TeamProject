using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        Application.Quit();
    }
}