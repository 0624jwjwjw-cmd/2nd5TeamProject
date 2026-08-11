using UnityEngine;
using UnityEngine.UI;

public class SettingPenel : MonoBehaviour
{
    [Header("SettingPanel")]
    [SerializeField] private GameObject SettingPanel;
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
        SettingPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        SettingPanel.SetActive(false);
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
    }
}
