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
            // Переводим мировые координаты игрока в экранные
            Vector2 screenPos = Camera.main.WorldToScreenPoint(player.position);
            // Устанавливаем позицию подсказки
            promptUI.position = screenPos + screenOffset;
        }
    }
}