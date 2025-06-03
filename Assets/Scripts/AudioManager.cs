using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Музыка")]
    public AudioClip backgroundMusic;
    public AudioSource musicSource;

    [Header("Звуки")]
    public AudioClip[] soundEffects;
    public AudioSource sfxSource;

    // Флаг для предотвращения смены музыки
    private bool musicLocked = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic(backgroundMusic);
    }

    // Заблокировать смену музыки (для сцен дома)
    public void LockMusic(bool locked)
    {
        musicLocked = locked;
    }

    // Включить музыку
    public void PlayMusic(AudioClip clip)
    {
        if (musicLocked) return; // Не меняем музыку если заблокировано

        musicSource.clip = clip;
        musicSource.Play();
    }

    // Принудительно включить музыку (игнорируя блокировку)
    public void ForcePlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void SwitchMusic(AudioClip newClip, float fadeDuration = 1f)
    {
        if (musicLocked) return; // Не меняем музыку если заблокировано

        StartCoroutine(FadeMusic(newClip, fadeDuration));
    }

    // Принудительная смена музыки с fade-эффектом
    public void ForceSwitchMusic(AudioClip newClip, float fadeDuration = 1f)
    {
        StartCoroutine(FadeMusic(newClip, fadeDuration));
    }

    private System.Collections.IEnumerator FadeMusic(AudioClip newClip, float fadeDuration)
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }
    }
}
