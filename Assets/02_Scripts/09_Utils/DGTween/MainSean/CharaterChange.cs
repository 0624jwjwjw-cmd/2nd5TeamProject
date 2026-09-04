using UnityEngine;
using DG.Tweening;

public class CharacterTitleSequence : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private CharacterTitleTween[] characters;

    [Header("Delay")]
    [SerializeField] private float delay = 0.2f;

    private Sequence sequence;

    private void Start()
    {
        PlaySequence();
    }

    public void PlaySequence()
    {
        sequence?.Kill();

        sequence = DOTween.Sequence();

        foreach (CharacterTitleTween character in characters)
        {
            sequence.AppendInterval(delay);
            sequence.Append(character.Play());
        }

        sequence.SetLoops(-1, LoopType.Restart);
    }

    private void OnDestroy()
    {
        sequence?.Kill();
    }
}