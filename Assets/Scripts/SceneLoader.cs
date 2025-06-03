using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadDataCoroutine());
    }

    public IEnumerator LoadDataCoroutine()
    {
        // Ждем один кадр для инициализации всех систем
        yield return null;

        if (SceneDataTransfer.InventoryData != null)
        {
            // Ждем инициализации InventoryManager и Player
            yield return new WaitUntil(() =>
                InventoryManager.Instance != null &&
                TimeManager.Instance != null &&
                GameObject.FindGameObjectWithTag("Player") != null);


            // Востанавливаем инвентарь
            Debug.Log("Restoring inventory...");
            InventoryManager.Instance.DeserializeInventory(SceneDataTransfer.InventoryData);

            // Восстанавливаем игровое время
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.currentDay = SceneDataTransfer.CurrentDay;
                TimeManager.Instance.currentTime = SceneDataTransfer.CurrentTime;
                TimeManager.Instance.timer = SceneDataTransfer.TimePhaseProgress; // Восстанавливаем таймер
                TimeManager.Instance.UpdateTimeUI();
            }
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var healthSystem = player.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.SetHealth(SceneDataTransfer.CurrentHealth);
                    healthSystem.UpdateHealthUI();
                }

                // Восстанавливаем позицию игрока
                if (SceneDataTransfer.PlayerSpawnPosition != Vector3.zero)
                {
                    player.transform.position = SceneDataTransfer.PlayerSpawnPosition;
                }
            }
            SceneDataTransfer.Clear();
        }
    }
}