using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadGamePanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform savesContent;
    [SerializeField] private GameObject saveButtonPrefab;
    [SerializeField] private GameObject noSavesText;
    [SerializeField] private Button backButton;
    [SerializeField] private PauseMenu pauseMenu;

    private List<GameObject> saveButtons = new List<GameObject>();

    private void OnEnable()
    {
        RefreshSavesList();
    }

    private void RefreshSavesList()
    {
        foreach (var button in saveButtons)
        {
            Destroy(button);
        }
        saveButtons.Clear();

        string savePath = Application.persistentDataPath;
        var saveFiles = Directory.GetFiles(savePath, "save_day_*.json")
            .OrderByDescending(f => File.GetCreationTime(f))
            .ToArray();

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

        SaveLoadManager.Instance.LoadGame(day);
        ClosePanel();
    }

    public void ClosePanel()
    {
        pauseMenu.ResumeGame();
    }
}