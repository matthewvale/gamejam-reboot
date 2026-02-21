/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections;
using UnityEngine;

namespace RPSCore
{

    public class AudioManager : MonoBehaviour
    {

        #region Instancing

        public static AudioManager Instance { get; private set; }

        #endregion

        #region Private Properties


        #endregion

        #region Public Properties

        public AudioSource source_MUSIC;
        public AudioSource source_AMBIENCE;
        public AudioSource source_WEATHER;
        public AudioSource source_SFX;
        public AudioSource source_UI;

        [Header("TIMINGS")]
        public float musicClipGapTime = 5f; // How much silence do we want inbetween tracks?
        public float musicFadeSpeed = 4f;

        [Header("MUSIC CLIPS")]
        public AudioClip[] regularMusic_Clips;
        public AudioClip[] intenseMusic_Clips;
        public AudioClip[] menuMusic_Clips;

        [Header("AMBIENCE CLIPS")]
        public AudioClip[] regularAmbience_Clips;

        [Header("WEATHER CLIPS")]
        public AudioClip[] rain_Clips;

        [Header("SFX CLIPS")]
        public AudioClip SFX_01_Clip;
        public AudioClip SFX_02_Clip;
        public AudioClip SFX_03_Clip;

        [Header("UI CLIPS")]
        public AudioClip UIButtonClick_Clip;
        public AudioClip UIButtonHover_Clip;

        [Header("PITCHING OPTIONS")]
        [Range(0.5f, 1f)]
        public float minRandomPitch = 1f;
        [Range(1f, 1.5f)]
        public float maxRandomPitch = 1f;

        public enum UIClipType
        {
            BUTTON_CLICK, BUTTON_HOVER
        }

        public enum MusicStyle
        {
            MENU, REGULAR, INTENSE
        }

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
            Init();
        }

        #endregion

        #region Pulic Methods

        public void PlaySound(AudioClip clip, float pitch = 1f, bool randomPitch = false)
        {
            if (randomPitch)
            {
                source_SFX.pitch = Random.Range(minRandomPitch, maxRandomPitch);
            }

            source_SFX.pitch = pitch;
            source_SFX.PlayOneShot(clip);
        }

        public void PlayUISound(UIClipType uiClipType, bool randomPitch = false)
        {
            if (randomPitch)
            {
                source_UI.pitch = Random.Range(minRandomPitch, maxRandomPitch);
            }

            switch (uiClipType)
            {
                case UIClipType.BUTTON_CLICK:
                    source_UI.PlayOneShot(UIButtonClick_Clip);
                    break;
                case UIClipType.BUTTON_HOVER:
                    source_UI.PlayOneShot(UIButtonHover_Clip);
                    break;
            }
        }

        public void PlayMusic(MusicStyle style)
        {
            SwitchMusicTrack(style);
        }

        public void PlayRainAmbience()
        {
            source_WEATHER.clip = rain_Clips[Random.Range(0, rain_Clips.Length)];
            source_WEATHER.Play();
            StartCoroutine(FadeWeather(1f));
        }

        public void FadeWeatherAmbience()
        {
            StopCoroutine(FadeMusic(0));
            StartCoroutine(FadeWeather(0f));
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            if (regularMusic_Clips.Length == 0)
            {
                RPSLib.Debug.Log("AudioManager :: No music clips found.", RPSLib.Debug.Style.Warning);
                return;
            }

            source_MUSIC.clip = regularMusic_Clips[Random.Range(0, regularMusic_Clips.Length)];
            source_MUSIC.Play();
            StartCoroutine(FadeMusic(1f));
            Invoke(nameof(Init), source_MUSIC.clip.length + musicClipGapTime);
        }

        private void SwitchMusicTrack(MusicStyle style)
        {
            switch (style)
            {
                case MusicStyle.MENU:
                    source_MUSIC.clip = menuMusic_Clips[Random.Range(0, menuMusic_Clips.Length)];
                    break;
                case MusicStyle.REGULAR:
                    source_MUSIC.clip = regularMusic_Clips[Random.Range(0, regularMusic_Clips.Length)];
                    break;
                case MusicStyle.INTENSE:
                    source_MUSIC.clip = intenseMusic_Clips[Random.Range(0, intenseMusic_Clips.Length)];
                    break;
            }

            source_MUSIC.Play();
            StartCoroutine(FadeMusic(1f));
        }

        private void SwitchAmbienceTrack()
        {
            source_AMBIENCE.clip = regularAmbience_Clips[Random.Range(0, regularAmbience_Clips.Length)];
            source_AMBIENCE.Play();
            StartCoroutine(FadeAmbience(1f));
        }

        private IEnumerator FadeMusic(float targetVol)
        {
            float curTime = 0;
            float curVol = source_MUSIC.volume;
            while (curTime < musicFadeSpeed)
            {
                curTime += Time.deltaTime;
                source_MUSIC.volume = Mathf.Lerp(curVol, targetVol, curTime / musicFadeSpeed);
                yield return null;
            }
            yield break;
        }

        private IEnumerator FadeAmbience(float targetVol)
        {
            float curTime = 0;
            float curVol = source_AMBIENCE.volume;
            while (curTime < musicFadeSpeed)
            {
                curTime += Time.deltaTime;
                source_AMBIENCE.volume = Mathf.Lerp(curVol, targetVol, curTime / musicFadeSpeed);
                yield return null;
            }
            yield break;
        }

        private IEnumerator FadeWeather(float targetVol)
        {
            float curTime = 0;
            float curVol = source_WEATHER.volume;
            while (curTime < musicFadeSpeed)
            {
                curTime += Time.deltaTime;
                source_WEATHER.volume = Mathf.Lerp(curVol, targetVol, curTime / musicFadeSpeed);
                yield return null;
            }
            yield break;
        }

        #endregion

    }

}