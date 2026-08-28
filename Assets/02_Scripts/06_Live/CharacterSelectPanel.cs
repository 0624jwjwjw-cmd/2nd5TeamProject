using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectPanel : MonoBehaviour
{
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

    private int _currentIndex;

    private void OnEnable()
    {
        _currentIndex = 0;
        UpdateCharacter();
    }

    private void Start()
    {
        _prevButton.onClick.AddListener(ShowPrevious);
        _nextButton.onClick.AddListener(ShowNext);
        _closeButton.onClick.AddListener(ClosePanel);
        _applyButton.onClick.AddListener(ApplyCharacter);
    }

    private void ShowPrevious()
    {
        if (_currentIndex <= 0)
            return;

        _currentIndex--;
        UpdateCharacter();
    }

    private void ShowNext()
    {
        if (_currentIndex >= _characterSprites.Length - 1)
            return;

        _currentIndex++;
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

        _characterDisplay.gameObject.SetActive(true);
        _characterDisplay.sprite = _characterSprites[_currentIndex];

        ClosePanel();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }
}