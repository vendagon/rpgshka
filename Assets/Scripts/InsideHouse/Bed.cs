using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Bed : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InteractionHint interactionHint;
    public Image fadeOverlay;
    public GameObject dayTextObject;
    public TextMeshProUGUI dayTextComponent;
    public GameObject saveSuccessNotification;
    public GameObject saveErrorNotification;
    public float notificationDuration = 2f;

    [Header("Settings")]
    public bool forceMorningAfterSleep = true;

    private bool canSleep = false;
    private bool isSleeping = false;

    void Start()
    {
        SetActiveSafe(dayTextObject, false);
        SetActiveSafe(saveSuccessNotification, false);
        SetActiveSafe(saveErrorNotification, false);
        interactionHint.HideHint();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isSleeping && IsNightTime())
        {
            canSleep = true;
            interactionHint.ShowHint();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canSleep = false;
            interactionHint.HideHint();
        }
    }

    void Update()
    {
        if (canSleep)
        {
            if (!IsNightTime())
            {
                canSleep = false;
                interactionHint.HideHint();
            }
            else if (Input.GetKeyDown(KeyCode.F) && !isSleeping)
            {
                StartCoroutine(SleepRoutine());
            }
        }
    }

    IEnumerator SleepRoutine()
    {
        isSleeping = true;

        // 1. Сохраняем текущий день перед переходом
        int currentDay = TimeManager.Instance.currentDay;

        // 2. Затемняем экран
        yield return StartCoroutine(FadeScreen(0, 1, 1f));

        // 3. Увеличиваем день
        TimeManager.Instance.StartNewDay();
        if (forceMorningAfterSleep)
        {
            TimeManager.Instance.currentTime = TimeOfDay.Morning;
        }

        // 4. Сохраняем игру сразу после смены дня
        SaveLoadManager.Instance.SaveGame();

        // 5. Показываем уведомление
        dayTextComponent.text = $"День {TimeManager.Instance.currentDay}";
        SetActiveSafe(dayTextObject, true);
        SetActiveSafe(saveSuccessNotification, true);

        yield return new WaitForSeconds(2f);

        // 6. Возвращаем прозрачность
        SetActiveSafe(dayTextObject, false);
        yield return StartCoroutine(FadeScreen(1, 0, 1f));

        isSleeping = false;
    }

    IEnumerator FadeScreen(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            fadeOverlay.color = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeOverlay.color = new Color(0, 0, 0, endAlpha);
    }

    bool IsNightTime()
    {
        return TimeManager.Instance != null &&
               TimeManager.Instance.currentTime == TimeOfDay.Night;
    }

    void SetActiveSafe(GameObject obj, bool state)
    {
        if (obj != null) obj.SetActive(state);
    }
}