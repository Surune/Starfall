using System;
using UnityEngine;
using Utilities;

namespace Audio
{
public class SoundManager
{
    private const string BgmVolumeKey = "BGM_VOLUME";
    private const string SfxVolumeKey = "SFX_VOLUME";
    private const float DefaultBgmVolume = 0.5f;
    private const float DefaultSfxVolume = 0.5f;

    private readonly AudioSource[] audioSources = new AudioSource[(int)SoundType.MaxCount];
    private SoundDictionary soundDictionary;
    private bool bgmOn;
    private bool sfxOn;
    private float bgmVolume;
    private float sfxVolume;

    public float BgmVolume => bgmVolume;
    public float SfxVolume => sfxVolume;

    public SoundManager(SoundDictionary dictionary)
    {
        soundDictionary = dictionary;

        var root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            UnityEngine.Object.DontDestroyOnLoad(root);
        }

        if (root.GetComponentInChildren<AudioListener>(true) == null)
        {
            var listener = new GameObject { name = "Listener" };
            listener.AddComponent<AudioListener>();
            listener.transform.SetParent(root.transform);
        }

        var soundNames = Enum.GetNames(typeof(SoundType));
        for (var i = 0; i < soundNames.Length - 1; i++)
        {
            var sourceTransform = root.transform.Find(soundNames[i]);
            if (sourceTransform == null)
            {
                var sourceObject = new GameObject { name = soundNames[i] };
                sourceObject.transform.SetParent(root.transform);
                sourceTransform = sourceObject.transform;
            }

            audioSources[i] = sourceTransform.gameObject.GetOrAddComponent<AudioSource>();
        }

        audioSources[(int)SoundType.Bgm].loop = true;
            
        bgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, DefaultBgmVolume);
        audioSources[(int)SoundType.Bgm].volume = bgmVolume;

        sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, DefaultSfxVolume);
        audioSources[(int)SoundType.Effect].volume = sfxVolume;
    }

    public void PlayBGM(SoundKey soundKey)
    {
        PlayBGM(soundDictionary.GetClip(soundKey));
    }

    public void PlayBGM(AudioClip audioClip)
    {
        var audioSource = audioSources[(int)SoundType.Bgm];
        if (audioSource.isPlaying && audioSource.clip == audioClip)
        {
            return;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        audioSource.volume = bgmVolume;
        audioSource.clip = audioClip; 
        audioSource.Play();
    }

    public void PlaySFX(SoundKey soundType)
    {
        var audioClip = soundDictionary.GetClip(soundType);
        
        var audioSource = audioSources[(int)SoundType.Effect];
        audioSource.volume = sfxVolume;
        audioSource.PlayOneShot(audioClip);
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(BgmVolumeKey, bgmVolume);
        PlayerPrefs.Save();
        audioSources[(int)SoundType.Bgm].volume = bgmVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
        PlayerPrefs.Save();
        audioSources[(int)SoundType.Effect].volume = sfxVolume;
    }
}
}
