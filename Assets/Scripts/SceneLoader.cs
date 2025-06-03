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
        // ���� ���� ���� ��� ������������� ���� ������
        yield return null;

        if (SceneDataTransfer.InventoryData != null)
        {
            // ���� ������������� InventoryManager � Player
            yield return new WaitUntil(() =>
                InventoryManager.Instance != null &&
                TimeManager.Instance != null &&
                GameObject.FindGameObjectWithTag("Player") != null);


            // �������������� ���������
            Debug.Log("Restoring inventory...");
            InventoryManager.Instance.DeserializeInventory(SceneDataTransfer.InventoryData);

            // ��������������� ������� �����
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.currentDay = SceneDataTransfer.CurrentDay;
                TimeManager.Instance.currentTime = SceneDataTransfer.CurrentTime;
                TimeManager.Instance.timer = SceneDataTransfer.TimePhaseProgress; // ��������������� ������
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

                // ��������������� ������� ������
                if (SceneDataTransfer.PlayerSpawnPosition != Vector3.zero)
                {
                    player.transform.position = SceneDataTransfer.PlayerSpawnPosition;
                }
            }
            SceneDataTransfer.Clear();
        }
    }
}