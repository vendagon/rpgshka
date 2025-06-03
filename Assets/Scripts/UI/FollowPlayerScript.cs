using UnityEngine;
using UnityEngine.UI;

public class FollowPlayerScript : MonoBehaviour
{
    public RectTransform promptUI;
    public Transform player;
    public Vector2 screenOffset = new Vector2(0, 50);

    void Update()
    {
        if (player != null)
        {
            // ��������� ������� ���������� ������ � ��������
            Vector2 screenPos = Camera.main.WorldToScreenPoint(player.position);
            // ������������� ������� ���������
            promptUI.position = screenPos + screenOffset;
        }
    }
}