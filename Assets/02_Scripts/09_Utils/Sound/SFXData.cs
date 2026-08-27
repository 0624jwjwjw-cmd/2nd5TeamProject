using UnityEngine;

[System.Serializable]
public class SFXData
{
    public SFXType type;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
}