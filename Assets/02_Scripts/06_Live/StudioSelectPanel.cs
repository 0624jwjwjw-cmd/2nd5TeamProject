using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudioSelectPanel : MonoBehaviour
{
    [SerializeField] private Image _studioImage;
    [SerializeField] private Image _studioBackground;

    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _applyButton;

    [SerializeField] private TMP_Text _studioNameText;
    [SerializeField] private TMP_Text _studioDescriptionText;

    [SerializeField] private Sprite[] _studioSprites;
    [SerializeField] private string[] _studioNames;
    [SerializeField] private string[] _studioDescriptions;

    private int _currentIndex;

    private void OnEnable()
    {
        _currentIndex = 0;
        UpdateStudio();
    }

    private void Start()
    {
        _prevButton.onClick.AddListener(ShowPrevious);
        _nextButton.onClick.AddListener(ShowNext);
        _closeButton.onClick.AddListener(ClosePanel);
        _applyButton.onClick.AddListener(ApplyStudio);
    }

    private void ShowPrevious()
    {
        if (_currentIndex <= 0)
            return;

        _currentIndex--;
        UpdateStudio();
    }

    private void ShowNext()
    {
        if (_currentIndex >= _studioSprites.Length - 1)
            return;

        _currentIndex++;
        UpdateStudio();
    }

    private void UpdateStudio()
    {
        if (_studioSprites.Length == 0)
            return;

        _studioImage.sprite = _studioSprites[_currentIndex];

        if (_currentIndex < _studioNames.Length)
            _studioNameText.text = _studioNames[_currentIndex];

        if (_currentIndex < _studioDescriptions.Length)
            _studioDescriptionText.text =
                _studioDescriptions[_currentIndex];

        _prevButton.gameObject.SetActive(_currentIndex > 0);

        _nextButton.gameObject.SetActive(
            _currentIndex < _studioSprites.Length - 1
        );
    }

    public void ApplyStudio()
    {
        if (_studioSprites.Length == 0)
            return;

        _studioBackground.gameObject.SetActive(true);
        _studioBackground.sprite = _studioSprites[_currentIndex];

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