using UnityEngine;

public class SoundManager : MonoBehaviour, ISaveable
{
    public static SoundManager Instance;


    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;


    [Header("BGM")]
    [SerializeField] private AudioClip mainBGM;
    [SerializeField, Range(0f, 1f)] private float bgmBaseVolume = 1f;

    [Header("SFX")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField, Range(0f, 1f)] private float buttonClickVolume = 1f;
    [SerializeField] private AudioClip cooking;
    [SerializeField, Range(0f, 1f)] private float cookingVolume = 1f;
    [SerializeField] private AudioClip coin;
    [SerializeField, Range(0f, 1f)] private float coinVolume = 1f;
    [SerializeField] private AudioClip heart;
    [SerializeField, Range(0f, 1f)] private float heartVolume = 1f;
    [SerializeField] private AudioClip win;
    [SerializeField, Range(0f, 1f)] private float winVolume = 1f;
    [SerializeField] private AudioClip lose;
    [SerializeField, Range(0f, 1f)] private float loseVolume = 1f;

    [Header("SettingVolume")]
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            bgmSource.volume = bgmVolume;
            sfxSource.volume = sfxVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        PlayBGM(mainBGM);
    }


    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }


    public void PlaySFX(SFXType type)
    {
        AudioClip clip = null;
        float baseVolume = 1f;

        switch (type)
        {
            case SFXType.ButtonClick:
                clip = buttonClick;
                baseVolume = buttonClickVolume;
                break;
            case SFXType.Cooking:
                clip = cooking;
                baseVolume = cookingVolume;
                break;
            case SFXType.Coin:
                clip = coin;
                baseVolume = coinVolume;
                break;
            case SFXType.Heart:
                clip = heart;
                baseVolume = heartVolume;
                break;
            case SFXType.Win:
                clip = win;
                baseVolume = winVolume;
                break;
            case SFXType.Lose:
                clip = lose;
                baseVolume = loseVolume;
                break;
        }
        if (clip == null)
        {
            Debug.LogWarning($"등록되지 않은 SFX : {type}");
            return;
        }
        float finalVolume = baseVolume * sfxVolume;
        sfxSource.PlayOneShot(clip, finalVolume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmBaseVolume * bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }
    private void ApplyVolume()
    {
        bgmSource.volume = bgmBaseVolume * bgmVolume;
    }

    public void Save(SaveData data)
    {
        data.bgmVolume = bgmVolume;
        data.sfxVolume = sfxVolume;
    }
    public void Load(SaveData data)
    {
        bgmVolume = data.bgmVolume;
        sfxVolume = data.sfxVolume;

        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
         ApplyVolume();
    }
}