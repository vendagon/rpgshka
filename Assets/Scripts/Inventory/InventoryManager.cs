using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIBG;
    public Transform inventoryPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public bool isOpened;
    public float reachDistance = 3f;

    [Header("Pickup Settings")]
    public LayerMask pickupLayer;
    public float pickupRadius = 1.5f;

    [Header("Cursor Settings")]
    public Texture2D customCursor;
    public Vector2 cursorHotspot = Vector2.zero;
    public bool useSoftwareCursor = true;

    private PlayerMovement _playerMovement;

    public static InventoryManager Instance;

    private PauseMenu pauseMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        pauseMenu = FindFirstObjectByType<PauseMenu>();
        UIBG.SetActive(true);
        inventoryPanel.gameObject.SetActive(true);
        _playerMovement = GetComponent<PlayerMovement>();

        Cursor.SetCursor(null, Vector2.zero, GetCursorMode());
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Start()
    {
        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            if (inventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                slots.Add(inventoryPanel.GetChild(i).GetComponent<InventorySlot>());
            }
        }
        UIBG.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (pauseMenu != null && pauseMenu.isPaused) return; // Не открываем инвентарь во время паузы

            isOpened = !isOpened;
            if (isOpened)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }
        if (!isOpened && Input.GetKeyDown(KeyCode.F))
        {
            TryPickupItems();
        }
    }
    private void OpenInventory()
    {
        UIBG.SetActive(true);
        inventoryPanel.gameObject.SetActive(true);
        _playerMovement.SetControlsEnabled(false);
        SetInventoryCursor();
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseInventory()
    {
        UIBG.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
        _playerMovement.SetControlsEnabled(true);
        SetGameCursor();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void SetGameCursor()
    {
        // Возвращаем стандартный курсор или скрываем
        Cursor.SetCursor(null, Vector2.zero, GetCursorMode());
        Cursor.visible = false;
    }
    private void SetInventoryCursor()
    {
        // Устанавливаем кастомный курсор
        if (customCursor != null)
        {
            Cursor.SetCursor(customCursor, cursorHotspot, GetCursorMode());
        }
        Cursor.visible = true;
    }
    private CursorMode GetCursorMode()
    {
        return useSoftwareCursor ? CursorMode.ForceSoftware : CursorMode.Auto;
    }

    private void TryPickupItems()
    {
        // Получаем все коллайдеры в радиусе подбора
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);

        foreach (Collider2D collider in hitColliders)
        {
            Item itemComponent = collider.GetComponent<Item>();
            if (itemComponent != null)
            {
                AddItem(itemComponent.item, itemComponent.amount);
                Destroy(collider.gameObject);
                break; // Подбираем только один предмет за нажатие
            }
        }
    }

    private void AddItem(ItemScriptableObject _item, int _amount)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == _item)
            {
                if (slot.amount + _amount <= _item.maximumAmount)
                {
                    slot.amount += _amount;
                    slot.itemAmountText.text = slot.amount.ToString();
                    return;
                }
                break;
            }
        }
        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty == true)
            {
                slot.item = _item;
                slot.amount = _amount;
                slot.isEmpty = false;
                slot.SetIcon(_item.icon);
                slot.itemAmountText.text = _amount.ToString();
                break;
            }
        }
    }
    public InventorySaveData SerializeInventory()
    {
        InventorySaveData data = new InventorySaveData();

        // Сохраняем обычные слоты
        foreach (InventorySlot slot in slots)
        {
            data.slots.Add(CreateSlotData(slot));
        }

        // Находим QuickSlotInventory в сцене
        QuickSlotInventory quickSlotInventory = FindFirstObjectByType<QuickSlotInventory>();
        if (quickSlotInventory != null && quickSlotInventory.quickslotParent != null)
        {
            foreach (Transform slotTransform in quickSlotInventory.quickslotParent)
            {
                InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    data.quickSlots.Add(CreateSlotData(slot));
                }
            }
            data.currentQuickslotID = quickSlotInventory.currentQuickslotID;
        }
        else
        {
            Debug.LogWarning("QuickSlotInventory not found or quickslotParent not assigned!");
        }

        return data;
    }
    private SlotSaveData CreateSlotData(InventorySlot slot)
    {
        SlotSaveData slotData = new SlotSaveData
        {
            amount = slot.amount,
            isEmpty = slot.isEmpty
        };

        if (!slot.isEmpty && slot.item != null)
        {
            slotData.itemName = slot.item.name;
            slotData.itemType = slot.item.itemType;
            slotData.isConsumeable = slot.item.isConsumeable;
            slotData.changeHealth = slot.item.changeHealth;
        }

        return slotData;
    }

    public void DeserializeInventory(InventorySaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("InventorySaveData is null!");
            return;
        }

        // Загружаем обычные слоты
        for (int i = 0; i < data.slots.Count && i < slots.Count; i++)
        {
            LoadSlotData(slots[i], data.slots[i]);
        }

        // Находим QuickSlotInventory в сцене
        QuickSlotInventory quickSlotInventory = FindFirstObjectByType<QuickSlotInventory>();
        if (quickSlotInventory != null && quickSlotInventory.quickslotParent != null)
        {
            if (data.quickSlots != null)
            {
                for (int i = 0; i < data.quickSlots.Count && i < quickSlotInventory.quickslotParent.childCount; i++)
                {
                    Transform slotTransform = quickSlotInventory.quickslotParent.GetChild(i);
                    if (slotTransform != null)
                    {
                        InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
                        if (slot != null)
                        {
                            LoadSlotData(slot, data.quickSlots[i]);
                        }
                    }
                }
            }

            if (data.currentQuickslotID >= 0)
            {
                quickSlotInventory.currentQuickslotID = data.currentQuickslotID;
                quickSlotInventory.UpdateQuickSlotsHighlight();
            }
        }
        else
        {
            Debug.LogWarning("QuickSlotInventory not found or quickslotParent not assigned!");
        }
    }
    private void LoadSlotData(InventorySlot slot, SlotSaveData slotData)
    {
        if (slot == null)
        {
            Debug.LogWarning("Trying to load data into null slot!");
            return;
        }

        if (slotData == null)
        {
            slot.NullifySlot();
            return;
        }

        if (!slotData.isEmpty && !string.IsNullOrEmpty(slotData.itemName))
        {
            ItemScriptableObject item = FindItemByName(slotData.itemName);
            if (item != null)
            {
                slot.item = item;
                slot.amount = slotData.amount;
                slot.isEmpty = false;
                slot.SetIcon(item.icon);
                slot.itemAmountText.text = slotData.amount.ToString();

                // Восстанавливаем специфичные данные предмета
                item.itemType = slotData.itemType;
                item.isConsumeable = slotData.isConsumeable;
                item.changeHealth = slotData.changeHealth;
            }
            else
            {
                Debug.LogWarning($"Item {slotData.itemName} not found!");
                slot.NullifySlot();
            }
        }
        else
        {
            slot.NullifySlot();
        }
    }
    public ItemScriptableObject FindItemByName(string itemName)
    {
        // поиск предмета по имени
        return Resources.Load<ItemScriptableObject>("Items/" + itemName);
    }
    public void ClearInventory()
    {
        foreach (InventorySlot slot in slots)
        {
            slot.NullifySlot();
        }

        // Очищаем квикслоты, если они есть
        QuickSlotInventory quickSlotInventory = FindFirstObjectByType<QuickSlotInventory>();
        if (quickSlotInventory != null)
        {
            foreach (Transform slotTransform in quickSlotInventory.quickslotParent)
            {
                InventorySlot slot = slotTransform.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    slot.NullifySlot();
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}