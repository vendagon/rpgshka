using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoadGamePanelMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform savesContent;
    [SerializeField] private GameObject saveButtonPrefab;
    [SerializeField] private GameObject noSavesText;
    [SerializeField] private Button backButton;
    [SerializeField] private string sceneToLoad = "InsideHouse";

    private List<GameObject> saveButtons = new List<GameObject>();

    private void OnEnable()
    {
        RefreshSavesList();

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(ClosePanel);
        }
    }

    private void RefreshSavesList()
    {
        // Очищаем старые кнопки
        foreach (var button in saveButtons)
        {
            Destroy(button);
        }
        saveButtons.Clear();

        // Получаем сохранения
        string savePath = Application.persistentDataPath;
        var saveFiles = Directory.GetFiles(savePath, "save_day_*.json")
            .OrderByDescending(f => File.GetCreationTime(f)) // Сортировка по дате
            .ToArray();

        // Создаем кнопки
        if (saveFiles.Length > 0)
        {
            noSavesText.SetActive(false);

            foreach (string file in saveFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                int day = int.Parse(fileName.Split('_').Last());
                string creationTime = File.GetCreationTime(file).ToString("dd.MM.yyyy HH:mm");

                GameObject buttonObj = Instantiate(saveButtonPrefab, savesContent);
                buttonObj.GetComponentInChildren<TMP_Text>().text = $"День {day}\n<size=80%>{creationTime}</size>";

                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => LoadGame(day));

                saveButtons.Add(buttonObj);
            }
        }
        else
        {
            noSavesText.SetActive(true);
        }
    }

    private void LoadGame(int day)
    {
        if (SaveLoadManager.Instance == null)
        {
            Debug.LogError("SaveLoadManager не найден!");
            return;
        }

        // Загружаем сцену перед загрузкой сохранения
        SceneManager.LoadScene(sceneToLoad);

        // Загружаем сохранение после загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == sceneToLoad)
            {
                SaveLoadManager.Instance.LoadGame(day);
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}