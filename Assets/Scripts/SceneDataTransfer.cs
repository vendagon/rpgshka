using System.Collections.Generic;
using UnityEngine;

public static class SceneDataTransfer
{
    public static InventorySaveData InventoryData;
    public static Vector3 PlayerSpawnPosition = Vector3.zero;
    public static int CurrentDay { get; set; }
    public static TimeOfDay CurrentTime;
    public static int CurrentHealth;
    public static float TimePhaseProgress;

    public static void PrepareTransition()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryData = InventoryManager.Instance.SerializeInventory();
        }


        if (TimeManager.Instance != null)
        {
            CurrentDay = TimeManager.Instance.currentDay;
            CurrentTime = TimeManager.Instance.currentTime;
            TimePhaseProgress = TimeManager.Instance.timer; // Сохраняем прогресс таймера
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var healthSystem = player.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                CurrentHealth = healthSystem.CurrentHealth;
            }
        }
    }

    public static void Clear()
    {
        InventoryData = null;
        PlayerSpawnPosition = Vector3.zero;
    }
}
