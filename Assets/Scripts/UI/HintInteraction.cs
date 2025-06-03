using UnityEngine;
using UnityEngine.UI;
// �������� �� ��������� ����
public class HintInteraction : MonoBehaviour
{
    public RectTransform promptUI;
    public Transform player;
    public Vector3 screenOffset = new Vector3(0, 50, 0);

    void Update()
    {
        if (player != null)
        {
            // ��������� ������� ���������� ������ � ��������
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position);
            promptUI.position = screenPos + screenOffset;
        }
    }
}
