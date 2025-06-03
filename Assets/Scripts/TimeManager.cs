using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public enum TimeOfDay { Morning, Day, Evening, Night }
public enum SceneType { Outdoor, Indoor }

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public TimeOfDay currentTime = TimeOfDay.Morning;
    public int currentDay = 1;
    public float timePerPhase = 60f;

    [Header("Scene Music Settings")]
    public SceneType currentSceneType = SceneType.Outdoor;
    public AudioClip indoorMusic; // Музыка для дома
    public string[] indoorSceneNames = { "InsideHouse" }; // Названия сцен дома

    [Header("Lighting Settings")]
    public Image screenOverlay;
    [Range(0, 1)] public float morningDarkness = 0.1f;
    [Range(0, 1)] public float dayDarkness = 0f;
    [Range(0, 1)] public float eveningDarkness = 0.3f;
    [Range(0, 1)] public float nightDarkness = 0.7f;
    public float lightingTransitionSpeed = 2f;

    [Header("UI References")]
    public Image timeIconUI;
    public Sprite morningIcon;
    public Sprite dayIcon;
    public Sprite eveningIcon;
    public Sprite nightIcon;

    [Header("TextMeshPro Settings")]
    public TextMeshProUGUI dayText;

    [Header("Animation Settings")]
    public float fadeDuration = 0.5f;

    [Header("Music Settings")]
    public AudioClip morningMusic;
    public AudioClip dayMusic;
    public AudioClip eveningMusic;
    public AudioClip nightMusic;

    public float timer;
    private bool isAnimating = false;

    void Awake()
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

    void Start()
    {
        if (SceneDataTransfer.TimePhaseProgress > 0)
        {
            timer = SceneDataTransfer.TimePhaseProgress;
        }

        UpdateDayText();
        UpdateTimeUI();
        DetermineSceneType();
        PlaySceneAppropriateMusic();

        if (screenOverlay != null)
        {
            screenOverlay.color = new Color(0, 0, 0, GetTargetDarkness(currentTime));
        }
    }

    void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DetermineSceneType();
        PlaySceneAppropriateMusic();
    }

    void DetermineSceneType()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Проверяем, является ли текущая сцена внутренней
        foreach (string indoorName in indoorSceneNames)
        {
            if (currentSceneName.Contains(indoorName))
            {
                currentSceneType = SceneType.Indoor;
                return;
            }
        }

        currentSceneType = SceneType.Outdoor;
    }

    void PlaySceneAppropriateMusic()
    {
        if (currentSceneType == SceneType.Indoor)
        {
            // Внутри дома - всегда одна музыка
            AudioManager.Instance.SwitchMusic(indoorMusic);
        }
        else
        {
            // На улице - музыка зависит от времени суток
            PlayTimeBasedMusic();
        }
    }

    void PlayTimeBasedMusic()
    {
        switch (currentTime)
        {
            case TimeOfDay.Morning:
                AudioManager.Instance.SwitchMusic(morningMusic);
                break;
            case TimeOfDay.Day:
                AudioManager.Instance.SwitchMusic(dayMusic);
                break;
            case TimeOfDay.Evening:
                AudioManager.Instance.SwitchMusic(eveningMusic);
                break;
            case TimeOfDay.Night:
                AudioManager.Instance.SwitchMusic(nightMusic);
                break;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timePerPhase && !isAnimating)
        {
            timer = 0;
            NextTimePhase();
        }
    }

    void NextTimePhase()
    {
        switch (currentTime)
        {
            case TimeOfDay.Morning:
                StartCoroutine(ChangeTimePhase(TimeOfDay.Day, dayIcon));
                // Музыка переключается только если мы на улице
                if (currentSceneType == SceneType.Outdoor)
                    AudioManager.Instance.SwitchMusic(dayMusic);
                break;
            case TimeOfDay.Day:
                StartCoroutine(ChangeTimePhase(TimeOfDay.Evening, eveningIcon));
                if (currentSceneType == SceneType.Outdoor)
                    AudioManager.Instance.SwitchMusic(eveningMusic);
                break;
            case TimeOfDay.Evening:
                StartCoroutine(ChangeTimePhase(TimeOfDay.Night, nightIcon));
                if (currentSceneType == SceneType.Outdoor)
                    AudioManager.Instance.SwitchMusic(nightMusic);
                break;
            case TimeOfDay.Night:
                if (currentSceneType == SceneType.Outdoor)
                    AudioManager.Instance.SwitchMusic(morningMusic);
                StartNewDay();
                break;
        }
    }

    IEnumerator ChangeTimePhase(TimeOfDay newTime, Sprite newSprite)
    {
        isAnimating = true;
        float targetAlpha = GetTargetDarkness(newTime);
        yield return StartCoroutine(ChangeTimeIcon(newSprite));
        yield return StartCoroutine(AdjustLighting(targetAlpha));
        currentTime = newTime;
        isAnimating = false;
    }

    private float GetTargetDarkness(TimeOfDay time)
    {
        return time switch
        {
            TimeOfDay.Morning => morningDarkness,
            TimeOfDay.Day => dayDarkness,
            TimeOfDay.Evening => eveningDarkness,
            TimeOfDay.Night => nightDarkness,
            _ => 0f
        };
    }

    IEnumerator AdjustLighting(float targetAlpha)
    {
        if (screenOverlay == null) yield break;

        float startAlpha = screenOverlay.color.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lightingTransitionSpeed;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            screenOverlay.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
    }

    IEnumerator ChangeTimeIcon(Sprite newSprite)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            timeIconUI.color = new Color(1, 1, 1, 1 - (t / fadeDuration));
            t += Time.deltaTime;
            yield return null;
        }

        timeIconUI.sprite = newSprite;
        UpdateDayText();

        t = 0;
        while (t < fadeDuration)
        {
            timeIconUI.color = new Color(1, 1, 1, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
    }

    void UpdateDayText()
    {
        if (dayText != null)
        {
            dayText.text = "" + currentDay;
        }
    }

    public void UpdateTimeUI()
    {
        switch (currentTime)
        {
            case TimeOfDay.Morning:
                timeIconUI.sprite = morningIcon;
                break;
            case TimeOfDay.Day:
                timeIconUI.sprite = dayIcon;
                break;
            case TimeOfDay.Evening:
                timeIconUI.sprite = eveningIcon;
                break;
            case TimeOfDay.Night:
                timeIconUI.sprite = nightIcon;
                break;
        }
        UpdateDayText();
    }

    public void StartNewDay()
    {
        currentTime = TimeOfDay.Morning;
        currentDay++;
        StartCoroutine(ChangeTimeIcon(morningIcon));
        Debug.Log($"Новый день! День {currentDay}");
    }
}
