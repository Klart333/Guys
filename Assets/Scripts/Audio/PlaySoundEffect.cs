using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent _audio;

    public void Play()
    {
        AudioManager.Instance.PlaySoundEffect(_audio);
    }
}
