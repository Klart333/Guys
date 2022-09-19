using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    private SimpleAudioEvent _clickSound;

    private Queue<AudioSource> _audioSources = new Queue<AudioSource>();

    public bool _useSoundEffects = true;
    public bool _useMusic = true;

    private void Start()
    {
        _useSoundEffects = PlayerPrefs.GetInt("SoundEffects") == 0;
        _useMusic = PlayerPrefs.GetInt("Music") == 0;
    }

    public void PlaySoundEffect(SimpleAudioEvent audio)
    {
        if (!_useSoundEffects)
        {
            return;
        }

        if (_audioSources.Count == 0)
        {
            _audioSources.Enqueue(gameObject.AddComponent<AudioSource>());
        }
        var source = _audioSources.Dequeue();
        int index = audio.Play(source);

        StartCoroutine(ReturnToQueue(source, audio.Clips[index].length));
    }

    private IEnumerator ReturnToQueue(AudioSource source, float length)
    {
        yield return new WaitForSeconds(length);

        _audioSources.Enqueue(source);
    }

    public void PlayClickSound()
    {
        PlaySoundEffect(_clickSound);
    }

    public void ToggleSoundEffects(bool isOn)
    {
        _useSoundEffects = isOn;
        PlayerPrefs.SetInt("SoundEffects", _useSoundEffects ? 0 : -1);
    }

    public void ToggleMusic(bool isOn)
    {
        _useMusic = isOn;
        PlayerPrefs.SetInt("Music", _useMusic ? 0 : -1);
    }
}
