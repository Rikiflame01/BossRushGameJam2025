using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct SFXClip
{
    public string key;
    public AudioClip clip;
}

public class SoundMenuManager : MonoBehaviour
{
    [Header("Audio Source for SFX")]
    [SerializeField] private AudioSource audioSource;

    [Header("List of SFX Clips")]
    [SerializeField] private SFXClip[] sfxClips;

    private Dictionary<string, AudioClip> sfxDictionary;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        sfxDictionary = new Dictionary<string, AudioClip>();

        foreach (SFXClip sfxClip in sfxClips)
        {
            if (!sfxDictionary.ContainsKey(sfxClip.key))
            {
                sfxDictionary.Add(sfxClip.key, sfxClip.clip);
            }
            else
            {
                Debug.LogWarning($"Duplicate key found: {sfxClip.key}. Skipping.");
            }
        }
    }

    public void PlaySFX(string sfxKey)
    {
        if (sfxDictionary.TryGetValue(sfxKey, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX key '{sfxKey}' not found in dictionary.");
        }
    }
}
