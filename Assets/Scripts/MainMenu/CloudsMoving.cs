using UnityEngine;
using UnityEngine.UI;

public class MovingClouds : MonoBehaviour
{
    [Header("��������� ��������")]
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
        // �������� �����
        rectTransform.anchoredPosition += Vector2.left * moveSpeed * Time.deltaTime;

        // �������� �� ����� �� �������
        if (rectTransform.anchoredPosition.x <= resetPositionX)
        {
            // ���������� ������ � ��������� �������
            Vector2 newPos = rectTransform.anchoredPosition;
            newPos.x = startPositionX;
            rectTransform.anchoredPosition = newPos;
        }
    }
}