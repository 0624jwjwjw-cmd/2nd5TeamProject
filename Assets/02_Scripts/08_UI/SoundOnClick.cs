using UnityEngine;
using UnityEngine.UI;

public class SoundOnClick : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {

        bgmSlider.value = SoundManager.Instance.BGMVolume;
        sfxSlider.value = SoundManager.Instance.SFXVolume;

        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
    }

    private void OnDestroy()
    {
        bgmSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSFXVolume);
    }
}
