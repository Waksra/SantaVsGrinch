using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class AudioRequester : MonoBehaviour
{
    [SerializeField] private float volume = 1f;
    [SerializeField] private AudioClip audioClip = default;
    
    public void RequestPlayAudio()
    {
        SoundManager.PlaySFX(audioClip, transform.position, volume);
    }
    
    public void RequestPlayAudioRandomized()
    {
        SoundManager.PlaySFXRandomized(audioClip, transform.position, volume);
    }
}
