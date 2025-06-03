using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    [SerializeField] public GameObject pauseMenuPanel;
    [SerializeField] private Slider volumeSlider;

    [Header("Cursor Settings")]
    [SerializeField] private Texture2D customCursor;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero; // Точка "активной зоны" курсора
    public bool isPaused = false;

    [Header("Load Game Settings")]
    [SerializeField] public GameObject loadGamePanel;

    [Header("Interaction Prompt Settings")]
    [SerializeField] private GameObject interactionPrompt;

    private InventoryManager inventory;

    private void Start()
    {
        SetGameCursor(false);
        loadGamePanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        inventory = InventoryManager.Instance;

        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void Update()
    {
        // Открытие/закрытие меню по Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
            interactionPrompt.SetActive(false);
        }
    }

    public void PauseGame()
    {
        if (inventory != null && inventory.isOpened)
        {
            inventory.CloseInventory();
        }

        Time.timeScale = 0f;
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        SetGameCursor(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuPanel.SetActive(false);

        if (loadGamePanel.activeSelf)
            loadGamePanel.SetActive(false);

        SetGameCursor(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void SetGameCursor(bool visible)
    {
        if (visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // Курсор свободен
            if (customCursor != null)
            {
                Cursor.SetCursor(customCursor, cursorHotspot, CursorMode.Auto);
            }
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; // Курсор зафиксирован и скрыт
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // Сброс на стандартный
        }
    }

    public void LoadGame()
    {
        pauseMenuPanel.SetActive(false);
        loadGamePanel.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }
}