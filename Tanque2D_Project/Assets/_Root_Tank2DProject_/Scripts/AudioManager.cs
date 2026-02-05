using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Settings")]
    public float musicFadeDuration = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("SFX Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Sound Effects")]
    public AudioClip playerShoot;
    public AudioClip enemyShoot;
    public AudioClip playerHit;
    public AudioClip enemyHit;
    public AudioClip enemyDeath;
    public AudioClip gameOver;

    private Coroutine musicFadeCoroutine;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup audio sources
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    // MÚSICA
    public void PlayMusic(AudioClip clip, bool fade = true)
    {
        if (clip == null) return;

        if (fade)
        {
            if (musicFadeCoroutine != null)
                StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = StartCoroutine(FadeMusic(clip));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        // Fade out
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < musicFadeDuration / 2)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (musicFadeDuration / 2));
            yield return null;
        }

        // Cambiar clip
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        elapsed = 0f;
        while (elapsed < musicFadeDuration / 2)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / (musicFadeDuration / 2));
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    public void StopMusic(bool fade = true)
    {
        if (fade)
        {
            StartCoroutine(FadeOutMusic());
        }
        else
        {
            musicSource.Stop();
        }
    }

    IEnumerator FadeOutMusic()
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / musicFadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = musicVolume;
    }

    // SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
        }
    }

    // Métodos específicos para sonidos comunes
    public void PlayPlayerShoot() => PlaySFX(playerShoot);
    public void PlayEnemyShoot() => PlaySFX(enemyShoot);
    public void PlayPlayerHit() => PlaySFX(playerHit);
    public void PlayEnemyHit() => PlaySFX(enemyHit);
    public void PlayEnemyDeath() => PlaySFX(enemyDeath);
    public void PlayGameOver() => PlaySFX(gameOver);

    // Ajustes de volumen
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}
