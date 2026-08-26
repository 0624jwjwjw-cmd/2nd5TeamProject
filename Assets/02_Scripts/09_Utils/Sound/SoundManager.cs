using UnityEngine;
using UnityEngine.SceneManagement;
public enum BGMType
{
    Normal,
    Kitchen,
    Studio
}
public class SoundManager : MonoBehaviour, ISaveable
{
    public static SoundManager Instance;


    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;


    [Header("BGM")]
    [SerializeField] private AudioClip mainBGM;
    [SerializeField, Range(0f, 1f)] private float mainBgmBaseVolume = 1f;
    [SerializeField] private AudioClip kitchenBGM;
    [SerializeField, Range(0f, 1f)] private float kitchenBgmBaseVolume = 1f;
    [SerializeField] private AudioClip studioBGM;
    [SerializeField, Range(0f, 1f)] private float studioBgmBaseVolume = 1f;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }


    private void Start()
    {
        PlaySceneBGM(SceneManager.GetActiveScene().name);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneBGM(scene.name);
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    public void PlayBGM(BGMType type)
    {
        AudioClip clip = null;
        float baseVolume = 1f;

        switch (type)
        {
            case BGMType.Normal:
                clip = mainBGM;
                baseVolume = mainBgmBaseVolume;
                break;

            case BGMType.Kitchen:
                clip = kitchenBGM;
                baseVolume = kitchenBgmBaseVolume;
                break;

            case BGMType.Studio:
                clip = studioBGM;
                baseVolume = studioBgmBaseVolume;
                break;
        }

        if (clip == null)
            return;

        // 이미 같은 BGM이면 재생하지 않음
        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            bgmSource.volume = baseVolume * bgmVolume;
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = baseVolume * bgmVolume;
        bgmSource.Play();
    }

    private void PlaySceneBGM(string sceneName)
    {
        switch (sceneName)
        {
            case "00_Title":
            case "01_Main_Living":
            case "02_Studio":
                PlayBGM(BGMType.Normal);
                break;

            case "03_Kitchen":
                PlayBGM(BGMType.Kitchen);
                break;
        }
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
        bgmSource.volume = GetCurrentBGMBaseVolume() * bgmVolume;
    }
    private float GetCurrentBGMBaseVolume()
    {
        if (bgmSource.clip == mainBGM)
            return mainBgmBaseVolume;

        if (bgmSource.clip == kitchenBGM)
            return kitchenBgmBaseVolume;

        if (bgmSource.clip == studioBGM)
            return studioBgmBaseVolume;

        return 1f;
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }
    //private void ApplyVolume()
    //{
    //    bgmSource.volume = mainBgmBaseVolume * bgmVolume;
    //}

    public void Save(SaveData data)
    {
        data.bgmVolume = bgmVolume;
        data.sfxVolume = sfxVolume;
    }
    public void Load(SaveData data)
    {
        bgmVolume = data.bgmVolume;
        sfxVolume = data.sfxVolume;

        bgmSource.volume = GetCurrentBGMBaseVolume() * bgmVolume;
        sfxSource.volume = sfxVolume;
    }
}