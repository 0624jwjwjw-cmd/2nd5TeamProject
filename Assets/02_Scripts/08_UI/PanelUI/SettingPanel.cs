using UnityEngine;
using UnityEngine.UI;

public class SettingPenel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject darkPanel;
    [Header("Sound")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        bgmSlider.value = SoundManager.Instance.BGMVolume;
        sfxSlider.value = SoundManager.Instance.SFXVolume;

        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
    }
    public void OpenPanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        settingPanel.SetActive(true);
        if (darkPanel != null)
        {
            darkPanel.SetActive(true);
        }
    }
    public void ClosePanel()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        settingPanel.SetActive(false);
        if (darkPanel != null)
        {
            darkPanel.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        bgmSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSFXVolume);
    }
    public void OnSFXSliderReleased()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    public void ResetGame()
    {
        SaveLoadManager.Instance.ResetGame();
        bgmSlider.value = SoundManager.Instance.BGMVolume;
        sfxSlider.value = SoundManager.Instance.SFXVolume;
        
    }
}
