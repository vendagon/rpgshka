using UnityEngine;
using UnityEngine.UI;

public class MovingClouds : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 10f;
    public float resetPositionX = -2000f;
    public float startPositionX = 2000f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Движение влево
        rectTransform.anchoredPosition += Vector2.left * moveSpeed * Time.deltaTime;

        // Проверка на выход за границы
        if (rectTransform.anchoredPosition.x <= resetPositionX)
        {
            // Возвращаем облако в начальную позицию
            Vector2 newPos = rectTransform.anchoredPosition;
            newPos.x = startPositionX;
            rectTransform.anchoredPosition = newPos;
        }
    }
}