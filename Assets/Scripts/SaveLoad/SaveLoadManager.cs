using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private Vector3 playerSpawnPosition;
    public static SaveLoadManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SaveData data = new SaveData();

        data.savedDay = TimeManager.Instance.currentDay;
        data.savedTime = TimeManager.Instance.currentTime;
        data.playerHealth = player.GetComponent<HealthSystem>().CurrentHealth;
        data.inventory = InventoryManager.Instance.SerializeInventory();

        string json = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + $"/save_day_{data.savedDay}.json";
        File.WriteAllText(path, json);

        Debug.Log("Игра сохранена! День: " + data.savedDay);
    }

    public void LoadGame(int day)
    {
        string path = Application.persistentDataPath + $"/save_day_{day}.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Переносим данные в SceneDataTransfer
            SceneDataTransfer.CurrentDay = data.savedDay;
            SceneDataTransfer.CurrentTime = data.savedTime;
            SceneDataTransfer.CurrentHealth = data.playerHealth;
            SceneDataTransfer.InventoryData = data.inventory;
            SceneDataTransfer.PlayerSpawnPosition = playerSpawnPosition;

            SceneManager.LoadScene("InsideHouse");
        }
        else
        {
            Debug.LogWarning("Сохранение дня " + day + " не найдено.");
        }
    }
}