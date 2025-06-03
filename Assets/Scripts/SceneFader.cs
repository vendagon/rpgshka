using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1f;

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

    public IEnumerator FadeOut()
    {
        fadeImage.raycastTarget = true; // Блокируем взаимодействие с UI
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.raycastTarget = false; // Разблокируем UI
    }

    private IEnumerator FindFadeImage()
    {
        fadeImage = null;
        while (fadeImage == null)
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                fadeImage = canvas.transform.Find("FadeOverlay")?.GetComponent<Image>();
            }
            yield return null;
        }
    }

    public IEnumerator TransitionToScene(string sceneName)
    {
        yield return FadeOut();
        SceneManager.LoadScene(sceneName);
        yield return FindFadeImage(); // Переподписываемся на новый Canvas
        yield return FadeIn();
    }
}