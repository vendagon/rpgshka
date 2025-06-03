using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    public Vector3 PlayerSpawnPosition { get; set; }
    public InventorySaveData InventoryData { get; private set; }
    public int CurrentDay { get; set; }
    public int CurrentHealth { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Подписываемся на событие загрузки сцены
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PrepareTransition()
    {
        InventoryData = InventoryManager.Instance.SerializeInventory();
        CurrentDay = TimeManager.Instance.currentDay;
        CurrentHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>().CurrentHealth;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // В момент загрузки сцены — запускаем процесс восстановления данных
        SceneLoader loader = FindFirstObjectByType<SceneLoader>();
        if (loader != null)
        {
            loader.StartCoroutine(loader.LoadDataCoroutine());
        }
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(SceneFader.Instance.TransitionToScene(sceneName));
    }
    public void ClearData()
    {
        InventoryData = null;
    }
}