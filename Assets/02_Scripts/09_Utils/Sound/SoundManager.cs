using UnityEngine;

public class SoundManager : MonoBehaviour, ISaveable
{
    public static SoundManager Instance;


    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;


    [Header("BGM")]
    [SerializeField] private AudioClip mainBGM;


    [Header("SFX")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip cooking;
    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip heart;

    [Header("Volume")]
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
        AudioClip clip = type switch
        {
            SFXType.ButtonClick => buttonClick,
            SFXType.Cooking => cooking,
            SFXType.Coin => coin,
            SFXType.Heart => heart,
            _ => null
        };


        if (clip == null)
        {
            Debug.LogWarning($"등록되지 않은 SFX : {type}");
            return;
        }


        sfxSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
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

    }
}