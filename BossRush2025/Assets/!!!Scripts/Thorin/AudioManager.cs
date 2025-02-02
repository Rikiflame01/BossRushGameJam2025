/*
 * Author : thorin
 * desc: audio manager using singleton pattern
 * intention: to be used accross the whole game
 */
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource; 
    [SerializeField] private AudioSource sfxSource; 

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] bgmClips; 
    [SerializeField] private AudioClip[] sfxClips;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Background Music
    public void PlayBGM(string bgmName)
    {
        AudioClip clip = GetClipByName(bgmClips, bgmName);
        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM Clip {bgmName} not found!");
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }
    #endregion

    #region Sound Effects
    public void PlaySFX(string sfxName)
    {
        AudioClip clip = GetClipByName(sfxClips, sfxName);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX Clip {sfxName} not found!");
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
    #endregion

    #region Utility
    private AudioClip GetClipByName(AudioClip[] clips, string clipName)
    {
        foreach (var clip in clips)
        {
            if (clip.name == clipName)
                return clip;
        }
        return null;
    }
    #endregion
}
