using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;

        private const float MaxVolume_BGM = 1.0f;
        private const float MaxVolume_SFX = 1.0f;
        private const float MaxPitch_Global = 1.0f;
        private static float CurrentVolumeNormalized_BGM = 1.0f;
        private static float CurrentVolumeNormalized_SFX = 1.0f;
        private static float CurrentPitchNormalized_Global = 1.0f;
        private static bool isMuted = false;

        private int loopingSFXCount;

        private SoundSourcePool sourcePool;
        
        private List<SourceStruct> sfxSources;
        private Dictionary<int, SourceStruct> loopingSfx;
        private AudioSource bgmSource;
        private AudioLowPassFilter lpf;

        public static SoundManager GetInstance()
        {
            if (!_instance)
            {
                GameObject soundManager = new GameObject("SoundManager");
                _instance = soundManager.AddComponent<SoundManager>();
                _instance.Initialize();
            }

            return _instance;
        }

        private void Initialize()
        {
            sourcePool = new SoundSourcePool(this.gameObject, 10, 5);
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.volume = GetBGMVolume();
            DontDestroyOnLoad(gameObject);
        }

        // Volume Getters
        public static float GetBGMVolume()
        {
            return isMuted ? 0.0f : MaxVolume_BGM * CurrentVolumeNormalized_BGM;
        }

        public static float GetSFXVolume()
        {
            return isMuted ? 0.0f : MaxVolume_SFX * CurrentVolumeNormalized_SFX;
        }

        public static float GetGlobalPitch()
        {
            return MaxPitch_Global * CurrentPitchNormalized_Global;
        }

        //BGM Utils
        private void FadeBGMOut(float fadeDuration)
        {
            SoundManager soundMan = GetInstance();
            float delay = 0.0f;
            float toVolume = 0.0f;

            soundMan.StartCoroutine(FadeBGM(toVolume, delay, fadeDuration));
        }

        private void FadeBGMIn(AudioClip bgmClip, float delay, float fadeDuration)
        {
            SoundManager soundMan = GetInstance();
            soundMan.bgmSource.clip = bgmClip;
            soundMan.bgmSource.Play();

            float toVolume = GetBGMVolume();

            soundMan.StartCoroutine(FadeBGM(toVolume, delay, fadeDuration));
        }

        private IEnumerator FadeBGM(float fadeToVolume, float delay, float duration)
        {
            yield return new WaitForSeconds(delay);

            SoundManager soundMan = GetInstance();
            float elapsed = 0.0f;
            while (duration > 0)
            {
                float t = (elapsed / duration);
                float volume = Mathf.Lerp(0.0f, fadeToVolume, t);
                soundMan.bgmSource.volume = volume;
                if (soundMan.bgmSource.volume == fadeToVolume)
                {
                    yield break;
                }

                elapsed += Time.deltaTime;
                yield return 0;
            }
        }

        //BGM Functions
        public static void PlayBGM(AudioClip bgmClip, bool fade, float fadeDuration)
        {
            SoundManager soundMan = GetInstance();
            if (fade)
            {
                if (soundMan.bgmSource.isPlaying)
                {
                    soundMan.FadeBGMOut(fadeDuration / 2);
                    soundMan.FadeBGMIn(bgmClip, fadeDuration / 2, fadeDuration / 2);
                }
                else
                {
                    float delay = 0f;
                    soundMan.FadeBGMIn(bgmClip, delay, fadeDuration);
                }
            }
            else
            {
                soundMan.bgmSource.volume = GetBGMVolume();
                soundMan.bgmSource.clip = bgmClip;
                soundMan.bgmSource.Play();
            }
        }

        public static void StopBGM(bool fade, float fadeDuration)
        {
            SoundManager soundMan = GetInstance();
            if (soundMan.bgmSource.isPlaying)
            {
                if (fade)
                {
                    soundMan.FadeBGMOut(fadeDuration);
                }
                else
                {
                    soundMan.bgmSource.Stop();
                }
            }
        }

        //SFX Utils
        private SourceStruct GetSFXSource()
        {
            AudioSource sfxSource = sourcePool.GetSource();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = GetSFXVolume();
            sfxSource.pitch = GetGlobalPitch();
            SourceStruct sfxStruct = new SourceStruct(sfxSource);

            if (sfxSources == null)
            {
                sfxSources = new List<SourceStruct>();
            }

            sfxSources.Add(sfxStruct);

            return sfxStruct;
        }

        private SourceStruct GetLoopingSFXSource(out int id)
        {
            AudioSource sfxSource = sourcePool.GetSource();
            sfxSource.loop = true;
            sfxSource.playOnAwake = false;
            sfxSource.volume = GetSFXVolume();
            sfxSource.pitch = GetGlobalPitch();
            SourceStruct sfxStruct = new SourceStruct(sfxSource);

            if (loopingSfx == null)
            {
                loopingSfx = new Dictionary<int, SourceStruct>();
            }

            loopingSfx.Add(loopingSFXCount, sfxStruct);
            id = loopingSFXCount;
            loopingSFXCount++;
            
            return sfxStruct;
        }

        private IEnumerator RemoveSFXSource(SourceStruct sfxStruct)
        {
            yield return new WaitForSeconds(sfxStruct.source.clip.length);
            sfxSources.Remove(sfxStruct);
            sourcePool.RepoolSource(sfxStruct.source);
        }

        private IEnumerator RemoveSFXSourceFixedLength(SourceStruct sfxStruct, float length)
        {
            yield return new WaitForSeconds(length);
            sfxSources.Remove(sfxStruct);
            sourcePool.RepoolSource(sfxStruct.source);
        }

        //SFX Functions
        public static void PlaySFX(AudioClip sfxClip, Vector3 position, float volumeMultiplier = 1f, float pitchMultiplier = 1f)
        {
            SoundManager soundMan = GetInstance();
            SourceStruct sourceStruct = soundMan.GetSFXSource();
            
            sourceStruct.volumeMultiplier = volumeMultiplier;
            sourceStruct.pitchMultiplier = pitchMultiplier;
            sourceStruct.source.transform.position = position;
            sourceStruct.source.volume = GetSFXVolume() * volumeMultiplier;
            sourceStruct.source.pitch = GetGlobalPitch() * pitchMultiplier;
            sourceStruct.source.clip = sfxClip;
            sourceStruct.source.Play();

            soundMan.StartCoroutine(soundMan.RemoveSFXSource(sourceStruct));
        }

        public static void PlaySFX(AudioClip[] sfxClips, Vector3 position, float volumeMultiplier = 1f, float pitchMultiplier = 1f)
        {
            AudioClip sfxClip = sfxClips[Random.Range(0, sfxClips.Length)];
            SoundManager soundMan = GetInstance();
            SourceStruct sourceStruct = soundMan.GetSFXSource();
            
            sourceStruct.volumeMultiplier = volumeMultiplier;
            sourceStruct.pitchMultiplier = pitchMultiplier;
            sourceStruct.source.transform.position = position;
            sourceStruct.source.volume = GetSFXVolume() * volumeMultiplier;
            sourceStruct.source.pitch = GetGlobalPitch() * pitchMultiplier;
            sourceStruct.source.clip = sfxClip;
            sourceStruct.source.Play();

            soundMan.StartCoroutine(soundMan.RemoveSFXSource(sourceStruct));
        }

        public static void PlaySFXRandomized(AudioClip sfxClip, Vector3 position, float volumeMultiplier = 1f)
        {
            SoundManager soundMan = GetInstance();
            SourceStruct sourceStruct = soundMan.GetSFXSource();
            float pitchMultiplier = Random.Range(0.85f, 1.2f);
            
            sourceStruct.volumeMultiplier = volumeMultiplier;
            sourceStruct.pitchMultiplier = pitchMultiplier;
            sourceStruct.source.transform.position = position;
            sourceStruct.source.volume = GetSFXVolume() * volumeMultiplier;
            sourceStruct.source.pitch = GetGlobalPitch() * pitchMultiplier;
            sourceStruct.source.clip = sfxClip;
            sourceStruct.source.Play();

            soundMan.StartCoroutine(soundMan.RemoveSFXSource(sourceStruct));
        }

        public static void PlaySFXRandomized(AudioClip[] sfxClips, Vector3 position, float volumeMultiplier = 1f)
        {
            AudioClip sfxClip = sfxClips[Random.Range(0, sfxClips.Length)];
            SoundManager soundMan = GetInstance();
            SourceStruct sourceStruct = soundMan.GetSFXSource();
            float pitchMultiplier = Random.Range(0.85f, 1.2f);
            
            sourceStruct.volumeMultiplier = volumeMultiplier;
            sourceStruct.pitchMultiplier = pitchMultiplier;
            sourceStruct.source.transform.position = position;
            sourceStruct.source.volume = GetSFXVolume() * volumeMultiplier;
            sourceStruct.source.pitch = GetGlobalPitch() * pitchMultiplier;
            sourceStruct.source.clip = sfxClip;
            sourceStruct.source.Play();

            soundMan.StartCoroutine(soundMan.RemoveSFXSource(sourceStruct));
        }

        public static void PlaySFXFixedDuration(AudioClip sfxClip, Vector3 position, float duration, 
            float volumeMultiplier = 1f, float pitchMultiplier = 1f)
        {
            SoundManager soundMan = GetInstance();
            SourceStruct sourceStruct = soundMan.GetSFXSource();
            
            sourceStruct.volumeMultiplier = volumeMultiplier;
            sourceStruct.pitchMultiplier = pitchMultiplier;
            sourceStruct.source.transform.position = position;
            sourceStruct.source.volume = GetSFXVolume() * volumeMultiplier;
            sourceStruct.source.pitch = GetGlobalPitch() * pitchMultiplier;
            sourceStruct.source.clip = sfxClip;
            sourceStruct.source.loop = true;
            sourceStruct.source.Play();

            soundMan.StartCoroutine(soundMan.RemoveSFXSourceFixedLength(sourceStruct, duration));
        }

        public static void PlaySFXFixedDuration(AudioClip[] sfxClips, Vector3 position,float duration, 
            float volumeMultiplier = 1f, float pitchMultiplier = 1f)
        {
            AudioClip sfxClip = sfxClips[Random.Range(0, sfxClips.Length)];
            SoundManager soundMan = GetInstance();
            SourceStruct sourceStruct = soundMan.GetSFXSource();
            
            sourceStruct.volumeMultiplier = volumeMultiplier;
            sourceStruct.pitchMultiplier = pitchMultiplier;
            sourceStruct.source.transform.position = position;
            sourceStruct.source.volume = GetSFXVolume() * volumeMultiplier;
            sourceStruct.source.pitch = GetGlobalPitch() * pitchMultiplier;
            sourceStruct.source.clip = sfxClip;
            sourceStruct.source.loop = true;
            sourceStruct.source.Play();

            soundMan.StartCoroutine(soundMan.RemoveSFXSourceFixedLength(sourceStruct, duration));
        }

        public static int PlayLoopingSFX(AudioClip sfxClip, Vector3 position, float volumeMultiplier = 1f,
            float pitchMultiplier = 1f)
        {
            SoundManager soundMan = GetInstance();
            SourceStruct sourceStruct = soundMan.GetLoopingSFXSource(out int id);
            
            sourceStruct.volumeMultiplier = volumeMultiplier;
            sourceStruct.pitchMultiplier = pitchMultiplier;
            sourceStruct.source.transform.position = position;
            sourceStruct.source.volume = GetSFXVolume() * volumeMultiplier;
            sourceStruct.source.pitch = GetGlobalPitch() * pitchMultiplier;
            sourceStruct.source.clip = sfxClip;
            sourceStruct.source.Play();

            return id;
        }
        
        public static int PlayLoopingSFX(AudioClip[] sfxClips, Vector3 position, float volumeMultiplier = 1f, 
            float pitchMultiplier = 1f)
        {
            AudioClip sfxClip = sfxClips[Random.Range(0, sfxClips.Length)];
            SoundManager soundMan = GetInstance();
            SourceStruct sourceStruct = soundMan.GetLoopingSFXSource(out int id);
            
            sourceStruct.volumeMultiplier = volumeMultiplier;
            sourceStruct.pitchMultiplier = pitchMultiplier;
            sourceStruct.source.transform.position = position;
            sourceStruct.source.volume = GetSFXVolume() * volumeMultiplier;
            sourceStruct.source.pitch = GetGlobalPitch() * pitchMultiplier;
            sourceStruct.source.clip = sfxClip;
            sourceStruct.source.Play();

            return id;
        }

        public static void StopLoopingSFX(int id)
        {
            SoundManager soundManager = GetInstance();
            if (soundManager.loopingSfx.ContainsKey(id))
                soundManager.loopingSfx.Remove(id);
        }

        //Volume Control Functions
        public static void DisableSoundImmediate()
        {
            SoundManager soundMan = GetInstance();
            soundMan.StopAllCoroutines();
            
            if (soundMan.sfxSources != null)
            {
                foreach (SourceStruct sourceStruct in soundMan.sfxSources)
                {
                    sourceStruct.source.volume = 0.0f;
                }
            }
            
            if (soundMan.loopingSfx != null)
            {
                foreach (SourceStruct sourceStruct in soundMan.loopingSfx.Values)
                {
                    sourceStruct.source.volume = 0.0f;
                }
            }

            soundMan.bgmSource.volume = 0.0f;
            isMuted = true;
        }

        public static void EnableSoundImmediate()
        {
            SoundManager soundMan = GetInstance();
            
            if (soundMan.sfxSources != null)
            {
                foreach (SourceStruct sourceStruct in soundMan.sfxSources)
                {
                    sourceStruct.source.volume = GetSFXVolume() * sourceStruct.volumeMultiplier;
                }
            }

            if (soundMan.loopingSfx != null)
            {
                foreach (SourceStruct sourceStruct in soundMan.loopingSfx.Values)
                {
                    sourceStruct.source.volume = GetSFXVolume() * sourceStruct.volumeMultiplier;
                }
            }

            soundMan.bgmSource.volume = GetBGMVolume();
            isMuted = false;
        }

        public static void SetGlobalPitch(float newPitch)
        {
            CurrentPitchNormalized_Global = newPitch;
            AdjustSoundImmediate();
        }

        public static void SetGlobalVolume(float newVolume)
        {
            CurrentVolumeNormalized_SFX = newVolume;
            CurrentVolumeNormalized_BGM = newVolume;
            AdjustSoundImmediate();
        }

        public static void SetSFXVolume(float newVolume)
        {
            CurrentVolumeNormalized_SFX = newVolume;
            AdjustSoundImmediate();
        }

        public static void SetBGMVolume(float newVolume)
        {
            CurrentVolumeNormalized_BGM = newVolume;
            AdjustSoundImmediate();
        }

        public static void AdjustSoundImmediate()
        {
            SoundManager soundMan = GetInstance();
            
            if (soundMan.sfxSources != null)
            {
                foreach (SourceStruct sourceStruct in soundMan.sfxSources)
                {
                    sourceStruct.source.volume = GetSFXVolume() * sourceStruct.volumeMultiplier;
                    sourceStruct.source.pitch = GetGlobalPitch() * sourceStruct.pitchMultiplier;
                }
            }
            
            if (soundMan.loopingSfx != null)
            {
                foreach (SourceStruct sourceStruct in soundMan.loopingSfx.Values)
                {
                    sourceStruct.source.volume = GetSFXVolume() * sourceStruct.volumeMultiplier;
                    sourceStruct.source.pitch = GetGlobalPitch() * sourceStruct.pitchMultiplier;
                }
            }

            soundMan.bgmSource.volume = GetBGMVolume();
            soundMan.bgmSource.pitch = GetGlobalPitch();
        }
        
        private struct SourceStruct
        {
            public readonly AudioSource source;
            public float volumeMultiplier;
            public float pitchMultiplier;

            public SourceStruct(AudioSource source, float volumeMultiplier = 1f, float pitchMultiplier = 1f)
            {
                this.source = source;
                this.volumeMultiplier = volumeMultiplier;
                this.pitchMultiplier = pitchMultiplier;
            }
        }
    }
}