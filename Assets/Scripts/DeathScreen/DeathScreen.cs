using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    [Header("Custom Cursor Settings")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotSpot = Vector2.zero; // ����� "�������� ����" �������
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
    [Header("Load Game Settings")]
    [SerializeField] public GameObject loadGamePanel;
    void Start()
    {
        loadGamePanel.SetActive(false);
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
        // �������� ������ � ������ ��� �������
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public void OnMainMenuButtonClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnLoadSaveButtonClick()
    {
        loadGamePanel.SetActive(true);
    }
}