using Managers;
using UnityEngine;

public class VolumeSlider : MonoBehaviour
{
    public void SetGlobalVolume(float volume)
    {
        SoundManager.SetGlobalVolume(volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        SoundManager.SetSFXVolume(volume);
    }
    
    public void SetBGMVolume(float volume)
    {
        SoundManager.SetBGMVolume(volume);
    }
}
