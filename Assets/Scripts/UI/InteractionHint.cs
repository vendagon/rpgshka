using UnityEngine;
using System.Collections;
//�������� �� ������, ��� ����� ���������

public class InteractionHint : MonoBehaviour
{
    [SerializeField] private GameObject hintObject;
    [SerializeField] private CanvasGroup hintCanvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine currentAnimation;
    private bool isHintVisible = false;

    private void Start()
    {
        if (hintObject != null)
        {
            hintObject.SetActive(false);
            if (hintCanvasGroup != null)
                hintCanvasGroup.alpha = 0;
        }
    }

    // ���������� ��������� � ���������
    public void ShowHint()
    {
        if (hintObject == null || isHintVisible) return;

        isHintVisible = true;
        hintObject.SetActive(true);

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(FadeAnimation(0, 1, fadeDuration));
    }

    // �������� ��������� � ���������
    public void HideHint()
    {
        if (hintObject == null || !isHintVisible) return;

        isHintVisible = false;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        if (gameObject.activeInHierarchy) 
        {

        currentAnimation = StartCoroutine(FadeAnimation(1, 0, fadeDuration, () =>
        {
            hintObject.SetActive(false);
        }));
        }
    }

    // �������� ��� �������� ��������� ������������
    private IEnumerator FadeAnimation(float startAlpha, float endAlpha, float duration, System.Action onComplete = null)
    {
        if (hintCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                hintCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            hintCanvasGroup.alpha = endAlpha;
        }
        onComplete?.Invoke();
    }
}