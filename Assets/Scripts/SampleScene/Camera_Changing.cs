using Unity.Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;

    private CinemachineCamera virtualCam;
    private PauseMenu pauseMenu;
    private InventoryManager inventory;

    private void Start()
    {
        virtualCam = GetComponent<CinemachineCamera>();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
        inventory = InventoryManager.Instance;
    }

    private void Update()
    {
        // ���������, �� ������� �� ���� ����� ��� ���������
        if ((pauseMenu != null && pauseMenu.isPaused) ||
            (inventory != null && inventory.isOpened))
        {
            return; // �������, ���� ���� �� ����� ��� ������ ���������
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            float newSize = virtualCam.Lens.OrthographicSize - scrollInput * zoomSpeed;
            newSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            virtualCam.Lens.OrthographicSize = newSize;
        }
    }
}