using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectPanel : MonoBehaviour
{
    [SerializeField] private LiveManager liveManager;
    [SerializeField] private Image _characterImage;
    [SerializeField] private Image _characterDisplay;

    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _applyButton;

    [SerializeField] private TMP_Text _characterNameText;
    [SerializeField] private TMP_Text _characterDescriptionText;

    [SerializeField] private Sprite[] _characterSprites;
    [SerializeField] private string[] _characterNames;
    [SerializeField] private string[] _characterDescriptions;
    private const string CharacterIndexKey = "SelectedCharacterIndex";
    private int _currentIndex;
    public int CurrentIndex => _currentIndex;
    private void OnEnable()
    {
        _currentIndex = PlayerPrefs.GetInt(CharacterIndexKey, 0);
        UpdateCharacter();
    }

    private void Start()
    {
        _prevButton.onClick.AddListener(ShowPrevious);
        _nextButton.onClick.AddListener(ShowNext);
        _closeButton.onClick.AddListener(ClosePanel);
        _applyButton.onClick.AddListener(ApplyCharacter);
        ApplySavedCharacter();
        gameObject.SetActive(false);
    }
    private void ApplySavedCharacter()
    {
        if (_characterSprites == null || _characterSprites.Length == 0)
            return;

        int savedIndex = PlayerPrefs.GetInt(CharacterIndexKey, 0);

        if (savedIndex < 0 || savedIndex >= _characterSprites.Length)
            savedIndex = 0;

        _currentIndex = savedIndex;

        _characterDisplay.gameObject.SetActive(true);
        _characterDisplay.sprite = _characterSprites[_currentIndex];
    }
    private void ShowPrevious()
    {
        if (_currentIndex <= 0)
            return;

        _currentIndex--;

        SoundManager.Instance?.PlaySFX(SFXType.ButtonClick);

        UpdateCharacter();
    }

    private void ShowNext()
    {
        if (_currentIndex >= _characterSprites.Length - 1)
            return;

        _currentIndex++;

        SoundManager.Instance?.PlaySFX(SFXType.ButtonClick);

        UpdateCharacter();
    }

    private void UpdateCharacter()
    {
        if (_characterSprites.Length == 0)
            return;

        _characterImage.sprite = _characterSprites[_currentIndex];

        if (_currentIndex < _characterNames.Length)
            _characterNameText.text = _characterNames[_currentIndex];

        if (_currentIndex < _characterDescriptions.Length)
            _characterDescriptionText.text =
                _characterDescriptions[_currentIndex];

        _prevButton.gameObject.SetActive(_currentIndex > 0);

        _nextButton.gameObject.SetActive(
            _currentIndex < _characterSprites.Length - 1
        );
    }

    public void ApplyCharacter()
    {
        if (_characterSprites.Length == 0)
            return;

        // 선택한 캐릭터 저장
        PlayerPrefs.SetInt(CharacterIndexKey, _currentIndex);
        PlayerPrefs.Save();

        // 현재 씬에 적용
        _characterDisplay.gameObject.SetActive(true);
        _characterDisplay.sprite = _characterSprites[_currentIndex];

        ClosePanel();
    }

    private void ClosePanel()
    {
        SoundManager.Instance?.PlaySFX(SFXType.ButtonClick);

        gameObject.SetActive(false);
    }

    public void OpenPanel()
    {
        if (liveManager.IsLive)
            return;
        SoundManager.Instance?.PlaySFX(SFXType.ButtonClick);
        gameObject.SetActive(true);
    }
}