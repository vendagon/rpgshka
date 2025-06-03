using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Custom Cursor Settings")]
    [SerializeField] private Texture2D menuCursorTexture;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    [Header("Load Game Settings")]
    [SerializeField] public GameObject loadGamePanel;
    private void Start()
    {
        loadGamePanel.SetActive(false);
        SetCustomCursor();

        // ¬сегда разблокируем и показываем курсор в меню
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void SetCustomCursor()
    {
        if (menuCursorTexture != null)
        {
            Cursor.SetCursor(menuCursorTexture, hotSpot, cursorMode);
        }
    }
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("InsideHouse");
    }

    public void OnLoadGameClicked()
    {
        loadGamePanel.SetActive(true);
    }

    public void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}