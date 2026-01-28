using UnityEngine;
using System.Collections;

namespace InternationalKarate.Audio
{
    /// <summary>
    /// Manages background music and sound effects for the game
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Background Music")]
        [SerializeField] private AudioClip[] backgroundMusic;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip tune3; // Hurt from strong kicks
        [SerializeField] private AudioClip tune4; // Strong kicks (FlyingKick, HighKick, LowKick, RoundHouseKick)
        [SerializeField] private AudioClip tune5; // Light attacks (CrouchKick, HighPunch, AnkleKick)
        [SerializeField] private AudioClip tune6; // Hurt from light attacks
        [SerializeField] private AudioClip tune7; // GroinPunch
        [SerializeField] private AudioClip tune8; // Hurt from GroinPunch

        [Header("Settings")]
        [SerializeField] private float musicVolume = 0.5f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private bool randomizeMusic = true;

        private int currentMusicIndex = 0;
        private Coroutine musicCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            SetupAudioSources();
        }

        private void Start()
        {
            StartBackgroundMusic();
        }

        private void SetupAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = false;
                musicSource.playOnAwake = false;
                musicSource.volume = musicVolume;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = sfxVolume;
            }
        }

        private void StartBackgroundMusic()
        {
            if (backgroundMusic == null || backgroundMusic.Length == 0) return;

            if (musicCoroutine != null)
                StopCoroutine(musicCoroutine);

            musicCoroutine = StartCoroutine(PlayMusicLoop());
        }

        private IEnumerator PlayMusicLoop()
        {
            while (true)
            {
                if (backgroundMusic.Length == 0) yield break;

                if (randomizeMusic && backgroundMusic.Length > 1)
                {
                    currentMusicIndex = Random.Range(0, backgroundMusic.Length);
                }
                else
                {
                    currentMusicIndex = (currentMusicIndex + 1) % backgroundMusic.Length;
                }

                AudioClip clip = backgroundMusic[currentMusicIndex];
                if (clip != null)
                {
                    musicSource.clip = clip;
                    musicSource.volume = musicVolume;
                    musicSource.Play();
                    yield return new WaitForSeconds(clip.length);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        /// <summary>
        /// Play sound effect for attack moves
        /// </summary>
        public void PlayAttackSound(string attackType)
        {
            AudioClip clip = GetAttackClip(attackType);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
        }

        /// <summary>
        /// Play sound effect for getting hurt
        /// </summary>
        public void PlayHurtSound(string attackType)
        {
            AudioClip clip = GetHurtClip(attackType);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
        }

        private AudioClip GetAttackClip(string attackType)
        {
            switch (attackType)
            {
                // Strong kicks - Tune4
                case "FlyingKick":
                case "HighKick":
                case "LowKick":
                case "RoundHouse":
                case "RoundHouseKick":
                    return tune4;

                // Light attacks - Tune5
                case "CrouchKick":
                case "HighPunch":
                case "AnkleKick":
                    return tune5;

                // GroinPunch - Tune7
                case "GroinPunch":
                    return tune7;

                default:
                    return null;
            }
        }

        private AudioClip GetHurtClip(string attackType)
        {
            switch (attackType)
            {
                // Hurt from strong kicks - Tune3
                case "FlyingKick":
                case "HighKick":
                case "LowKick":
                case "RoundHouse":
                case "RoundHouseKick":
                    return tune3;

                // Hurt from light attacks - Tune6
                case "CrouchKick":
                case "HighPunch":
                case "AnkleKick":
                    return tune6;

                // Hurt from GroinPunch - Tune8
                case "GroinPunch":
                    return tune8;

                default:
                    return tune3; // Default hurt sound
            }
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
                musicSource.volume = musicVolume;
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
                sfxSource.volume = sfxVolume;
        }

        public void StopMusic()
        {
            if (musicCoroutine != null)
            {
                StopCoroutine(musicCoroutine);
                musicCoroutine = null;
            }
            if (musicSource != null)
                musicSource.Stop();
        }

        public void ResumeMusic()
        {
            StartBackgroundMusic();
        }
    }
}
