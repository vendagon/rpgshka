using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("������")]
    public AudioClip backgroundMusic;
    public AudioSource musicSource;

    [Header("�����")]
    public AudioClip[] soundEffects;
    public AudioSource sfxSource;

    // ���� ��� �������������� ����� ������
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

    // ������������� ����� ������ (��� ���� ����)
    public void LockMusic(bool locked)
    {
        musicLocked = locked;
    }

    // �������� ������
    public void PlayMusic(AudioClip clip)
    {
        if (musicLocked) return; // �� ������ ������ ���� �������������

        musicSource.clip = clip;
        musicSource.Play();
    }

    // ������������� �������� ������ (��������� ����������)
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
        if (musicLocked) return; // �� ������ ������ ���� �������������

        StartCoroutine(FadeMusic(newClip, fadeDuration));
    }

    // �������������� ����� ������ � fade-��������
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
